using System.Diagnostics;
using System.Net;
using System.Text;
using System.Net.Mail;
using System.Reflection;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BotoxInjectorSite.DAO
{
    public class SFUtils
    {
        //Expose variables to calling function
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Code { get; set; }
        public string AuthorizeURL { get; set; }
        public string TokenURL { get; set; }
        public string RedirectURL { get; set; }
        public int QuerySize { get; set; }
        public int Sessionlength { get; set; }

        public TokenResponseDTO token;

        #region SFFunctions
        public enum EnumProvince
        {
            ON,
            QC,
            NS,
            NB,
            MB,
            BC,
            PE,
            SK,
            AB,
            NL,
            PQ,
            NT,
            NU,
            YT
        }

        //private DateTime _nextLoginTime;
        //Initialize private variables for the class
        public SFUtils(IConfiguration configuration)
        {

            this.QuerySize = 500;
            this.Sessionlength = 30;
            this.TokenURL = configuration["TokenURL"].ToString();
            this.UserName = configuration["UserName"];
            this.Password = configuration["Password"];
            this.ClientId = configuration["ClientId"];
            this.ClientSecret = configuration["ClientSecret"];
        }


        public TokenResponseDTO GetTokenViaUuserPass()
        {
            try
            {
                StringBuilder body = new StringBuilder();
                body.Append("grant_type=password&");
                body.Append("client_id=" + ClientId + "&");
                body.Append("client_secret=" + ClientSecret + "&");
                body.Append("username=" + UserName + "&");
                body.Append("password=" + Password + "&");

                string result = HttpPostJson(TokenURL, body.ToString());

                // Convert the JSON response into a token object
                token = JsonSerializer.Deserialize<TokenResponseDTO>(result);
                return token;
            }
            catch
            {
                return null;
            }

        }

        public string GetTokenViaUuserPassTest()
        {
            StringBuilder body = new StringBuilder();
            body.Append("grant_type=password&");
            body.Append("client_id=" + ClientId + "&");
            body.Append("client_secret=" + ClientSecret + "&");
            body.Append("username=" + UserName + "&");
            body.Append("password=" + Password + "&");

            string URI = TokenURL;
            string Parameters = body.ToString();
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";

                // Add parameters to post
                byte[] data = System.Text.Encoding.ASCII.GetBytes(Parameters);
                req.ContentLength = data.Length;
                System.IO.Stream os = req.GetRequestStream();
                os.Write(data, 0, data.Length);
                os.Close();

                // Do the post and get the response.
                System.Net.WebResponse resp = req.GetResponse();
                if (resp == null) return null;
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public List<ProvinceDTO> GetProvinces(string InstanceUrl, string AccessToken, string language)
        {
            // Read the REST resources
            try
            {
                string provinceJson =
                    HttpGetJson(
                        InstanceUrl +
                        @"/services/data/v26.0/query?q=select DL_PROVINCE__C from Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes'  GROUP BY DL_PROVINCE__C order by DL_Province__C",
                        "", AccessToken);

                Debug.WriteLine("\n*******\n");
                Debug.WriteLine(provinceJson);
                Debug.WriteLine("\n*******\n");

                // Convert the JSON response into a  object
                JsonResponseDTO<ProvinceDTO> provinceObj =
                    JsonSerializer.Deserialize<JsonResponseDTO<ProvinceDTO>>(provinceJson);
                List<ProvinceDTO> provinceList = new List<ProvinceDTO>();
                //This has been done to remove null records
                foreach (var provinceDto in provinceObj.records)
                {
                    if (!String.IsNullOrEmpty(provinceDto.DL_Province__c))
                    {
                        if (provinceDto.DL_Province__c != "PQ")
                        {
                            ProvinceDTO province = new ProvinceDTO();
                            province.DL_Province__c = provinceDto.DL_Province__c;
                            if (language == "en")
                            {
                                province.DL_ProvinceName = ChangeProvinceText(provinceDto.DL_Province__c);
                            }
                            else
                            {
                                province.DL_ProvinceName = ChangeProvinceTextFrench(provinceDto.DL_Province__c);
                            }
                            provinceList.Add(province);
                        }
                    }
                }
                return provinceList;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }

        }


        public JsonResponseDTO<ProviderDetailDTO> GetProviderDetails(string InstanceUrl, string AccessToken, string ProviderId)
        {
            // Read the REST resources
            try
            {
                //string providerDetailJson =
                //    HttpGetJson(
                //        InstanceUrl +
                //        @"/services/data/v26.0/query?q=select Id,Name,Firstname,Lastname,DL_City__c,DL_Email__c,DL_Mailing_Address__c,DL_Postal_Code__c,DL_Province__c,Website, Condition_Treatment__c,DL_Office_Phone__c, DL_Office_Fax__c,DL_Location__Latitude__s,DL_Location__Longitude__s,Treatments_to_Manage_Headache_AGN__c FROM Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes' and id ='" +
                //        ProviderId + "'", "", AccessToken);


                string providerDetailJson =
                    HttpGetJson(
                        InstanceUrl +
                        @"/services/data/v26.0/query?q=select Id,Name,Firstname,Lastname,DL_City__c,DL_Mailing_Address__c,DL_Postal_Code__c,DL_Province__c, Condition_Treatment__c,DL_Office_Phone__c, DL_Office_Fax__c,DL_Location__Latitude__s,DL_Location__Longitude__s,Wait_Time__c,AGREE_TO_PARTICIPATE__C,Treatments_to_Manage_Headache_AGN__c FROM Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes' and id ='" +
                        ProviderId + "'", "", AccessToken);

                // Convert the JSON response into a  object
                JsonResponseDTO<ProviderDetailDTO> providerDetail =
                    JsonSerializer.Deserialize<JsonResponseDTO<ProviderDetailDTO>>(providerDetailJson);

                return providerDetail;
            }
            catch
            {
                return null;
            }

        }

        public JsonResponseDTO<ProviderDetailDTO> GetProviderDetails(string InstanceUrl, string AccessToken, string ProviderId, string latitude, string longitude)
        {
            // Read the REST resources
            try
            {
                string providerDetailJson =
                    HttpGetJson(
                        InstanceUrl +
                        @"/services/data/v26.0/query?q=select Id,Name,Firstname,Lastname,DL_City__c,DL_Email__c,DL_Mailing_Address__c,DL_Postal_Code__c,DL_Province__c,Website, Condition_Treatment__c,DL_Office_Phone__c, DL_Office_Fax__c,DL_Location__Latitude__s,DL_Location__Longitude__s,Treatments_to_Manage_Headache_AGN__c FROM Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes' and id ='" +
                        ProviderId + "'", "", AccessToken);

                Debug.WriteLine(InstanceUrl +
                        @"/services/data/v26.0/query?q=select Id,Name,Firstname,Lastname,DL_City__c,DL_Email__c,DL_Mailing_Address__c,DL_Postal_Code__c,DL_Province__c,Website, Condition_Treatment__c,DL_Office_Phone__c, DL_Office_Fax__c,DL_Location__Latitude__s,DL_Location__Longitude__s,Treatments_to_Manage_Headache_AGN__c FROM Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes' and id ='" +
                        ProviderId + "'", "", AccessToken);

                // Convert the JSON response into a  object
                JsonResponseDTO<ProviderDetailDTO> providerDetail =
                JsonSerializer.Deserialize<JsonResponseDTO<ProviderDetailDTO>>(providerDetailJson);
                //update the distance field
                if (providerDetail != null)
                {
                    Haversine hv = new Haversine();
                    foreach (var rec in providerDetail.records)
                    {
                        Position pos1 = new Position();
                        pos1.Latitude = Convert.ToDouble(rec.DL_Location__Latitude__s);
                        pos1.Longitude = Convert.ToDouble(rec.DL_Location__Longitude__s);

                        Position pos2 = new Position();
                        pos2.Latitude = Convert.ToDouble(latitude);
                        pos2.Longitude = Convert.ToDouble(longitude);
                        double result = hv.Distance(pos1, pos2, DistanceType.Kilometers);
                        rec.Distance = Math.Round(result, 2).ToString();
                    }

                }
                return providerDetail;
            }
            catch
            {
                return null;
            }

        }

        public JsonResponseDTO<CityDTO> GetProvinceCities(string InstanceUrl, string AccessToken)
        {
            try
            {
                var cityJson =
                    HttpGetJson(
                        InstanceUrl +
                        @"/services/data/v26.0/query?q=select DL_CITY__C, DL_PROVINCE__C from Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes'  AND Areas_of_Interest__c INCLUDES ('Headache/Migraine Management') GROUP BY DL_CITY__C, DL_PROVINCE__C order by DL_PROVINCE__C,DL_CITY__C",
                        "", AccessToken);

                // Convert the JSON response into a  object

                JsonResponseDTO<CityDTO> cities =
                JsonSerializer.Deserialize<JsonResponseDTO<CityDTO>>(cityJson);
                return cities;
            }
            catch
            {
                return null;
            }


        }

        string GetWaitTimeFilter(string waitTime)
        {
            if (waitTime == "1")
                return " and (Wait_Time__c = 'Less than 1 month')";
            if (waitTime == "3")
                return " and (Wait_Time__c = 'Less than 1 month' OR Wait_Time__c = 'Less than 3 months')";
            if (waitTime == "4")
                return " and (Wait_Time__c = 'Less than 1 month' OR Wait_Time__c = 'Less than 3 months OR Wait_Time__c = 'Less than 6 months')";

            return "";
        }

        public JsonResponseDTO<ProviderDetailDTO> GetDoctorListByName(string InstanceUrl, string AccessToken, string name, string waitTime)
        {
            // Read the REST resources
            string providerListJson = null;
            try
            {


                //if waitime is passed then add filter
                if (String.IsNullOrEmpty(waitTime) || waitTime == "0")
                {
                    if (name == "tester")
                    {
                        providerListJson =
                        HttpGetJson(
                            InstanceUrl +
                            @"/services/data/v26.0/query?q=select Id,Name,Firstname,Lastname,DL_City__c,DL_Mailing_Address__c,DL_Postal_Code__c,DL_Province__c, Condition_Treatment__c,DL_Office_Phone__c, DL_Office_Fax__c,DL_Location__Latitude__s,DL_Location__Longitude__s,Wait_Time__c,Status_AGN__c,Specialty_1_AGN__r.Name,Treatments_to_Manage_Headache_AGN__c  FROM Account WHERE Id='0014I00001uC41fQAC'", "", AccessToken);
                    }
                    // Read the REST resources
                    else
                    {

                        providerListJson =
                            HttpGetJson(
                                InstanceUrl +
                                    @"/services/data/v26.0/query?q=select Id,Name,Firstname,Lastname,DL_City__c,DL_Mailing_Address__c,DL_Postal_Code__c,DL_Province__c, Condition_Treatment__c,DL_Office_Phone__c, DL_Office_Fax__c,DL_Location__Latitude__s,DL_Location__Longitude__s,Wait_Time__c FROM Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes' and Name like '%25" +
                                name + "%25' AND Areas_of_Interest__c INCLUDES ('Headache/Migraine Management')", "", AccessToken);
                    }
                }
                else
                {

                    providerListJson =
                        HttpGetJson(
                            InstanceUrl +
                            @"/services/data/v26.0/query?q=select Id,Name,Firstname,Lastname,DL_City__c,DL_Mailing_Address__c,DL_Postal_Code__c,DL_Province__c, Condition_Treatment__c,DL_Office_Phone__c, DL_Office_Fax__c,DL_Location__Latitude__s,DL_Location__Longitude__s,Wait_Time__c,Status_AGN__c,Specialty_1_AGN__r.Name,Treatments_to_Manage_Headache_AGN__c FROM Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes' AND Status_AGN__c='Active' and Name like '%25" +
                            name + "%25' AND Areas_of_Interest__c INCLUDES ('Headache/Migraine Management') and Wait_Time_Numeric__c <= " + GetWaitTimeFilter(waitTime)
                            + "", "", AccessToken);



                }

                // Convert the JSON response into a  object
                JsonResponseDTO<ProviderDetailDTO> providerList =
                JsonSerializer.Deserialize<JsonResponseDTO<ProviderDetailDTO>>(providerListJson);
                return providerList;
            }
            catch
            {
                return null;
            }

        }

        public JsonResponseDTO<AttachmentDTO> GetReferralForm(string InstanceUrl, string AccessToken, string ProviderId)
        {
            // Read the REST resources
            try
            {
                string referralJson = HttpGetJson(InstanceUrl + @"/services/data/v26.0/query?q=Select a.Name,a.Body,a.ContentType From Attachment a where a.parentid = '" + ProviderId + "'", "", AccessToken);
                // Convert the JSON response into a  object

                JsonResponseDTO<AttachmentDTO> deserializedAttachment =
                    JsonSerializer.Deserialize<JsonResponseDTO<AttachmentDTO>>(referralJson);
                return deserializedAttachment;
            }
            catch (Exception)
            {
                return null;
            }


        }


        public JsonResponseDTO<ProviderDetailDTO> GetDoctorListByCity(string InstanceUrl, string AccessToken, string City, string waitTime)
        {
            string providerListJson = null;
            try
            {
                //if waitime is passed then add filter
                if (String.IsNullOrEmpty(waitTime) || waitTime == "0")
                {
                    providerListJson =
                        HttpGetJson(
                            InstanceUrl +
                            @"/services/data/v26.0/query?q=select Id,Name,Firstname,Lastname,DL_City__c,DL_Mailing_Address__c,DL_Postal_Code__c,DL_Province__c, Condition_Treatment__c,DL_Office_Phone__c, DL_Office_Fax__c,DL_Location__Latitude__s,DL_Location__Longitude__s,Wait_Time__c,AGREE_TO_PARTICIPATE__C,Treatments_to_Manage_Headache_AGN__c FROM Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes' and DL_City__c like '%25" +
                            City + "%25' AND Areas_of_Interest__c INCLUDES ('Headache/Migraine Management')", "", AccessToken);

                }
                else
                {
                    providerListJson =
                        HttpGetJson(
                            InstanceUrl +
                            @"/services/data/v26.0/query?q=select Id,Name,Firstname,Lastname,DL_City__c,DL_Mailing_Address__c,DL_Postal_Code__c,DL_Province__c, Condition_Treatment__c,DL_Office_Phone__c, DL_Office_Fax__c,DL_Location__Latitude__s,DL_Location__Longitude__s,Wait_Time__c,Status_AGN__c,Treatments_to_Manage_Headache_AGN__c,Specialty_1_AGN__r.Name FROM Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes AND Status_AGN__c='Active'' and DL_City__c like '%25" +
                            City + "%25' AND Areas_of_Interest__c INCLUDES ('Headache/Migraine Management') " + GetWaitTimeFilter(waitTime)
                            + "", "", AccessToken);

                }
                // Convert the JSON response into a  object
                JsonResponseDTO<ProviderDetailDTO> providerList =
                JsonSerializer.Deserialize<JsonResponseDTO<ProviderDetailDTO>>(providerListJson);
                return providerList;
            }
            catch
            {
                return null;
            }
        }

        public JsonResponseDTO<ProviderDetailDTO> GetDoctorListByZip(string InstanceUrl, string AccessToken, double longitude, double latitude, string waitTime, string radius)
        {
            // Read the REST resources
            string providerListJson = null;
            string ActualError = null;

            //try
            //{


            //if waitime is passed then add filter
            if (String.IsNullOrEmpty(waitTime) || waitTime == "0")
            {

                //string query = System.Web.HttpUtility.UrlEncode("select Id,Name,Firstname,Lastname,DL_City__c,DL_Mailing_Address__c,DL_Postal_Code__c,DL_Province__c, Condition_Treatment__c,DL_Office_Phone__c, DL_Office_Fax__c,DL_Location__Latitude__s,DL_Location__Longitude__s,Wait_Time__c,Status_AGN__c,Specialty_1_AGN__r.Name,Treatments_to_Manage_Headache_AGN__c FROM Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes' AND Status_AGN__c='Active' and  DISTANCE(DL_Location__c , GEOLOCATION(" + latitude.ToString(CultureInfo.InvariantCulture) + "," + longitude.ToString(CultureInfo.InvariantCulture) + "), 'km') < " + radius + " AND Areas_of_Interest__c INCLUDES ('Headache/Migraine Management')");

                string queryText = "select Id,Name,Firstname,Lastname,DL_City__c,DL_Mailing_Address__c,DL_Postal_Code__c,DL_Province__c, Condition_Treatment__c,DL_Office_Phone__c, DL_Office_Fax__c,DL_Location__Latitude__s,DL_Location__Longitude__s,Wait_Time__c FROM Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes' AND  DISTANCE(DL_Location__c , GEOLOCATION(" + latitude.ToString(CultureInfo.InvariantCulture) + "," + longitude.ToString(CultureInfo.InvariantCulture) + "), 'km') < " + radius + " AND Areas_of_Interest__c INCLUDES ('Headache/Migraine Management')";

                string query = System.Web.HttpUtility.UrlEncode(queryText);


                Debug.WriteLine(queryText);

                //string query = System.Web.HttpUtility.UrlEncode("select Id,Name,Firstname,Lastname,DL_City__c,DL_Mailing_Address__c,DL_Postal_Code__c,DL_Province__c, Condition_Treatment__c,DL_Office_Phone__c, DL_Office_Fax__c,DL_Location__Latitude__s,DL_Location__Longitude__s,Wait_Time__c,Status_AGN__c,Specialty_1_AGN__r.Name,Treatments_to_Manage_Headache_AGN__c FROM Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes' AND Status_AGN__c='Active'");


                providerListJson = HttpGetJson(InstanceUrl + @"/services/data/v26.0/query?q=" + query, "", AccessToken);

                Debug.WriteLine(providerListJson);

            }
            else
            {
                string query = System.Web.HttpUtility.UrlEncode("select Id,Name,Specialty_1_AGN__r.Name,Firstname,Lastname,DL_City__c,DL_Mailing_Address__c,DL_Postal_Code__c,DL_Province__c, Condition_Treatment__c,DL_Office_Phone__c, DL_Office_Fax__c,DL_Location__Latitude__s,DL_Location__Longitude__s,Wait_Time__c,Status_AGN__c,Treatments_to_Manage_Headache_AGN__c,Specialty_1_AGN__r.Name FROM Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes' and AND Status_AGN__c='Active' DISTANCE(DL_Location__c , GEOLOCATION(" + latitude.ToString(CultureInfo.InvariantCulture) + "," + longitude.ToString(CultureInfo.InvariantCulture) + "), 'km') < " + radius.ToString(CultureInfo.InvariantCulture) + " AND Areas_of_Interest__c INCLUDES ('Headache/Migraine Management') " + GetWaitTimeFilter(waitTime) + "");

                providerListJson =
                    HttpGetJson(
                        InstanceUrl + @"/services/data/v26.0/query?q=" + query, "", AccessToken);

            }

            Debug.WriteLine(providerListJson);

            // Convert the JSON response into a  object
            JsonResponseDTO<ProviderDetailDTO> providerList =
            JsonSerializer.Deserialize<JsonResponseDTO<ProviderDetailDTO>>(providerListJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


            //update the distance field
            if (providerList != null)
            {
                Haversine hv = new Haversine();

                foreach (var rec in providerList.records)
                {
                    Position pos1 = new Position();
                    pos1.Latitude = double.Parse(rec.DL_Location__Latitude__s, CultureInfo.InvariantCulture);
                    pos1.Longitude = double.Parse(rec.DL_Location__Longitude__s, CultureInfo.InvariantCulture);

                    Position pos2 = new Position();
                    pos2.Latitude = latitude;
                    pos2.Longitude = longitude;

                    double result = hv.Distance(pos1, pos2, DistanceType.Kilometers);
                    rec.Distance = Math.Round(result, 2).ToString();
                }

            }

            return providerList;
            //}
            //catch (Exception e)
            //{
            //    string URI = "ERROR URL : '" + InstanceUrl + @"/services/data/v26.0/query?q=select Id,Name,Firstname,Lastname,DL_City__c,DL_Mailing_Address__c,DL_Postal_Code__c,DL_Province__c, Condition_Treatment__c,DL_Office_Phone__c, DL_Office_Fax__c,DL_Location__Latitude__s,DL_Location__Longitude__s,Status_AGN__c FROM Account WHERE AGREE_TO_PARTICIPATE__C = 'Yes' AND Status_AGN__c='Active' and  DISTANCE(DL_Location__c , GEOLOCATION(" +
            //                latitude + "," + longitude + "), 'km') < " + radius + " AND Areas_of_Interest__c INCLUDES ('Headache/Migraine Management')'";
            //    string error = "<br/><br/>" + e.Message + "<br/><br/>" + e.InnerException + "<br/><br/>" + e.StackTrace;
            //    ActualError = URI + error + ActualError;
            //    Debug.WriteLine(ActualError);
            //    //SendErrorEmail(ActualError);
            //    //throw;
            //    return null;
            //}
        }

        #endregion

        #region UtilityFunctions

        private string ChangeProvinceText(string ProvinceCode)
        {
            string result = null;

            SFUtils.EnumProvince province = (SFUtils.EnumProvince)Enum.Parse(typeof(SFUtils.EnumProvince), ProvinceCode);
            switch (province)
            {
                case SFUtils.EnumProvince.AB:
                    result = this.ConvertEnumtoString(SFUtils.EnumProvince.AB);
                    break;
                case SFUtils.EnumProvince.BC:
                    result = ConvertEnumtoString(SFUtils.EnumProvince.BC);
                    break;
                case SFUtils.EnumProvince.MB:
                    result = ConvertEnumtoString(SFUtils.EnumProvince.MB);
                    break;
                case SFUtils.EnumProvince.NB:
                    result = ConvertEnumtoString(SFUtils.EnumProvince.NB);
                    break;
                case SFUtils.EnumProvince.NL:
                    result = ConvertEnumtoString(SFUtils.EnumProvince.NL);
                    break;
                case SFUtils.EnumProvince.NS:
                    result = ConvertEnumtoString(SFUtils.EnumProvince.NS);
                    break;
                case SFUtils.EnumProvince.ON:
                    result = ConvertEnumtoString(SFUtils.EnumProvince.ON);
                    break;
                case SFUtils.EnumProvince.PE:
                    result = ConvertEnumtoString(SFUtils.EnumProvince.PE);
                    break;
                case SFUtils.EnumProvince.QC:
                case SFUtils.EnumProvince.PQ:
                    result = ConvertEnumtoString(SFUtils.EnumProvince.QC);
                    break;
                case SFUtils.EnumProvince.SK:
                    result = ConvertEnumtoString(SFUtils.EnumProvince.SK);
                    break;
                default:
                    result = this.ConvertEnumtoStringFrench(SFUtils.EnumProvince.AB);
                    break;

            }
            return result;
        }

        private string ChangeProvinceTextFrench(string ProvinceCode)
        {
            string result = null;

            SFUtils.EnumProvince province = (SFUtils.EnumProvince)Enum.Parse(typeof(SFUtils.EnumProvince), ProvinceCode);
            switch (province)
            {
                case SFUtils.EnumProvince.AB:
                    result = this.ConvertEnumtoStringFrench(SFUtils.EnumProvince.AB);
                    break;
                case SFUtils.EnumProvince.BC:
                    result = ConvertEnumtoStringFrench(SFUtils.EnumProvince.BC);
                    break;
                case SFUtils.EnumProvince.MB:
                    result = ConvertEnumtoStringFrench(SFUtils.EnumProvince.MB);
                    break;
                case SFUtils.EnumProvince.NB:
                    result = ConvertEnumtoStringFrench(SFUtils.EnumProvince.NB);
                    break;
                case SFUtils.EnumProvince.NL:
                    result = ConvertEnumtoStringFrench(SFUtils.EnumProvince.NL);
                    break;
                case SFUtils.EnumProvince.NS:
                    result = ConvertEnumtoStringFrench(SFUtils.EnumProvince.NS);
                    break;
                case SFUtils.EnumProvince.ON:
                    result = ConvertEnumtoStringFrench(SFUtils.EnumProvince.ON);
                    break;
                case SFUtils.EnumProvince.PE:
                    result = ConvertEnumtoStringFrench(SFUtils.EnumProvince.PE);
                    break;
                case SFUtils.EnumProvince.QC:
                case SFUtils.EnumProvince.PQ:
                    result = ConvertEnumtoStringFrench(SFUtils.EnumProvince.QC);
                    break;
                case SFUtils.EnumProvince.SK:
                    result = ConvertEnumtoStringFrench(SFUtils.EnumProvince.SK);
                    break;
                case SFUtils.EnumProvince.NT:
                    result = ConvertEnumtoStringFrench(SFUtils.EnumProvince.NT);
                    break;
                case SFUtils.EnumProvince.NU:
                    result = ConvertEnumtoStringFrench(SFUtils.EnumProvince.NU);
                    break;
                case SFUtils.EnumProvince.YT:
                    result = ConvertEnumtoStringFrench(SFUtils.EnumProvince.YT);
                    break;
                default:
                    result = this.ConvertEnumtoStringFrench(SFUtils.EnumProvince.AB);
                    break;

            }
            return result;
        }

        public byte[] GetBinary(string InstanceUrl, string AccessToken, string documentUrl)
        {
            byte[] doc = null;
            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("Authorization: OAuth " + AccessToken);
                doc = client.DownloadData(InstanceUrl + documentUrl);

                return doc;
            }
            catch
            {
                return doc;
            }
        }



        public string HttpPostJson(string URI, string Parameters)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";

                // Add parameters to post
                byte[] data = System.Text.Encoding.ASCII.GetBytes(Parameters);
                req.ContentLength = data.Length;
                System.IO.Stream os = req.GetRequestStream();
                os.Write(data, 0, data.Length);
                os.Close();

                // Do the post and get the response.
                System.Net.WebResponse resp = req.GetResponse();
                if (resp == null) return null;
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            catch
            {
                return null;
            }
        }


        private string HttpGetJson(string URI, string Parameters, string AccessToken)
        {
            //try
            //{
            //SendErrorEmail(URI);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);

            req.Method = "GET";
            req.Headers.Add("X-PrettyPrint:1");
            req.ContentType = "application/xml";
            req.Headers.Add("Authorization: OAuth " + AccessToken);

            System.Net.WebResponse resp = req.GetResponse();
            if (resp == null) return null;
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string result = sr.ReadToEnd().Trim();

            result = result.Replace("\"Wait_Time__c\" : \"Less than 1 month\"", "\"Wait_Time__c\" : \"Less than 1 month\",\n \"Wait_time_Numeric__c\" : \"1\"");
            result = result.Replace("\"Wait_Time__c\" : \"Less than 3 months\"", "\"Wait_Time__c\" : \"Less than 3 months\",\n \"Wait_time_Numeric__c\" : \"3\"");
            result = result.Replace("\"Wait_Time__c\" : \"Less than 6 months\"", "\"Wait_Time__c\" : \"Less than 6 months\",\n \"Wait_time_Numeric__c\" : \"6\"");
            result = result.Replace("\"Wait_Time__c\" : \"Greater than 12 months\"", "\"Wait_Time__c\" : \"Greater than 12 months\",\n \"Wait_time_Numeric__c\" : \"12\"");
            result = result.Replace("\"Wait_Time__c\" : \"Unknown\"", "\"Wait_Time__c\" : \"Unknown\",\n \"Wait_time_Numeric__c\" : \"0\"");
            result = result.Replace("\"Wait_Time__c\" : \"\"", "\"Wait_Time__c\" : \"\",\n \"Wait_time_Numeric__c\" : \"0\"");
            return result;
            //}
            //catch
            //{
            //    return URI;
            //}

        }

        public string ConvertEnumtoString(EnumProvince enumVal)
        {
            string returnval = null;

            switch (enumVal)
            {
                case EnumProvince.AB:
                    returnval = "Alberta";
                    break;
                case EnumProvince.BC:
                    returnval = "British Columbia";
                    break;
                case EnumProvince.MB:
                    returnval = "Manitoba";
                    break;
                case EnumProvince.NB:
                    returnval = "New Brunswick";
                    break;
                case EnumProvince.NL:
                    returnval = "Newfoundland and Labrador";
                    break;
                case EnumProvince.NS:
                    returnval = "Nova Scotia";
                    break;
                case EnumProvince.ON:
                    returnval = "Ontario";
                    break;
                case EnumProvince.PE:
                    returnval = "Prince Edward Island";
                    break;
                case EnumProvince.QC:
                    returnval = "Quebec";
                    break;
                case EnumProvince.SK:
                    returnval = "Saskatchewan";
                    break;
                case EnumProvince.NT:
                    returnval = "Northwest Territories";
                    break;
                case EnumProvince.NU:
                    returnval = "Nunavut";
                    break;
                case EnumProvince.YT:
                    returnval = "Yukon";
                    break;

            }
            return returnval;

        }

        public string ConvertEnumtoStringFrench(EnumProvince enumVal)
        {
            string returnval = null;

            switch (enumVal)
            {
                case EnumProvince.AB:
                    returnval = "Alberta";
                    break;
                case EnumProvince.BC:
                    returnval = "Colombie Britannique";
                    break;
                case EnumProvince.MB:
                    returnval = "Manitoba";
                    break;
                case EnumProvince.NB:
                    returnval = "Nouveau-Brunswick";
                    break;
                case EnumProvince.NL:
                    returnval = "Terre-Neuve-et-Labrador";
                    break;
                case EnumProvince.NS:
                    returnval = "Nouvelle-Écosse";
                    break;
                case EnumProvince.ON:
                    returnval = "Ontario";
                    break;
                case EnumProvince.PE:
                    returnval = "Île-du-Prince-Édouard";
                    break;
                case EnumProvince.QC:
                    returnval = "Québec";
                    break;
                case EnumProvince.SK:
                    returnval = "Saskatchewan";
                    break;
                case EnumProvince.NT:
                    returnval = "(territoires du) Nord-Ouest";
                    break;
                case EnumProvince.NU:
                    returnval = "Nunavut";
                    break;
                case EnumProvince.YT:
                    returnval = "Yukon";
                    break;

            }
            return returnval;

        }


        public string ConvertConditionToEnglish(string frenchVal)
        {
            string englishVal = null;
            switch (frenchVal.ToLower())
            {
                case "migraine chronique":
                    englishVal = "Chronic Migraine";
                    break;
                case "blépharospasme et troubles du nerf VII":
                    englishVal = "Blepharospasm and VII Nerve Disorders";
                    break;
                case "strabisme":
                    englishVal = "Strabismus";
                    break;
                case "dystonie cervicale":
                    englishVal = "Cervical Dystonia";
                    break;
                case "spasticité focale":
                    englishVal = "Focal Spasticity";
                    break;
                case "pied bot équin":
                    englishVal = "Equinus Foot";
                    break;

            }
            return englishVal;
        }

        public string ConvertConditionToFrench(string englishVal)
        {
            string frenchVal = null;
            switch (englishVal.ToLower())
            {
                case "chronic migraine":
                case "Chronic Migraine":
                    frenchVal = "Migraine chronique";
                    break;
                case "Blepharospasm and VII Nerve Disorders":
                    frenchVal = "Blépharospasme et troubles du nerf VII";
                    break;
                case "strabismus":
                    frenchVal = "strabisme";
                    break;
                case "cervical dystonia":
                    frenchVal = "Dystonie cervicale";
                    break;
                case "focal spasticity":
                    frenchVal = "spasticité focale";
                    break;
                case "Equinus Foot":
                    frenchVal = "pied bot équin";
                    break;

            }
            return frenchVal;
        }

        public string ConvertWaitTimeToFrench(string englishVal)
        {
            string frenchVal = null;
            if (!String.IsNullOrEmpty(englishVal))
            {
                switch (englishVal.ToLower())
                {
                    case "less than 1 month":
                        frenchVal = "Moins de 1 mois";
                        break;
                    case "less than 3 months":
                        frenchVal = "Moins de 3 mois";
                        break;
                    case "less than 6 months":
                        frenchVal = "Moins de 6 mois";
                        break;
                    case "less than 12 months":
                        frenchVal = "Moins de 12 mois";
                        break;
                    case "more than 12 months":
                        frenchVal = "Plus de 12 mois";
                        break;
                }
            }
            return frenchVal;
        }

        public JsonResponseDTO<ProviderDetailDTO> ConvertResultsToFrench(JsonResponseDTO<ProviderDetailDTO> results)
        {
            foreach (ProviderDetailDTO result in results.records)
            {
                if (result.Wait_time_Numeric__c != null)
                {
                    result.Wait_time__c = this.ConvertWaitTimeToFrench(result.Wait_time__c);
                }

                if (result.Specialty != null)
                {
                    result.Condition_Treatment__c = this.ConvertConditionToFrench(result.Condition_Treatment__c);
                }
            }
            return results;
        }

        public string ConvertWaitTimeToFrench(int numericVal)
        {
            string frenchVal = null;

            switch (numericVal)
            {
                case 1:
                    frenchVal = "Moins de 1 mois";
                    break;
                case 3:
                    frenchVal = "Moins de 3 mois";
                    break;
                case 6:
                    frenchVal = "Moins de 6 mois";
                    break;
                case 12:
                    frenchVal = "Moins de 12 mois";
                    break;
                case 16:
                    frenchVal = "Plus de 12 mois";
                    break;
            }

            return frenchVal;


        }

        public List<string> ConvertSpecialtyToFrench(List<string> lstcnenglishVal)
        {
            List<string> Result = new List<string>();
            foreach (var englishVal in lstcnenglishVal)
            {
                if (!String.IsNullOrEmpty(englishVal))
                {
                    string addval = "";
                    switch (englishVal.ToLower())
                    {
                        case "anesthesiology":
                            addval = "Anesthésiologie";
                            break;
                        case "pain care":
                            addval = "Traitement de la douleur";
                            break;
                        case "respiratory medicine":
                            addval = "Médecine respiratoire";
                            break;
                        case "otolaryngology (ent)":
                            addval = "Oto-rhyno-laryngologie (ORL)";
                            break;
                        case "community medicine/public health":
                            addval = "Médecine communautaire/santé publique";
                            break;
                        case "anatomical pathology":
                            addval = "Anatomo-pathologie";
                            break;
                        case "dermatology":
                            addval = "Dermatologie";
                            break;
                        case "diagnostic radiology":
                            addval = "Radiologie diagnostique";
                            break;
                        case "emergency medicine":
                            addval = "Urgentologie";
                            break;
                        case "family medicine":
                            addval = "Médecine familiale";
                            break;
                        case "general practice":
                            addval = "Omnipratique";
                            break;
                        case "internal medicine":
                            addval = "Médecine interne";
                            break;
                        case "neurology":
                            addval = "Neurologie";
                            break;
                        case "neurosurgery":
                            addval = "Neurochirurgie";
                            break;
                        case "ophthalmology":
                            addval = "Ophtalmologie";
                            break;
                        case "orthopedic surgery":
                            addval = "Chirurgie orthopédique";
                            break;
                        case "pediatrics":
                            addval = "Pédiatrie";
                            break;
                        case "physical medicine & rehabilitation":
                            addval = "Physiatrie et réadaptation";
                            break;
                        case "thoracic surgery":
                            addval = "Chirurgie thoracique";
                            break;
                        case "nurse practictioner":
                            addval = "Infirmier praticien/infirmière praticienne";
                            break;
                    }
                    Result.Add(addval);
                }
            }

            return Result;
        }

        public bool CheckListEnglish(string Engval)
        {
            bool returnval = false;
            string[] values = {"anesthesiology","pain care","respiratory medicine",
        "otolaryngology (ent)","community medicine/public health","anatomical pathology","dermatology",
        "diagnostic radiology","emergency medicine","family medicine","general practice",
        "internal medicine","neurology","neurosurgery","ophthalmology",
        "orthopedic surgery","pediatrics","physical medicine & rehabilitation",
        "thoracic surgery","nurse practictioner",
};
            returnval = Array.Exists(values, element => element == Engval.ToLower());
            return returnval;
        }

        public List<string> ConvertTreatmentToFrench(List<string> lstcnenglishVal)
        {
            List<string> Result = new List<string>();
            foreach (var englishVal in lstcnenglishVal)
            {
                if (!String.IsNullOrEmpty(englishVal))
                {
                    string addval = englishVal;
                    switch (englishVal.ToLower())
                    {
                        case "treatments offered":
                            addval = "Traitements offerts";
                            break;
                        case "physician-administered injectables":
                        case "physician administered injectables":
                            addval = "Injections administrées par un médecin";
                            break;

                        case "patient-administered injectables":
                        case "patient administered injectables":
                            addval = "Injections administrées par le patient";
                            break;
                        case "orals":
                            addval = "Médicaments oraux";
                            break;
                    }
                    Result.Add(addval);
                }
            }
            return Result;
        }

        /// <summary>
        /// send email for error!
        /// </summary>
        /// <param name="From"></param>
        /// <param name="To"></param>
        /// <param name="Subject"></param>
        /// <param name="Body"></param>

        #endregion
    }

}