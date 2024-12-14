namespace BotoxInjectorSite.DAO
{
    public class PharmacistDAO : HCPDAO
    {
        private Dictionary<string, Province> _provinces;

        public PharmacistDAO()
        {
            _provinces = new Dictionary<string, Province>
            {
                { "newfoundland", new Province("newfoundland", @"^\d{2}-\d{3,4}$") },
                { "nova_scotia", new Province("nova_scotia", @"^\d{2,4}$") },
                { "prince_edward_island", new Province("prince_edward_island", @"^\d{4,5}$") },
                { "new_brunswick", new Province("new_brunswick", @"^\d{4}$") },
                { "quebec", new Province("quebec", @"^\d{4,6}$") },
                { "ontario", new Province("ontario", @"^\d{3,6}$") },
                { "manitoba", new Province("manitoba", @"^\d{5}$") },
                { "saskatchewan", new Province("saskatchewan", @"^[a-zA-Z]\d{4}$") },
                { "alberta", new Province("alberta", @"^\d{4,5}$") },
                { "british_columbia", new Province("british_columbia", @"^0\d{4}$") },
                { "northwest_territories", new Province("northwest_territories", @"^\d{4}-\d{5}$") },
                { "nunavut", new Province("nunavut", @"^1\d{3}-\d{5}$") },
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