using Microsoft.AspNetCore.Mvc;
using System.IO;
using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using WLFSystem.Models;
using Emgu.CV.Dnn;
using WLFSystem.Controllers.Services;
using static Emgu.CV.Stitching.Stitcher;
using Azure;
using System.Linq;

namespace WLFSystem.Controllers
{

    [Route("api/")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWareHouseItemService _warehouseItemService;
        private readonly DataBaseContext _context;

        public UploadController(IWareHouseItemService wareHouseItemService, DataBaseContext context)
        {
            _warehouseItemService = wareHouseItemService;
            _context = context;
        }

        [HttpPost]
        [Route("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFile file, [FromForm] WareHouseItemViewModel item)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var filePath = Path.Combine(uploadsPath, item.Id + "_" + file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            if (item == null)
            {
                return BadRequest("Item data is required.");
            }

            try
            {

                var warehouseItem = new WareHouseItem()
                {
                    Id = item.Id,
                    Category = item.Category == "null" || string.IsNullOrEmpty(item.Category) ? "Unknown" : item.Category,
                    CreatedBy = "System",
                    CreatedDate = DateTime.Now,
                    FilePath = item.Id + "_" + file.FileName,
                    WarehouseLocation = "Atlanta",
                    Status = "Photo Captured",
                    Tags = String.Join(",", item.Tags)
                };
                
                // Add the new item to the context
                await _context.TblWareHouseItem.AddAsync(warehouseItem);

                // Save the changes to the database
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                // Log the exception if necessary and return an error response
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }



            return Ok(new { FilePath = filePath, Message = "File uploaded successfully." });
        }

        [HttpPost]
        [Route("search")]
        public async Task<IActionResult> Search(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "search");

            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            var filePath = Path.Combine(uploadsPath, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            string uploadedImagePath = "C:\\Prasad\\Dev\\WLFSystem\\WLFSystem\\search\\" + file.FileName;
            string folderPath = "C:\\Prasad\\Dev\\WLFSystem\\WLFSystem\\uploads";

            // Load the uploaded image
            var uploadedImage = new Image<Bgr, byte>(uploadedImagePath);
            var uploadedImageGray = uploadedImage.Convert<Gray, byte>();

            // Initialize ORB detector
            //var orb = new ORBDetector(500);
            ORB orb = new ORB(500);
            var uploadedKeypoints = new VectorOfKeyPoint();
            var uploadedDescriptors = new Mat();

            Mat descriptors1 = new Mat();
            Mat descriptors2 = new Mat();

            // Detect keypoints and compute descriptors for the uploaded image
            orb.DetectAndCompute(uploadedImageGray, null, uploadedKeypoints, uploadedDescriptors, false);

            List<string> filesMatched = new List<string>();

            // Iterate through all images in the specified folder
            foreach (var sourceFile in Directory.GetFiles(folderPath, "*.jpg"))
            {
                var candidateImage = new Image<Bgr, byte>(sourceFile);
                var candidateGray = candidateImage.Convert<Gray, byte>();

                // Detect keypoints and compute descriptors for the candidate image
                var candidateKeypoints = new VectorOfKeyPoint();
                var candidateDescriptors = new Mat();
                orb.DetectAndCompute(candidateGray, null, candidateKeypoints, candidateDescriptors, false);

                // Match descriptors using BFMatcher
                var matcher = new BFMatcher(DistanceType.Hamming);
                VectorOfDMatch matches = new VectorOfDMatch();
                matcher.Match(uploadedDescriptors, candidateDescriptors, matches);
                //var matches = matcher.Match(descriptors1, descriptors2);

                // Filter matches based on distance (similarity threshold)
                const double threshold = 45.0; // Adjust as needed
                var goodMatches = new List<MDMatch>();
                //var goodMatches = matches.Where(m => m.Distance < 30).ToList();
                foreach (var match in matches.ToArray())
                {
                    if (match.Distance < threshold)
                    {
                        goodMatches.Add(match);
                    }
                }

                // If enough good matches are found, consider it a match
                if (goodMatches.Count > 10) // Adjust the match count threshold as needed
                {
                    filesMatched.Add(sourceFile.Split("\\")[sourceFile.Split("\\").Length - 1]);
                    //Console.WriteLine($"Image '{sourceFile}' matches with the uploaded image.");
                }
            }

            return Ok(new { FilesMatched = filesMatched, Message = filesMatched.Count + " items found" });
        }

        [HttpGet("images/{imageName}")]
        public IActionResult GetImage(string imageName)
        {
            var imagePath = Path.Combine("uploads", imageName);
            if (System.IO.File.Exists(imagePath))
            {
                var image = System.IO.File.OpenRead(imagePath);
                return File(image, "image/jpeg");
            }
            return NotFound();
        }

        [HttpGet("images/search/{tag}")]
        public async Task<IActionResult> SearchImages(string tag)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                var wareHouseItems = _context.TblWareHouseItem.Where(x => x.Tags.Contains(tag))?.ToList();
               return Ok(wareHouseItems);
            }
            return NotFound();
        }

        [HttpGet("images")]
        public async Task<IActionResult> GetAllImages()
        {
            var wareHouseItems = _context.TblWareHouseItem.AsParallel<WareHouseItem>().ToList();
            return Ok(wareHouseItems);
        }
    }
}

