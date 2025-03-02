using System;
using System.IO;
using System.Threading.Tasks;

namespace ERP.Service
{
    public class TestUpload
    {
        private readonly CloudStorageService _cloudStorageService;

        public TestUpload(CloudStorageService cloudStorageService)
        {
            _cloudStorageService = cloudStorageService;
        }

        public void RunTest()
        {
            using (var stream = new MemoryStream())
            {
                // Simulate a PDF file upload
                var writer = new StreamWriter(stream);
                writer.Write("This is a test PDF file.");
                writer.Flush();
                stream.Position = 0;

                // Commenting out the upload logic
                // string result = await _cloudStorageService.UploadToCloudStorageAsync(stream, fileName);
                // Console.WriteLine($"File uploaded to: {result}");
            }
        }
    }
}
