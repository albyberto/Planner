// using System.Text.Json;
// using Planner.Domain;
//
// namespace Planner.Model;
//
// public record IssueCommentModel
// {
//     public string Id { get; set; }
//     public UserModel Author { get; set; }
//     public string Body { get; set; }
//     public DateTime? Created { get; set; }
//     public DateTime? Updated { get; set; }
//
//     public IssueCommentModel(Comment dto)
//     {
//         Id = dto.Id;
//         Author = new(dto.Author);
//         Body = ParseAdfBody(dto.Body?.RootElement);
//         Created = dto.Created;
//         Updated = dto.Updated;
//     }
//
//     private static string ParseAdfBody(JsonElement? root)
//     {
//         if (root == null) return string.Empty;
//
//         // Se è già una stringa piatta
//         if (root.Value.ValueKind == JsonValueKind.String)
//             return root.Value.GetString() ?? string.Empty;
//
//         // Se è il formato ADF (Atlassian Document Format)
//         if (root.Value.ValueKind == JsonValueKind.Object)
//         {
//             var textParts = new List<string>();
//             ExtractTextFromAdf(root.Value, textParts);
//             return string.Join(" ", textParts).Trim();
//         }
//
//         return root.Value.GetRawText();
//     }
//
//     private static void ExtractTextFromAdf(JsonElement element, List<string> textParts)
//     {
//         if (element.ValueKind == JsonValueKind.Object)
//         {
//             if (element.TryGetProperty("type", out var typeProp))
//             {
//                 if (typeProp.GetString() == "text" &&
//                     element.TryGetProperty("text", out var textProp) && textProp.ValueKind == JsonValueKind.String)
//                 {
//                     var text = textProp.GetString();
//                     if (!string.IsNullOrWhiteSpace(text))
//                         textParts.Add(text);
//                 }
//                 else if (typeProp.GetString() == "mention" &&
//                          element.TryGetProperty("attrs", out var attrsProp) &&
//                          attrsProp.TryGetProperty("text", out var mentionTextProp) && mentionTextProp.ValueKind == JsonValueKind.String)
//                 {
//                     var text = mentionTextProp.GetString();
//                     if (!string.IsNullOrWhiteSpace(text))
//                     {
//                         var alias = text.Replace(" ", "");
//                         if (!alias.StartsWith("@")) alias = "@" + alias;
//                         textParts.Add(alias);
//                     }
//                 }
//             }
//
//             if (element.TryGetProperty("content", out var contentProp) && contentProp.ValueKind == JsonValueKind.Array)
//             {
//                 foreach (var child in contentProp.EnumerateArray())
//                 {
//                     ExtractTextFromAdf(child, textParts);
//                 }
//             }
//         }
//         else if (element.ValueKind == JsonValueKind.Array)
//         {
//             foreach (var child in element.EnumerateArray())
//             {
//                 ExtractTextFromAdf(child, textParts);
//             }
//         }
//     }
// }
