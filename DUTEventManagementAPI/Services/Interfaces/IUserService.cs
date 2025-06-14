﻿using DUTEventManagementAPI.Models;

namespace DUTEventManagementAPI.Services.Interfaces
{
    public interface IUserService
    {
        List<AppUser> GetAllUsers();
        AppUser Update(string userId, AppUser updatedUser);
        bool Delete(string userId);
    }
}