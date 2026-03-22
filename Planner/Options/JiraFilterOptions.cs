using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Planner.Options;

public class JiraFilterOptions : IValidatableObject
{
    public const string SectionName = nameof(JiraFilterOptions);

    [Required, MinLength(2)]
    public string DefaultProject { get; init; } = string.Empty;
 
    [Required(ErrorMessage = "Il campo Me è obbligatorio.")]
    [EmailAddress(ErrorMessage = "Il campo Me deve essere un indirizzo email valido.")]
    public string Me { get; init; } = string.Empty;

    public List<string> TeamMembers { get; init; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (TeamMembers.Count <= 0) yield break;
        
        var emailValidator = new EmailAddressAttribute();
        foreach (var email in TeamMembers.Where(email => !emailValidator.IsValid(email)))
        {
            yield return new ValidationResult(
                $"L'indirizzo '{email}' in TeamMembers non è un'email valida.", [nameof(TeamMembers)]);
        }
    }
}