namespace Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Guid userId, string email, string fullName, IEnumerable<string>? roles = null);
}
