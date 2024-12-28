#if WINDOWS
using Microsoft.UI.Windowing;
#endif

namespace SocialLoginWithMaui
{
    public partial class App : Application
    {
#if WINDOWS
        public static AppWindow appWindow { get; set; }
#endif
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
