using EnterpriseEmployeeManagementInc.Services.Entities;
using System.Collections.Generic;
using System.Security.Claims;

namespace EnterpriseEmployeeManagementInc.Services
{
    public static class Extensions
    {
        public static ClaimsPrincipal AsPrincipal(this User user, string authenticationScheme)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(CustomClaimTypes.TenantId, user.Tenant.Id.ToString()),
                new Claim(CustomClaimTypes.TenantName, user.Tenant.Name),
            };
            var identity = new ClaimsIdentity(claims, authenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        public static int TenantId(this ClaimsPrincipal principal)
        {
            return int.Parse(principal.FindFirstValue(CustomClaimTypes.TenantId));
        }
    }
}
