namespace FutureOfWork.Services.DTOs;

public class CandidateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string Education { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<SkillDto> Skills { get; set; } = new();
    public List<Link> Links { get; set; } = new();
}

