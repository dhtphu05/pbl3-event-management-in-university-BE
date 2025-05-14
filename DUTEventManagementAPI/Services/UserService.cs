using Microsoft.AspNetCore.Identity;
using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public List<AppUser> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            return users;
        }
        public AppUser Update(string userId, AppUser updatedUser)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null)
            {
                throw new Exception("User not found");
            }
            user.FullName = updatedUser.FullName ?? user.FullName;
            user.StudentId = updatedUser.StudentId ?? user.StudentId;
            user.Email = updatedUser.Email ?? user.Email;
            user.UserName = updatedUser.UserName ?? user.UserName;
            user.PhoneNumber = updatedUser.PhoneNumber ?? user.PhoneNumber;
            user.Class = updatedUser.Class ?? user.Class;
            user.FacultyId = updatedUser.FacultyId ?? user.FacultyId;
            user.UserImageUrl = updatedUser.UserImageUrl ?? user.UserImageUrl;
            var result = _userManager.UpdateAsync(user).Result;
            if (!result.Succeeded)
            {
                throw new Exception("Failed to update user");
            }
            return user;
        }
        public bool Delete(string userId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var result = _userManager.DeleteAsync(user);
            if (!result.Result.Succeeded)
            {
                throw new Exception("Failed to delete user");
            }
            return true;
        }
    }
}
