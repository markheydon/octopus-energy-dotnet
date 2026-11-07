using System.Net;

namespace OctopusEnergy.Client.Tests.TestSupport;

internal sealed class QueuedHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<Func<HttpRequestMessage, HttpResponseMessage>> _responses = new();
    private readonly List<HttpRequestMessage> _sentRequests = [];
    private readonly List<string> _sentRequestBodies = [];

    internal IReadOnlyList<HttpRequestMessage> SentRequests => _sentRequests;

    internal IReadOnlyList<string> SentRequestBodies => _sentRequestBodies;

    internal void Enqueue(HttpStatusCode statusCode, string content, string mediaType = "application/json")
    {
        Enqueue(statusCode, content, configureResponse: null, mediaType);
    }

    internal void Enqueue(
        HttpStatusCode statusCode,
        string content,
        Action<HttpResponseMessage>? configureResponse,
        string mediaType = "application/json")
    {
        _responses.Enqueue(_ =>
        {
            HttpResponseMessage response = new(statusCode)
            {
                Content = new StringContent(content, System.Text.Encoding.UTF8, mediaType),
            };

            configureResponse?.Invoke(response);
            return response;
        });
    }

    internal void EnqueueResponse(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
    {
        _responses.Enqueue(responseFactory);
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return await Task.FromCanceled<HttpResponseMessage>(cancellationToken).ConfigureAwait(false);
        }

        if (_responses.Count == 0)
        {
            throw new InvalidOperationException($"No queued response for {request.Method} {request.RequestUri}.");
        }

        _sentRequests.Add(request);
        if (request.Content is not null)
        {
            _sentRequestBodies.Add(await request.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
        }
        else
        {
            _sentRequestBodies.Add(string.Empty);
        }

        Func<HttpRequestMessage, HttpResponseMessage> responseFactory = _responses.Dequeue();
        return responseFactory(request);
    }
}
