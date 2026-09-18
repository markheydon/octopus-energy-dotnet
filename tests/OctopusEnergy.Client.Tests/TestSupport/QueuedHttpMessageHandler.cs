using System.Net;

namespace OctopusEnergy.Client.Tests.TestSupport;

internal sealed class QueuedHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<Func<HttpRequestMessage, HttpResponseMessage>> _responses = new();

    internal void Enqueue(HttpStatusCode statusCode, string content, string mediaType = "application/json")
    {
        _responses.Enqueue(_ => new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, System.Text.Encoding.UTF8, mediaType),
        });
    }

    internal void EnqueueResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        _responses.Enqueue(responseFactory);
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (_responses.Count == 0)
        {
            throw new InvalidOperationException($"No queued response for {request.Method} {request.RequestUri}.");
        }

        Func<HttpRequestMessage, HttpResponseMessage> responseFactory = _responses.Dequeue();
        return Task.FromResult(responseFactory(request));
    }
}
