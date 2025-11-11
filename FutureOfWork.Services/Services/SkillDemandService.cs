using FutureOfWork.Data.Repositories;
using FutureOfWork.Domain.Entities;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace FutureOfWork.Services.Services;

public class SkillDemandService : ISkillDemandService
{
    private readonly IRepository<Skill> _skillRepository;
    private readonly IJobRepository _jobRepository;
    private readonly MLContext _mlContext;
    private ITransformer? _model;

    public SkillDemandService(IRepository<Skill> skillRepository, IJobRepository jobRepository)
    {
        _skillRepository = skillRepository;
        _jobRepository = jobRepository;
        _mlContext = new MLContext(seed: 0);
        InitializeModel();
    }

    private void InitializeModel()
    {
        // Simple demand prediction model based on job postings and historical data
        // In a real scenario, you would train this model with historical market data
        var pipeline = _mlContext.Transforms.Concatenate("Features", "JobCount", "ApplicationCount", "SkillAge")
            .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "DemandScore", maximumNumberOfIterations: 100));

        // Create dummy data for training (in production, use real historical data)
        var data = new List<SkillDemandData>
        {
            new SkillDemandData { JobCount = 100f, ApplicationCount = 500f, SkillAge = 1f, DemandScore = 90f },
            new SkillDemandData { JobCount = 50f, ApplicationCount = 200f, SkillAge = 2f, DemandScore = 70f },
            new SkillDemandData { JobCount = 20f, ApplicationCount = 50f, SkillAge = 5f, DemandScore = 40f },
            new SkillDemandData { JobCount = 200f, ApplicationCount = 1000f, SkillAge = 0.5f, DemandScore = 95f },
            new SkillDemandData { JobCount = 10f, ApplicationCount = 20f, SkillAge = 10f, DemandScore = 20f },
        };

        var trainingData = _mlContext.Data.LoadFromEnumerable(data);
        _model = pipeline.Fit(trainingData);
    }

    public async Task<int> PredictSkillDemandAsync(int skillId)
    {
        var skill = await _skillRepository.GetByIdAsync(skillId);
        if (skill == null)
            return 0;

        // Get current job count for this skill
        var jobs = await _jobRepository.GetActiveJobsAsync();
        var jobCount = jobs.Count(j => j.JobSkills.Any(js => js.SkillId == skillId));

        // Get application count (simplified - in real scenario, query applications)
        var applicationCount = jobCount * 5; // Estimated ratio

        // Calculate skill age in years
        var skillAge = (DateTime.UtcNow - skill.CreatedAt).TotalDays / 365.0;
        if (skillAge < 1) skillAge = 1;

        // Predict demand using ML model
        var predictionData = new SkillDemandData
        {
            JobCount = jobCount,
            ApplicationCount = applicationCount,
            SkillAge = (float)skillAge
        };

        var predictionEngine = _mlContext.Model.CreatePredictionEngine<SkillDemandData, SkillDemandPrediction>(_model!);
        var prediction = predictionEngine.Predict(predictionData);

        // Ensure score is between 0 and 100
        var demandScore = (int)Math.Max(0, Math.Min(100, prediction.DemandScore));

        // Update skill demand score
        skill.DemandScore = demandScore;
        await _skillRepository.UpdateAsync(skill);

        return demandScore;
    }

    public async Task<Dictionary<int, int>> PredictAllSkillsDemandAsync()
    {
        var skills = await _skillRepository.GetAllAsync();
        var predictions = new Dictionary<int, int>();

        foreach (var skill in skills)
        {
            var demandScore = await PredictSkillDemandAsync(skill.Id);
            predictions[skill.Id] = demandScore;
        }

        return predictions;
    }

    private class SkillDemandData
    {
        public float JobCount { get; set; }
        public float ApplicationCount { get; set; }
        public float SkillAge { get; set; }
        public float DemandScore { get; set; }
    }

    private class SkillDemandPrediction
    {
        [ColumnName("Score")]
        public float DemandScore { get; set; }
    }
}

