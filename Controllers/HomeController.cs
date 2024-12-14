using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using findabotoxspecialist.ca.Models;
using botoxcounselling.ca.Filters;

namespace findabotoxspecialist.ca.Controllers;

[ServiceFilter(typeof(LanguageFilter))]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }


    public String GetMasterLayout()
    {
        return "_Layout";
    }

    public String GetLayout()
    {
        return String.Format("{1}_{0}", RouteData.Values["language"], RouteData.Values["action"]);
    }

    [ServiceFilter<HCPLoggedIn>]
    public IActionResult Index()
    {
        return View(GetLayout(), GetMasterLayout());
    }

    [ServiceFilter<HCPLoggedIn>]
    public ActionResult Results()
    {
        @ViewBag.GoogleMapsAPIClientKey = _configuration["GoogleMapsAPIClientKey"].ToString();
        return View(this.GetLayout(), this.GetMasterLayout());
    }

    public ActionResult Legal()
    {
        return View(this.GetLayout(), this.GetMasterLayout());
    }

    [HttpPost]
    // [ValidateGoogleCaptcha]
    [ValidateAntiForgeryToken]
    public ActionResult LoginSubmit()
    {
        HttpContext.Session.SetString("HCP_Logged_In", "true");
        return new RedirectToRouteResult(new RouteValueDictionary(
           new { action = "Index", controller = "Home" }));
    }

    public ActionResult Login()
    {
        return View(this.GetLayout(), this.GetMasterLayout());
    }

    public ActionResult Contact()
    {
        return View(this.GetLayout(), this.GetMasterLayout());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
