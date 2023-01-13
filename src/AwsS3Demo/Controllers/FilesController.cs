using Amazon.S3;
using Amazon.S3.Model;
using AwsS3Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace AwsS3Demo.Controllers;

// Basic file management on s3

[Route("api/files")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IAmazonS3 s3Client;

    public FilesController(IAmazonS3 s3Client)
    {
        this.s3Client = s3Client;
    }

    [HttpPost("Upload")]
    public async Task<IActionResult> UploadFileAsync(
        IFormFile file,
        string bucketName,
        string? prefix)
    {
        var doesBucketExists = await s3Client.DoesS3BucketExistAsync(bucketName);

        if (!doesBucketExists)
        {
            return NotFound($"Bucket {bucketName} does not exist");
        }

        PutObjectRequest request = new()
        {
            BucketName = bucketName,
            InputStream = file.OpenReadStream(),
            Key = string.IsNullOrEmpty(prefix)
                ? file.FileName
                : $"{prefix.TrimEnd('/')}/{file.FileName}"
        };

        request.Metadata.Add("Content-Type", file.ContentType);
        var putObjectResponse = await s3Client.PutObjectAsync(request);

        return Ok(putObjectResponse);
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAllFilesAsync(string bucketName, string? prefix)
    {
        var doesBucketExists = await s3Client.DoesS3BucketExistAsync(bucketName);

        if (!doesBucketExists)
        {
            return NotFound($"Bucket {bucketName} does not exist");
        }

        ListObjectsV2Request request = new()
        {
            BucketName = bucketName,
            Prefix = prefix
        };

        var result = await s3Client.ListObjectsV2Async(request);
        var s3Objects = result.S3Objects
            .Select(s =>
            {
                GetPreSignedUrlRequest urlRequest = new()
                {
                    BucketName = bucketName,
                    Key = s.Key,
                    Expires = DateTime.UtcNow.AddMinutes(1)
                };

                return new S3ObjectDto() 
                {
                    Name = s.Key.ToString(),
                    PresignedUrl = s3Client.GetPreSignedURL(urlRequest)
                };
            });

        return Ok(s3Objects);
    }

    [HttpGet("get-by-key")]
    public async Task<IActionResult> GetFileByKeyAsync(string bucketName, string key)
    {
        var doesBucketExists = await s3Client.DoesS3BucketExistAsync(bucketName);

        if (!doesBucketExists)
        {
            return NotFound($"Bucket {bucketName} does not exist");
        }

        var s3Object = await s3Client.GetObjectAsync(bucketName, key);

        return File(s3Object.ResponseStream, s3Object.Headers.ContentType);
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> DeleteFileAsync(string bucketName, string key)
    {
        var doesBucketExists = await s3Client.DoesS3BucketExistAsync(bucketName);

        if (!doesBucketExists)
        {
            return NotFound($"Bucket {bucketName} does not exist");
        }

        var deleteResponse = await s3Client.DeleteObjectAsync(bucketName, key);

        return NoContent();
    }
}