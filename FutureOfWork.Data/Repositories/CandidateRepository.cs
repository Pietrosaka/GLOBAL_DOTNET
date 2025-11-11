using FutureOfWork.Domain.Entities;
using FutureOfWork.Data;
using Microsoft.EntityFrameworkCore;

namespace FutureOfWork.Data.Repositories;

public class CandidateRepository : Repository<Candidate>, ICandidateRepository
{
    public CandidateRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Candidate?> GetByIdWithSkillsAsync(int id)
    {
        return await _dbSet
            .Include(c => c.CandidateSkills)
                .ThenInclude(cs => cs.Skill)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Candidate?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Email == email);
    }
}

