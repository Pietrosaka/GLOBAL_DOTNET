using FutureOfWork.Data.Repositories;
using FutureOfWork.Domain.Entities;
using FutureOfWork.Services.DTOs;
using FutureOfWork.Services.Services;
using Microsoft.Extensions.Configuration;

namespace FutureOfWork.Services.Services;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IJobRepository _jobRepository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly ICompatibilityService _compatibilityService;
    private readonly string _baseUrl;

    public ApplicationService(
        IApplicationRepository applicationRepository,
        IJobRepository jobRepository,
        ICandidateRepository candidateRepository,
        ICompatibilityService compatibilityService,
        IConfiguration configuration)
    {
        _applicationRepository = applicationRepository;
        _jobRepository = jobRepository;
        _candidateRepository = candidateRepository;
        _compatibilityService = compatibilityService;
        _baseUrl = configuration["BaseUrl"] ?? "https://localhost:7000";
    }

    public async Task<PagedResult<ApplicationDto>> GetApplicationsAsync(int pageNumber, int pageSize, int? jobId, int? candidateId)
    {
        IEnumerable<Application> applications;

        if (jobId.HasValue)
        {
            applications = await _applicationRepository.GetByJobIdAsync(jobId.Value);
        }
        else if (candidateId.HasValue)
        {
            applications = await _applicationRepository.GetByCandidateIdAsync(candidateId.Value);
        }
        else
        {
            applications = await _applicationRepository.GetAllAsync();
        }

        var applicationsList = applications.ToList();
        var totalCount = applicationsList.Count;
        var pagedApplications = applicationsList
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var applicationDtos = pagedApplications.Select(a => MapToDto(a)).ToList();

        var result = new PagedResult<ApplicationDto>
        {
            Items = applicationDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        // Add HATEOAS pagination links
        AddPaginationLinks(result, pageNumber, pageSize, jobId, candidateId);

        return result;
    }

    public async Task<ApplicationDto?> GetApplicationByIdAsync(int id)
    {
        var application = await _applicationRepository.GetByIdWithDetailsAsync(id);
        if (application == null) return null;

        var dto = MapToDto(application);
        AddLinks(dto, id);
        return dto;
    }

    public async Task<ApplicationDto> CreateApplicationAsync(int jobId, int candidateId, string? coverLetter)
    {
        // Check if application already exists
        var existingApplication = await _applicationRepository.GetByJobAndCandidateAsync(jobId, candidateId);
        if (existingApplication != null)
        {
            throw new InvalidOperationException("Application already exists for this job and candidate");
        }

        // Check if job and candidate exist
        var job = await _jobRepository.GetByIdAsync(jobId);
        var candidate = await _candidateRepository.GetByIdAsync(candidateId);

        if (job == null || candidate == null)
        {
            throw new InvalidOperationException("Job or candidate not found");
        }

        // Calculate compatibility score using ML.NET
        var compatibilityScore = await _compatibilityService.CalculateCompatibilityAsync(jobId, candidateId);

        var application = new Application
        {
            JobId = jobId,
            CandidateId = candidateId,
            Status = "Pending",
            AppliedAt = DateTime.UtcNow,
            CoverLetter = coverLetter,
            CompatibilityScore = compatibilityScore
        };

        var createdApplication = await _applicationRepository.AddAsync(application);
        var result = await GetApplicationByIdAsync(createdApplication.Id);
        return result!;
    }

    public async Task<ApplicationDto?> UpdateApplicationStatusAsync(int id, string status)
    {
        var application = await _applicationRepository.GetByIdAsync(id);
        if (application == null) return null;

        application.Status = status;
        application.ReviewedAt = DateTime.UtcNow;
        await _applicationRepository.UpdateAsync(application);

        return await GetApplicationByIdAsync(id);
    }

    private ApplicationDto MapToDto(Application application)
    {
        return new ApplicationDto
        {
            Id = application.Id,
            JobId = application.JobId,
            JobTitle = application.Job?.Title ?? "",
            CandidateId = application.CandidateId,
            CandidateName = application.Candidate?.Name ?? "",
            Status = application.Status,
            AppliedAt = application.AppliedAt,
            CompatibilityScore = application.CompatibilityScore
        };
    }

    private void AddLinks(ApplicationDto dto, int id)
    {
        dto.Links.Add(new Link { Href = $"{_baseUrl}/api/v1/applications/{id}", Rel = "self", Method = "GET" });
        dto.Links.Add(new Link { Href = $"{_baseUrl}/api/v1/applications/{id}", Rel = "update", Method = "PUT" });
        dto.Links.Add(new Link { Href = $"{_baseUrl}/api/v1/jobs/{dto.JobId}", Rel = "job", Method = "GET" });
        dto.Links.Add(new Link { Href = $"{_baseUrl}/api/v1/candidates/{dto.CandidateId}", Rel = "candidate", Method = "GET" });
    }

    private void AddPaginationLinks(PagedResult<ApplicationDto> result, int pageNumber, int pageSize, int? jobId, int? candidateId)
    {
        var queryParams = new List<string>();
        if (jobId.HasValue) queryParams.Add($"jobId={jobId.Value}");
        if (candidateId.HasValue) queryParams.Add($"candidateId={candidateId.Value}");
        queryParams.Add($"pageSize={pageSize}");
        
        var baseQuery = queryParams.Any() ? "?" + string.Join("&", queryParams) : $"?pageSize={pageSize}";

        // Self link
        result.Links.Add(new Link 
        { 
            Href = $"{_baseUrl}/api/v1/applications{baseQuery}&pageNumber={pageNumber}", 
            Rel = "self", 
            Method = "GET" 
        });

        // First page
        if (result.TotalPages > 0)
        {
            result.Links.Add(new Link 
            { 
                Href = $"{_baseUrl}/api/v1/applications{baseQuery}&pageNumber=1", 
                Rel = "first", 
                Method = "GET" 
            });
        }

        // Previous page
        if (result.HasPreviousPage)
        {
            result.Links.Add(new Link 
            { 
                Href = $"{_baseUrl}/api/v1/applications{baseQuery}&pageNumber={pageNumber - 1}", 
                Rel = "prev", 
                Method = "GET" 
            });
        }

        // Next page
        if (result.HasNextPage)
        {
            result.Links.Add(new Link 
            { 
                Href = $"{_baseUrl}/api/v1/applications{baseQuery}&pageNumber={pageNumber + 1}", 
                Rel = "next", 
                Method = "GET" 
            });
        }

        // Last page
        if (result.TotalPages > 0)
        {
            result.Links.Add(new Link 
            { 
                Href = $"{_baseUrl}/api/v1/applications{baseQuery}&pageNumber={result.TotalPages}", 
                Rel = "last", 
                Method = "GET" 
            });
        }
    }
}

