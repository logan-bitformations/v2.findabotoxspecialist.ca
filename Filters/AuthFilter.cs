using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace botoxcounselling.ca.Filters
{
    public class HCPLoggedIn : IResultFilter
    {
        private readonly IConfiguration _config;
        private readonly ILogger<HCPLoggedIn> _logger;

        public HCPLoggedIn(IConfiguration config, ILogger<HCPLoggedIn> logger)
        {
            _config = config;
            _logger = logger;
        }

        void IResultFilter.OnResultExecuting(ResultExecutingContext filterContext)
        {
            _logger.LogInformation("HCPLoggedIn Filter Executing");
            if (filterContext.HttpContext.Session.GetString("HCP_Logged_In") == null) {
                filterContext.Result = new RedirectResult("/Home/Login");
            }
        }

        void IResultFilter.OnResultExecuted(ResultExecutedContext filterContext)
        {
            _logger.LogInformation("HCPLoggedIn Filter Executed");
        }
    }
}