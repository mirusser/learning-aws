using Amazon.S3;
using Amazon.S3.Model;

await SimpleUpload();

static async Task SimpleUpload()
{
    var s3Client = new AmazonS3Client();

    await using var inputStream = new FileStream("./face.jpg", FileMode.Open, FileAccess.Read);

    var putObjectRequest = new PutObjectRequest
    {
        BucketName = "awscoursefromnick",
        Key = "images/face.jpg",
        ContentType = "image/jpeg",
        InputStream = inputStream
    };

    await s3Client.PutObjectAsync(putObjectRequest);
}