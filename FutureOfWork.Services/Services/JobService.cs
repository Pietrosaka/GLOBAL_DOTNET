using FutureOfWork.Data.Repositories;
using FutureOfWork.Domain.Entities;
using FutureOfWork.Services.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FutureOfWork.Services.Services;

public class JobService : IJobService
{
    private readonly IJobRepository _jobRepository;
    private readonly IRepository<Skill> _skillRepository;
    private readonly string _baseUrl;

    public JobService(IJobRepository jobRepository, IRepository<Skill> skillRepository, IConfiguration configuration)
    {
        _jobRepository = jobRepository;
        _skillRepository = skillRepository;
        _baseUrl = configuration["BaseUrl"] ?? "https://localhost:7000";
    }

    public async Task<PagedResult<JobDto>> GetJobsAsync(int pageNumber, int pageSize, string? title, string? location, string? company)
    {
        var jobs = await _jobRepository.SearchJobsAsync(title, location, company);
        var activeJobs = jobs.Where(j => j.IsActive).ToList();

        var totalCount = activeJobs.Count;
        var pagedJobs = activeJobs
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var jobDtos = pagedJobs.Select(j => MapToDto(j)).ToList();

        var result = new PagedResult<JobDto>
        {
            Items = jobDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        // Add HATEOAS pagination links
        AddPaginationLinks(result, pageNumber, pageSize, title, location, company);
        
        return result;
    }

    public async Task<JobDto?> GetJobByIdAsync(int id)
    {
        var job = await _jobRepository.GetByIdWithSkillsAsync(id);
        if (job == null) return null;

        var dto = MapToDto(job);
        AddLinks(dto, id);
        return dto;
    }

    public async Task<JobDto> CreateJobAsync(JobDto jobDto)
    {
        var job = new Job
        {
            Title = jobDto.Title,
            Description = jobDto.Description,
            Company = jobDto.Company,
            Location = jobDto.Location,
            SalaryMin = jobDto.SalaryMin,
            SalaryMax = jobDto.SalaryMax,
            EmploymentType = jobDto.EmploymentType,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createdJob = await _jobRepository.AddAsync(job);

        // Add skills if provided
        if (jobDto.Skills.Any())
        {
            // Note: In a real scenario, you would handle skill creation/assignment here
            // This is simplified for the example
        }

        var result = MapToDto(createdJob);
        AddLinks(result, createdJob.Id);
        return result;
    }

    public async Task<JobDto?> UpdateJobAsync(int id, JobDto jobDto)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job == null) return null;

        job.Title = jobDto.Title;
        job.Description = jobDto.Description;
        job.Company = jobDto.Company;
        job.Location = jobDto.Location;
        job.SalaryMin = jobDto.SalaryMin;
        job.SalaryMax = jobDto.SalaryMax;
        job.EmploymentType = jobDto.EmploymentType;
        job.UpdatedAt = DateTime.UtcNow;

        await _jobRepository.UpdateAsync(job);

        var result = await GetJobByIdAsync(id);
        return result;
    }

    public async Task<bool> DeleteJobAsync(int id)
    {
        var job = await _jobRepository.GetByIdAsync(id);
        if (job == null) return false;

        job.IsActive = false;
        job.UpdatedAt = DateTime.UtcNow;
        await _jobRepository.UpdateAsync(job);
        return true;
    }

    private JobDto MapToDto(Job job)
    {
        return new JobDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            Company = job.Company,
            Location = job.Location,
            SalaryMin = job.SalaryMin,
            SalaryMax = job.SalaryMax,
            EmploymentType = job.EmploymentType,
            CreatedAt = job.CreatedAt,
            IsActive = job.IsActive,
            Skills = job.JobSkills.Select(js => new SkillDto
            {
                Id = js.SkillId,
                Name = js.Skill?.Name ?? "",
                Category = js.Skill?.Category ?? "",
                Level = js.Level
            }).ToList()
        };
    }

    private void AddLinks(JobDto dto, int id)
    {
        dto.Links.Add(new Link { Href = $"{_baseUrl}/api/v1/jobs/{id}", Rel = "self", Method = "GET" });
        dto.Links.Add(new Link { Href = $"{_baseUrl}/api/v1/jobs/{id}", Rel = "update", Method = "PUT" });
        dto.Links.Add(new Link { Href = $"{_baseUrl}/api/v1/jobs/{id}", Rel = "delete", Method = "DELETE" });
        dto.Links.Add(new Link { Href = $"{_baseUrl}/api/v1/jobs/{id}/applications", Rel = "applications", Method = "GET" });
    }

    private void AddPaginationLinks(PagedResult<JobDto> result, int pageNumber, int pageSize, string? title, string? location, string? company)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrEmpty(title)) queryParams.Add($"title={Uri.EscapeDataString(title)}");
        if (!string.IsNullOrEmpty(location)) queryParams.Add($"location={Uri.EscapeDataString(location)}");
        if (!string.IsNullOrEmpty(company)) queryParams.Add($"company={Uri.EscapeDataString(company)}");
        queryParams.Add($"pageSize={pageSize}");
        
        var baseQuery = queryParams.Any() ? "?" + string.Join("&", queryParams) : $"?pageSize={pageSize}";

        // Self link
        result.Links.Add(new Link 
        { 
            Href = $"{_baseUrl}/api/v1/jobs{baseQuery}&pageNumber={pageNumber}", 
            Rel = "self", 
            Method = "GET" 
        });

        // First page
        if (result.TotalPages > 0)
        {
            result.Links.Add(new Link 
            { 
                Href = $"{_baseUrl}/api/v1/jobs{baseQuery}&pageNumber=1", 
                Rel = "first", 
                Method = "GET" 
            });
        }

        // Previous page
        if (result.HasPreviousPage)
        {
            result.Links.Add(new Link 
            { 
                Href = $"{_baseUrl}/api/v1/jobs{baseQuery}&pageNumber={pageNumber - 1}", 
                Rel = "prev", 
                Method = "GET" 
            });
        }

        // Next page
        if (result.HasNextPage)
        {
            result.Links.Add(new Link 
            { 
                Href = $"{_baseUrl}/api/v1/jobs{baseQuery}&pageNumber={pageNumber + 1}", 
                Rel = "next", 
                Method = "GET" 
            });
        }

        // Last page
        if (result.TotalPages > 0)
        {
            result.Links.Add(new Link 
            { 
                Href = $"{_baseUrl}/api/v1/jobs{baseQuery}&pageNumber={result.TotalPages}", 
                Rel = "last", 
                Method = "GET" 
            });
        }
    }
}

