using Jawel_be.Dtos.CustomerAccount;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Jawel_be.Services.CustomerAccountService;
using Jawel_be.Utils;
using Jawel_be.Validators.CustomerAccount;
using Microsoft.AspNetCore.Mvc;

namespace Jawel_be.Controllers
{
    [ApiController]
    [Route("api/customer-accounts")]
    public class CustomerAccountController : ControllerBase
    {
        private readonly ICustomerAccountService _customerAccountService;
        private readonly IConfiguration _config;

        public CustomerAccountController(ICustomerAccountService customerAccountService, IConfiguration config)
        {
            _customerAccountService = customerAccountService;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerAccounts()
        {
            var customerAccounts = await _customerAccountService.GetCustomerAccounts();
            return Ok(customerAccounts.Select(u => u.AsDto()).ToList());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerAccount(int id)
        {
            var customerAccount = await _customerAccountService.GetCustomerAccountById(id);

            if (customerAccount == null)
            {
                return NotFound();
            }

            return Ok(customerAccount.AsDto());

        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerAccount([FromBody] CreateCustomerAccountDto createCustomerAccountDto)
        {
            try
            {
                var validator = new CreateCustomerAccountDtoValidator();
                var validationResult = await validator.ValidateAsync(createCustomerAccountDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                CustomerAccount newCustomerAccount = await _customerAccountService.CreateCustomerAccount(createCustomerAccountDto);
                return CreatedAtAction(nameof(GetCustomerAccount), new { id = newCustomerAccount.Id }, newCustomerAccount.AsDto());
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCustomerAccountDto loginCustomerAccountDto)
        {
            var validator = new LoginCustomerAccountDtoValidator();
            var validationResult = await validator.ValidateAsync(loginCustomerAccountDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var hashPassword = HashPassword.GetMD5(loginCustomerAccountDto.Password);
            var customerAccount = await _customerAccountService.GetCustomerAccountByPhoneAndPassword(loginCustomerAccountDto.Phone, hashPassword);
            
            if (customerAccount != null)
            {
                Jwt jwt = new Jwt(_config);
                var loginResultDto = new LoginResultCustomerAccountDto()
                {
                    Id = customerAccount.Id,
                    Phone = customerAccount.Phone,
                    Name = customerAccount.Name,
                    Gender = customerAccount.Gender,
                    Avatar = customerAccount.Avatar,
                    Address= customerAccount.Address,
                    Token = jwt.GenerateToken(customerAccount)
                };
                return Ok(loginResultDto);
            } else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerAccount(int id, [FromBody] UpdateCustomerAccountDto updateCustomerAccountDto)
        {
            try
            {
                var validator = new UpdateCustomerAccountDtoValidator();
                var validationResult = await validator.ValidateAsync(updateCustomerAccountDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var customerAccount = await _customerAccountService.UpdateCustomerAccount(id, updateCustomerAccountDto);
                return Ok(customerAccount.AsDto());
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound();
            }
        }


        [HttpPut("change-password/{id}")]
        public async Task<IActionResult> ChangePasswordCustomerAccount(int id, [FromBody] ChangePasswordCustomerAccountDto changePasswordCustomerAccountDto)
        {
            try
            {
                var validator = new ChangePasswordCustomerAccountDtoValidator();
                var validationResult = await validator.ValidateAsync(changePasswordCustomerAccountDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var hashPassword = new ChangePasswordCustomerAccountDto()
                {
                    CurrentPassword = HashPassword.GetMD5(changePasswordCustomerAccountDto.CurrentPassword),
                    NewPassword = HashPassword.GetMD5(changePasswordCustomerAccountDto.NewPassword)
                };

                await _customerAccountService.ChangePasswordCustomerAccount(id, hashPassword);
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
        public async Task<IActionResult> DeleteCustomerAccount(int id)
        {
            try
            {
                await _customerAccountService.DeleteCustomerAccount(id);
                return Ok();
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound();
            }
        }
    }
}
