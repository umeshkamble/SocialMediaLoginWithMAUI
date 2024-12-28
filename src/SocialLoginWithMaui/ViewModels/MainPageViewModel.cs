using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Authentication;
using SocialLoginWithMaui.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;

namespace SocialLoginWithMaui.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        const string authenticationUrl = "https://webapisociallogin.azurewebsites.net/mobileauth/";

        [ObservableProperty]
        public string? authToken;

        CancellationTokenSource? cts;
        public MainPageViewModel()
        {
            WeakReferenceMessenger.Default.Register<ProtocolMessage>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
        }

        [RelayCommand]
        private async Task OnAuthenticate(string scheme)
        {
            try
            {
                AuthToken = string.Empty;
                WebAuthenticatorResult result = null;

                if (scheme.Equals("Apple")
                    && DeviceInfo.Platform == DevicePlatform.iOS
                    && DeviceInfo.Version.Major >= 13)
                {
                    // Use Native Apple Sign In API's
                    result = await AppleSignInAuthenticator.AuthenticateAsync();
                }
                else
                {
                    // Web Authentication flow
                    var authUrl = new Uri($"{authenticationUrl}{scheme}");
                    var callbackUrl = new Uri("myapp://");

#if WINDOWS
                    cts = new CancellationTokenSource();
                    await WinUIEx.WebAuthenticator.AuthenticateAsync(authUrl, callbackUrl, cts.Token);
#else
                    result = await WebAuthenticator.Default.AuthenticateAsync(
                   new WebAuthenticatorOptions()
                   {
                       Url = authUrl,
                       CallbackUrl = callbackUrl,
                       PrefersEphemeralWebBrowserSession = true
                   });
#endif
                }

                if (result is null)
                    return;

                OnMessageReceived(result.CallbackUri.OriginalString);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed: {ex.Message}");

                await App.Current.MainPage.DisplayAlert("Alert", $"Failed: {ex.Message}", "OK");
            }
        }

        private void OnMessageReceived(string value)
        {
            try
            {
                Console.WriteLine($"New message received: {value}");
#if WINDOWS
                cts?.Cancel();
#endif
                Uri uri = new Uri(value);
                string query = uri.Fragment;
                query = query.TrimStart('#');
                NameValueCollection queryParameters = HttpUtility.ParseQueryString(query);

                // Extract parameters
                string accessToken = queryParameters["access_token"];
                string name = queryParameters["name"];
                string email = queryParameters["email"];

                AuthToken = $"Name: {name}{Environment.NewLine}Email: {email}{Environment.NewLine}AccessToken: {accessToken}";

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}