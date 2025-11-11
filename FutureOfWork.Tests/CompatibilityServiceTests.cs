using FutureOfWork.Data.Repositories;
using FutureOfWork.Domain.Entities;
using FutureOfWork.Services.Services;
using Moq;
using Xunit;

namespace FutureOfWork.Tests;

public class CompatibilityServiceTests
{
    private readonly Mock<IJobRepository> _mockJobRepository;
    private readonly Mock<ICandidateRepository> _mockCandidateRepository;
    private readonly CompatibilityService _service;

    public CompatibilityServiceTests()
    {
        _mockJobRepository = new Mock<IJobRepository>();
        _mockCandidateRepository = new Mock<ICandidateRepository>();
        _service = new CompatibilityService(_mockJobRepository.Object, _mockCandidateRepository.Object);
    }

    [Fact]
    public async Task CalculateCompatibility_ReturnsZero_WhenJobNotFound()
    {
        // Arrange
        _mockJobRepository.Setup(r => r.GetByIdWithSkillsAsync(999))
            .ReturnsAsync((Job?)null);

        // Act
        var result = await _service.CalculateCompatibilityAsync(999, 1);

        // Assert
        Assert.Equal(0.0, result);
    }

    [Fact]
    public async Task CalculateCompatibility_ReturnsZero_WhenCandidateNotFound()
    {
        // Arrange
        var job = new Job { Id = 1, JobSkills = new List<JobSkill>() };
        _mockJobRepository.Setup(r => r.GetByIdWithSkillsAsync(1))
            .ReturnsAsync(job);
        _mockCandidateRepository.Setup(r => r.GetByIdWithSkillsAsync(999))
            .ReturnsAsync((Candidate?)null);

        // Act
        var result = await _service.CalculateCompatibilityAsync(1, 999);

        // Assert
        Assert.Equal(0.0, result);
    }

    [Fact]
    public async Task CalculateCompatibility_ReturnsScore_WhenJobAndCandidateExist()
    {
        // Arrange
        var skill = new Skill { Id = 1, Name = "C#" };
        var job = new Job 
        { 
            Id = 1, 
            JobSkills = new List<JobSkill> 
            { 
                new JobSkill { SkillId = 1, Skill = skill, Level = "Advanced" } 
            } 
        };
        var candidate = new Candidate 
        { 
            Id = 1, 
            ExperienceYears = 5,
            CandidateSkills = new List<CandidateSkill> 
            { 
                new CandidateSkill { SkillId = 1, Skill = skill, Level = "Advanced" } 
            } 
        };

        _mockJobRepository.Setup(r => r.GetByIdWithSkillsAsync(1))
            .ReturnsAsync(job);
        _mockCandidateRepository.Setup(r => r.GetByIdWithSkillsAsync(1))
            .ReturnsAsync(candidate);

        // Act
        var result = await _service.CalculateCompatibilityAsync(1, 1);

        // Assert
        Assert.InRange(result, 0.0, 1.0);
    }
}

