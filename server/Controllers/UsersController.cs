using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using server.Repositories.users;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository<User> _userRepository;

        public UsersController(IUserRepository<User> userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser(User user)
        {
            try
            {
                if (user != null)
                {
                    var res = await _userRepository.AddUser(user);
                    if (res.ErrorMessage != null)
                    {
                        Console.WriteLine(res.ErrorMessage);
                        return HandleException(res.ErrorMessage);
                    }
                    if (res.User == null)
                        return HandleException("Failed to add user.");
                    return Ok(new { Message = "User added successfully" });
                }
                else
                {
                    return HandleException("Failed to add user.");
                }

            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        [HttpGet("get-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var res = await _userRepository.GetAllUsers();
                if (res.ErrorMessage != null)
                {
                    Console.WriteLine(res.ErrorMessage);
                    return HandleException(res.ErrorMessage);
                }
                if (res.Users == null)
                    return HandleException("Failed to get users.");
                return Ok(res.Users);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        [HttpGet("get-user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var res = await _userRepository.GetUserById(id);
                if (res.ErrorMessage != null)
                {
                    Console.WriteLine(res.ErrorMessage);
                    return HandleException(res.ErrorMessage);
                }
                if(res.User == null)
                    return HandleException("Failed to get user.");
                return Ok(res.User);
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser(User updatedUser)
        {
            try
            {
                if (updatedUser == null)
                {
                    return BadRequest("Invalid user data");
                }

                var  res = await _userRepository.UpdateUser(updatedUser);
                if (res.ErrorMessage != null)
                {
                    Console.WriteLine(res.ErrorMessage);
                    return HandleException(res.ErrorMessage);
                }
                if (res.User == null)
                    return HandleException("Failed to update user.");
                return Ok(new { Message = "User updated successfully" });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(new { ErrorMessage = $"Failed to update user. Error: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }

        [HttpDelete("remove-user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var res = await _userRepository.DeleteUser(id);
                if (res.ErrorMessage != null)
                {
                    Console.WriteLine(res.ErrorMessage);
                    return HandleException(res.ErrorMessage);
                }
                if (res.User == null)
                    return HandleException("Failed to remove user.");
                return Ok(new { Message = "User removed successfully" });
            }
            catch (Exception ex)
            {
                return HandleException(ex.Message);
            }
        }
        private IActionResult HandleException(string ex)
        {
            var errorResponse = new { ErrorMessage = $"Internal Server Error: {ex}" };

            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }
}