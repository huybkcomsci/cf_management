using AutoMapper;
using CafeManagement.DTOs;
using CafeManagement.Models.Entities;
using CafeManagement.Repositories;
using CafeManagement.Services.Interfaces;

namespace CafeManagement.Services;

public class SanphamService : ISanphamService
{
    private readonly ISanphamRepository _repository;
    private readonly IMapper _mapper;

    public SanphamService(ISanphamRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SanphamIndexVm> GetPagedAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var totalItems = await _repository.CountAsync(search, cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var items = await _repository.GetPagedAsync(search, page, pageSize, cancellationToken);

        return new SanphamIndexVm
        {
            Search = search,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages,
            Items = _mapper.Map<IReadOnlyList<SanphamListItemDto>>(items)
        };
    }

    public async Task<SanphamDetailsDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<SanphamDetailsDto>(entity);
    }

    public async Task<IReadOnlyList<NhomOptionDto>> GetNhomOptionsAsync(CancellationToken cancellationToken = default)
    {
        var nhoms = await _repository.GetNhomListAsync(cancellationToken);
        return nhoms.Select(x => new NhomOptionDto { Id = x.Id, TenNhom = x.TenNhom }).ToList();
    }

    public async Task<Guid> CreateAsync(SanphamUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<Sanpham>(dto);
        entity.Id = Guid.NewGuid();
        entity.NgayTao = DateTime.UtcNow;
        entity.NgayCapNhat = DateTime.UtcNow;

        await _repository.AddAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, SanphamUpsertDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        _mapper.Map(dto, entity);
        entity.NgayCapNhat = DateTime.UtcNow;

        await _repository.UpdateAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        await _repository.DeleteAsync(entity, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}