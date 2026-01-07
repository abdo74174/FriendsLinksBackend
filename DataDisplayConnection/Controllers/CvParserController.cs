using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataDisplayConnection.models;
using DataDisplayConnection.Services;
using System.Text;
using UglyToad.PdfPig;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

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
            
            if (!Directory.Exists(_cvStoragePath))
            {
                Directory.CreateDirectory(_cvStoragePath);
            }
        }

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

        [HttpPost("parse-file")]
        public async Task<ActionResult<CvParseResult>> ParseCvFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded" });
            }

            var allowedExtensions = new[] { ".pdf", ".docx", ".doc", ".txt" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { error = "Only PDF, DOCX, DOC, and TXT files are allowed" });
            }

            try
            {
                var fileName = $"{SanitizeFileName(Path.GetFileNameWithoutExtension(file.FileName))}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(_cvStoragePath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string cvText = await ExtractTextFromFile(filePath, fileExtension);

                var result = _parserService.ParseCvText(cvText);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error parsing CV file", details = ex.Message });
            }
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
                    var pdfText = new StringBuilder();
                    using (var document = PdfDocument.Open(filePath))
                    {
                        foreach (var page in document.GetPages())
                        {
                            pdfText.AppendLine(page.Text);
                        }
                    }
                    return pdfText.ToString();

                case ".docx":
                    var docxText = new StringBuilder();
                    using (var wordDocument = WordprocessingDocument.Open(filePath, false))
                    {
                        var body = wordDocument.MainDocumentPart?.Document.Body;
                        if (body != null)
                        {
                            foreach (var text in body.Descendants<Text>())
                            {
                                docxText.Append(text.Text);
                            }
                        }
                    }
                    return docxText.ToString();

                case ".doc":
                    return "Legacy .doc format not fully supported for text extraction. Please use .pdf or .docx.";

                default:
                    throw new NotSupportedException($"File type {extension} is not supported");
            }
        }
    }
}
