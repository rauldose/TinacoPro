using TinacoPro.Application.DTOs;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;

namespace TinacoPro.Application.Services;

public class SiteService
{
    private readonly ISiteRepository _repository;

    public SiteService(ISiteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<SiteDto>> GetAllSitesAsync()
    {
        var sites = await _repository.GetAllAsync();
        return sites.Select(s => new SiteDto
        {
            Id = s.Id,
            Name = s.Name,
            Code = s.Code,
            Address = s.Address,
            City = s.City,
            State = s.State,
            Phone = s.Phone,
            ManagerName = s.ManagerName,
            Notes = s.Notes,
            IsActive = s.IsActive
        });
    }

    public async Task<SiteDto?> GetSiteByIdAsync(int id)
    {
        var site = await _repository.GetByIdAsync(id);
        if (site == null) return null;

        return new SiteDto
        {
            Id = site.Id,
            Name = site.Name,
            Code = site.Code,
            Address = site.Address,
            City = site.City,
            State = site.State,
            Phone = site.Phone,
            ManagerName = site.ManagerName,
            Notes = site.Notes,
            IsActive = site.IsActive
        };
    }

    public async Task<SiteDto> CreateSiteAsync(CreateSiteDto dto)
    {
        var site = new Site
        {
            Name = dto.Name,
            Code = dto.Code,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            Phone = dto.Phone,
            ManagerName = dto.ManagerName,
            Notes = dto.Notes,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(site);

        return new SiteDto
        {
            Id = created.Id,
            Name = created.Name,
            Code = created.Code,
            Address = created.Address,
            City = created.City,
            State = created.State,
            Phone = created.Phone,
            ManagerName = created.ManagerName,
            Notes = created.Notes,
            IsActive = created.IsActive
        };
    }

    public async Task UpdateSiteAsync(SiteDto dto)
    {
        var site = await _repository.GetByIdAsync(dto.Id);
        if (site != null)
        {
            site.Name = dto.Name;
            site.Code = dto.Code;
            site.Address = dto.Address;
            site.City = dto.City;
            site.State = dto.State;
            site.Phone = dto.Phone;
            site.ManagerName = dto.ManagerName;
            site.Notes = dto.Notes;
            site.IsActive = dto.IsActive;
            site.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(site);
        }
    }

    public async Task DeleteSiteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
