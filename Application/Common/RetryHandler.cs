using System.Net;

namespace Application.Common;

public class RetryHandler : DelegatingHandler
{
    private const int MaxRetries = 10;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        HttpResponseMessage response = null;
        for (int i = 0; i < MaxRetries; i++)
        {
            
            response = await base.SendAsync(request, cancellationToken);
            Console.WriteLine(response.RequestMessage?.RequestUri);
            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound) {
                return response;
            }
            

            Console.WriteLine("Waiting for 30 Seconds");
            await Task.Delay(TimeSpan.FromSeconds(30));
        }

        return response;
    }
}