using MudBlazor;

namespace MudBlazorServer.Data
{
    public class ImageModel
    {
        public string Url { get; set; }
        
        public string Name { get; set; }                
        public string ImageSource { get; set; }
        public byte[] ImageBytes{ get; set; }                 
    }
}
