using Microsoft.AspNetCore.SignalR;

namespace MudBlazorServer.Hubs;

// curl https://api.openai.com/v1/chat/completions \
//   -H "Content-Type: application/json" \
//   -H "Authorization: Bearer $OPENAI_API_KEY" \
//   -d '{
//     "model": "gpt-3.5-turbo",
//     "messages": [{"role": "user", "content": "Hello!"}]
//   }'


public class AIChatHub : Hub
{

    string apiKey = "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";

    //Ä£ÐÍURL
    string chatApiUrl = "https://api.openai.com/v1/chat/completions";

    HttpClient httpClient;

    public AIChatHub() : base()
    {

        //        Create a HttpClient object
        httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(chatApiUrl);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        httpClient.DefaultRequestHeaders.Add("Organization", "org-jzBeBdUq4xCadFXGMYwiljWI");


    }

    public async Task AIChat(string userClientId, string requestJson)
    {
        Console.WriteLine(requestJson);
        Console.WriteLine($"AIChat From user: {userClientId}");
        var requestBody = new StringContent(requestJson, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(chatApiUrl, requestBody);
        string? content = null;
        try
        {
            var responseJson = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseJson);

            ChatGPTResponse? chatGPTResponse = JsonSerializer.Deserialize<ChatGPTResponse>(responseJson);
            content = chatGPTResponse?.choices?[0].message?.content;

            Console.WriteLine("Content:" + content);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }
        finally
        {
            await Clients.Caller.SendAsync(SignalR.SignalRMethod.ClientMethod.AIChat, content);
        }



    }
    public async override Task OnConnectedAsync()
    {
        Console.WriteLine("One Client Connected to AIChatHub : " + Context.ConnectionId);
    }

    public async override Task OnDisconnectedAsync(Exception? exception)
    {


        Console.WriteLine("One Client Disconnected with AIChatHub:" + Context.ConnectionId + "\tReason:" + exception + "\n");
    }


}


class ChatGPTResponse
{
    public List<Choice>? choices { get; set; }

}
class Choice
{
    public int? index { get; set; }
    public Message? message { get; set; }
    public string? finish_reason { get; set; }
}
class Message
{
    public string? role { get; set; }
    public string? content { get; set; }
}
