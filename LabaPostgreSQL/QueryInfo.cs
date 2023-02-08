namespace LabaPostgreSQL;

public class QueryInfo
{
    public int id { get; set; }
    public string handle { get; set; }
    public string data { get; set; }

    public QueryInfo()
    {
        
    }
    public QueryInfo(string h, string d)
    {
        handle = h;
        data = d;
    }
}