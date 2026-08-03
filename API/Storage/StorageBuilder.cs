using Amazon.Runtime;
using Amazon.S3;
using KeepGrouped.API.Storage;
using Microsoft.Extensions.Options;

static class StorageBuilder
{
    public static void BuildStorage(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<StorageOptions>()
            .Bind(builder.Configuration.GetSection(StorageOptions.SectionName))
            .ValidateDataAnnotations().ValidateOnStart();
        builder.Services.AddScoped<IStorage, Garage>();
        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<StorageOptions>>().Value;
            var s3config = new AmazonS3Config()
            {
                ServiceURL = options.ServiceUrl,
                AuthenticationRegion = options.Region,
                ForcePathStyle = true,
                RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
            };

            var creds = new BasicAWSCredentials(options.AccessKey, options.SecretKey);
            return new AmazonS3Client(creds, s3config);
        });
    }
}