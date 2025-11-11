using FutureOfWork.Domain.Entities;

namespace FutureOfWork.Data.Repositories;

public interface IApplicationRepository : IRepository<Application>
{
    Task<Application?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Application>> GetByJobIdAsync(int jobId);
    Task<IEnumerable<Application>> GetByCandidateIdAsync(int candidateId);
    Task<Application?> GetByJobAndCandidateAsync(int jobId, int candidateId);
}

