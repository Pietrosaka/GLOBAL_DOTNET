namespace FutureOfWork.Domain.Entities;

public class Application
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int CandidateId { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Reviewed, Interview, Accepted, Rejected
    public DateTime AppliedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? CoverLetter { get; set; }
    public double? CompatibilityScore { get; set; } // ML.NET prediction score

    // Navigation properties
    public Job Job { get; set; } = null!;
    public Candidate Candidate { get; set; } = null!;
}

