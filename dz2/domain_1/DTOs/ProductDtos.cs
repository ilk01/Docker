using System.ComponentModel.DataAnnotations;

namespace mongo.DTOs;

public record CreateProductDto([Required] string Name, [Required] double Price, bool InStock);
public record UpdateProductDto([Required] string Name, [Required] double Price, bool InStock);
