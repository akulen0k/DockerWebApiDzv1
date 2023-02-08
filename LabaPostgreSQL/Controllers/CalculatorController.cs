using System.Text.Json.Serialization;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Npgsql;

namespace LabaPostgreSQL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalculatorController : ControllerBase
{
    private readonly ILogger<CalculatorController> _logger;
    private readonly string _connectionSTring;

    [Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
    public enum operation
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
        _connectionSTring = postgresOptions.Value.ConnectionString;
        using (var con = new NpgsqlConnection(_connectionSTring))
        {
            con.Open();
            var command = new NpgsqlCommand(@"CREATE TABLE IF NOT EXISTS opers(
                        id SERIAL PRIMARY KEY,
                        handle TEXT NOT NULL,
                        data TEXT NOT NULL
                        )", con);
            command.ExecuteNonQuery();
        }
    }

    [HttpGet("calc/{op}/{fi}/{si}")]
    public async Task<IResult> Get(operation op, double fi, double si)
    {
        if (op == operation.Division && si == 0)
        {
            string resp = "Divide by zero ;(";
            WriteToDb(new QueryInfo($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/Calculator/calc/" +
                                    $"{op.ToString()}/{fi:#.###}/{si:#.###}",
                                JsonConvert.SerializeObject(new {
                                    response = resp,
                                    status_code = 200})));
            return Results.BadRequest(resp);
        }

        double ans = op switch
        {
            operation.Addition => fi + si,
            operation.Substraction => fi - si,
            operation.Multiplication => fi * si,
            operation.Division => fi / si
        };
        
        // как прочитать request body?
        // HttpContext.Current - удален
        WriteToDb(new QueryInfo($"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/Calculator/calc/" +
                                $"{op.ToString()}/{fi:#.###}/{si:#.###}",
            JsonConvert.SerializeObject(new {
                                        response = $"{ans:#.###}",
                                        status_code = 200})));
        return Results.Ok(ans);
    }
    
    [HttpGet("test")]
    private async void WriteToDb(QueryInfo qi)
    {
        using (var con = new NpgsqlConnection(_connectionSTring))
        {
            con.Open();
            var command = new NpgsqlCommand(@"INSERT INTO opers (handle, data) VALUES (@han, @dat)", con);
            command.Parameters.AddWithValue("han", qi.handle);
            command.Parameters.AddWithValue("dat", qi.data);
            
            command.ExecuteNonQuery();
        }
    }

    [HttpGet("getall")]
    public async Task<IResult> GetAll()
    {
        using (var con = new NpgsqlConnection(_connectionSTring))
        {
            con.Open();
            var com = @"SELECT * FROM opers";
            var ls = await con.QueryAsync<QueryInfo>(com);

            return Results.Ok(ls.ToArray());
        }
    }
}