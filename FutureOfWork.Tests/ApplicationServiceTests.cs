using FutureOfWork.Data.Repositories;
using FutureOfWork.Domain.Entities;
using FutureOfWork.Services.DTOs;
using FutureOfWork.Services.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace FutureOfWork.Tests;

public class ApplicationServiceTests
{
    private readonly Mock<IApplicationRepository> _mockApplicationRepository;
    private readonly Mock<IJobRepository> _mockJobRepository;
    private readonly Mock<ICandidateRepository> _mockCandidateRepository;
    private readonly Mock<ICompatibilityService> _mockCompatibilityService;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly ApplicationService _service;

    public ApplicationServiceTests()
    {
        _mockApplicationRepository = new Mock<IApplicationRepository>();
        _mockJobRepository = new Mock<IJobRepository>();
        _mockCandidateRepository = new Mock<ICandidateRepository>();
        _mockCompatibilityService = new Mock<ICompatibilityService>();
        _mockConfiguration = new Mock<IConfiguration>();

        _mockConfiguration.Setup(c => c["BaseUrl"]).Returns("https://localhost:7000");

        _service = new ApplicationService(
            _mockApplicationRepository.Object,
            _mockJobRepository.Object,
            _mockCandidateRepository.Object,
            _mockCompatibilityService.Object,
            _mockConfiguration.Object);
    }

    [Fact]
    public async Task CreateApplication_ThrowsException_WhenApplicationAlreadyExists()
    {
        // Arrange
        var existingApplication = new Application { Id = 1, JobId = 1, CandidateId = 1 };
        _mockApplicationRepository.Setup(r => r.GetByJobAndCandidateAsync(1, 1))
            .ReturnsAsync(existingApplication);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.CreateApplicationAsync(1, 1, null));
    }

    [Fact]
    public async Task CreateApplication_CreatesApplication_WhenJobAndCandidateExist()
    {
        // Arrange
        var job = new Job { Id = 1, Title = "Software Developer" };
        var candidate = new Candidate { Id = 1, Name = "John Doe" };
        var newApplication = new Application { Id = 1, JobId = 1, CandidateId = 1, CompatibilityScore = 0.85 };

        _mockApplicationRepository.Setup(r => r.GetByJobAndCandidateAsync(1, 1))
            .ReturnsAsync((Application?)null);
        _mockJobRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(job);
        _mockCandidateRepository.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(candidate);
        _mockCompatibilityService.Setup(s => s.CalculateCompatibilityAsync(1, 1))
            .ReturnsAsync(0.85);
        _mockApplicationRepository.Setup(r => r.AddAsync(It.IsAny<Application>()))
            .ReturnsAsync(newApplication);
        _mockApplicationRepository.Setup(r => r.GetByIdWithDetailsAsync(1))
            .ReturnsAsync(newApplication);

        // Act
        var result = await _service.CreateApplicationAsync(1, 1, "Cover letter");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(0.85, result.CompatibilityScore);
    }
}

