namespace DataDisplayConnection.models
{
    public class CvParseResult
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new List<string>();
        public int ExperienceYears { get; set; }
        public string Education { get; set; } = string.Empty;
        public List<string> Certifications { get; set; } = new List<string>();
        public List<string> Languages { get; set; } = new List<string>();
        public bool CvSaved { get; set; } = true;
    }

    public class CvUploadRequest
    {
        public string CvText { get; set; } = string.Empty;
        public string? FileName { get; set; }
    }
}
