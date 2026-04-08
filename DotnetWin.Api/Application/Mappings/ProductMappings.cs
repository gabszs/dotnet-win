using DotnetWin.Api.Application.DTOs;
using DotnetWin.Api.Domain.Entities;

namespace DotnetWin.Api.Application.Mappings;

public static class ProductMappings
{
    public static ProductResponseDto ToResponseDto(this Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CreatedAt = product.CreatedAt
        };
    }
}
