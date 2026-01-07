using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataDisplayConnection.models;
using DataDisplayConnection.Services;
using System.Text;

namespace DataDisplayConnection.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CvParserController : ControllerBase
    {
        private readonly CvParserService _parserService;
        private readonly string _cvStoragePath;

        public CvParserController(CvParserService parserService, IWebHostEnvironment env)
        {
            _parserService = parserService;
            _cvStoragePath = Path.Combine(env.ContentRootPath, "UploadedCVs");
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(_cvStoragePath))
            {
                Directory.CreateDirectory(_cvStoragePath);
            }
        }

        /// <summary>
        /// Parse CV from text input
        /// </summary>
        [HttpPost("parse-text")]
        public ActionResult<CvParseResult> ParseCvText([FromBody] CvUploadRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CvText))
            {
                return BadRequest(new { error = "CV text is required" });
            }

            try
            {
                var result = _parserService.ParseCvText(request.CvText);
                
                // Optionally save the text to a file
                if (!string.IsNullOrEmpty(request.FileName))
                {
                    var fileName = $"{SanitizeFileName(request.FileName)}_{DateTime.Now:yyyyMMddHHmmss}.txt";
                    var filePath = Path.Combine(_cvStoragePath, fileName);
                    System.IO.File.WriteAllText(filePath, request.CvText);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error parsing CV", details = ex.Message });
            }
        }

        /// <summary>
        /// Parse CV from uploaded file (PDF, DOCX, TXT)
        /// </summary>
        [HttpPost("parse-file")]
        public async Task<ActionResult<CvParseResult>> ParseCvFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded" });
            }

            // Validate file type
            var allowedExtensions = new[] { ".pdf", ".docx", ".doc", ".txt" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { error = "Only PDF, DOCX, DOC, and TXT files are allowed" });
            }

            try
            {
                // Save the file
                var fileName = $"{SanitizeFileName(Path.GetFileNameWithoutExtension(file.FileName))}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(_cvStoragePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Extract text from file
                string cvText = await ExtractTextFromFile(filePath, fileExtension);

                // Parse the text
                var result = _parserService.ParseCvText(cvText);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error parsing CV file", details = ex.Message });
            }
        }

        /// <summary>
        /// Get example CV parse result
        /// </summary>
        [HttpGet("example")]
        public ActionResult<CvParseResult> GetExample()
        {
            var example = new CvParseResult
            {
                FullName = "John Smith",
                Email = "john.smith@example.com",
                Phone = "+1-234-567-8900",
                Skills = new List<string> { "Flutter", ".NET", "SQL", "React", "Node.js" },
                ExperienceYears = 5,
                Education = "Bachelor of Science in Computer Science",
                Certifications = new List<string> { "AWS Certified Developer", "Microsoft Certified: Azure Developer" },
                Languages = new List<string> { "English", "Spanish" },
                CvSaved = true
            };

            return Ok(example);
        }

        private string SanitizeFileName(string fileName)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var sanitized = string.Join("_", fileName.Split(invalid, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
            return sanitized;
        }

        private async Task<string> ExtractTextFromFile(string filePath, string extension)
        {
            switch (extension)
            {
                case ".txt":
                    return await System.IO.File.ReadAllTextAsync(filePath);

                case ".pdf":
                    // For PDF extraction, you would need a library like iTextSharp or PdfPig
                    // For now, return a message
                    return "PDF text extraction requires additional library (iTextSharp/PdfPig). Please use text upload for now.";

                case ".docx":
                case ".doc":
                    // For DOCX extraction, you would need DocumentFormat.OpenXml or similar
                    // For now, return a message
                    return "DOCX text extraction requires additional library (DocumentFormat.OpenXml). Please use text upload for now.";

                default:
                    throw new NotSupportedException($"File type {extension} is not supported");
            }
        }
    }
}
