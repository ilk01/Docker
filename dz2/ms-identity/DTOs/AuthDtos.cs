using System.ComponentModel.DataAnnotations;

namespace ms_identity.DTOs;

public record RegisterDto([Required] string Username, [Required] string Password);
public record LoginDto([Required] string Username, [Required] string Password);
public record AuthResponseDto(string Token);
