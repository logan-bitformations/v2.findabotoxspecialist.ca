using Microsoft.AspNetCore.Mvc.Filters;

namespace botoxcounselling.ca.Filters
{
    public class LanguageFilter : IActionFilter
    {
        private readonly IConfiguration _config;
        private readonly ILogger<LanguageFilter> _logger;

        public LanguageFilter(IConfiguration config, ILogger<LanguageFilter> logger)
        {
            _config = config;
            _logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var baseUrl = context.HttpContext.Request.Scheme + "://" + context.HttpContext.Request.Host;
            var enSiteUrl = _config.GetValue<string>("ENSiteUrl");
            var frSiteUrl = _config.GetValue<string>("FRSiteUrl");
            
            var language = context.RouteData.Values["language"];
            if (baseUrl.Contains(enSiteUrl)) {
                context.RouteData.Values["language"] = "en";
            } else if (baseUrl.Contains(frSiteUrl)) {
                context.RouteData.Values["language"] = "fr";
            }
            else
            {
                // development
                context.RouteData.Values["language"] = "en";
                // context.RouteData.Values["language"] = "fr";
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}