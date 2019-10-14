using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AwesomeSauceCompanyLtd.Infrastructure
{
    public class AcceptHeaderOutputFormatterSelector : OutputFormatterSelector
    {
        private DefaultOutputFormatterSelector _fallbackSelector;
        private List<IOutputFormatter> _formatters;
        private IDictionary<string, string> _contentTypeMap = new Dictionary<string, string> {
            { "json", "application/json" },
            { "xml", "text/xml" }
        };

        public AcceptHeaderOutputFormatterSelector(IOptions<MvcOptions> options, ILoggerFactory loggerFactory)
        {
            _fallbackSelector = new DefaultOutputFormatterSelector(options, loggerFactory);
            _formatters = new List<IOutputFormatter>(options.Value.OutputFormatters);
        }

        public override IOutputFormatter SelectFormatter(OutputFormatterCanWriteContext context, IList<IOutputFormatter> formatters, MediaTypeCollection mediaTypes)
        {
            if (context.HttpContext.Request.Headers["Accept"].First().IndexOf("application/vnd.") < 0)
                return _fallbackSelector.SelectFormatter(context, formatters, mediaTypes);

            if (formatters.Count == 0)
            {
                formatters = _formatters;
            }

            context.ContentType = GetContentTypeFromAcceptHeader(context.HttpContext.Request);

            var formatter = formatters.FirstOrDefault(x => x.CanWriteResult(context));

            context.ContentType = context.HttpContext.Request.Headers["Accept"].First();

            return formatter;
        }

        private string GetContentTypeFromAcceptHeader(HttpRequest request)
        {
            var acceptHeaderValue = request.Headers["Accept"].First();
            if (acceptHeaderValue.IndexOf("+") > 0)
            {
                var contentType = acceptHeaderValue.Substring(acceptHeaderValue.IndexOf("+") + 1);
                if (_contentTypeMap.ContainsKey(contentType))
                    return _contentTypeMap[contentType];
            }
            return _contentTypeMap["json"];
        }
    }
}