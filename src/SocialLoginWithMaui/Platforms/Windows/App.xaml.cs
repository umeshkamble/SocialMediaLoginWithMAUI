using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using System.Diagnostics;
using Windows.ApplicationModel.Activation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SocialLoginWithMaui.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            if (WinUIEx.WebAuthenticator.CheckOAuthRedirectionActivation())
                return;
            CheckAppInstance();
            this.InitializeComponent();
        }
       
        private void CheckAppInstance()
        {
            try
            {
                var singleInstance = AppInstance.FindOrRegisterForKey("SocialLoginWithMaui");
                if (!singleInstance.IsCurrent)
                {
                    var currentInstance = AppInstance.GetCurrent();
                    var args = currentInstance.GetActivatedEventArgs();
                    singleInstance.RedirectActivationToAsync(args).GetAwaiter().GetResult();

                    Process.GetCurrentProcess().Kill();
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }

}
