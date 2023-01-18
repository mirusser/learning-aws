using Microsoft.AspNetCore.Mvc;

namespace AWSCloudwatch.Serilog.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HelloController : ControllerBase
{
    private readonly ILogger<HelloController> logger;

    public HelloController(ILogger<HelloController> logger)
    {
        this.logger = logger;
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        logger.LogDebug("Received request with is as {id}", id);
        logger.LogInformation("Processing your request");
        logger.LogError("Some errors occured");

        return Ok("Logged");
    }
}