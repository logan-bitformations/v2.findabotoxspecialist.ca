using System.Text.RegularExpressions;

public class Province
{
    public string Name { get; private set; }
    private string _idPattern;

    public Province(string name, string idPattern)
    {
        Name = name;
        _idPattern = idPattern;
    }

    public bool ValidateId(string id)
    {
        return Regex.IsMatch(id, _idPattern);
    }
}