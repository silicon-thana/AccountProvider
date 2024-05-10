using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AccountProvider.Functions;

public class SignIn(ILogger<SignIn> logger, SignInManager<UserAccount> signInManager)
{
    private readonly ILogger<SignIn> _logger = logger;
    private readonly SignInManager<UserAccount> _signInManager = signInManager;

    [Function("SignIn")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        string body = null!;
        try
        {
            body = await new StreamReader(req.Body).ReadToEndAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : SignIn.Run.StreamReader :: {ex.Message}");
        }
        if (body != null)
        {

            UserLogInRequest userLogInRequest = null!;

            try
            {
                userLogInRequest = JsonConvert.DeserializeObject<UserLogInRequest>(body)!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : JsonConvert.DeserializeObject<UserLogInRequest> :: {ex.Message}");
            }

            if (userLogInRequest != null && !string.IsNullOrEmpty(userLogInRequest.Email) && !string.IsNullOrEmpty(userLogInRequest.Password))
            {
                try
                {
                    var result = await _signInManager.PasswordSignInAsync(userLogInRequest.Email, userLogInRequest.Password, userLogInRequest.IsPresistent, false);
                    if (result.Succeeded)
                    {

                        //Get Token from TokenProvider
                        return new OkObjectResult("accestoken");
                    }
                    return new UnauthorizedResult();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ERROR : await _signInManager.PasswordSignInAsync :: {ex.Message}");
                }

            }


            return new BadRequestResult();
            }
    }
}
