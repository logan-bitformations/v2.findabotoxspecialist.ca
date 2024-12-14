// using BotoxInjectorSite.DAO;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Web;
// using System.Web.Mvc;
// using BotoxInjectorSite.Interceptor;
// using Microsoft.AspNetCore.Mvc;

// namespace BotoxInjectorSite.Controllers
// {

//     public class APIController : Controller
//     {

//         private class ISFCredentials
//         {
//             public string SFtoken { get; set; }
//             public string SFInstanceURL { get; set; }
//         }

//         private ISFCredentials GetCredentials()
//         {
//             SFUtils sfUtil = new SFUtils();
//             if (Session["SFtoken"] == null)
//             {
//                 TokenResponseDTO token = sfUtil.GetTokenViaUuserPass();
//                 if (token != null)
//                 {
//                     Session["SFtoken"] = token.access_token;
//                     Session["SFInstanceURL"] = token.instance_url;
//                 }
//             }

//             ISFCredentials response = new ISFCredentials();
//             response.SFtoken = Session["SFtoken"].ToString();
//             response.SFInstanceURL = Session["SFInstanceURL"].ToString();
//             return response;
//         }

//         // GET: API

//         public JsonResult GetProvincesList()
//         {
//             SFUtils sfUtil = new SFUtils();
//             ISFCredentials creds = new ISFCredentials();
//             creds = this.GetCredentials();
            
//             string language = this.Request.RequestContext.RouteData.Values["language"].ToString();

//             List<ProvinceDTO> results = sfUtil.GetProvinces(creds.SFInstanceURL, creds.SFtoken, language);
//             return Json(results, JsonRequestBehavior.AllowGet);
//         }

//         public JsonResult GetCitiesList()
//         {
//             SFUtils sfUtil = new SFUtils();
//             ISFCredentials creds = new ISFCredentials();
//             creds = this.GetCredentials();

//             JsonResponseDTO<CityDTO> results = sfUtil.GetProvinceCities(creds.SFInstanceURL, creds.SFtoken);
//             return Json(results, JsonRequestBehavior.AllowGet);
//         }

//         public JsonResult FindByPostalCode()
//         {
//             string postalCode = Request.QueryString.Get("postal_code");
//             string distance = Request.QueryString.Get("distance");
//             string language = this.Request.RequestContext.RouteData.Values["language"].ToString();

//             Geocoder gc = new Geocoder();
//             GeocoderLocation loc = Geocoder.Locate(postalCode + " Canada");
                
//             SFUtils sfUtil = new SFUtils();
//             ISFCredentials creds = new ISFCredentials();
//             creds = this.GetCredentials();

//             JsonResponseDTO<ProviderDetailDTO> results = sfUtil.GetDoctorListByZip(creds.SFInstanceURL, creds.SFtoken, loc.Longitude, loc.Latitude, "", distance);

//             if (language == "fr")
//             {
//                 results = sfUtil.ConvertResultsToFrench(results);
//             }

//             return Json(results, JsonRequestBehavior.AllowGet);
//         }

//         public JsonResult FindById()
//         {
//             string id = Request.QueryString.Get("id");
//             string language = this.ControllerContext.RouteData.Values["language"].ToString();

//             SFUtils sfUtil = new SFUtils();
//             ISFCredentials creds = new ISFCredentials();
//             creds = this.GetCredentials();

//             JsonResponseDTO<ProviderDetailDTO> results = sfUtil.GetProviderDetails(creds.SFInstanceURL, creds.SFtoken, id);

//             if (language == "fr")
//             {
//                 results = sfUtil.ConvertResultsToFrench(results);
//             }

//             return Json(results, JsonRequestBehavior.AllowGet);
//         }

//         public JsonResult FindByCity()
//         {
//             string city = Request.QueryString.Get("city");
//             string language = this.ControllerContext.RouteData.Values["language"].ToString();

//             SFUtils sfUtil = new SFUtils();
//             ISFCredentials creds = new ISFCredentials();
//             creds = this.GetCredentials();

//             JsonResponseDTO<ProviderDetailDTO> results = sfUtil.GetDoctorListByCity(creds.SFInstanceURL, creds.SFtoken, city, "");

//             if (language == "fr")
//             {
//                 results = sfUtil.ConvertResultsToFrench(results);
//             }

//             return Json(results, JsonRequestBehavior.AllowGet);
//         }

//         public JsonResult FindByName()
//         {
//             string name = Request.QueryString.Get("name");
//             string language = this.ControllerContext.RouteData.Values["language"].ToString();

//             SFUtils sfUtil = new SFUtils();
//             ISFCredentials creds = new ISFCredentials();
//             creds = this.GetCredentials();

//             JsonResponseDTO<ProviderDetailDTO> results = sfUtil.GetDoctorListByName(creds.SFInstanceURL, creds.SFtoken, name, "");

//             if (language == "fr")
//             {
//                 results = sfUtil.ConvertResultsToFrench(results);
//             }

//             return Json(results, JsonRequestBehavior.AllowGet);
//         }

//     }
// }