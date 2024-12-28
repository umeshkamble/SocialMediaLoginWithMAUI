using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using SocialLoginWithMaui.ViewModels;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using SocialLoginWithMaui.Messages;

#if WINDOWS
using Microsoft.Windows.AppLifecycle;
using Windows.ApplicationModel.Activation;
#endif

namespace SocialLoginWithMaui
{
    public static class MauiProgram
    {

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<MainPageViewModel>();

#if WINDOWS
            builder.ConfigureLifecycleEvents(lc =>
            {
                lc.AddWindows(static win =>
                {
                    win.OnWindowCreated(window =>
                    {
                        window.Title = "Social Login With MAUI";
                        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                        App.appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
                    });
                    win.OnLaunched(static async (app, args) =>
                    {
                        var appInstance = AppInstance.GetCurrent();
                        var e = appInstance.GetActivatedEventArgs();

                        if (e.Kind != ExtendedActivationKind.Protocol
                            || e.Data is not ProtocolActivatedEventArgs protocol)
                        {
                            appInstance.Activated += AppInstance_Activated;
                            return;
                        }

                        var instances = AppInstance.GetInstances();
                        await Task.WhenAll(instances.Select(async q => await q.RedirectActivationToAsync(e)));

                        app.Exit();
                    });
                });
            });
#endif
            return builder.Build();
        }

#if WINDOWS
        private static void AppInstance_Activated(object? sender, AppActivationArguments e)
        {
            if (e.Kind != ExtendedActivationKind.Protocol ||
                e.Data is not ProtocolActivatedEventArgs protocol)
            {
                return;
            }
            BringAppToFront();
            // Process your activation here
            Debug.WriteLine("URI activated: " + protocol.Uri);
            WeakReferenceMessenger.Default.Send(new ProtocolMessage(protocol.Uri.OriginalString));
        }
        public static void BringAppToFront()
        {
            try
            {
                var windowPresenter = App.appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
                if (windowPresenter is not null)
                {
                    windowPresenter.IsAlwaysOnTop = true;
                    windowPresenter.IsAlwaysOnTop = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
#endif
    }
}
