using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotoxInjectorSite.DAO
{
    /// <summary>
    /// Summary description for ProvinceDTO
    /// </summary>
    /// 

    public class JsonResponseDTO<T>
    {
        public int totalSize { get; set; }
        public List<T> records { get; set; }
    }

    public class ProviderDetailDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DL_City__c { get; set; }
        public string DL_Email__c { get; set; }
        public string DL_Mailing_Address__c { get; set; }
        public string DL_Postal_Code__c { get; set; }
        public string DL_Province__c { get; set; }
        public string Website { get; set; }
        public string Condition_Treatment__c { get; set; }
        public string DL_Office_Phone__c { get; set; }
        public string DL_Office_Fax__c { get; set; }
        public string Wait_time__c { get; set; }
        public string Wait_time_Numeric__c { get; set; }
        public string DL_Location__Latitude__s { get; set; }
        public string DL_Location__Longitude__s { get; set; }
        public string Distance { get; set; }

        public attributes Specialty_1_AGN__r { get; set; }
        public string Treatments_to_Manage_Headache_AGN__c { get; set; }

        public List<string> Specialty { get; set; }
        public List<string> Treatments_to_Manage_Headache { get; set; }
    }


    public class ProvinceDTO
    {

        public string DL_Province__c { get; set; }
        public string DL_ProvinceName { get; set; }


    }

    public class CityDTO
    {
        public string DL_City__c { get; set; }
        public string DL_Province__c { get; set; }
    }


    public class attributes
    {
        public string Name { get; set; }
    }

    public class AttachmentDTO
    {
        public string Name { get; set; }
        public string Body { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }

    public class TokenResponseDTO
    {

        public string id { get; set; }
        public string issued_at { get; set; }
        public string refresh_token { get; set; }
        public string instance_url { get; set; }
        public string signature { get; set; }
        public string access_token { get; set; }

    }
}