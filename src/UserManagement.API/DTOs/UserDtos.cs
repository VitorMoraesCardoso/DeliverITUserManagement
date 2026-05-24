namespace UserManagement.API.DTOs;

public record CreateUserDto(string Nome, string Email, DateTime DataNascimento);

public record UpdateUserDto(string Nome, string Email, DateTime DataNascimento);

public record UserResponseDto(
    int Id,
    string Nome,
    string Email,
    DateTime DataNascimento,
    DateTime DataCriacao,
    DateTime DataEdicao
);

public record PagedResponseDto<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);