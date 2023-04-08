using MudBlazor;

namespace MudBlazorServer.Data
{
    public class ThemeService
    {
        public bool IsDarkMode { get; set; } = true;
        public MudTheme Theme { get; set; } = new MudTheme();
    }

}
