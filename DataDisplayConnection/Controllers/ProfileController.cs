using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataDisplayConnection.data;
using DataDisplayConnection.models;

namespace DataDisplayConnection.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfileClass>>> GetProfiles()
        {
            // Exclude CvBase64 from the list to reduce payload size
            return await _context.Profiles
                .Select(p => new ProfileClass
                {
                    Id = p.Id,
                    Name = p.Name,
                    Email = p.Email,
                    Linkedin = p.Linkedin,
                    Github = p.Github,
                    Facebook = p.Facebook,
                    Portfolio = p.Portfolio,
                    Phone = p.Phone,
                    Skills = p.Skills,
                    ExperienceYears = p.ExperienceYears,
                    Education = p.Education,
                    Certifications = p.Certifications,
                    Languages = p.Languages,
                    CvBase64 = null // Don't return the large base64 string in the list
                })
                .ToListAsync();
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<ProfileClass>> GetProfile(string email)
        {
            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == email);

            if (profile == null)
            {
                return NotFound();
            }

            return profile;
        }

        [HttpPost]
        public async Task<ActionResult<ProfileClass>> PostProfile(ProfileClass profile)
        {
            var existingProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.Email == profile.Email);
            if (existingProfile != null)
            {
                // Update
                existingProfile.Name = profile.Name;
                existingProfile.Linkedin = profile.Linkedin;
                existingProfile.Github = profile.Github;
                existingProfile.Facebook = profile.Facebook;
                existingProfile.Portfolio = profile.Portfolio;
                existingProfile.CvBase64 = profile.CvBase64;
                existingProfile.Phone = profile.Phone;
                existingProfile.Skills = profile.Skills;
                existingProfile.ExperienceYears = profile.ExperienceYears;
                existingProfile.Education = profile.Education;
                existingProfile.Certifications = profile.Certifications;
                existingProfile.Languages = profile.Languages;
                
                await _context.SaveChangesAsync();
                return Ok(existingProfile);
            }
            else
            {
                // Create
                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetProfile", new { email = profile.Email }, profile);
            }
        }
    }
}
