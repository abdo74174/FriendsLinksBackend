namespace DataDisplayConnection.models
{
    public class ProfileClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Linkedin { get; set; } = string.Empty;
        public string Github { get; set; } = string.Empty;
        public string Facebook { get; set; } = string.Empty;
        public string Portfolio { get; set; } = string.Empty;
        public string CvBase64 { get; set; } = string.Empty;
        
        // CV Parsed Data
        public string Phone { get; set; } = string.Empty;
        public string Skills { get; set; } = string.Empty; // JSON array as string
        public int ExperienceYears { get; set; } = 0;
        public string Education { get; set; } = string.Empty;
        public string Certifications { get; set; } = string.Empty; // JSON array as string
        public string Languages { get; set; } = string.Empty; // JSON array as string
    }
}
