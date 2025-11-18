using PTJ.Application.Common;
using PTJ.Application.DTOs.Company;
using PTJ.Application.Services;
using PTJ.Domain.Entities;
using PTJ.Domain.Interfaces;

namespace PTJ.Infrastructure.Services;

public class CompanyService : ICompanyService
{
    private readonly IUnitOfWork _unitOfWork;

    public CompanyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CompanyDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id, cancellationToken);

        if (company == null)
        {
            return Result<CompanyDto>.FailureResult("Company not found");
        }

        var dto = MapToDto(company);
        return Result<CompanyDto>.SuccessResult(dto);
    }

    public async Task<Result<CompanyDto>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var company = await _unitOfWork.Companies.FirstOrDefaultAsync(
            c => c.OwnerId == userId,
            cancellationToken);

        if (company == null)
        {
            return Result<CompanyDto>.FailureResult("Company not found for this user");
        }

        var dto = MapToDto(company);
        return Result<CompanyDto>.SuccessResult(dto);
    }

    public async Task<Result<PaginatedList<CompanyDto>>> GetAllAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var allCompanies = await _unitOfWork.Companies.GetAllAsync(cancellationToken);

        var totalCount = allCompanies.Count();
        var items = allCompanies
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToDto)
            .ToList();

        var result = new PaginatedList<CompanyDto>(items, totalCount, pageNumber, pageSize);

        return Result<PaginatedList<CompanyDto>>.SuccessResult(result);
    }

    public async Task<Result<CompanyDto>> CreateAsync(int userId, CreateCompanyDto dto, CancellationToken cancellationToken = default)
    {
        // Check if user already has a company
        var existingCompany = await _unitOfWork.Companies.FirstOrDefaultAsync(
            c => c.OwnerId == userId,
            cancellationToken);

        if (existingCompany != null)
        {
            return Result<CompanyDto>.FailureResult("User already has a company");
        }

        // Check if tax code already exists
        if (!string.IsNullOrEmpty(dto.TaxCode))
        {
            var companyWithTaxCode = await _unitOfWork.Companies.FirstOrDefaultAsync(
                c => c.TaxCode == dto.TaxCode,
                cancellationToken);

            if (companyWithTaxCode != null)
            {
                return Result<CompanyDto>.FailureResult("Tax code already exists");
            }
        }

        var company = new Company
        {
            OwnerId = userId,
            Name = dto.Name,
            Description = dto.Description,
            Address = dto.Address,
            Website = dto.Website,
            TaxCode = dto.TaxCode,
            Industry = dto.Industry,
            EmployeeCount = dto.EmployeeCount,
            FoundedYear = dto.FoundedYear,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Companies.AddAsync(company, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var responseDto = MapToDto(company);

        return Result<CompanyDto>.SuccessResult(responseDto, "Company created successfully");
    }

    public async Task<Result<CompanyDto>> UpdateAsync(int id, int userId, CreateCompanyDto dto, CancellationToken cancellationToken = default)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id, cancellationToken);

        if (company == null)
        {
            return Result<CompanyDto>.FailureResult("Company not found");
        }

        // Check ownership
        if (company.OwnerId != userId)
        {
            return Result<CompanyDto>.FailureResult("You don't have permission to update this company");
        }

        // Check if tax code already exists (for other companies)
        if (!string.IsNullOrEmpty(dto.TaxCode) && dto.TaxCode != company.TaxCode)
        {
            var companyWithTaxCode = await _unitOfWork.Companies.FirstOrDefaultAsync(
                c => c.TaxCode == dto.TaxCode && c.Id != id,
                cancellationToken);

            if (companyWithTaxCode != null)
            {
                return Result<CompanyDto>.FailureResult("Tax code already exists");
            }
        }

        // Update fields
        company.Name = dto.Name;
        company.Description = dto.Description;
        company.Address = dto.Address;
        company.Website = dto.Website;
        company.TaxCode = dto.TaxCode;
        company.Industry = dto.Industry;
        company.EmployeeCount = dto.EmployeeCount;
        company.FoundedYear = dto.FoundedYear;
        company.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Companies.Update(company);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var responseDto = MapToDto(company);

        return Result<CompanyDto>.SuccessResult(responseDto, "Company updated successfully");
    }

    public async Task<Result> DeleteAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(id, cancellationToken);

        if (company == null)
        {
            return Result.FailureResult("Company not found");
        }

        // Check ownership
        if (company.OwnerId != userId)
        {
            return Result.FailureResult("You don't have permission to delete this company");
        }

        // Check if company has job posts
        var jobPosts = await _unitOfWork.JobPosts.FindAsync(jp => jp.CompanyId == id, cancellationToken);
        if (jobPosts.Any())
        {
            return Result.FailureResult("Cannot delete company with existing job posts");
        }

        _unitOfWork.Companies.Remove(company);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.SuccessResult("Company deleted successfully");
    }

    private CompanyDto MapToDto(Company company)
    {
        return new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            Description = company.Description,
            Address = company.Address,
            Website = company.Website,
            LogoUrl = company.LogoUrl,
            TaxCode = company.TaxCode,
            Industry = company.Industry,
            EmployeeCount = company.EmployeeCount,
            FoundedYear = company.FoundedYear,
            IsVerified = company.IsVerified,
            CreatedAt = company.CreatedAt
        };
    }
}
