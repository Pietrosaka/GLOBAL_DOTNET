namespace FutureOfWork.Domain.Entities;

public class JobSkill
{
    public int JobId { get; set; }
    public int SkillId { get; set; }
    public string Level { get; set; } = "Intermediate"; // Beginner, Intermediate, Advanced, Expert
    public bool IsRequired { get; set; } = true;

    // Navigation properties
    public Job Job { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}

