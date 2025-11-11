using FutureOfWork.Domain.Entities;

namespace FutureOfWork.Data.Repositories;

public interface IJobRepository : IRepository<Job>
{
    Task<Job?> GetByIdWithSkillsAsync(int id);
    Task<IEnumerable<Job>> GetActiveJobsAsync();
    Task<IEnumerable<Job>> SearchJobsAsync(string? title, string? location, string? company);
}

