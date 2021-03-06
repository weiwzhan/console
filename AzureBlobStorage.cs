using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


namespace PEngineModule.Logs
{
    public class AzureBlobStorage
    {
        public static async Task Blob()
        {
            Console.WriteLine("Azure Blob storage v12 - .NET quickstart sample\n");

            // Retrieve the connection string for use with the application. The storage
            // connection string is stored in an environment variable on the machine
            // running the application called AZURE_STORAGE_CONNECTION_STRING. If the
            // environment variable is created after the application is launched in a
            // console or with Visual Studio, the shell or application needs to be closed
            // and reloaded to take the environment variable into account.
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            connectionString = "DefaultEndpointsProtocol=https;AccountName=dockerlogstorage;AccountKey=41aKNW1IxpaHWBEFpq5YzOnEpXRFTxYKKg1QnHwYgqxWXRd2HQolO08dD02DpgRs9P+ktjOCtAnLW3u70SIwkw==;EndpointSuffix=core.windows.net";
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            //Create a unique name for the container
            string containerName = "penginelogsblobs-" + Guid.NewGuid().ToString();


            // Create the container and return a container client object
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
            // Create a local file in the ./data/ directory for uploading and downloading
            string localPath = "./data/";
            string fileName = "/quickstart-" + Guid.NewGuid().ToString() + ".txt";
            string localFilePath = Path.Combine(localPath, fileName);


            // Write text to the file
            await File.WriteAllTextAsync(localFilePath, "Hello, World!");

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(fileName);


            Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

            // Open the file and upload its data
            using (FileStream uploadFileStream = File.OpenRead(localFilePath))
            {
                await blobClient.UploadAsync(uploadFileStream);
            }


            Console.WriteLine("Listing blobs...");

            // List all blobs in the container
            foreach (BlobItem blobItem in containerClient.GetBlobs())
            {
                Console.WriteLine("\t" + blobItem.Name);
            }

            // Download the blob to a local file
            // Append the string "DOWNLOAD" before the .txt extension 
            // so you can compare the files in the data directory
            string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOAD.txt");

            Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);

            // Download the blob's contents and save it to a file
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
            {
                await download.Content.CopyToAsync(downloadFileStream);
            }

            // Clean up
            Console.Write("Press any key to begin clean up");
            Console.ReadLine();

            Console.WriteLine("Deleting blob container...");
            await containerClient.DeleteAsync();

            Console.WriteLine("Deleting the local source and downloaded files...");
            File.Delete(localFilePath);
            File.Delete(downloadFilePath);

            Console.WriteLine("Done");
        }
    }
}