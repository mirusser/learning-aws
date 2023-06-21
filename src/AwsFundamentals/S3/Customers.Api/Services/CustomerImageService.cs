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

    public Task<DeleteObjectResponse> DeleteImageAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<GetObjectResponse> GetImageResponse(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<PutObjectResponse> UploadImageAsync(Guid id, IFormFile formFile)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = $"image/{id}",
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