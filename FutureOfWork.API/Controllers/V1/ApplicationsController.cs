using FutureOfWork.Services.DTOs;
using FutureOfWork.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FutureOfWork.API.Controllers.V1;

[ApiController]
[Route("api/v1/applications")]
[ApiVersion("1.0")]
[Authorize]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _applicationService;
    private readonly ILogger<ApplicationsController> _logger;

    public ApplicationsController(IApplicationService applicationService, ILogger<ApplicationsController> logger)
    {
        _applicationService = applicationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all applications with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ApplicationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<ApplicationDto>>> GetApplications(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? jobId = null,
        [FromQuery] int? candidateId = null)
    {
        _logger.LogInformation("Getting applications - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);

        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var result = await _applicationService.GetApplicationsAsync(pageNumber, pageSize, jobId, candidateId);
        return Ok(result);
    }

    /// <summary>
    /// Get application by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApplicationDto>> GetApplication(int id)
    {
        _logger.LogInformation("Getting application with ID: {ApplicationId}", id);

        var application = await _applicationService.GetApplicationByIdAsync(id);
        if (application == null)
        {
            return NotFound(new { message = $"Application with ID {id} not found" });
        }

        return Ok(application);
    }

    /// <summary>
    /// Create a new application
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApplicationDto>> CreateApplication([FromBody] CreateApplicationRequest request)
    {
        _logger.LogInformation("Creating application for Job: {JobId}, Candidate: {CandidateId}", request.JobId, request.CandidateId);

        try
        {
            var application = await _applicationService.CreateApplicationAsync(
                request.JobId,
                request.CandidateId,
                request.CoverLetter);

            return CreatedAtAction(nameof(GetApplication), new { id = application.Id }, application);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update application status
    /// </summary>
    [HttpPut("{id}/status")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApplicationDto>> UpdateApplicationStatus(int id, [FromBody] UpdateStatusRequest request)
    {
        _logger.LogInformation("Updating application status - ID: {ApplicationId}, Status: {Status}", id, request.Status);

        var application = await _applicationService.UpdateApplicationStatusAsync(id, request.Status);
        if (application == null)
        {
            return NotFound(new { message = $"Application with ID {id} not found" });
        }

        return Ok(application);
    }
}

public record CreateApplicationRequest(int JobId, int CandidateId, string? CoverLetter);
public record UpdateStatusRequest(string Status);

