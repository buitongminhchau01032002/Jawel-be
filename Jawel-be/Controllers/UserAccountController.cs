using Jawel_be.Dtos.UserAccount;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Jawel_be.Services.UserAccountService;
using Jawel_be.Utils;
using Jawel_be.Validators.UserAccount;
using Microsoft.AspNetCore.Mvc;

namespace Jawel_be.Controllers
{
    [ApiController]
    [Route("api/user-accounts")]
    public class UserAccountController : ControllerBase
    {
        private readonly IUserAccountService _userAccountService;
        private readonly IConfiguration _config;

        public UserAccountController(IUserAccountService userAccountService, IConfiguration config)
        {
            _userAccountService = userAccountService;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAccounts()
        {
            var userAccounts = await _userAccountService.GetUserAccounts();
            return Ok(userAccounts.Select(u => u.AsDto()).ToList());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAccount(int id)
        {
            var userAccount = await _userAccountService.GetUserAccountById(id);

            if (userAccount == null)
            {
                return NotFound();
            }

            return Ok(userAccount.AsDto());

        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAccount([FromBody] CreateUserAccountDto createUserAccountDto)
        {
            try
            {
                var validator = new CreateUserAccountDtoValidator();
                var validationResult = await validator.ValidateAsync(createUserAccountDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                UserAccount newUserAccount = await _userAccountService.CreateUserAccount(createUserAccountDto);
                return CreatedAtAction(nameof(GetUserAccount), new { id = newUserAccount.Id }, newUserAccount.AsDto());
            }
            catch (AlreadyExistUserAccountException ex)
            {
                return BadRequest();
            }
        }
       

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserAccount(int id, [FromBody] UpdateUserAccountDto updateUserAccountDto)
        {
            try
            {
                var validator = new UpdateUserAccountDtoValidator();
                var validationResult = await validator.ValidateAsync(updateUserAccountDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var userAccount = await _userAccountService.UpdateUserAccount(id, updateUserAccountDto);
                return Ok(userAccount.AsDto());
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound();
            }
        }

        [HttpPut("change-password/{id}")]
        public async Task<IActionResult> ChangePasswordUserAccount(int id, [FromBody] ChangePasswordUserAccountDto changePasswordUserAccountDto)
        {
            try
            {
                var validator = new ChangePasswordUserAccountDtoValidator();
                var validationResult = await validator.ValidateAsync(changePasswordUserAccountDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                await _userAccountService.ChangePasswordUserAccount(id, changePasswordUserAccountDto);
                return Ok();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound();
            }
            catch (CurrentPasswordIncorrectException ex)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAccount(int id)
        {
            try
            {
                await _userAccountService.DeleteUserAccount(id);
                return Ok();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound();
            }
        }
    }
}
