namespace FutureOfWork.Services.DTOs;

public class ApplicationDto
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public int CandidateId { get; set; }
    public string CandidateName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
    public double? CompatibilityScore { get; set; }
    public List<Link> Links { get; set; } = new();
}

