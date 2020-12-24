using vts.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace vts.Repositories
{
    public interface IAccountRepository
    {
        Task<IdentityResult> CreateUserAsync(RegisterViewModel userModel);
        Task<bool> IsUserExist(string email);
        Task<SignInResult> Login(LoginViewModel userModel);
        Task Logout();
        Task<string[]> ResetPassword(string email);
        Task<IdentityResult> ResetPassword(User user, ResetPasswordViewModel userModel);
        Task<User> FindByIdAsync(string uid);
        Task<User> FindByEmailAsync(string email);
    }
}
