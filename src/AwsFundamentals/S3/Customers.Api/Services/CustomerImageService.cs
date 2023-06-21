using Amazon.S3;
using Amazon.S3.Model;

namespace Customers.Api.Services;

public class CustomerImageService : ICustomerImageService
{
    private readonly IAmazonS3 s3;

    //for simplicity sake, but should be in configuration
    private readonly string bucketName = "awscoursefromnick";

    public CustomerImageService(IAmazonS3 s3)
    {
        this.s3 = s3;
    }

    public async Task<DeleteObjectResponse> DeleteImageAsync(Guid id)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = $"images/{id}"
        };

        return await s3.DeleteObjectAsync(deleteObjectRequest);
    }

    public async Task<GetObjectResponse> GetImageAsync(Guid id)
    {
        var getObjectRequest = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = $"images/{id}"
        };

        return await s3.GetObjectAsync(getObjectRequest);
    }

    public async Task<PutObjectResponse> UploadImageAsync(Guid id, IFormFile formFile)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = $"images/{id}",
            ContentType = formFile.ContentType,
            InputStream = formFile.OpenReadStream(),
            Metadata =
            {
                ["x-amz-meta-originalname"] = formFile.FileName,
                ["x-amz-meta-extension"] = Path.GetExtension(formFile.FileName)
            }
        };

        return await s3.PutObjectAsync(putObjectRequest);
    }
}