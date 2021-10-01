using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace brons
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // connection string for Azure Storage
             string connectionString = Environment.GetEnvironmentVariable("CONNECT_STR");
            // connection string for Azurite 
            //string connectionString = "UseDevelopmentStorage=true";
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            
            string containerName = "image";
            var containers = blobServiceClient.GetBlobContainers();
            BlobContainerClient containerClient;
            if (containers.Count(c => c.Name == containerName) == 0)
            {
                // Create a new container if there is no one with this name
                containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName, PublicAccessType.Blob);
            }
            else
            {
                // Get containerClient if conteiner exists 
                containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            }

            // Choose a blob to upload
            var images = ImageNames(); // get all names of the images in the folder "data"
            //TODO: loop
            var selectedOption = ShowMenu("Select an image to upload it to the container", images);
            string fileName = images[selectedOption];
            string localPath = "./data/";
            string localFilePath = Path.Combine(localPath, fileName);

            // Upload blob to a container
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            try
            {
                using FileStream uploadFileStream = File.OpenRead(localFilePath);
                await blobClient.UploadAsync(uploadFileStream);
                uploadFileStream.Close();
                Console.WriteLine($"Uploading to Blob Storage:\n\t {blobClient.Uri}\n");
            }
            catch (Exception e) // for exampel - file with that name already exists
            {
                Console.WriteLine(e.Message);
            }
        }

        public static List<string> ImageNames()
        {
            List<string> names = new List<string>();

            foreach (var item in Directory.GetFiles($@".\data"))
            {
                int index = item.LastIndexOf('\\');
                names.Add(item.Substring(index + 1));
            }
            return names;
        }

        public static int ShowMenu(string prompt, List<string> options)
        {
            if (options == null || options.Count == 0)
            {
                throw new ArgumentException("Cannot show a menu for an empty array of options.");
            }

            Console.WriteLine(prompt);

            int selected = 0;

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                // If this is not the first iteration, move the cursor to the first line of the menu.
                if (key != null)
                {
                    Console.CursorLeft = 0;
                    Console.CursorTop = Console.CursorTop - options.Count;
                }

                // Print all the options, highlighting the selected one.
                for (int i = 0; i < options.Count; i++)
                {
                    var option = options[i];
                    if (i == selected)
                    {
                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine("- " + option);
                    Console.ResetColor();
                }

                // Read another key and adjust the selected value before looping to repeat all of this.
                key = Console.ReadKey().Key;
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Count - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }
            }

            // Reset the cursor and return the selected option.
            Console.CursorVisible = true;
            return selected;
        }
    }
}


