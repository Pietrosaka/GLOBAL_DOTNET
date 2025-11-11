using FutureOfWork.Domain.Entities;
using FutureOfWork.Data;
using Microsoft.EntityFrameworkCore;

namespace FutureOfWork.Data.Repositories;

public class ApplicationRepository : Repository<Application>, IApplicationRepository
{
    public ApplicationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Application?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(a => a.Job)
                .ThenInclude(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
            .Include(a => a.Candidate)
                .ThenInclude(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Application>> GetByJobIdAsync(int jobId)
    {
        return await _dbSet
            .Include(a => a.Candidate)
                .ThenInclude(c => c.CandidateSkills)
                    .ThenInclude(cs => cs.Skill)
            .Where(a => a.JobId == jobId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Application>> GetByCandidateIdAsync(int candidateId)
    {
        return await _dbSet
            .Include(a => a.Job)
                .ThenInclude(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
            .Where(a => a.CandidateId == candidateId)
            .ToListAsync();
    }

    public async Task<Application?> GetByJobAndCandidateAsync(int jobId, int candidateId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.JobId == jobId && a.CandidateId == candidateId);
    }
}

