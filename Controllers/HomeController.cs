using AzureTranslator.Services;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace AzureTranslator.Controllers;

[Route("home")]
[ApiController]
public class HomeController : ControllerBase
{
    private readonly TranslatorService _translatorService;

    private readonly ComputerVisionService _computerVisionService;
    public HomeController(IConfiguration configuration)
    {
        var subscriptionKeyTranslator = configuration["Azure:Translator:TranslatorSubscriptionKey"];
        var endpointTranslator = configuration["Azure:Translator:TranslatorEndpoint"];
        var region = configuration["Azure:Translator:Region"];

        var subscriptionKeyConputerVision = configuration["Azure:AzureCognitiveServices:ComputerVision:SubscriptionKey"];
        var endpointComputerVision = configuration["Azure:AzureCognitiveServices:ComputerVision:Endpoint"];

        _translatorService = new TranslatorService(subscriptionKeyTranslator, endpointTranslator, region);
        _computerVisionService = new ComputerVisionService(subscriptionKeyConputerVision, endpointComputerVision);
    }

    [HttpPost("text")]
    public async Task<ActionResult> TranslateText(string to, string text)
    {
        return Ok(await _translatorService.TranslateTextAsync(text, to));
    }

    [HttpPost("image")]
    public async Task<ActionResult> TranslateImageText(string to, IFormFile image)
    {
        string resp = await _translatorService
            .TranslateTextAsync(await _computerVisionService.ExtractTextUsingReadApiAsync(image.OpenReadStream()), to);
        return Ok( new
        {
           Response = resp,
           Text = await _computerVisionService.ExtractTextUsingReadApiAsync(image.OpenReadStream())
        });
    }
}
