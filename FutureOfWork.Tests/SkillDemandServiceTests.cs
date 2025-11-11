using FutureOfWork.Data.Repositories;
using FutureOfWork.Domain.Entities;
using FutureOfWork.Services.Services;
using Moq;
using Xunit;

namespace FutureOfWork.Tests;

public class SkillDemandServiceTests
{
    private readonly Mock<IRepository<Skill>> _mockSkillRepository;
    private readonly Mock<IJobRepository> _mockJobRepository;
    private readonly SkillDemandService _service;

    public SkillDemandServiceTests()
    {
        _mockSkillRepository = new Mock<IRepository<Skill>>();
        _mockJobRepository = new Mock<IJobRepository>();
        _service = new SkillDemandService(_mockSkillRepository.Object, _mockJobRepository.Object);
    }

    [Fact]
    public async Task PredictSkillDemand_ReturnsZero_WhenSkillNotFound()
    {
        // Arrange
        _mockSkillRepository.Setup(r => r.GetByIdAsync(999))
            .ReturnsAsync((Skill?)null);

        // Act
        var result = await _service.PredictSkillDemandAsync(999);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task PredictSkillDemand_ReturnsScore_WhenSkillExists()
    {
        // Arrange
        var skill = new Skill 
        { 
            Id = 1, 
            Name = "C#", 
            CreatedAt = DateTime.UtcNow.AddYears(-1) 
        };
        var jobs = new List<Job>
        {
            new Job { Id = 1, IsActive = true, JobSkills = new List<JobSkill> 
            { 
                new JobSkill { SkillId = 1 } 
            }}
        };

        _mockSkillRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(skill);
        _mockJobRepository.Setup(r => r.GetActiveJobsAsync())
            .ReturnsAsync(jobs);
        _mockSkillRepository.Setup(r => r.UpdateAsync(It.IsAny<Skill>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.PredictSkillDemandAsync(1);

        // Assert
        Assert.InRange(result, 0, 100);
    }

    [Fact]
    public async Task PredictAllSkillsDemand_ReturnsDictionary_WithAllSkills()
    {
        // Arrange
        var skills = new List<Skill>
        {
            new Skill { Id = 1, Name = "C#", CreatedAt = DateTime.UtcNow.AddYears(-1) },
            new Skill { Id = 2, Name = "Python", CreatedAt = DateTime.UtcNow.AddYears(-2) }
        };
        var jobs = new List<Job>();

        _mockSkillRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(skills);
        _mockJobRepository.Setup(r => r.GetActiveJobsAsync())
            .ReturnsAsync(jobs);
        _mockSkillRepository.Setup(r => r.UpdateAsync(It.IsAny<Skill>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.PredictAllSkillsDemandAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(1, result.Keys);
        Assert.Contains(2, result.Keys);
    }
}

