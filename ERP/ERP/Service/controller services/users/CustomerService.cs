using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERP.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ERP.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ApplicationDbContext dbContext, ILogger<CustomerService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Guid> AddCustomer(ApplicationDbContext.Customer customer)
        {
            try
            {
                customer.Id = Guid.NewGuid();
                await _dbContext.Customers.AddAsync(customer);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Customer {Name} added successfully with ID {CustomerId}.", customer.Name, customer.Id);
                return customer.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add customer {Name}.", customer.Name);
                return Guid.Empty;
            }
        }

        public async Task<bool> UpdateCustomer(Guid customerId, ApplicationDbContext.Customer updatedCustomer)
        {
            try
            {
                var customer = await _dbContext.Customers.FindAsync(customerId);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found.", customerId);
                    return false;
                }

                customer.Name = updatedCustomer.Name;
                customer.Email = updatedCustomer.Email;
                customer.Phone = updatedCustomer.Phone;
                customer.Address = updatedCustomer.Address;
                customer.Company = updatedCustomer.Company;
                customer.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Customer with ID {CustomerId} updated successfully.", customerId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update customer with ID {CustomerId}.", customerId);
                return false;
            }
        }

        public async Task<bool> DeleteCustomer(Guid customerId)
        {
            try
            {
                var customer = await _dbContext.Customers.FindAsync(customerId);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found.", customerId);
                    return false;
                }

                _dbContext.Customers.Remove(customer);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Customer with ID {CustomerId} deleted successfully.", customerId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete customer with ID {CustomerId}.", customerId);
                return false;
            }
        }

        public async Task<ApplicationDbContext.Customer?> GetCustomerByIdAsync(Guid customerId)
        {
            return await _dbContext.Customers.FindAsync(customerId);
        }

        public async Task<List<ApplicationDbContext.Customer>> GetAllCustomersAsync()
        {
            return await _dbContext.Customers.ToListAsync();
        }
    }

    public interface ICustomerService
    {
        Task<Guid> AddCustomer(ApplicationDbContext.Customer customer);
        Task<bool> UpdateCustomer(Guid customerId, ApplicationDbContext.Customer updatedCustomer);
        Task<bool> DeleteCustomer(Guid customerId);
        Task<ApplicationDbContext.Customer?> GetCustomerByIdAsync(Guid customerId);
        Task<List<ApplicationDbContext.Customer>> GetAllCustomersAsync();
    }
}
