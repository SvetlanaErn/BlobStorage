using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace silver.Pages
{
    public class IndexModel : PageModel
    {
        public string containerName = "image";
        public Azure.Pageable<BlobItem> blobs;
        public Uri pathcontainer;

        public void OnGet()
        {
            // TODO: if container doesn't exist...
            // connection string for Azure Storage
            string connectionString = Environment.GetEnvironmentVariable("CONNECT_STR");
            // connection string for Azurite 
            // string connectionString = "UseDevelopmentStorage=true";
            
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            
            blobs = containerClient.GetBlobs();
            pathcontainer = containerClient.Uri;
        }
    }
}
