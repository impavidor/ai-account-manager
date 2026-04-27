namespace AccountManager.IntegrationTests;

public static class ActorContext
{
    public static HttpRequestMessage WithActor(this HttpRequestMessage request, Guid actorId, string actorType)
    {
        request.Headers.Add("X-Actor-Id", actorId.ToString());
        request.Headers.Add("X-Actor-Type", actorType);
        return request;
    }
}
