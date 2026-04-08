using System.Text.Json;
using DotnetWin.Api.Application.DTOs;
using DotnetWin.Api.Application.Interfaces;
using DotnetWin.Api.Application.Mappings;
using DotnetWin.Api.Domain.Entities;
using DotnetWin.Api.Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace DotnetWin.Api.Application.Services;

public class ProductService : IProductService
{
    private const string AllProductsCacheKey = "products:all";
    private readonly IProductRepository _repository;
    private readonly IDistributedCache _cache;

    public ProductService(IProductRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<List<ProductResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cached = await _cache.GetStringAsync(AllProductsCacheKey, cancellationToken);
        if (!string.IsNullOrWhiteSpace(cached))
        {
            var products = JsonSerializer.Deserialize<List<ProductResponseDto>>(cached);
            if (products is not null)
            {
                return products;
            }
        }

        var result = await _repository.GetAllAsync(cancellationToken);
        var dtoResult = result.Select(product => product.ToResponseDto()).ToList();

        await _cache.SetStringAsync(
            AllProductsCacheKey,
            JsonSerializer.Serialize(dtoResult),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },
            cancellationToken);

        return dtoResult;
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        return product?.ToResponseDto();
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductRequestDto request, CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Name = request.Name,
            Price = request.Price
        };

        var created = await _repository.CreateAsync(product, cancellationToken);
        await InvalidateProductListCacheAsync(cancellationToken);
        return created.ToResponseDto();
    }

    public async Task<ProductResponseDto?> UpdateAsync(int id, UpdateProductRequestDto request, CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Id = id,
            Name = request.Name,
            Price = request.Price
        };

        var updated = await _repository.UpdateAsync(product, cancellationToken);

        if (updated is not null)
        {
            await InvalidateProductListCacheAsync(cancellationToken);
        }

        return updated?.ToResponseDto();
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _repository.DeleteAsync(id, cancellationToken);
        if (deleted)
        {
            await InvalidateProductListCacheAsync(cancellationToken);
        }

        return deleted;
    }

    private Task InvalidateProductListCacheAsync(CancellationToken cancellationToken)
    {
        return _cache.RemoveAsync(AllProductsCacheKey, cancellationToken);
    }
}
