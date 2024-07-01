namespace AzureTranslator.Services;

using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class ComputerVisionService
{
    private readonly ComputerVisionClient client;

    public ComputerVisionService(string subscriptionKey, string endpoint)
    {
        this.client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(subscriptionKey))
        {
            Endpoint = endpoint
        };
    }

    public async Task<string> ExtractTextUsingReadApiAsync(Stream imageStream)
    {
        var textHeaders = await client.ReadInStreamAsync(imageStream);
        string operationLocation = textHeaders.OperationLocation;

        string operationId = operationLocation.Substring(operationLocation.Length - 36);
        Guid operationGuid = Guid.Parse(operationId);

        int i = 0;
        int maxRetries = 10;
        ReadOperationResult results;
        do
        {
            results = await client.GetReadResultAsync(operationGuid);
            await Task.Delay(1000);
        } while ((results.Status == OperationStatusCodes.Running ||
                  results.Status == OperationStatusCodes.NotStarted) && i++ < maxRetries);

        var textResult = results.AnalyzeResult.ReadResults;
        var stringBuilder = new System.Text.StringBuilder();
        foreach (var page in textResult)
        {
            foreach (var line in page.Lines)
            {
                stringBuilder.AppendLine(line.Text);
            }
        }
        return stringBuilder.ToString();
    }

}
