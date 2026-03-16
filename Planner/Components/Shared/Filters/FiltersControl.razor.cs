using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using MudBlazor;
using Planner.Model;
using Planner.Options;
using Planner.Services;
using Planner.Stores;

namespace Planner.Components.Shared.Filters;

public partial class FiltersControl : ComponentBase, IDisposable
{
    [Inject] public required IOptions<JiraFilterOptions> Options { get; set; }
    [Inject] public required ProjectsService ProjectsService { get; set; }
    [Inject] public required FilterStore FilterStore { get; set; }
    [Inject] public required ISnackbar Snackbar { get; set; }

    [Parameter] public Guid PageKey { get; set; }
    [Parameter] public bool ShowAssigneeFilter { get; set; } = true;
        
    private ImmutableArray<ProjectModel> _projects = [];
    private ImmutableArray<UserModel> _assignees = [];
    private ImmutableArray<ComponentModel> _components = [];
    private ImmutableArray<StatusModel> _statuses = [];
    private ImmutableArray<TypeModel> _types = [];
    private ImmutableArray<LabelModel> _labels = [];

    private IssuesSearchCriteria _searchCriteria = IssuesSearchCriteria.Empty;
    private bool _isLoading = true;
    private DateTime? _timeFrom;
    private DateTime? _timeTo;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _isLoading = true;
            var options = Options.Value;

            _searchCriteria = _searchCriteria with
            {
                ProjectKey = options.DefaultProject,
                Types = options.DefaultTypes.Select(x => new FilterValue(x.Value, x.IsExcluded)).ToHashSet(),
                Statuses = options.DefaultStatuses.Select(x => new FilterValue(x.Value, x.IsExcluded)).ToHashSet(),
                Assignees = options.DefaultAssignees.Select(x => new FilterValue(x.Value, x.IsExcluded)).ToHashSet(),
                Components = options.DefaultComponents.Select(x => new FilterValue(x.Value, x.IsExcluded)).ToHashSet(),
                Labels = options.DefaultLabels.Select(x => new FilterValue(x.Value, x.IsExcluded)).ToHashSet(),
                IncludeUnassigned = options.IncludeUnassignedByDefault
            };

            FilterStore.Register(PageKey);
            Emit(_searchCriteria);

            await BuildAsync(options.DefaultProject);

            _searchCriteria = _searchCriteria with { };
        }
        catch(Exception)
        {
            Snackbar.Add("Errore durante l'inizializzazione dei filtri.", Severity.Error);
        }
        finally
        {
            _isLoading = false; 
        }
    }

    // --- HELPER UNIVERSALI PER I FILTER VALUE ---

    // Allinea le stringhe in arrivo dal MudSelect con la nostra memoria di FilterValue (preservando lo stato IsExcluded)
    private HashSet<FilterValue> ReconcileSet(HashSet<FilterValue> currentSet, IEnumerable<string> incomingValues)
    {
        var newSet = new HashSet<FilterValue>();
        foreach (var val in incomingValues)
        {
            var existing = currentSet.FirstOrDefault(x => x.Value == val);
            newSet.Add(existing ?? new FilterValue(val));
        }
        return newSet;
    }

    // Inverte lo stato In/NotIn di un singolo elemento quando si clicca la MudChip
    private HashSet<FilterValue> ToggleItemState(HashSet<FilterValue> set, FilterValue item)
    {
        var newSet = set.ToHashSet();
        newSet.Remove(item);
        newSet.Add(item with { IsExcluded = !item.IsExcluded });
        return newSet;
    }

    #region Projects
    private Task<IEnumerable<string>> SearchProjectsAsync(string? value, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(value)) return Task.FromResult<IEnumerable<string>>([]);

        var token = value.Trim().ToUpperInvariant();
        var result = _projects
            .Where(p => p.Key.StartsWith(token, StringComparison.OrdinalIgnoreCase))
            .Select(p => p.Key);

        return Task.FromResult(result);
    }

    private async Task OnProjectChangedAsync(string? projectKey)
    {
        if (string.IsNullOrWhiteSpace(projectKey)) return;

        var token = projectKey.Trim().ToUpperInvariant();
        if (!_projects.Select(project => project.Key).Contains(projectKey, StringComparer.OrdinalIgnoreCase)) return;
        
        try
        {
            _isLoading = true;
            _searchCriteria = IssuesSearchCriteria.Create(projectKey);
            Emit(_searchCriteria);
            await BuildAsync(projectKey);
        }
        finally
        {
            _isLoading = false;
        }
    }
    #endregion
    
    #region Types
    private void OnTypesChanged(IEnumerable<string> values) => 
        Emit(_searchCriteria = _searchCriteria with { Types = ReconcileSet(_searchCriteria.Types, values) });
    
    private void ToggleType(FilterValue item) => 
        Emit(_searchCriteria = _searchCriteria with { Types = ToggleItemState(_searchCriteria.Types, item) });
    
    private void RemoveType(FilterValue item) 
    { 
        var s = _searchCriteria.Types.ToHashSet(); 
        s.Remove(item); 
        Emit(_searchCriteria = _searchCriteria with { Types = s }); 
    }
    #endregion

    #region Statuses
    private void OnStatusesChanged(IEnumerable<string> values) => 
        Emit(_searchCriteria = _searchCriteria with { Statuses = ReconcileSet(_searchCriteria.Statuses, values) });
    
    private void ToggleStatus(FilterValue item) => 
        Emit(_searchCriteria = _searchCriteria with { Statuses = ToggleItemState(_searchCriteria.Statuses, item) });
    
    private void RemoveStatus(FilterValue item) 
    { 
        var s = _searchCriteria.Statuses.ToHashSet(); 
        s.Remove(item); 
        Emit(_searchCriteria = _searchCriteria with { Statuses = s }); 
    }
    #endregion

    #region Assignees
    
    private void OnAssigneesChanged(IEnumerable<string> values) => 
        Emit(_searchCriteria = _searchCriteria with { Assignees = ReconcileSet(_searchCriteria.Assignees, values) });
    
    private void ToggleAssignee(FilterValue item) => 
        Emit(_searchCriteria = _searchCriteria with { Assignees = ToggleItemState(_searchCriteria.Assignees, item) });
    
    private void RemoveAssignee(FilterValue item) 
    { 
        var s = _searchCriteria.Assignees.ToHashSet(); 
        s.Remove(item); 
        Emit(_searchCriteria = _searchCriteria with { Assignees = s }); 
    }
    
    private void OnUnassignedChanged(bool value)
    {
        _searchCriteria = _searchCriteria with { IncludeUnassigned = value };
        Emit(_searchCriteria);
    }
    
    #endregion

    #region Components
    private void OnComponentsChanged(IEnumerable<string> values) => 
        Emit(_searchCriteria = _searchCriteria with { Components = ReconcileSet(_searchCriteria.Components, values) });
    
    private void ToggleComponent(FilterValue item) => 
        Emit(_searchCriteria = _searchCriteria with { Components = ToggleItemState(_searchCriteria.Components, item) });
    
    private void RemoveComponent(FilterValue item) 
    { 
        var s = _searchCriteria.Components.ToHashSet(); 
        s.Remove(item); 
        Emit(_searchCriteria = _searchCriteria with { Components = s }); 
    }
    #endregion

    #region Labels
    private string? _labelSearchInput;

    private Task<IEnumerable<string>> SearchLabelsAsync(string? value, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(value)) 
        {
            var defaultResult = _labels
                .Select(l => l.Name)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(50);
                
            return Task.FromResult(defaultResult);
        }

        var token = value.Trim();

        var result = _labels
            .Where(l => l.Name.Contains(token, StringComparison.OrdinalIgnoreCase))
            .Select(l => l.Name)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(50);

        return Task.FromResult(result);
    }

    private void OnLabelAdded(string? newLabel)
    {
        _labelSearchInput = null;

        if (string.IsNullOrWhiteSpace(newLabel)) return;

        var currentLabels = _searchCriteria.Labels.ToHashSet();
        
        if (currentLabels.Any(x => x.Value.Equals(newLabel, StringComparison.OrdinalIgnoreCase))) return;
        
        currentLabels.Add(new(newLabel));
        Emit(_searchCriteria = _searchCriteria with { Labels = currentLabels });
    }

    private void ToggleLabel(FilterValue item) => 
        Emit(_searchCriteria = _searchCriteria with { Labels = ToggleItemState(_searchCriteria.Labels, item) });

    private void RemoveLabel(FilterValue item)
    {
        var s = _searchCriteria.Labels.ToHashSet();
        s.Remove(item);
        Emit(_searchCriteria = _searchCriteria with { Labels = s });
    }
    #endregion

    #region Time filter
   
    private void OnTimeFilterChanged(TimeFilter? newTimeFilter)
    {
        _searchCriteria = _searchCriteria with { TimeFilter = newTimeFilter };
        Emit(_searchCriteria);
    }
    
    #endregion

    private void Emit(IssuesSearchCriteria criteria) => FilterStore.Emit(PageKey, criteria);

    private async Task BuildAsync(string projectKey)
    {
        var projectsTask = ProjectsService.GetProjectsAsync(CancellationToken.None);
        var typesTask = ProjectsService.GetTypesAsync(projectKey);
        var statusesTask = ProjectsService.GetStatusesAsync(projectKey);
        var assigneesTask = ProjectsService.GetAssigneesAsync(projectKey);
        var componentsTask = ProjectsService.GetComponentsAsync(projectKey);
        var labelsTask = ProjectsService.GetLabelsAsync(CancellationToken.None);

        await Task.WhenAll(projectsTask, typesTask, statusesTask, assigneesTask, componentsTask, labelsTask);

        _projects = projectsTask.Result;
        _types = typesTask.Result;
        _statuses = statusesTask.Result;
        _assignees = assigneesTask.Result;
        _components = componentsTask.Result;
        _labels = labelsTask.Result;
    }

    public void Dispose() => FilterStore.UnRegister(PageKey);
}