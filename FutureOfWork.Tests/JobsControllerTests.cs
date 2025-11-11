using FutureOfWork.API.Controllers.V1;
using FutureOfWork.Services.DTOs;
using FutureOfWork.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FutureOfWork.Tests;

public class JobsControllerTests
{
    private readonly Mock<IJobService> _mockJobService;
    private readonly Mock<ILogger<JobsController>> _mockLogger;
    private readonly JobsController _controller;

    public JobsControllerTests()
    {
        _mockJobService = new Mock<IJobService>();
        _mockLogger = new Mock<ILogger<JobsController>>();
        _controller = new JobsController(_mockJobService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetJobs_ReturnsOkResult_WithJobs()
    {
        // Arrange
        var expectedResult = new PagedResult<JobDto>
        {
            Items = new List<JobDto>
            {
                new JobDto { Id = 1, Title = "Software Developer", Company = "Tech Corp" },
                new JobDto { Id = 2, Title = "Data Scientist", Company = "Data Inc" }
            },
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 10
        };

        _mockJobService.Setup(s => s.GetJobsAsync(1, 10, null, null, null))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.GetJobs();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<PagedResult<JobDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Items.Count());
    }

    [Fact]
    public async Task GetJob_ReturnsNotFound_WhenJobDoesNotExist()
    {
        // Arrange
        _mockJobService.Setup(s => s.GetJobByIdAsync(999))
            .ReturnsAsync((JobDto?)null);

        // Act
        var result = await _controller.GetJob(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetJob_ReturnsOkResult_WhenJobExists()
    {
        // Arrange
        var expectedJob = new JobDto { Id = 1, Title = "Software Developer", Company = "Tech Corp" };
        _mockJobService.Setup(s => s.GetJobByIdAsync(1))
            .ReturnsAsync(expectedJob);

        // Act
        var result = await _controller.GetJob(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<JobDto>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
        Assert.Equal("Software Developer", returnValue.Title);
    }

    [Fact]
    public async Task CreateJob_ReturnsCreatedResult_WithJob()
    {
        // Arrange
        var newJob = new JobDto { Title = "New Job", Company = "New Company" };
        var createdJob = new JobDto { Id = 1, Title = "New Job", Company = "New Company" };

        _mockJobService.Setup(s => s.CreateJobAsync(It.IsAny<JobDto>()))
            .ReturnsAsync(createdJob);

        // Act
        var result = await _controller.CreateJob(newJob);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<JobDto>(createdResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task DeleteJob_ReturnsNoContent_WhenJobExists()
    {
        // Arrange
        _mockJobService.Setup(s => s.DeleteJobAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteJob(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteJob_ReturnsNotFound_WhenJobDoesNotExist()
    {
        // Arrange
        _mockJobService.Setup(s => s.DeleteJobAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteJob(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}

