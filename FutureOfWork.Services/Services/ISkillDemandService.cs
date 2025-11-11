namespace FutureOfWork.Services.Services;

public interface ISkillDemandService
{
    Task<int> PredictSkillDemandAsync(int skillId);
    Task<Dictionary<int, int>> PredictAllSkillsDemandAsync();
}

