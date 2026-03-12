using System.Collections.Immutable;
using Planner.Model;

namespace Planner.Stores;

/// <summary>
/// Singleton store for issue results. Background services emit issues here, pages subscribe.
/// Keyed by pageKey so different pages can have independent issue streams.
/// </summary>
public class IssueStore() : DynamicQueryStoreBase<ImmutableArray<IssueModel>>(ImmutableArray<IssueModel>.Empty);
