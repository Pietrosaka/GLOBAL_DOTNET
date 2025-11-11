using FutureOfWork.Data.Repositories;
using FutureOfWork.Domain.Entities;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace FutureOfWork.Services.Services;

public class CompatibilityService : ICompatibilityService
{
    private readonly IJobRepository _jobRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly MLContext _mlContext;
    private ITransformer? _model;

    public CompatibilityService(IJobRepository jobRepository, ICandidateRepository candidateRepository)
    {
        _jobRepository = jobRepository;
        _candidateRepository = candidateRepository;
        _mlContext = new MLContext(seed: 0);
        InitializeModel();
    }

    private void InitializeModel()
    {
        // Simple compatibility model based on skill matching
        // In a real scenario, you would train this model with historical data
        var pipeline = _mlContext.Transforms.Concatenate("Features", "SkillMatchRatio", "ExperienceMatch", "LevelMatch")
            .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "CompatibilityScore", maximumNumberOfIterations: 100));

        // Create dummy data for training (in production, use real data)
        var data = new List<CompatibilityData>
        {
            new CompatibilityData { SkillMatchRatio = 1.0f, ExperienceMatch = 1.0f, LevelMatch = 1.0f, CompatibilityScore = 1.0f },
            new CompatibilityData { SkillMatchRatio = 0.8f, ExperienceMatch = 0.9f, LevelMatch = 0.8f, CompatibilityScore = 0.85f },
            new CompatibilityData { SkillMatchRatio = 0.5f, ExperienceMatch = 0.6f, LevelMatch = 0.5f, CompatibilityScore = 0.55f },
            new CompatibilityData { SkillMatchRatio = 0.3f, ExperienceMatch = 0.4f, LevelMatch = 0.3f, CompatibilityScore = 0.35f },
        };

        var trainingData = _mlContext.Data.LoadFromEnumerable(data);
        _model = pipeline.Fit(trainingData);
    }

    public async Task<double> CalculateCompatibilityAsync(int jobId, int candidateId)
    {
        var job = await _jobRepository.GetByIdWithSkillsAsync(jobId);
        var candidate = await _candidateRepository.GetByIdWithSkillsAsync(candidateId);

        if (job == null || candidate == null)
            return 0.0;

        // Calculate skill match ratio
        var jobSkillIds = job.JobSkills.Select(js => js.SkillId).ToHashSet();
        var candidateSkillIds = candidate.CandidateSkills.Select(cs => cs.SkillId).ToHashSet();
        var matchedSkills = jobSkillIds.Intersect(candidateSkillIds).Count();
        var skillMatchRatio = jobSkillIds.Count > 0 ? (float)matchedSkills / jobSkillIds.Count : 0f;

        // Calculate experience match (simplified)
        var experienceMatch = Math.Min(1.0f, (float)candidate.ExperienceYears / 10.0f);

        // Calculate level match (average of skill levels)
        var levelMatch = CalculateLevelMatch(job.JobSkills, candidate.CandidateSkills);

        // Predict compatibility using ML model
        var predictionData = new CompatibilityData
        {
            SkillMatchRatio = skillMatchRatio,
            ExperienceMatch = experienceMatch,
            LevelMatch = levelMatch
        };

        var predictionEngine = _mlContext.Model.CreatePredictionEngine<CompatibilityData, CompatibilityPrediction>(_model!);
        var prediction = predictionEngine.Predict(predictionData);

        // Ensure score is between 0 and 1
        return Math.Max(0.0, Math.Min(1.0, prediction.CompatibilityScore));
    }

    private float CalculateLevelMatch(ICollection<JobSkill> jobSkills, ICollection<CandidateSkill> candidateSkills)
    {
        if (!jobSkills.Any()) return 0f;

        var levelValues = new Dictionary<string, float>
        {
            { "Beginner", 0.25f },
            { "Intermediate", 0.5f },
            { "Advanced", 0.75f },
            { "Expert", 1.0f }
        };

        var matches = 0f;
        var total = 0f;

        foreach (var jobSkill in jobSkills)
        {
            var candidateSkill = candidateSkills.FirstOrDefault(cs => cs.SkillId == jobSkill.SkillId);
            if (candidateSkill != null)
            {
                var jobLevel = levelValues.GetValueOrDefault(jobSkill.Level, 0.5f);
                var candidateLevel = levelValues.GetValueOrDefault(candidateSkill.Level, 0.5f);
                matches += Math.Min(jobLevel, candidateLevel) / jobLevel;
            }
            total += 1f;
        }

        return total > 0 ? matches / total : 0f;
    }

    private class CompatibilityData
    {
        public float SkillMatchRatio { get; set; }
        public float ExperienceMatch { get; set; }
        public float LevelMatch { get; set; }
        public float CompatibilityScore { get; set; }
    }

    private class CompatibilityPrediction
    {
        [ColumnName("Score")]
        public float CompatibilityScore { get; set; }
    }
}

