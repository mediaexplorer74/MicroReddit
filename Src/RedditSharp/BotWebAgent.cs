// Decompiled with JetBrains decompiler
// Type: RedditSharp.BotWebAgent
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using System;
using System.Net;

namespace RedditSharp
{
  public class BotWebAgent : WebAgent
  {
    private AuthProvider TokenProvider;
    private string Username;
    private string Password;

    public DateTimeOffset TokenValidTo { get; set; }

    public BotWebAgent(
      string username,
      string password,
      string clientID,
      string clientSecret,
      string redirectURI)
    {
      this.Username = username;
      this.Password = password;
      WebAgent.EnableRateLimit = true;
      WebAgent.RateLimit = WebAgent.RateLimitMode.Burst;
      WebAgent.RootDomain = "oauth.reddit.com";
      this.TokenProvider = new AuthProvider(clientID, clientSecret, redirectURI, (IWebAgent) this);
      this.GetNewToken();
    }

    public override HttpWebRequest CreateRequest(string url, string method)
    {
      if (url != "https://ssl.reddit.com/api/v1/access_token" && DateTimeOffset.UtcNow.AddMinutes(5.0) > this.TokenValidTo)
        this.GetNewToken();
      return base.CreateRequest(url, method);
    }

    protected override HttpWebRequest CreateRequest(Uri uri, string method)
    {
      if (uri.ToString() != "https://ssl.reddit.com/api/v1/access_token" && DateTimeOffset.UtcNow.AddMinutes(5.0) > this.TokenValidTo)
        this.GetNewToken();
      return base.CreateRequest(uri, method);
    }

    private void GetNewToken()
    {
      this.AccessToken = this.TokenProvider.GetOAuthToken(this.Username, this.Password);
      this.TokenValidTo = DateTimeOffset.UtcNow.AddHours(1.0);
    }
  }
}
