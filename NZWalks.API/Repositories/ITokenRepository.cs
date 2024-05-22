using Microsoft.AspNetCore.Identity;

namespace NZWalks.API.Repositories
{
    public interface ITokenRepository
    {
        // if this method is executed, it will give us a token in string format.
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
