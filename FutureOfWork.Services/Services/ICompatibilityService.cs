namespace FutureOfWork.Services.Services;

public interface ICompatibilityService
{
    Task<double> CalculateCompatibilityAsync(int jobId, int candidateId);
}

