using Microsoft.AspNetCore.SignalR;

public class UTTTHub : Hub
{
    public async Task SubmitMove(string json)
    {
        Console.WriteLine(json);

        await Clients.All.SendAsync("announceMove", json);
    }
}
