namespace AzureTranslator.Services;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

public class TranslatorService
{
    private readonly string subscriptionKey;
    private readonly string endpoint;
    private readonly string region;

    public TranslatorService(string subscriptionKey, string endpoint, string region)
    {
        this.subscriptionKey = subscriptionKey;
        this.endpoint = endpoint;
        this.region = region;
    }

    public async Task<string> TranslateTextAsync(string text, string toLanguage)
    {
        using (var client = new HttpClient())
        {
            var route = "/translate?api-version=3.0&to=" + toLanguage;
            var url = endpoint + route;

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", region);

            var requestBody = new object[] { new { Text = text } };
            var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestBody), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            return jsonResponse;
        }
    }
}

