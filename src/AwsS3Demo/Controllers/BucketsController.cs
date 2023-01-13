using Amazon.S3;
using Microsoft.AspNetCore.Mvc;

namespace AwsS3Demo.Controllers;

// This is more of a POC than fully-fletched controller to maintain buckets on S#

[Route("api/buckets")]
[ApiController]
public class BucketsController : ControllerBase
{
    private readonly IAmazonS3 s3Client;

    public BucketsController(IAmazonS3 s3Client)
    {
        this.s3Client = s3Client;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateBucketAsync(string bucketName)
    {
        var doesBucketExists = await s3Client.DoesS3BucketExistAsync(bucketName);

        if (doesBucketExists)
        {
            return BadRequest($"Bucket {bucketName} already exists");
        }

        // TODO: check put bucket response
        var putBucketResponse = await s3Client.PutBucketAsync(bucketName);

        return Ok(putBucketResponse);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllBucketsAsync()
    {
        var listBucketsResponse = await s3Client.ListBucketsAsync();
        var bucketNames = listBucketsResponse.Buckets
            .Select(b => b.BucketName);

        return Ok(bucketNames);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteBucketAsync(string bucketName)
    {
        await s3Client.DeleteBucketAsync(bucketName);

        return NoContent();
    }
}