using FutureOfWork.Services.DTOs;
using FutureOfWork.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FutureOfWork.API.Controllers.V2;

[ApiController]
[Route("api/v2/jobs")]
[ApiVersion("2.0")]
[Authorize]
public class JobsV2Controller : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly ILogger<JobsV2Controller> _logger;

    public JobsV2Controller(IJobService jobService, ILogger<JobsV2Controller> logger)
    {
        _jobService = jobService;
        _logger = logger;
    }

    /// <summary>
    /// Get all jobs with pagination (V2 - Enhanced with additional filtering)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<JobDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<JobDto>>> GetJobs(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? title = null,
        [FromQuery] string? location = null,
        [FromQuery] string? company = null,
        [FromQuery] string? employmentType = null,
        [FromQuery] decimal? minSalary = null,
        [FromQuery] decimal? maxSalary = null)
    {
        _logger.LogInformation("Getting jobs (V2) - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _jobService.GetJobsAsync(pageNumber, pageSize, title, location, company);
        
        // V2 enhancements: Additional filtering
        if (!string.IsNullOrEmpty(employmentType))
        {
            result.Items = result.Items.Where(j => j.EmploymentType == employmentType);
        }

        if (minSalary.HasValue)
        {
            result.Items = result.Items.Where(j => j.SalaryMin >= minSalary.Value);
        }

        if (maxSalary.HasValue)
        {
            result.Items = result.Items.Where(j => j.SalaryMax <= maxSalary.Value);
        }

        return Ok(result);
    }

    /// <summary>
    /// Get job by ID (V2)
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(JobDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<JobDto>> GetJob(int id)
    {
        _logger.LogInformation("Getting job (V2) with ID: {JobId}", id);

        var job = await _jobService.GetJobByIdAsync(id);
        if (job == null)
        {
            return NotFound(new { message = $"Job with ID {id} not found" });
        }

        return Ok(job);
    }
}

