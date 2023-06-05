// Type: RedditSharp.IWebAgent
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;

namespace RedditSharp
{
  public interface IWebAgent
  {
    CookieContainer Cookies { get; set; }

    string AuthCookie { get; set; }

    string AccessToken { get; set; }

    bool UseProxy { get; set; }

    //WebProxy Proxy { get; set; }

    HttpWebRequest InjectProxy(HttpWebRequest request);

    HttpWebRequest CreateRequest(string url, string method);

    HttpWebRequest CreateGet(string url);

    HttpWebRequest CreatePost(string url);

    HttpWebRequest CreatePut(string url);

    HttpWebRequest CreateDelete(string url);

    string GetResponseString(Stream stream);

    void WritePostBody(Stream stream, object data, params string[] additionalFields);

    JToken CreateAndExecuteRequest(string url);

    JToken ExecuteRequest(HttpWebRequest request);
        string GetResponseString(object value);
    }
}
