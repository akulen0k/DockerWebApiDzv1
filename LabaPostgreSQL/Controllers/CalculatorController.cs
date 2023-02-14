using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LabaPostgreSQL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalculatorController : ControllerBase
{
    private readonly ILogger<CalculatorController> _logger;
    private readonly string _connectionString;
    private const string DBNAME = "operations";
    
    [Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Operation
    {
        Addition,
        Substraction,
        Multiplication,
        Division
    }

    public CalculatorController(ILogger<CalculatorController> logger,
        IOptions<PostgresOptions> postgresOptions)
    {
        _logger = logger;
        _connectionString = postgresOptions.Value.ConnectionString;
        DbComs.CreateDatabase(_connectionString, DBNAME);
    }

    [HttpGet("calc/{op}/{fi:double}/{si:double}")]
    public async Task<IResult> Get(Operation op, double fi, double si)
    {
        if (op == Operation.Division && si == 0)
        {
            const string resp = "Divide by zero ;(";
            DbComs.WriteToDb(       // ok ?? 
                _connectionString,
                DBNAME,
                new QueryInfo(
                    $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/Calculator/calc/" +
                    $"{op.ToString()}/{fi:#.###}/{si:#.###}",
                    JsonConvert.SerializeObject(new
                    {
                        response = resp,
                        status_code = 200
                    })
                ));
            return Results.BadRequest(resp);
        }

        double ans = op switch
        {
            Operation.Addition => fi + si,
            Operation.Substraction => fi - si,
            Operation.Multiplication => fi * si,
            Operation.Division => fi / si, 
            _ => throw new NotSupportedException($"Operation {op} is not supported")
        };
        
        // как прочитать request body?
        // HttpContext.Current - удален
        DbComs.WriteToDb(_connectionString, DBNAME, new QueryInfo(
            $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/Calculator/calc/" +
            $"{op.ToString()}/{fi:#.###}/{si:#.###}",
            JsonConvert.SerializeObject(new
            {
                response = $"{ans:#.###}",
                status_code = 200
            })));
        return Results.Ok(ans);
    }
    

    [HttpGet("getall")]
    public async Task<IResult> GetAll()
    {
        return Results.Ok(await DbComs.GetAllQueries(_connectionString, DBNAME));
    }
}