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

        public CustomerAccountController(ICustomerAccountService customerAccountService)
        {
            _customerAccountService = customerAccountService;
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
            catch (AlreadyExistCustomerAccountException ex)
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

                await _customerAccountService.ChangePasswordCustomerAccount(id, changePasswordCustomerAccountDto);
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
