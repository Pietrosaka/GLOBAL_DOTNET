using FutureOfWork.Services.DTOs;

namespace FutureOfWork.Services.Services;

public interface IJobService
{
    Task<PagedResult<JobDto>> GetJobsAsync(int pageNumber, int pageSize, string? title, string? location, string? company);
    Task<JobDto?> GetJobByIdAsync(int id);
    Task<JobDto> CreateJobAsync(JobDto jobDto);
    Task<JobDto?> UpdateJobAsync(int id, JobDto jobDto);
    Task<bool> DeleteJobAsync(int id);
}

