namespace MudBlazorServer.Hubs;

public class AIGenerateHub : Hub
{
    string generationApiUrl = "https://api.openai.com/v1/images/generations";

    string editApiUrl = "https://api.openai.com/v1/images/edits";


    string apiKey = "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";

    HttpClient httpClient;

    public AIGenerateHub() : base()
    {

        //        Create a HttpClient object
        httpClient = new HttpClient();

        httpClient.BaseAddress = new Uri(generationApiUrl);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        httpClient.DefaultRequestHeaders.Add("Organization", "org-jzBeBdUq4xCadFXGMYwiljWI");
    }

    public async Task<List<string>> GetDALLE2ImageGenerationHttpResponse(string prompt, Int16 n, string size, string? user)
    {
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
        try
        {
            var dalle2HttpResponse = await httpClient.PostAsync(generationApiUrl, content);
            var responseContent = await dalle2HttpResponse.Content.ReadAsStringAsync();
            Console.WriteLine("HttpResponseContent:\n" + responseContent);

            var dalle2ResponseClass = JsonSerializer.Deserialize<DALLE2ResponseJson>(responseContent);
            dalle2ResponseData = dalle2ResponseClass?.data ?? null;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        if (dalle2ResponseData == null)
            return null;
        else
        {
            List<string> imageUrlList = new List<string>();
            dalle2ResponseData.ForEach((v) => imageUrlList.Add(v.url!));
            return imageUrlList;
        }

    }
    public async Task AIGenerate(string userClientId, string prompt, Int16 n, string size, string? user = null)
    {
        Console.WriteLine($"Generate images: {prompt}\nFrom user: {userClientId}");
        List<string> imageList = await GetDALLE2ImageGenerationHttpResponse(prompt: prompt, n: n, size: size, user: user);
        await Clients.Caller.SendAsync(SignalR.SignalRMethod.ClientMethod.AIGenerate, imageList, "ImageUrlList:");
    }

    public async override Task OnConnectedAsync()
    {
        Console.WriteLine("One Client Connected to AIGenerateHub : " + Context.ConnectionId);
    }

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("One Client Disconnected with AIGenerateHub:" + Context.ConnectionId + "\tReason:" + exception + "\n");
    }

    //反序列化类
    class DALLE2ResponseJson
    {
        public Int64? created { get; set; }
        public List<Url>? data { get; set; }
    }
    //反序列化类
    class Url
    {
        public string? url { get; set; }
    }


}

