using PetPal.Models.Db;

namespace PetPal.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
