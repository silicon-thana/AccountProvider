using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace AccountProvider.Functions
{
    public class SignUp(ILogger<SignUp> logger, UserManager<UserAccount> userManager)
    {
        private readonly ILogger<SignUp> _logger = logger;
        private readonly UserManager<UserAccount> _userManager = userManager;

        [Function("SignUp")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {

            string body = null!;
            try
            {
                body = await new StreamReader(req.Body).ReadToEndAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : SignUp.Run.StreamReader :: {ex.Message}");
            }


            if (body != null)
            {

                UserRegRequest userRegRequest = null!;

                try
                {
                    userRegRequest = JsonConvert.DeserializeObject<UserRegRequest>(body)!;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ERROR : JsonConvert.DeserializeObject<UserRegRequest> :: {ex.Message}");
                }


                if (userRegRequest != null && !string.IsNullOrEmpty(userRegRequest.Email) && !string.IsNullOrEmpty(userRegRequest.Password))
                {
                    if (! await _userManager.Users.AnyAsync(x=>x.Email == userRegRequest.Email))
                    {
                        var userAccount = new UserAccount
                        {
                            Firstname = userRegRequest.Email,
                            Lastname = userRegRequest.Lastname,
                            Email = userRegRequest.Email,
                            UserName = userRegRequest.Email
                        };

                        try
                        {
                            var result = await _userManager.CreateAsync(userAccount, userRegRequest.Password);
                            if (result.Succeeded)
                            {
                                //HTTP REQUEST - Väntar på svar
                                try
                                {
                                    using var http = new HttpClient();
                                    StringContent content = new StringContent(JsonConvert.SerializeObject(new { Email = userAccount.Email}), Encoding.UTF8, "application/json");
                                    var response = await http.PutAsync("", content);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError($"ERROR : http.PutAsync :: {ex.Message}");
                                }
                                return new OkResult();
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"ERROR : _userManager.CreateAsync :: {ex.Message}");
                        }
                    }
                    else
                    {
                        return new ConflictResult();
                    }
                }

            }
            return new BadRequestResult();
        }
    }
}
