using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
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
        
    private ImmutableArray<UserModel> _assignees = [];
    private ImmutableArray<ComponentModel> _components = [];
    private ImmutableArray<LabelModel> _labels = [];
    private ImmutableArray<ProjectModel> _projects = [];
    private ImmutableArray<StatusModel> _statuses = [];
    private ImmutableArray<TypeModel> _types = [];

    public IssueSearchCriteria SearchCriteria = IssueSearchCriteria.Empty;

    protected override async Task OnInitializedAsync()
    {
        FilterStore.Register(PageKey);

        try
        {
            var options = Options.Value;

            SearchCriteria = SearchCriteria with
            {
                ProjectKey = options.DefaultProject,
                Types = options.DefaultTypes,
                Statuses = options.DefaultStatuses,
                Assignees = options.DefaultAssignees,
                Components = options.DefaultComponents,
                Labels = options.DefaultLabels,
                IncludeUnassigned = options.IncludeUnassignedByDefault
            };

            Emit(SearchCriteria);

            var projects = await ProjectsService.GetAsync();
            _projects = [..projects.OrderBy(project => project.Key)];

            await BuildAsync(options.DefaultProject);
        }
        catch
        {
            Snackbar.Add("Errore durante il recupero dei progetti.", Severity.Error);
        }
    }

    private async Task OnProjectChangedAsync(string projectKey)
    {
        SearchCriteria = IssueSearchCriteria.Create(projectKey);
        Emit(SearchCriteria);
        
        await BuildAsync(projectKey);
    }

    private void OnTypesChanged(IEnumerable<string> values)
    {
        SearchCriteria = SearchCriteria with { Types = values.ToHashSet() };
        Emit(SearchCriteria);
    }

    private string GetSelectedTypesText(IReadOnlyCollection<string?>? values)
    {
        if (values is null || values.Count == 0) return string.Empty;
        return string.Join(", ", _types.Where(x => values.Contains(x.Value)).Select(x => x.Name));
    }

    private void OnStatusesChanged(IEnumerable<string> values)
    {
        SearchCriteria = SearchCriteria with { Statuses = values.ToHashSet() };
        Emit(SearchCriteria);
    }

    private string GetSelectedStatusesText(IReadOnlyCollection<string?>? values)
    {
        if (values is null || values.Count == 0) return string.Empty;
        return string.Join(", ", _statuses.Where(x => values.Contains(x.Name)).Select(x => x.Name));
    }

    private void OnAssigneesChanged(IEnumerable<string> values)
    {
        SearchCriteria = SearchCriteria with { Assignees = values.ToHashSet() };
        Emit(SearchCriteria);
    }

    private string GetSelectedAssigneesText(IReadOnlyCollection<string?>? values)
    {
        if (values is null || values.Count == 0) return string.Empty;
        return string.Join(", ", _assignees.Where(x => values.Contains(x.EmailAddress)).Select(x => x.DisplayName));
    }

    private void OnComponentsChanged(IEnumerable<string> values)
    {
        SearchCriteria = SearchCriteria with { Components = values.ToHashSet() };
        Emit(SearchCriteria);
    }

    private string GetSelectedComponentsText(IReadOnlyCollection<string?>? values)
    {
        if (values is null || values.Count == 0) return string.Empty;
        return string.Join(", ", _components.Where(x => values.Contains(x.Value)).Select(x => x.Name));
    }

    private void OnLabelsChanged(IEnumerable<string> values)
    {
        SearchCriteria = SearchCriteria with { Labels = values.ToHashSet() };
        Emit(SearchCriteria);
    }

    private string GetSelectedLabelsText(IReadOnlyCollection<string?>? values)
    {
        if (values is null || values.Count == 0) return string.Empty;
        return string.Join(", ", _labels.Where(x => values.Contains(x.Value)).Select(x => x.Name));
    }

    private void OnUnassignedChanged(bool value)
    {
        SearchCriteria = SearchCriteria with { IncludeUnassigned = value };
        Emit(SearchCriteria);
    }

    private void Emit(IssueSearchCriteria criteria) => FilterStore.Emit(PageKey, criteria);

    private async Task BuildAsync(string projectKey)
    {
        var project = await ProjectsService.GetAsync(projectKey);

        if (project is null) return;

        var types = await project.GetIssueTypesAsync();
        _types = [..types.OrderBy(t => t.Name)];

        var statuses = await project.GetStatusesAsync();
        _statuses = [..statuses.OrderBy(s => s.Name)];

        var assignees = await project.GetAssigneesAsync();
        _assignees = [..assignees.OrderBy(a => a.DisplayName)];

        var components = await project.GetComponentsAsync();
        _components = [..components.OrderBy(c => c.Name)];

        var labels = await project.GetLabelsAsync();
        _labels = [..labels.OrderBy(l => l.Name)];
    }

    public void Dispose() => FilterStore.UnRegister(PageKey);
}