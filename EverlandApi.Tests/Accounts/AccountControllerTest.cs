using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EverlandApi.Accounts.Controllers;
using EverlandApi.Accounts.Models;
using EverlandApi.Accounts.Services;
using EverlandApi.Core;
using EverlandApi.Core.Models;
using EverlandApi.Core.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace EverlandApi.Tests.Accounts
{
    public class AccountControllerTest
    {
        private IPasswordHasher<Account> _passwordHasher;
        private AccountController _accountController;
        private Account _testAccount;

        public AccountControllerTest()
        {
            _passwordHasher = new BCryptPasswordHasher<Account>(
                new OptionsWrapper<BCryptOptions>(new BCryptOptions())
            );

            _testAccount = new Account
            {
                Id = Guid.NewGuid(),
                Username = "Joshsora",
                Email = "joshsoragaming@gmail.com"
            };
            _testAccount.Password = _passwordHasher.HashPassword(
                _testAccount,
                "password123"
            );

            var accounts = new List<Account>
            {
                _testAccount,
                new Account
                {
                    Id = Guid.NewGuid(),
                    Username = "test.conflict.user",
                    Email = "test.conflict.email@example.com"
                }
            };

            var accountContext = new Mock<IAccountContext>();
            accountContext
                .SetupGet(ac => ac.Accounts)
                .Returns(accounts.AsQueryable().BuildMockDbSet().Object);
            _accountController = new AccountController(
                new AccountService(
                    accountContext.Object,
                    _passwordHasher
                )
            );
        }

        [Fact]
        public async Task Create_ReturnsAnApiResult_WithNewAccountInfo()
        {
            var result = await _accountController.Create(
                new AccountCreationRequest
                {
                    Username = "test.user",
                    Email = "test.email@example.com",
                    Password = "test.password"
                }
            );

            var response = result.Value as ApiResponse<AccountCreationData>;
            Assert.True(response != null);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task CreateBadUsername_ReturnsAnApiResult_WithInvalidError()
        {
            var result = await _accountController.Create(
                new AccountCreationRequest
                {
                    Username = "bad",
                    Email = "real.email@example.com",
                    Password = "good.password"
                }
            );

            var response = result.Value as ApiResponse;
            Assert.True(response != null);
            Assert.False(response.Success);
            Assert.Contains(
                response.Errors,
                e => e.ErrorCode == ApiErrorCode.AccountCreationFailed
            );
        }

        [Fact]
        public async Task CreateBadEmail_ReturnsAnApiResult_WithInvalidError()
        {
            var result = await _accountController.Create(
                new AccountCreationRequest
                {
                    Username = "good.username",
                    Email = "not.an.email",
                    Password = "good.password"
                }
            );

            var response = result.Value as ApiResponse;
            Assert.True(response != null);
            Assert.False(response.Success);
            Assert.Contains(
                response.Errors,
                e => e.ErrorCode == ApiErrorCode.AccountCreationFailed
            );
        }

        [Fact]
        public async Task CreateBadPassword_ReturnsAnApiResult_WithInvalidError()
        {
            var result = await _accountController.Create(
                new AccountCreationRequest
                {
                    Username = "good.username",
                    Email = "not.an.email",
                    Password = "bad"
                }
            );

            var response = result.Value as ApiResponse;
            Assert.True(response != null);
            Assert.False(response.Success);
            Assert.Contains(
                response.Errors,
                e => e.ErrorCode == ApiErrorCode.AccountCreationFailed
            );
        }

        [Fact]
        public async Task Create_ReturnsAnApiResult_WithTakenUsernameError()
        {
            var result = await _accountController.Create(
                new AccountCreationRequest
                {
                    Username = "test.conflict.user",
                    Email = "another.test.email@example.com",
                    Password = "test.password"
                }
            );

            var response = result.Value as ApiResponse;
            Assert.True(response != null);
            Assert.False(response.Success);
            Assert.Contains(
                response.Errors,
                e => e.ErrorCode == ApiErrorCode.AccountUsernameInUse
            );
        }

        [Fact]
        public async Task Create_ReturnsAnApiResult_WithTakenEmailError()
        {
            var result = await _accountController.Create(
                new AccountCreationRequest
                {
                    Username = "another.test.user",
                    Email = "test.conflict.email@example.com",
                    Password = "test.password"
                }
            );

            var response = result.Value as ApiResponse;
            Assert.True(response != null);
            Assert.False(response.Success);
            Assert.Contains(
                response.Errors,
                e => e.ErrorCode == ApiErrorCode.AccountEmailInUse
            );
        }

        private void TestGetResult(ApiResult result)
        {
            Assert.True(result != null);
            var response = result.Value as ApiResponse<AccountRetrievalData>;
            Assert.True(response != null);
            Assert.True(response.Success);
            Assert.Equal(_testAccount.Id, response.Data.Id);
            Assert.Equal(_testAccount.Username, response.Data.Username);
            Assert.Equal(_testAccount.Email, response.Data.Email);
        }

        [Fact]
        public void GetAuthedAccount_ReturnsAnApiResult_WithAccountInfo()
        {
            TestGetResult(_accountController.Get(_testAccount) as ApiResult);
        }

        [Fact]
        public async Task GetById_ReturnsAnApiResult_WithAccountInfo()
        {
            TestGetResult(
                await _accountController.Get(_testAccount.Id) as ApiResult
            );
        }

        [Fact]
        public async Task GetByUsername_ReturnsAnApiResult_WithAccountInfo()
        {
            TestGetResult(
                await _accountController.Get(_testAccount.Username) as ApiResult
            );
        }

        private void TestUpdateEmailSuccessResult(ApiResult result)
        {
            Assert.True(result != null);
            var response = result.Value as ApiResponse;
            Assert.True(response != null);
            Assert.True(response.Success);
            Assert.Equal("another.email@example.com", _testAccount.Email);
        }

        private void TestUpdateEmailTakenResult(ApiResult result)
        {
            Assert.True(result != null);
            var response = result.Value as ApiResponse;
            Assert.True(response != null);
            Assert.False(response.Success);
            Assert.Contains(
                response.Errors,
                e => e.ErrorCode == ApiErrorCode.AccountEmailInUse
            );
            Assert.Equal("joshsoragaming@gmail.com", _testAccount.Email);
        }

        [Fact]
        public async Task UpdateEmailAuthedAccount_ReturnsAnApiResult_WithSuccess()
        {
            TestUpdateEmailSuccessResult(
                await _accountController.Update(
                    _testAccount,
                    new AccountUpdateRequest
                    {
                        Email = "another.email@example.com"
                    }
                ) as ApiResult
            );
        }

        [Fact]
        public async Task UpdateEmailAuthedAccount_ReturnsAnApiResult_WithInUseError()
        {
            TestUpdateEmailTakenResult(
                await _accountController.Update(
                    _testAccount,
                    new AccountUpdateRequest
                    {
                        Email = "test.conflict.email@example.com"
                    }
                ) as ApiResult
            );
        }

        [Fact]
        public async Task UpdateEmailById_ReturnsAnApiResult_WithSuccess()
        {
            TestUpdateEmailSuccessResult(
                await _accountController.Update(
                    _testAccount.Id,
                    new AccountUpdateRequest
                    {
                        Email = "another.email@example.com"
                    }
                ) as ApiResult
            );
        }

        [Fact]
        public async Task UpdateEmailById_ReturnsAnApiResult_WithInUseError()
        {
            TestUpdateEmailTakenResult(
                await _accountController.Update(
                    _testAccount.Id,
                    new AccountUpdateRequest
                    {
                        Email = "test.conflict.email@example.com"
                    }
                ) as ApiResult
            );
        }

        [Fact]
        public async Task UpdateEmailByUsername_ReturnsAnApiResult_WithSuccess()
        {
            TestUpdateEmailSuccessResult(
                await _accountController.Update(
                    _testAccount.Username,
                    new AccountUpdateRequest
                    {
                        Email = "another.email@example.com"
                    }
                ) as ApiResult
            );
        }

        [Fact]
        public async Task UpdateEmailByUsername_ReturnsAnApiResult_WithInUseError()
        {
            TestUpdateEmailTakenResult(
                await _accountController.Update(
                    _testAccount.Username,
                    new AccountUpdateRequest
                    {
                        Email = "test.conflict.email@example.com"
                    }
                ) as ApiResult
            );
        }

        private void TestUpdatePasswordSuccessResult(ApiResult result)
        {
            Assert.True(result != null);
            var response = result.Value as ApiResponse;
            Assert.True(response != null);
            Assert.True(response.Success);
            Assert.Equal(
                PasswordVerificationResult.Success,
                _passwordHasher.VerifyHashedPassword(
                    _testAccount, _testAccount.Password, "a.new.password"
                )
            );
        }

        [Fact]
        public async Task UpdatePasswordAuthedAccount_ReturnsAnApiResult_WithSuccess()
        {
            TestUpdatePasswordSuccessResult(
                await _accountController.Update(
                    _testAccount,
                    new AccountUpdateRequest
                    {
                        Password = "a.new.password"
                    }
                ) as ApiResult
            );
        }

        [Fact]
        public async Task UpdatePasswordById_ReturnsAnApiResult_WithSuccess()
        {
            TestUpdatePasswordSuccessResult(
                await _accountController.Update(
                    _testAccount.Id,
                    new AccountUpdateRequest
                    {
                        Password = "a.new.password"
                    }
                ) as ApiResult
            );
        }

        [Fact]
        public async Task UpdatePasswordByUsername_ReturnsAnApiResult_WithSuccess()
        {
            TestUpdatePasswordSuccessResult(
                await _accountController.Update(
                    _testAccount.Username,
                    new AccountUpdateRequest
                    {
                        Password = "a.new.password"
                    }
                ) as ApiResult
            );
        }

        private void TestDeleteSuccessResult(ApiResult result)
        {
            Assert.True(result != null);
            var response = result.Value as ApiResponse;
            Assert.True(response != null);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task DeleteAuthedAccount_ReturnsAnApiResult_WithSuccess()
        {
            TestDeleteSuccessResult(
                await _accountController.Delete(_testAccount) as ApiResult
            );
        }

        [Fact]
        public async Task DeleteById_ReturnsAnApiResult_WithSuccess()
        {
            TestDeleteSuccessResult(
                await _accountController.Delete(_testAccount.Id) as ApiResult
            );
        }

        [Fact]
        public async Task DeleteByUsername_ReturnsAnApiResult_WithSuccess()
        {
            TestDeleteSuccessResult(
                await _accountController.Delete(_testAccount.Username) as ApiResult
            );
        }
    }
}
