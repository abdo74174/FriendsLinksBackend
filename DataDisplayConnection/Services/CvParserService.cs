using System.Text.RegularExpressions;
using DataDisplayConnection.models;

namespace DataDisplayConnection.Services
{
    public class CvParserService
    {
        // Common skill keywords to look for
        private static readonly string[] SkillKeywords = new[]
        {
            "Flutter", "Dart", "React", "Angular", "Vue", "Node.js", "Express", "NestJS",
            ".NET", "C#", "ASP.NET", "Java", "Spring", "Python", "Django", "Flask",
            "JavaScript", "TypeScript", "HTML", "CSS", "SQL", "MongoDB", "PostgreSQL",
            "MySQL", "Redis", "Docker", "Kubernetes", "AWS", "Azure", "GCP",
            "Git", "GitHub", "GitLab", "CI/CD", "Jenkins", "REST", "GraphQL",
            "Microservices", "Agile", "Scrum", "TDD", "Firebase", "Unity",
            "Android", "iOS", "Swift", "Kotlin", "React Native", "Xamarin",
            "PHP", "Laravel", "WordPress", "Ruby", "Rails", "Go", "Rust"
        };

        private static readonly string[] CertificationKeywords = new[]
        {
            "certified", "certification", "certificate", "AWS Certified", "Azure Certified",
            "Google Cloud", "Microsoft Certified", "Oracle Certified", "CompTIA",
            "CISSP", "PMP", "Scrum Master", "Professional"
        };

        private static readonly string[] LanguageKeywords = new[]
        {
            "English", "Arabic", "French", "Spanish", "German", "Chinese", "Japanese",
            "Russian", "Portuguese", "Italian", "Dutch", "Korean", "Hindi", "Turkish"
        };

        public CvParseResult ParseCvText(string cvText)
        {
            var result = new CvParseResult
            {
                CvSaved = true
            };

            if (string.IsNullOrWhiteSpace(cvText))
            {
                return result;
            }

            // Extract full name (usually at the top, first non-empty line with letters only or with spaces)
            result.FullName = ExtractName(cvText);

            // Extract email
            result.Email = ExtractEmail(cvText);

            // Extract phone
            result.Phone = ExtractPhone(cvText);

            // Extract skills
            result.Skills = ExtractSkills(cvText);

            // Extract experience years
            result.ExperienceYears = ExtractExperienceYears(cvText);

            // Extract education
            result.Education = ExtractEducation(cvText);

            // Extract certifications
            result.Certifications = ExtractCertifications(cvText);

            // Extract languages
            result.Languages = ExtractLanguages(cvText);

            return result;
        }

        private string ExtractName(string text)
        {
            // Look for name patterns at the beginning of the CV
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var line in lines.Take(10)) // Check first 10 lines
            {
                var trimmed = line.Trim();
                
                // Skip common headers
                if (trimmed.ToLower().Contains("curriculum") || 
                    trimmed.ToLower().Contains("resume") ||
                    trimmed.ToLower().Contains("cv") ||
                    trimmed.Length < 3)
                    continue;

                // Check if line looks like a name (2-4 words, mostly letters)
                var words = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (words.Length >= 2 && words.Length <= 4 && 
                    words.All(w => Regex.IsMatch(w, @"^[A-Za-z\.\-']+$")))
                {
                    return trimmed;
                }
            }

            return string.Empty;
        }

        private string ExtractEmail(string text)
        {
            var emailPattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b";
            var match = Regex.Match(text, emailPattern);
            return match.Success ? match.Value : string.Empty;
        }

        private string ExtractPhone(string text)
        {
            // Match various phone formats
            var patterns = new[]
            {
                @"\+?\d{1,4}[-.\s]?\(?\d{1,4}\)?[-.\s]?\d{1,4}[-.\s]?\d{1,9}",
                @"\(?\d{3}\)?[-.\s]?\d{3}[-.\s]?\d{4}",
                @"\d{10,15}"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(text, pattern);
                if (match.Success && match.Value.Replace("-", "").Replace(".", "").Replace(" ", "").Length >= 10)
                {
                    return match.Value.Trim();
                }
            }

            return string.Empty;
        }

        private List<string> ExtractSkills(string text)
        {
            var skills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var skill in SkillKeywords)
            {
                if (Regex.IsMatch(text, $@"\b{Regex.Escape(skill)}\b", RegexOptions.IgnoreCase))
                {
                    skills.Add(skill);
                }
            }

            return skills.ToList();
        }

        private int ExtractExperienceYears(string text)
        {
            // Look for patterns like "5 years", "5+ years", "3-5 years"
            var patterns = new[]
            {
                @"(\d+)\s*\+?\s*years?\s+(?:of\s+)?experience",
                @"experience[:\s]+(\d+)\s*\+?\s*years?",
                @"(\d+)\s*\+?\s*years?\s+in\s+(?:the\s+)?(?:industry|field|software|development)"
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int years))
                {
                    return years;
                }
            }

            // Try to estimate from date ranges (e.g., 2018-2023)
            var dateRanges = Regex.Matches(text, @"(20\d{2})\s*[-–]\s*(20\d{2}|Present|Current)", RegexOptions.IgnoreCase);
            var totalYears = 0;

            foreach (Match match in dateRanges)
            {
                if (int.TryParse(match.Groups[1].Value, out int startYear))
                {
                    var endYearStr = match.Groups[2].Value;
                    var endYear = endYearStr.ToLower().Contains("present") || endYearStr.ToLower().Contains("current")
                        ? DateTime.Now.Year
                        : int.TryParse(endYearStr, out int ey) ? ey : DateTime.Now.Year;

                    totalYears += Math.Max(0, endYear - startYear);
                }
            }

            return totalYears > 0 ? totalYears : 0;
        }

        private string ExtractEducation(string text)
        {
            var degrees = new[] { "Ph\\.?D", "PhD", "Doctorate", "Master", "M\\.?S", "M\\.?A", "MBA", 
                                  "Bachelor", "B\\.?S", "B\\.?A", "B\\.?Tech", "B\\.?E", "Associate" };

            foreach (var degree in degrees)
            {
                var pattern = $@"({degree})[\s\w,\.]*(?:in|of)?\s+[\w\s,]+";
                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                
                if (match.Success)
                {
                    // Get the matched sentence (up to 100 chars)
                    var education = match.Value.Length > 100 
                        ? match.Value.Substring(0, 100).Trim() 
                        : match.Value.Trim();
                    return education;
                }
            }

            return string.Empty;
        }

        private List<string> ExtractCertifications(string text)
        {
            var certifications = new List<string>();
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                foreach (var keyword in CertificationKeywords)
                {
                    if (Regex.IsMatch(line, $@"\b{Regex.Escape(keyword)}\b", RegexOptions.IgnoreCase))
                    {
                        var cleaned = line.Trim();
                        if (cleaned.Length > 10 && cleaned.Length < 150)
                        {
                            certifications.Add(cleaned);
                            break; // Only add once per line
                        }
                    }
                }
            }

            return certifications.Distinct().Take(5).ToList();
        }

        private List<string> ExtractLanguages(string text)
        {
            var languages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var language in LanguageKeywords)
            {
                if (Regex.IsMatch(text, $@"\b{Regex.Escape(language)}\b", RegexOptions.IgnoreCase))
                {
                    languages.Add(language);
                }
            }

            return languages.ToList();
        }
    }
}
