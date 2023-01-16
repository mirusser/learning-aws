using Amazon.DynamoDBv2.DataModel;
using DynamoDBDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace DynamoDBDemo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly IDynamoDBContext dBContext;

    public StudentController(IDynamoDBContext dBContext)
    {
        this.dBContext = dBContext;
    }

    [HttpGet("{studentId}")]
    public async Task<IActionResult> GetById(int studentId)
    {
        var student = await dBContext.LoadAsync<Student>(studentId);
        if (student is null)
        {
            return NotFound();
        }

        return Ok(student);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStudents()
    {
        var students = await dBContext
            .ScanAsync<Student>(default)
            .GetRemainingAsync();

        return Ok(students);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent(Student studentRequest)
    {
        var student = await dBContext.LoadAsync<Student>(studentRequest.Id);
        if (student is not null)
        {
            return BadRequest($"Student with Id: {studentRequest.Id} already exists");
        }

        await dBContext.SaveAsync(studentRequest);
        return Ok(studentRequest);
    }

    [HttpDelete("{studentId}")]
    public async Task<IActionResult> DeleteStudent(int studentId)
    {
        var student = await dBContext.LoadAsync<Student>(studentId);
        if (student is null)
        {
            return NotFound();
        }

        await dBContext.DeleteAsync(student);

        return NoContent();
    }

    [HttpPut]
    public async Task<IActionResult> UpdateStudent(Student studentRequest)
    {
        var student = await dBContext.LoadAsync<Student>(studentRequest.Id);
        if (student is null)
        {
            return NotFound();
        }

        await dBContext.SaveAsync(studentRequest);

        return Ok(studentRequest);
    }
}