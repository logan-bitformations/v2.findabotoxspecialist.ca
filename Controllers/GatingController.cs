using BotoxInjectorSite.DAO;
using Microsoft.AspNetCore.Mvc;

namespace BotoxInjectorSite.Controllers
{
    public class GatingController : Controller
    {
        private readonly ILogger<GatingController> _logger;
        private readonly IConfiguration _configuration;

        public GatingController(ILogger<GatingController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public ActionResult Index()
        {
            return View();
        }

        public Boolean ValidateId()
        {
            string id = Request.Query["id"];
            string province = Request.Query["province"];
            string specialty = Request.Query["specialty"];
            Boolean result;
            switch (specialty)
            {
                case "Nurse Practitioner":
                case "INFIRMIER PRATICIEN/INFIRMIÈRE PRATICIENNE":
                    NurseDAO nurseDao = new NurseDAO();
                    NursePractitionerDAO nursePractitionerDAO = new NursePractitionerDAO();
                    result = nurseDao.ValidateId(province, id) || nursePractitionerDAO.ValidateId(province, id);
                    return result;
                case "Neurologist":
                case "Primary Care Provider":
                case "Médecin en soins primaires":
                case "Neurologue":
                    PhysicianDAO physicianDAO = new PhysicianDAO();
                    result = physicianDAO.ValidateId(province, id);
                    return result;
                case "Other Healthcare Provider":
                case "AUTRE PROFESSIONNEL DE LA SANTÉ":
                    PharmacistDAO pharmacistDAO = new PharmacistDAO();
                    result = pharmacistDAO.ValidateId(province, id);
                    return result;
                default:
                    return false;
            }
        }
    }
}