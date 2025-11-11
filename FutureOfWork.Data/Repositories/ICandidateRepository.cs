using FutureOfWork.Domain.Entities;

namespace FutureOfWork.Data.Repositories;

public interface ICandidateRepository : IRepository<Candidate>
{
    Task<Candidate?> GetByIdWithSkillsAsync(int id);
    Task<Candidate?> GetByEmailAsync(string email);
}

