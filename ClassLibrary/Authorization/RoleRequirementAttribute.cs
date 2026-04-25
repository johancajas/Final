using Microsoft.AspNetCore.Authorization;

namespace ClassLibrary.Authorization;

/// <summary>
/// Attribute for requiring specific roles to access endpoints
/// </summary>
public class RoleRequirementAttribute : AuthorizeAttribute
{
    public RoleRequirementAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
    }
}
