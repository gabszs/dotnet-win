namespace DotnetWin.Api.Application.DTOs;

public class CreateProductRequestDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
