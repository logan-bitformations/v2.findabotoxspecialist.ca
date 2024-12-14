namespace BotoxInjectorSite.DAO
{
    public class NurseDAO : HCPDAO
    {
        private Dictionary<string, Province> _provinces;

        public NurseDAO()
        {
            _provinces = new Dictionary<string, Province>
            {
                { "newfoundland", new Province("newfoundland", @"^\d{4}$") },
                { "nova_scotia", new Province("nova_scotia", @"^\d{4,5}$") },
                { "prince_edward_island", new Province("prince_edward_island", @"^\d{6}$") },
                { "new_brunswick", new Province("new_brunswick", @"^\d{6}$") },
                { "quebec", new Province("quebec", @"^\d{6,7}$") },
                { "ontario", new Province("ontario", @"^([a-zA-Z]{2}\d{5,6}|\d{7})$") },
                { "manitoba", new Province("manitoba", @"^\d{6}$") },
                { "saskatchewan", new Province("saskatchewan", @"^\d{7}$") },
                { "alberta", new Province("alberta", @"^\d{4,5}$") },
                { "british_columbia", new Province("british_columbia", @"^0\d{6}$") },
                { "northwest_territories", new Province("northwest_territories", @"^\d{4}$") },
                { "nunavut", new Province("nunavut", @"^\d{4}$") },
                { "yukon", new Province("yukon", @"^\d{4}$") }
            };
        }

        public bool ValidateId(string provinceName, string id)
        {
            if (_provinces.ContainsKey(provinceName))
            {
                return _provinces[provinceName].ValidateId(id) && IsValidSequence(id);
            }
            return false;
        }
    }
}