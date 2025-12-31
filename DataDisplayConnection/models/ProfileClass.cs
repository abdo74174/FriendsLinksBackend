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
    }
}
