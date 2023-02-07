using Microsoft.AspNetCore.Mvc;

namespace LabaPostgreSQL.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalculatorController : ControllerBase
{
    private readonly ILogger<CalculatorController> _logger;

    private static char[] operations = new[] { '+', '-', '*', ':' };

    public CalculatorController(ILogger<CalculatorController> logger)
    {
        _logger = logger;
    }

    [HttpGet("calc/{op}/{fi}/{si}")]
    public async Task<IResult> Get(char op, double fi, double si)
    {
        if (!operations.ToList().Contains(op))
            return Results.BadRequest($"Operation {op} wasnt found");

        if (op == ':' && si == 0)
            return Results.BadRequest($"Divide by zero ;(");

        double ans = op switch
        {
            '+' => fi + si,
            '-' => fi - si,
            '*' => fi * si,
            ':' => fi / si
        };
        return Results.Ok(ans);
    }
}