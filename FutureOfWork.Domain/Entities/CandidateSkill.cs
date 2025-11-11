namespace FutureOfWork.Domain.Entities;

public class CandidateSkill
{
    public int CandidateId { get; set; }
    public int SkillId { get; set; }
    public string Level { get; set; } = "Intermediate"; // Beginner, Intermediate, Advanced, Expert
    public int YearsOfExperience { get; set; }

    // Navigation properties
    public Candidate Candidate { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}

