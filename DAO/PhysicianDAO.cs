namespace BotoxInjectorSite.DAO
{
    public class PhysicianDAO : HCPDAO
    {
        private Dictionary<string, Province> _provinces;

        public PhysicianDAO()
        {
            _provinces = new Dictionary<string, Province>
            {
                { "newfoundland", new Province("newfoundland", @"^[a-zA-Z]\d{5}$|^0\d{4}$") },
                { "nova_scotia", new Province("nova_scotia", @"^\d{6}$") },
                { "prince_edward_island", new Province("prince_edward_island", @"^\d{3}$|^\d{4}$|^[a-zA-Z]\d{4}$|^[a-zA-Z]{2}\d{4}$") },
                { "new_brunswick", new Province("new_brunswick", @"^\d{4}$|^\d{5}$|^\d{2}-\d{4,5}$") },
                { "quebec", new Province("quebec", @"^\d{5}$") },
                { "ontario", new Province("ontario", @"^\d{5}$|^\d{6}$") },
                { "manitoba", new Province("manitoba", @"^\d{2}-\d{3}$") },
                { "saskatchewan", new Province("saskatchewan", @"^\d{4}$|^\d{6}$") },
                { "alberta", new Province("alberta", @"^0\d{5}$") },
                { "british_columbia", new Province("british_columbia", @"^[1-9]{1,5}$") },
                { "northwest_territories", new Province("northwest_territories", @"^2500-\d{5}\d{4}$|^2502-\d{5}\d{4}$") },
                { "nunavut", new Province("nunavut", @"^\d{6}$") },
                { "yukon", new Province("yukon", @"^\d{4}-\d-\d{3}$") }
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