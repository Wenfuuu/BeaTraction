using BeaTraction.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;

namespace BeaTraction.Infrastructure.Services;

public class MinioService : IMinioService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;
    private readonly string _endpoint;
    private readonly bool _useSsl;

    public MinioService(IConfiguration configuration)
    {
        var endpoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT") ?? "localhost:9000";
        var accessKey = Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY") ?? "minioadmin";
        var secretKey = Environment.GetEnvironmentVariable("MINIO_SECRET_KEY") ?? "minioadmin123";
        _bucketName = Environment.GetEnvironmentVariable("MINIO_BUCKET_NAME") ?? "bucket-images";
        _useSsl = bool.Parse(Environment.GetEnvironmentVariable("MINIO_USE_SSL") ?? "false");
        
        _endpoint = endpoint;

        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(_useSsl)
            .Build();
    }

    public async Task EnsureBucketExistsAsync()
    {
        try
        {
            var beArgs = new BucketExistsArgs()
                .WithBucket(_bucketName);
            
            var exists = await _minioClient.BucketExistsAsync(beArgs);
            
            if (!exists)
            {
                var mbArgs = new MakeBucketArgs()
                    .WithBucket(_bucketName);
                
                await _minioClient.MakeBucketAsync(mbArgs);
                
                var policy = $@"{{
                    ""Version"": ""2012-10-17"",
                    ""Statement"": [
                        {{
                            ""Effect"": ""Allow"",
                            ""Principal"": {{
                                ""AWS"": [""*""]
                            }},
                            ""Action"": [""s3:GetObject""],
                            ""Resource"": [""arn:aws:s3:::{_bucketName}/*""]
                        }}
                    ]
                }}";

                var policyArgs = new SetPolicyArgs()
                    .WithBucket(_bucketName)
                    .WithPolicy(policy);

                await _minioClient.SetPolicyAsync(policyArgs);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error ensuring bucket exists: {ex.Message}", ex);
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            var extension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(uniqueFileName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs);

            return uniqueFileName;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error uploading file: {ex.Message}", ex);
        }
    }

    public async Task<Stream> DownloadFileAsync(string fileName)
    {
        try
        {
            var memoryStream = new MemoryStream();
            
            var getObjectArgs = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithCallbackStream((stream) =>
                {
                    stream.CopyTo(memoryStream);
                });

            await _minioClient.GetObjectAsync(getObjectArgs);
            
            memoryStream.Position = 0;
            return memoryStream;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error downloading file: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteFileAsync(string fileName)
    {
        try
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName);

            await _minioClient.RemoveObjectAsync(removeObjectArgs);
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Task<string> GetFileUrlAsync(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return Task.FromResult(string.Empty);

        var protocol = _useSsl ? "https" : "http";
        var url = $"{protocol}://{_endpoint}/{_bucketName}/{fileName}";
        
        return Task.FromResult(url);
    }
}
