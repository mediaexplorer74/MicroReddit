using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MicroReddit.Entities;
using MicroReddit.Interfaces;
using MicroReddit.Services;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MicroReddit
{
    sealed partial class App : Application
    {
        // App type: script (see https://www.reddit.com/prefs/apps/)

        private static readonly string REDDIT_USER = xxx";//Environment.GetEnvironmentVariable("REDDIT_USER");
        private static readonly string REDDIT_PASSWORD = "xxx";//Environment.GetEnvironmentVariable("REDDIT_PASSWORD");
        private static readonly string REDDIT_CLIENT_ID = "xxxxxxxx";//Environment.GetEnvironmentVariable("REDDIT_CLIENT_ID");
        private static readonly string REDDIT_CLIENT_SECRET = "xxxxxxxxxx";//Environment.GetEnvironmentVariable("REDDIT_CLIENT_SECRET");

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        public async Task<List<Post>> Main()
        {
            IAuthService authService = AuthService.GetAuthService();

            var authInfo = await authService.GetAuthInfo(
                REDDIT_USER, REDDIT_PASSWORD, REDDIT_CLIENT_ID, REDDIT_CLIENT_SECRET);

            IRedditService redditService = new RedditService(authInfo);

            List<Post> TopPosts = default;

            try
            {
                TopPosts = await redditService.GetTopPostsByLimit(50);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ex] GetTopPostsByLimit error: " + ex.Message);
            }

            return TopPosts;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage));
                }

                Window.Current.Content = rootFrame;
            }

            Window.Current.Activate();
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
