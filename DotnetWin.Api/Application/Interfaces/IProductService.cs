using DotnetWin.Api.Application.DTOs;

namespace DotnetWin.Api.Application.Interfaces;

public interface IProductService
{
    Task<List<ProductResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductResponseDto> CreateAsync(CreateProductRequestDto request, CancellationToken cancellationToken = default);
    Task<ProductResponseDto?> UpdateAsync(int id, UpdateProductRequestDto request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
