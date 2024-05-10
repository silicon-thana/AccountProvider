using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace AccountProvider.Functions;

public class Verification(ILogger<Verification> logger, UserManager<UserAccount> userManager)
{
    private readonly ILogger<Verification> _logger = logger;
    private readonly UserManager<UserAccount> _userManager = userManager;

    [Function("Verification")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function,"post")] HttpRequest req)
    {
        string body = null!;
        try
        {
            body = await new StreamReader(req.Body).ReadToEndAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR : Verification.Run.StreamReader :: {ex.Message}");
        }
        if (body != null)
        {
            VerificationRequest verifyRequest = null!;

            try
            {
                verifyRequest = JsonConvert.DeserializeObject<VerificationRequest>(body)!;
            }
            catch (Exception ex)
            {
                _logger.LogError($"ERROR : JsonConvert.DeserializeObject<VerificationRequest> :: {ex.Message}");
            }

            if (verifyRequest != null && !string.IsNullOrEmpty(verifyRequest.Email) && !string.IsNullOrEmpty(verifyRequest.VerificationCode))
            {
                //VerificationProvider

                using var http = new HttpClient();
                StringContent content = new StringContent(JsonConvert.SerializeObject(verifyRequest), Encoding.UTF8, "application/json");
                //var response = await http.PutAsync("", content);

                if (true)
                {
                    var userAccount = await _userManager.FindByEmailAsync(verifyRequest.Email);
                    if (userAccount != null)
                    {
                        userAccount.EmailConfirmed = true;
                        await _userManager.UpdateAsync(userAccount);
                        if (await _userManager.IsEmailConfirmedAsync(userAccount))
                        {
                            return new OkResult();
                        }
                    }
                }
            }
        }
        return new UnauthorizedResult();

    }
}
