using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using server.Utils;
using System.Reflection;

namespace server.Repositories.users
{
    public class GetUserResult
    {
        public IEnumerable<User>? Users { get; set; }
        public User? User { get; set; }
        public string? ErrorMessage { get; set; }
    }
    public class UserRepository : IUserRepository<User>
    {
        private readonly AppDbContext _appDbContext;
        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<GetUserResult> GetUserById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    Console.WriteLine("Invalid user id");
                    return new GetUserResult { ErrorMessage = "Invalid user id" };
                }
                var user = await _appDbContext.Users.FindAsync(id);
                return new GetUserResult { User = user, ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new GetUserResult { Users = null, ErrorMessage = $"Failed to retrieve user. Error: {ex.Message}" };
            }
        }

        public async Task<GetUserResult> GetAllUsers()
        {
            try
            {
                var users = await _appDbContext.Users.ToListAsync();
                return new GetUserResult { Users = users, ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new GetUserResult { Users = null, ErrorMessage = $"Failed to retrieve users. Error: {ex.Message}" };
            }
        }

        public async Task<GetUserResult> AddUser(User user)
        {
            try
            {    
                if(!ValidateUserData(user))
                    return new GetUserResult { User = null, ErrorMessage = "Failed to add user" };

                // Check if user already exist
                if (_appDbContext.Users.Any(u => u.Id == user.Id))
                    return new GetUserResult { User = null, ErrorMessage = "User with the same ID already exists" };

                if (!string.IsNullOrWhiteSpace(user.BirthDate))
                    user.BirthDate = FormatBirthDate(user.BirthDate);

                _appDbContext.Add(user);
                await _appDbContext.SaveChangesAsync();
                return new GetUserResult { User = user, ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new GetUserResult { User = null, ErrorMessage = $"Failed to add user. Error: {ex.Message}" };
            }
        }

        public async Task<GetUserResult> UpdateUser(User updateUser)
        {
            try
            {
                if (!ValidateUserData(updateUser))
                    return new GetUserResult { User = null, ErrorMessage = "Failed to update user" };
                var user = await _appDbContext.Users.FindAsync(updateUser.Id);

                if (user != null)
                {
                    user.Name = updateUser.Name;
                    user.Email = updateUser.Email;
                    if (!string.IsNullOrWhiteSpace(updateUser.BirthDate))
                        user.BirthDate = FormatBirthDate(updateUser.BirthDate);
                    user.Gender = updateUser.Gender;
                    user.Tel = updateUser.Tel;

                    await _appDbContext.SaveChangesAsync();
                    return new GetUserResult { User = user, ErrorMessage = null }; // Return the updated user
                }
                else
                {
                    return new GetUserResult { User = null, ErrorMessage = "User not found" };
                }
            }
            catch (Exception ex)
            {
                return new GetUserResult { User = null, ErrorMessage = $"Failed to update user. Error: {ex.Message}" };
            }
        }

        public async Task<GetUserResult> DeleteUser(string id)
        {
            try
            {
                var user = await _appDbContext.Users.FindAsync(id);

                if (user != null)
                {
                    _appDbContext.Users.Remove(user);
                    await _appDbContext.SaveChangesAsync();

                    return new GetUserResult { User = user, ErrorMessage = null }; // Return the deleted user
                }
                else
                {
                    return new GetUserResult { User = null, ErrorMessage = "User not found" };
                }
            }
            catch (Exception ex)
            {
                return new GetUserResult { User = null, ErrorMessage = $"Failed to delete user. Error: {ex.Message}" };
            }
        }

        public bool ValidateUserData(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Id) || !Utils.Utils.IsNumeric(user.Id) || string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.BirthDate) || (!string.IsNullOrWhiteSpace(user.Tel) && !Utils.Utils.IsNumeric(user.Tel)) || (!string.IsNullOrWhiteSpace(user.Email) && !Utils.Utils.IsValidEmail(user.Email)) || (!string.IsNullOrEmpty(user.Gender) && !IsGenderValid(user.Gender)))
                return false;
            return true;
        }

        public bool IsGenderValid(string gender)
        {
            var validGenders = Enum.GetNames(typeof(Gender)).Select(g => g.ToLower());

            var normalizedGender = gender?.ToLower();

            return validGenders.Contains(normalizedGender);
        }
        private string FormatBirthDate(string birthDate)
        {
            if (DateTime.TryParse(birthDate, out var parsedDate))
            {
                return parsedDate.ToString("dd/MM/yyyy");
            }

            return birthDate;
        }

    }


}
