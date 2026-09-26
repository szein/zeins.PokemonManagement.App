using System.CodeDom.Compiler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<HealthController> _logger;
    public HealthController(AppDbContext context, IWebHostEnvironment env, ILogger<HealthController> logger)
    {
        _context = context;
        _env = env;
        _logger = logger;
    }
    public string Get()
    {
        _logger?.LogInformation("Health endpoint called");
        return "I'm Alive!";
    }

    [HttpGet("db")]
    public string GetDb()
    {
        _logger?.LogInformation("Health DB check called");
        return _context.Database?.GetConnectionString() ?? "NOT CONNECTED";
    }

    [HttpGet("env")]
    public string GetEnv()
    {
        _logger?.LogInformation("Health ENV check called");
        return _env?.EnvironmentName ?? "Environement Name is Empty!";
    }
}
