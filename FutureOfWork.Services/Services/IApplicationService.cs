using FutureOfWork.Services.DTOs;

namespace FutureOfWork.Services.Services;

public interface IApplicationService
{
    Task<PagedResult<ApplicationDto>> GetApplicationsAsync(int pageNumber, int pageSize, int? jobId, int? candidateId);
    Task<ApplicationDto?> GetApplicationByIdAsync(int id);
    Task<ApplicationDto> CreateApplicationAsync(int jobId, int candidateId, string? coverLetter);
    Task<ApplicationDto?> UpdateApplicationStatusAsync(int id, string status);
}

