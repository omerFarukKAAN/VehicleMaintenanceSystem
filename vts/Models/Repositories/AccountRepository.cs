using vts.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace vts.Repositories
{
    public class AccountRepository : IAccountRepository
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountRepository(UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> CreateUserAsync(RegisterViewModel userModel)
        {
            var newUser = new User
            {
                UserName = userModel.Email,
                Email = userModel.Email,
                Name = userModel.FirstName,
                Surname = userModel.LastName,
                Vehicles = new List<Vehicle>()
            };

            var result = await _userManager.CreateAsync(newUser, userModel.Password);

            return result;
        }

        public async Task<bool> IsUserExist(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user != null;
        }

        public async Task<SignInResult> Login(LoginViewModel userModel)
        {
            var user = await _userManager.FindByEmailAsync(userModel.Email);

            var result = await _signInManager.CheckPasswordSignInAsync(user, userModel.Password, true);
           
            if(result.Succeeded)
            {
                await _signInManager.SignInAsync(user, userModel.RememberMe);
            }
            
            return result;

        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<string[]> ResetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                string[] result =
                {
                    await _userManager.GeneratePasswordResetTokenAsync(user),
                    user.Id
                };
                 
                return result;
            }

            return null;
        }

        public async Task<IdentityResult> ResetPassword(User user, ResetPasswordViewModel userModel)
        {
            var result = await _userManager.ResetPasswordAsync(user,
                    userModel.Token,
                    userModel.Password);

            return result; 
        }

        public async Task<User> FindByIdAsync(string uid)
        {
            var user = await  _userManager.FindByIdAsync(uid);

            return user;
        }

        public async Task<User> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }
    }
}
