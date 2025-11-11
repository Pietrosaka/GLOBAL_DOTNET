using FutureOfWork.Domain.Entities;
using FutureOfWork.Data;
using Microsoft.EntityFrameworkCore;

namespace FutureOfWork.Data.Repositories;

public class JobRepository : Repository<Job>, IJobRepository
{
    public JobRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Job?> GetByIdWithSkillsAsync(int id)
    {
        return await _dbSet
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<IEnumerable<Job>> GetActiveJobsAsync()
    {
        return await _dbSet
            .Where(j => j.IsActive)
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .ToListAsync();
    }

    public async Task<IEnumerable<Job>> SearchJobsAsync(string? title, string? location, string? company)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(j => j.Title.Contains(title));
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(j => j.Location.Contains(location));
        }

        if (!string.IsNullOrWhiteSpace(company))
        {
            query = query.Where(j => j.Company.Contains(company));
        }

        return await query
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .ToListAsync();
    }
}

