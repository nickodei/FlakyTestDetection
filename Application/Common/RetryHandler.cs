using System.Net;

namespace Application.Common;

public class RetryHandler : DelegatingHandler
{
    private const int MaxRetries = 20;

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

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                Console.WriteLine("Got 403 so waiting for 15 min ...");
                throw new Exception("Got 403");
                //await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);
                continue;
            }

            Console.WriteLine("Waiting for 30 Seconds");
            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);
        }

        return response;
    }
}