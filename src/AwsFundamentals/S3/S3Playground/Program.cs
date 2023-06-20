using System.Text;
using Amazon.S3;
using Amazon.S3.Model;

await SimpleUpload();
await SimpleDownload();

Console.ReadLine();

static async Task SimpleDownload()
{
    var s3Client = new AmazonS3Client();

    var getObjectRequest = new GetObjectRequest
    {
        BucketName = "awscoursefromnick",
        Key = "files/movies.csv",
    };

    var response = await s3Client.GetObjectAsync(getObjectRequest);

    await using var memoryStream = new MemoryStream();
    response.ResponseStream.CopyTo(memoryStream);

    var text = Encoding.Default.GetString(memoryStream.ToArray());

    //Console.WriteLine(text);
    await Console.Out.WriteLineAsync(text);
}

static async Task SimpleUpload()
{
    var s3Client = new AmazonS3Client();

    await using var inputStream = new FileStream("./movies.csv", FileMode.Open, FileAccess.Read);

    var putObjectRequest = new PutObjectRequest
    {
        BucketName = "awscoursefromnick",
        Key = "files/movies.csv",
        ContentType = "text/csv",
        InputStream = inputStream
    };

    await s3Client.PutObjectAsync(putObjectRequest);
}