namespace FutureOfWork.Domain.Entities;

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Technical, Soft, Language, etc.
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int DemandScore { get; set; } = 0; // Score for ML.NET predictions (0-100)

    // Navigation properties
    public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    public ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
}

