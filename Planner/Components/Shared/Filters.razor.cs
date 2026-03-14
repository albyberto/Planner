using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.Extensions.Options;
using MudBlazor;
using Planner.Model;
using Planner.Options;
using Planner.Services;
using Planner.Stores;

namespace Planner.Components.Shared;

public partial class Filters : ComponentBase, IDisposable
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

    private IssuesSearchCriteria _searchCriteria = IssuesSearchCriteria.Empty;

    protected override async Task OnInitializedAsync()
    {
        FilterStore.Register(PageKey);

        try
        {
            var options = Options.Value;

            _searchCriteria = _searchCriteria with
            {
                ProjectKey = options.DefaultProject,
                Types = options.DefaultTypes,
                Statuses = options.DefaultStatuses,
                Assignees = options.DefaultAssignees,
                Components = options.DefaultComponents,
                Labels = options.DefaultLabels,
                IncludeUnassigned = options.IncludeUnassignedByDefault
            };

            Emit(_searchCriteria);
            
            await BuildAsync(options.DefaultProject);
        }
        catch
        {
            Snackbar.Add("Errore durante l'inizializzazione dei filtri.", Severity.Error);
        }
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
        
        _searchCriteria = IssuesSearchCriteria.Create(projectKey);
        Emit(_searchCriteria);
        await BuildAsync(projectKey);
    }

    #endregion
    
    #region Types

    private void OnTypesChanged(IEnumerable<string> values)
    {
        _searchCriteria = _searchCriteria with { Types = values.ToHashSet() };
        Emit(_searchCriteria);
    }

    private string GetSelectedTypesText(IReadOnlyCollection<string?>? values)
    {
        if (values is null || values.Count == 0) return string.Empty;
        
        return string.Join(", ", _types.Where(x => values.Contains(x.Value)).Select(x => x.Name));
    }

    #endregion

    #region Statuses

    private void OnStatusesChanged(IEnumerable<string> values)
    {
        _searchCriteria = _searchCriteria with { Statuses = values.ToHashSet() };
        Emit(_searchCriteria);
    }

    private string GetSelectedStatusesText(IReadOnlyCollection<string?>? values)
    {
        if (values is null || values.Count == 0) return string.Empty;
        return string.Join(", ", _statuses.Where(x => values.Contains(x.Name)).Select(x => x.Name));
    }

    #endregion

    #region  Assignees

    private void OnAssigneesChanged(IEnumerable<string> values)
    {
        _searchCriteria = _searchCriteria with { Assignees = values.ToHashSet() };
        Emit(_searchCriteria);
    }

    private string GetSelectedAssigneesText(IReadOnlyCollection<string?>? values)
    {
        if (values is null || values.Count == 0) return string.Empty;
        return string.Join(", ", _assignees.Where(x => values.Contains(x.EmailAddress)).Select(x => x.DisplayName));
    }

    #endregion

    #region Components

    private void OnComponentsChanged(IEnumerable<string> values)
    {
        _searchCriteria = _searchCriteria with { Components = values.ToHashSet() };
        Emit(_searchCriteria);
    }

    private string GetSelectedComponentsText(IReadOnlyCollection<string?>? values)
    {
        if (values is null || values.Count == 0) return string.Empty;
        return string.Join(", ", _components.Where(x => values.Contains(x.Value)).Select(x => x.Name));
    }

    #endregion

    #region Labels

    private async ValueTask<ItemsProviderResult<LabelModel>> LoadLabels(ItemsProviderRequest request)
    {
        var take = request.Count == 0 ? 50 : (uint)request.Count;
        var labels = await ProjectsService.GetLabelsPageAsync((uint)request.StartIndex, take, request.CancellationToken);
        
        var total = request.StartIndex + labels.Length + (labels.Length == request.Count ? 1 : 0);
        return new(labels, total);
    }
    
    private void OnLabelsChanged(IEnumerable<string> values)
    {
        _searchCriteria = _searchCriteria with { Labels = values.ToHashSet() };
        Emit(_searchCriteria);
    }

    private static string GetSelectedLabelsText(IReadOnlyCollection<string?>? values)
    {
        if (values is null || values.Count == 0) return string.Empty;
        return string.Join(", ", values);
    }

    #endregion

    private void OnUnassignedChanged(bool value)
    {
        _searchCriteria = _searchCriteria with { IncludeUnassigned = value };
        Emit(_searchCriteria);
    }

    private void Emit(IssuesSearchCriteria criteria) => FilterStore.Emit(PageKey, criteria);

    private async Task BuildAsync(string projectKey)
    {
        var projectsTask = ProjectsService.SearchProjectsAsync(CancellationToken.None);
        var typesTask = ProjectsService.GetTypesAsync(projectKey);
        var statusesTask = ProjectsService.GetStatusesAsync(projectKey);
        var assigneesTask = ProjectsService.GetAssigneesAsync(projectKey);
        var componentsTask = ProjectsService.GetComponentsAsync(projectKey);

        await Task.WhenAll(projectsTask, typesTask, statusesTask, assigneesTask, componentsTask);

        _projects = projectsTask.Result;
        _types = typesTask.Result;
        _statuses = statusesTask.Result;
        _assignees = assigneesTask.Result;
        _components = componentsTask.Result;
    }

    public void Dispose() => FilterStore.UnRegister(PageKey);
}