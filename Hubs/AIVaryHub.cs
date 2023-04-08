
namespace MudBlazorServer.Hubs;


public class AIVaryHub : Hub
{
    string apiKey = "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";
    string varyApiUrl = "https://api.openai.com/v1/images/variations";
    HttpClient httpClient;

    public AIVaryHub() : base()
    {

        //Create a HttpClient object
        httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(varyApiUrl);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        httpClient.DefaultRequestHeaders.Add("Organization", "org-jzBeBdUq4xCadFXGMYwiljWI");
    }

    public async Task<List<string>> GetDALLE2ImageVaryHttpResponse(byte[] imageBytes, Int16 n, string size, string? user)
    {

        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(imageBytes), "image", "A.png");
        content.Add(new StringContent(n.ToString()), "n");
        content.Add(new StringContent(size), "size");

        List<Url> data = null;

        try
        {
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
        if (data == null)
            return null;
        else
        {
            List<string> imageUrlList = new List<string>();
            data.ForEach((v) => imageUrlList.Add(v.url!));
            return imageUrlList;
        }

    }

    public async Task AIVary(string userClientId, byte[] imageBytes, Int16 n, string size, string? user = null)
    {
        Console.WriteLine($"Vary images:\nFrom user: {userClientId}");
        List<string> imageList = await GetDALLE2ImageVaryHttpResponse(imageBytes: imageBytes, n: n, size: size, user: user);
        await Clients.Caller.SendAsync(SignalR.SignalRMethod.ClientMethod.AIVary, imageList, "ImageUrlList:\n");
    }

    public async override Task OnConnectedAsync()
    {
        Console.WriteLine("One Client Connected to AIVaryHub : " + Context.ConnectionId);

    }

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("One Client Disconnected with AIVaryHub:" + Context.ConnectionId + "\tReason:" + exception + "\n");
    }

}


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



