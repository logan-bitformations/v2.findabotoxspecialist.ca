namespace BotoxInjectorSite.DAO
{
    public class NursePractitionerDAO : HCPDAO
    {
        private Dictionary<string, Province> _provinces;

        public NursePractitionerDAO()
        {
            _provinces = new Dictionary<string, Province>
            {
                { "newfoundland", new Province("newfoundland", @"^0?\d{5}$") },
                { "nova_scotia", new Province("nova_scotia", @"^0?\d{5}$") },
                { "prince_edward_island", new Province("prince_edward_island", @"^0?\d{6}$") },
                { "new_brunswick", new Province("new_brunswick", @"^0?\d{6}$") },
                { "quebec", new Province("quebec", @"^(0?\d{6}|0?\d{9})$") },
                { "ontario", new Province("ontario", @"^(0?\d{6}|0?\d{8})$") },
                { "manitoba", new Province("manitoba", @"^0?\d{6}$") },
                { "saskatchewan", new Province("saskatchewan", @"^0?\d{6}$") },
                { "alberta", new Province("alberta", @"^0?\d{5,7}$") },
                { "british_columbia", new Province("british_columbia", @"^0?\d{8}$") },
                { "northwest_territories", new Province("northwest_territories", @"^0?\d{6}$") },
                { "nunavut", new Province("nunavut", @"^0?\d{6}$") },
                { "yukon", new Province("yukon", @"^0?\d{6}$") }
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