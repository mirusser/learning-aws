﻿using Amazon.S3.Model;

namespace Customers.Api.Services;

public interface ICustomerImageService
{
    Task<PutObjectResponse> UploadImageAsync(Guid id, IFormFile formFile);
    Task<GetObjectResponse> GetImageAsync(Guid id);
    Task<DeleteObjectResponse> DeleteImageAsync(Guid id);
}