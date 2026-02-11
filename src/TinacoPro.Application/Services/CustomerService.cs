using TinacoPro.Application.DTOs;
using TinacoPro.Domain.Entities;
using TinacoPro.Domain.Interfaces;

namespace TinacoPro.Application.Services;

public class CustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _repository.GetAllAsync();
        return customers.Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            ContactName = c.ContactName,
            Phone = c.Phone,
            Email = c.Email,
            Address = c.Address,
            City = c.City,
            Zone = c.Zone,
            Notes = c.Notes,
            IsActive = c.IsActive
        });
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
    {
        var customer = await _repository.GetByIdAsync(id);
        if (customer == null) return null;

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            ContactName = customer.ContactName,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address,
            City = customer.City,
            Zone = customer.Zone,
            Notes = customer.Notes,
            IsActive = customer.IsActive
        };
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            Name = dto.Name,
            ContactName = dto.ContactName,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            City = dto.City,
            Zone = dto.Zone,
            Notes = dto.Notes,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(customer);

        return new CustomerDto
        {
            Id = created.Id,
            Name = created.Name,
            ContactName = created.ContactName,
            Phone = created.Phone,
            Email = created.Email,
            Address = created.Address,
            City = created.City,
            Zone = created.Zone,
            Notes = created.Notes,
            IsActive = created.IsActive
        };
    }

    public async Task UpdateCustomerAsync(CustomerDto dto)
    {
        var customer = await _repository.GetByIdAsync(dto.Id);
        if (customer != null)
        {
            customer.Name = dto.Name;
            customer.ContactName = dto.ContactName;
            customer.Phone = dto.Phone;
            customer.Email = dto.Email;
            customer.Address = dto.Address;
            customer.City = dto.City;
            customer.Zone = dto.Zone;
            customer.Notes = dto.Notes;
            customer.IsActive = dto.IsActive;
            customer.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(customer);
        }
    }

    public async Task DeleteCustomerAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
