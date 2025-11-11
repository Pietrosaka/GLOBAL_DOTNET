using FutureOfWork.Services.DTOs;
using FutureOfWork.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FutureOfWork.API.Controllers.V1;

[ApiController]
[Route("api/v1/skills")]
[ApiVersion("1.0")]
[Authorize]
public class SkillsController : ControllerBase
{
    private readonly ISkillDemandService _skillDemandService;
    private readonly ILogger<SkillsController> _logger;

    public SkillsController(ISkillDemandService skillDemandService, ILogger<SkillsController> logger)
    {
        _skillDemandService = skillDemandService;
        _logger = logger;
    }

    /// <summary>
    /// Predict demand score for a specific skill using ML.NET
    /// </summary>
    [HttpPost("{skillId}/predict-demand")]
    [ProducesResponseType(typeof(SkillDemandPredictionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SkillDemandPredictionResponse>> PredictSkillDemand(int skillId)
    {
        _logger.LogInformation("Predicting demand for skill ID: {SkillId}", skillId);

        try
        {
            var demandScore = await _skillDemandService.PredictSkillDemandAsync(skillId);
            return Ok(new SkillDemandPredictionResponse
            {
                SkillId = skillId,
                DemandScore = demandScore,
                PredictedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error predicting demand for skill ID: {SkillId}", skillId);
            return NotFound(new { message = $"Skill with ID {skillId} not found" });
        }
    }

    /// <summary>
    /// Predict demand scores for all skills using ML.NET
    /// </summary>
    [HttpPost("predict-all-demand")]
    [ProducesResponseType(typeof(Dictionary<int, int>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Dictionary<int, int>>> PredictAllSkillsDemand()
    {
        _logger.LogInformation("Predicting demand for all skills");

        var predictions = await _skillDemandService.PredictAllSkillsDemandAsync();
        return Ok(predictions);
    }
}

public record SkillDemandPredictionResponse
{
    public int SkillId { get; set; }
    public int DemandScore { get; set; }
    public DateTime PredictedAt { get; set; }
}

