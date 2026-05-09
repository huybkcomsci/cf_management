using CafeManagement.DTOs;
using CafeManagement.Models.Entities;
using CafeManagement.Repositories;
using CafeManagement.Data;
using System.Linq;

namespace CafeManagement.Services;

public class ProductService : Services.Interfaces.IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ApplicationDbContext _context;

    public ProductService(IProductRepository productRepository, ApplicationDbContext context)
    {
        _productRepository = productRepository;
        _context = context;
    }

    public async Task<ProductDto> CreateAsync(ProductDto dto)
    {
        var entity = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price
        };

        await _productRepository.AddAsync(entity);
        await _context.SaveChangesAsync();

        dto.Id = entity.Id;
        return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _productRepository.GetByIdAsync(id);
        if (entity is null) return;
        _productRepository.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var list = await _productRepository.GetAllAsync();
        return list.Select(p => new ProductDto { Id = p.Id, Name = p.Name, Description = p.Description, Price = p.Price });
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var p = await _productRepository.GetByIdAsync(id);
        if (p is null) return null;
        return new ProductDto { Id = p.Id, Name = p.Name, Description = p.Description, Price = p.Price };
    }

    public async Task UpdateAsync(ProductDto dto)
    {
        var entity = await _productRepository.GetByIdAsync(dto.Id);
        if (entity is null) return;
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Price = dto.Price;
        _productRepository.Update(entity);
        await _context.SaveChangesAsync();
    }
}
