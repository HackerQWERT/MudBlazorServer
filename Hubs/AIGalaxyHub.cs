namespace MudBlazorServer.Hubs;


public class AIGalaxyHub : Hub
{
    string apiKey = "YOUR API KEYS";
    string varyApiUrl = "https://api.openai.com/v1/images/variations";
    string generationApiUrl = "https://api.openai.com/v1/images/generations";
    string chatApiUrl = "https://api.openai.com/v1/chat/completions";

    public AIGalaxyHub() : base()
    {
    }

    public async Task AIVary(string userClientId, byte[] imageBytes, Int16 n, string size, string? user = null)
    {
        Console.WriteLine($"Vary images:\nFrom user: {userClientId}");


        List<Url>? data = null;
        try
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(varyApiUrl);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            httpClient.DefaultRequestHeaders.Add("Organization", "org-jzBeBdUq4xCadFXGMYwiljWI");

            var content = new MultipartFormDataContent();
            content.Add(new ByteArrayContent(imageBytes), "image", "A.png");
            content.Add(new StringContent(n.ToString()), "n");
            content.Add(new StringContent(size), "size");

            var dalle2HttpResponse = await httpClient.PostAsync(varyApiUrl, content);

            var responseContent = await dalle2HttpResponse.Content.ReadAsStringAsync();
            Console.WriteLine("HttpResponseContent:\n" + responseContent);

            var dalle2ResponseClass = JsonSerializer.Deserialize<DALLE2ResponseJson>(responseContent);
            data = dalle2ResponseClass?.data ?? null;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        if (data is null)
            await Clients.Caller.SendAsync(SignalR.SignalRMethod.ClientMethod.AIVary, null, null);
        else
        {
            List<string> imageUrlList = new List<string>();
            data.ForEach((v) => imageUrlList.Add(v.url!));
            await Clients.Caller.SendAsync(SignalR.SignalRMethod.ClientMethod.AIVary, imageUrlList, "Success");

        }
    }

    public async Task AIGenerate(string userClientId, string prompt, Int16 n, string size, string? user = null)
    {
        Console.WriteLine($"Generate images: {prompt}\nFrom user: {userClientId}");

        var data = new
        {
            n = n,
            prompt = prompt,
            size = size,
            // user = user
        };
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        List<Url> dalle2ResponseData = null;
        string? responseContent = null;
        try
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            httpClient.DefaultRequestHeaders.Add("Organization", "org-jzBeBdUq4xCadFXGMYwiljWI");


            var dalle2HttpResponse = await httpClient.PostAsync(generationApiUrl, content);
            responseContent = await dalle2HttpResponse.Content.ReadAsStringAsync();
            Console.WriteLine("HttpResponseContent:\n" + responseContent);

            var dalle2ResponseClass = JsonSerializer.Deserialize<DALLE2ResponseJson>(responseContent);
            dalle2ResponseData = dalle2ResponseClass?.data ?? null;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        if (dalle2ResponseData == null)
            await Clients.Caller.SendAsync(SignalR.SignalRMethod.ClientMethod.AIGenerate, null, responseContent);
        else
        {
            List<string> imageUrlList = new List<string>();
            dalle2ResponseData.ForEach((v) => imageUrlList.Add(v.url!));
            await Clients.Caller.SendAsync(SignalR.SignalRMethod.ClientMethod.AIGenerate, imageUrlList, "Success");
        }

    }

    public async Task AIChat(string userClientId, string requestJson)
    {

        Console.WriteLine(requestJson);
        Console.WriteLine($"AIChat From user: {userClientId}");

        string? content = null;
        try
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(chatApiUrl);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            httpClient.DefaultRequestHeaders.Add("Organization", "org-jzBeBdUq4xCadFXGMYwiljWI");

            var requestBody = new StringContent(requestJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(chatApiUrl, requestBody);

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
        Console.WriteLine("One Client Connected to AIGalaxyHub : " + Context.ConnectionId);
    }

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("One Client Disconnected with AIGalaxyHub:" + Context.ConnectionId + "\tReason:" + exception + "\n");
    }

    public class DALLE2ResponseJson
    {
        public Int64? created { get; set; }
        public List<Url>? data { get; set; }
    }

    //·´ÐòÁÐ»¯Àà
    public class Url
    {
        public string? url { get; set; }
    }

    public class ChatGPTResponse
    {
        public List<Choice>? choices { get; set; }

    }

    public class Choice
    {
        public int? index { get; set; }
        public Message? message { get; set; }
        public string? finish_reason { get; set; }
    }

    public class Message
    {
        public string? role { get; set; }
        public string? content { get; set; }
    }

}




