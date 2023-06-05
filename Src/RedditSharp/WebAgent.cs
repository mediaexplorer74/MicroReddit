// Type: RedditSharp.WebAgent
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Web;

namespace RedditSharp
{
  public class WebAgent : IWebAgent
  {
    private static DateTimeOffset _lastRequest;
    private static DateTimeOffset _burstStart;
    private static int _requestsThisBurst;

    public static string UserAgent { get; set; }

    public static bool EnableRateLimit { get; set; }

    public static string Protocol { get; set; }

    public static WebAgent.RateLimitMode RateLimit { get; set; }

    public static string RootDomain { get; set; }

    public string AccessToken { get; set; }

    public CookieContainer Cookies { get; set; }

    public string AuthCookie { get; set; }

    public DateTimeOffset LastRequest => WebAgent._lastRequest;

    public DateTimeOffset BurstStart => WebAgent._burstStart;

    public int RequestsThisBurst => WebAgent._requestsThisBurst;

    public bool UseProxy { get; set; }


    public IWebProxy Proxy { get; set; }

    //public WebProxy Proxy { get; set; }

        static WebAgent()
    {
      WebAgent.UserAgent = "";
      WebAgent.RateLimit = WebAgent.RateLimitMode.Pace;
      WebAgent.Protocol = "https";
      WebAgent.RootDomain = "www.reddit.com";
    }

    public virtual JToken CreateAndExecuteRequest(string url)
    {
      Uri result;
      if (!Uri.TryCreate(url, UriKind.Absolute, out result) && !Uri.TryCreate(string.Format("{0}://{1}{2}", (object) WebAgent.Protocol, (object) WebAgent.RootDomain, (object) url), UriKind.Absolute, out result))
        throw new Exception("Could not parse Uri");
      HttpWebRequest get = this.CreateGet(result);
      try
      {
        return this.ExecuteRequest(get);
      }
      catch (Exception ex)
      {
        string protocol = WebAgent.Protocol;
        string rootDomain = WebAgent.RootDomain;
        WebAgent.Protocol = "http";
        WebAgent.RootDomain = "www.reddit.com";
        JToken andExecuteRequest = this.CreateAndExecuteRequest(url);
        WebAgent.Protocol = protocol;
        WebAgent.RootDomain = rootDomain;
        return andExecuteRequest;
      }
    }

    public virtual JToken ExecuteRequest(HttpWebRequest request)
    {
      this.EnforceRateLimit();

      if (this.UseProxy)
        request.Proxy = (IWebProxy) this.Proxy;
      
      HttpWebResponse response = (HttpWebResponse) request.GetResponseAsync().Result;

      string responseString = this.GetResponseString(response.GetResponseStream());
      JToken jtoken;
      if (!string.IsNullOrEmpty(responseString))
      {
        jtoken = JToken.Parse(responseString);
        try
        {
          if (jtoken[(object) "json"] != null)
            jtoken = jtoken[(object) "json"];
          if (jtoken[(object) "error"] != null)
          {
            switch (jtoken[(object) "error"].ToString())
            {
              case "404":
                throw new Exception("File Not Found");
              case "403":
                throw new Exception("Restricted");
            }
          }
        }
        catch
        {
        }
      }
      else
        jtoken = JToken.Parse("{'method':'" + response.Method + "','uri':'" + response.ResponseUri.AbsoluteUri + "','status':'" + response.StatusCode.ToString() + "'}");
      return jtoken;
    }

    /// <summary>
    /// Enforce the api throttle.
    /// </summary>
    [MethodImpl(/*MethodImplOptions.Synchronized*/MethodImplOptions.AggressiveInlining)]
    protected virtual void EnforceRateLimit()
    {
      double num = WebAgent.IsOAuth() ? 60.0 : 30.0;
      switch (WebAgent.RateLimit)
      {
        case WebAgent.RateLimitMode.Pace:
          while ((DateTimeOffset.UtcNow - WebAgent._lastRequest).TotalSeconds < 60.0 / num)
            Thread.Sleep(250);
          WebAgent._lastRequest = DateTimeOffset.UtcNow;
          break;
        case WebAgent.RateLimitMode.SmallBurst:
          if (WebAgent._requestsThisBurst == 0 || (DateTimeOffset.UtcNow - WebAgent._burstStart).TotalSeconds >= 10.0)
          {
            WebAgent._burstStart = DateTimeOffset.UtcNow;
            WebAgent._requestsThisBurst = 0;
          }
          if ((double) WebAgent._requestsThisBurst >= num / 6.0)
          {
            while ((DateTimeOffset.UtcNow - WebAgent._burstStart).TotalSeconds < 10.0)
              Thread.Sleep(250);
            WebAgent._burstStart = DateTimeOffset.UtcNow;
            WebAgent._requestsThisBurst = 0;
          }
          WebAgent._lastRequest = DateTimeOffset.UtcNow;
          ++WebAgent._requestsThisBurst;
          break;
        case WebAgent.RateLimitMode.Burst:
          if (WebAgent._requestsThisBurst == 0 || (DateTimeOffset.UtcNow - WebAgent._burstStart).TotalSeconds >= 60.0)
          {
            WebAgent._burstStart = DateTimeOffset.UtcNow;
            WebAgent._requestsThisBurst = 0;
          }
          if ((double) WebAgent._requestsThisBurst >= num)
          {
            while ((DateTimeOffset.UtcNow - WebAgent._burstStart).TotalSeconds < 60.0)
              Thread.Sleep(250);
            WebAgent._burstStart = DateTimeOffset.UtcNow;
            WebAgent._requestsThisBurst = 0;
          }
          WebAgent._lastRequest = DateTimeOffset.UtcNow;
          ++WebAgent._requestsThisBurst;
          break;
      }
    }

    public virtual HttpWebRequest CreateRequest(string url, string method)
    {
      this.EnforceRateLimit();
      HttpWebRequest request = !(!(Type.GetType("Mono.Runtime") != (Type) null) ? !Uri.IsWellFormedUriString(url, UriKind.Absolute) : !url.StartsWith("http://") && !url.StartsWith("https://")) ? (HttpWebRequest) WebRequest.Create(url) : (HttpWebRequest) WebRequest.Create(string.Format("{0}://{1}{2}", (object) WebAgent.Protocol, (object) WebAgent.RootDomain, (object) url));
      request.CookieContainer = this.Cookies;
      if (Type.GetType("Mono.Runtime") != (Type) null)
      {
        string cookieHeader = this.Cookies.GetCookieHeader(new Uri("http://reddit.com"));
        //request.Headers.Set("Cookie", cookieHeader);
      }

        //request.Host
        if (WebAgent.IsOAuth() 
            && request.RequestUri.ToString().ToLower() == "oauth.reddit.com")
        {
            //request.Headers.Set("Authorization", "bearer " + this.AccessToken);
        }
      
            request.Method = method;
      //request.UserAgent = WebAgent.UserAgent + " - with RedditSharp by /u/meepster23";
      return this.InjectProxy(request);
    }

    protected virtual HttpWebRequest CreateRequest(Uri uri, string method)
    {
      this.EnforceRateLimit();
      HttpWebRequest request = (HttpWebRequest) WebRequest.Create(uri);
      request.CookieContainer = this.Cookies;

      if (Type.GetType("Mono.Runtime") != (Type) null)
      {
        string cookieHeader = this.Cookies.GetCookieHeader(new Uri("http://reddit.com"));
        //request.Headers.Set("Cookie", cookieHeader);
      }

        if (WebAgent.IsOAuth() && uri.Host.ToLower() == "oauth.reddit.com")
        {
            //request.Headers.Set("Authorization", "bearer " + this.AccessToken);
        }

      request.Method = method;

      //TODO
      //request.UserAgent = WebAgent.UserAgent + " - with RedditSharp by /u/meepster23";
      return this.InjectProxy(request);
    }

    public virtual HttpWebRequest CreateGet(string url) => this.CreateRequest(url, "GET");

    private HttpWebRequest CreateGet(Uri url) => this.CreateRequest(url, "GET");

    public virtual HttpWebRequest CreatePost(string url)
    {
      HttpWebRequest request = this.CreateRequest(url, "POST");
      request.ContentType = "application/x-www-form-urlencoded";
      return request;
    }

    public virtual HttpWebRequest CreatePut(string url)
    {
      HttpWebRequest request = this.CreateRequest(url, "PUT");
      request.ContentType = "application/x-www-form-urlencoded";
      return request;
    }

    public virtual HttpWebRequest CreateDelete(string url)
    {
      HttpWebRequest request = this.CreateRequest(url, "DELETE");
      request.ContentType = "application/x-www-form-urlencoded";
      return request;
    }

    public virtual string GetResponseString(Stream stream)
    {
      string end = new StreamReader(stream).ReadToEnd();
      stream.Flush();//.Close();
      return end;
    }

    public virtual void WritePostBody(Stream stream, object data, params string[] additionalFields)
    {
      PropertyInfo[] properties = data.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      string str1 = "";
      foreach (PropertyInfo propertyInfo in properties)
      {
        string str2 = !(((IEnumerable<object>) propertyInfo.GetCustomAttributes(typeof (RedditAPINameAttribute), false)).FirstOrDefault<object>() is RedditAPINameAttribute apiNameAttribute) ? propertyInfo.Name : apiNameAttribute.Name;
        string str3 = Convert.ToString(propertyInfo.GetValue(data, (object[]) null));
        str1 = str1 + str2 + "=" + HttpUtility.UrlEncode(str3).Replace(";", "%3B").Replace("&", "%26") + "&";
      }
      for (int index = 0; index < additionalFields.Length; index += 2)
      {
        string str4 = Convert.ToString(additionalFields[index + 1]) ?? string.Empty;
        str1 = str1 + additionalFields[index] + "=" + HttpUtility.UrlEncode(str4).Replace(";", "%3B").Replace("&", "%26") + "&";
      }
      byte[] bytes = Encoding.UTF8.GetBytes(str1.Remove(str1.Length - 1));
      stream.Write(bytes, 0, bytes.Length);
      stream.Flush();//.Close();
    }

    private static bool IsOAuth() => WebAgent.RootDomain == "oauth.reddit.com";

    public virtual HttpWebRequest InjectProxy(HttpWebRequest request)
    {
      if (this.UseProxy)
        request.Proxy = (IWebProxy) this.Proxy;
      return request;
    }

        public string GetResponseString(object value)
        {
            throw new NotImplementedException();
        }

        public enum RateLimitMode
    {
      Pace,
      SmallBurst,
      Burst,
      None,
    }
  }
}
