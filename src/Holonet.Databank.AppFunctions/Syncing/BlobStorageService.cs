using Azure.Storage.Blobs;
using System.Text.Json;

namespace Holonet.Databank.AppFunctions.Syncing;
public class BlobStorageService(string connectionString, string containerName)
{
    private readonly BlobServiceClient _blobServiceClient = new BlobServiceClient(connectionString);
    private readonly string _containerName = containerName;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public async Task<bool> UploadJsonObjectAsync<T>(T obj, string blobName)
    { 
        // Serialize the object to JSON
        string jsonString = JsonSerializer.Serialize(obj);

        // Get a reference to the blob container
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();

        // Get a reference to the blob
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        // Upload the JSON string to the blob
        using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString)))
        {
            try
            {
                var response = await blobClient.UploadAsync(stream, overwrite: true);
                return response.GetRawResponse().Status == 201; // Check if the status code is 201 (Created)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload failed: {ex.Message}");
                return false;
            }
        }
    }

    public async Task<bool> UploadJsonObjectAsync<T>(T obj, string folderName, string blobName)
    {
        // Serialize the object to JSON
        string jsonString = JsonSerializer.Serialize(obj, _jsonSerializerOptions);

        // Get a reference to the blob container
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();

        // Combine folder and blob name to create the blob path
        string blobPath = $"{folderName}/{blobName}";

        // Get a reference to the blob
        BlobClient blobClient = containerClient.GetBlobClient(blobPath);

        // Upload the JSON string to the blob
        using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString)))
        {
            try
            {
                var response = await blobClient.UploadAsync(stream, overwrite: true); 
                return response.GetRawResponse().Status == 201; // Check if the status code is 201 (Created)
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine($"Upload failed: {ex.Message}"); 
                return false; 
            }
        }
    }
}

