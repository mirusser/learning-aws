using System.Net;
using Customers.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Customers.Api.Controllers;

[ApiController]
public class CustomerImageController : ControllerBase
{
    private readonly ICustomerImageService _customerImageService;

    public CustomerImageController(ICustomerImageService customerImageService)
    {
        _customerImageService = customerImageService;
    }

    [HttpPost("customers/{id:guid}/image")]
    public async Task<IActionResult> Upload(
        [FromRoute] Guid id,
        [FromForm(Name = "Data")] IFormFile file)
    {
        var response = await _customerImageService.UploadImageAsync(id, file);

        if (response.HttpStatusCode == HttpStatusCode.OK)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpGet("customers/{id:guid}/image")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("customers/{id:guid}/image")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        throw new NotImplementedException();
    }
}