using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;
using System.Linq;

namespace AwesomeSauceCompanyLtd.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AcceptHeaderAttribute : Attribute, IActionConstraint
    {
        private readonly string _acceptHeader;

        public AcceptHeaderAttribute(string acceptHeader)
        {
            _acceptHeader = acceptHeader;
        }

        public bool Accept(ActionConstraintContext context)
        {
            return context.RouteContext.HttpContext.Request.Headers["Accept"].Any(x => x.StartsWith(_acceptHeader));
        }

        public int Order { get; set; }
    }
}