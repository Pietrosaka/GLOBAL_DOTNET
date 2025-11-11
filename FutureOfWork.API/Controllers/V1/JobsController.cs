using FutureOfWork.Services.DTOs;
using FutureOfWork.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FutureOfWork.API.Controllers.V1;

[ApiController]
[Route("api/v1/jobs")]
[ApiVersion("1.0")]
[Authorize]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IJobService jobService, ILogger<JobsController> logger)
    {
        _jobService = jobService;
        _logger = logger;
    }

    /// <summary>
    /// Get all jobs with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<JobDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<JobDto>>> GetJobs(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? title = null,
        [FromQuery] string? location = null,
        [FromQuery] string? company = null)
    {
        _logger.LogInformation("Getting jobs - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _jobService.GetJobsAsync(pageNumber, pageSize, title, location, company);
        return Ok(result);
    }

    /// <summary>
    /// Get job by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(JobDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<JobDto>> GetJob(int id)
    {
        _logger.LogInformation("Getting job with ID: {JobId}", id);

        var job = await _jobService.GetJobByIdAsync(id);
        if (job == null)
        {
            return NotFound(new { message = $"Job with ID {id} not found" });
        }

        return Ok(job);
    }

    /// <summary>
    /// Create a new job
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(JobDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<JobDto>> CreateJob([FromBody] JobDto jobDto)
    {
        _logger.LogInformation("Creating new job: {JobTitle}", jobDto.Title);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var createdJob = await _jobService.CreateJobAsync(jobDto);
        return CreatedAtAction(nameof(GetJob), new { id = createdJob.Id }, createdJob);
    }

    /// <summary>
    /// Update an existing job
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(JobDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<JobDto>> UpdateJob(int id, [FromBody] JobDto jobDto)
    {
        _logger.LogInformation("Updating job with ID: {JobId}", id);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updatedJob = await _jobService.UpdateJobAsync(id, jobDto);
        if (updatedJob == null)
        {
            return NotFound(new { message = $"Job with ID {id} not found" });
        }

        return Ok(updatedJob);
    }

    /// <summary>
    /// Delete a job (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteJob(int id)
    {
        _logger.LogInformation("Deleting job with ID: {JobId}", id);

        var result = await _jobService.DeleteJobAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Job with ID {id} not found" });
        }

        return NoContent();
    }
}

