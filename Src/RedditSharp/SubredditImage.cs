// Decompiled with JetBrains decompiler
// Type: RedditSharp.SubredditImage
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace RedditSharp
{
  public class SubredditImage
  {
    private const string DeleteImageUrl = "/api/delete_sr_img";

    private Reddit Reddit { get; set; }

    private IWebAgent WebAgent { get; set; }

    public SubredditImage(
      Reddit reddit,
      SubredditStyle subredditStyle,
      string cssLink,
      string name,
      IWebAgent webAgent)
    {
      this.Reddit = reddit;
      this.WebAgent = webAgent;
      this.SubredditStyle = subredditStyle;
      this.Name = name;
      this.CssLink = cssLink;
    }

    public SubredditImage(
      Reddit reddit,
      SubredditStyle subreddit,
      string cssLink,
      string name,
      string url,
      IWebAgent webAgent)
      : this(reddit, subreddit, cssLink, name, webAgent)
    {
      this.Url = new Uri(url);
      if (!int.TryParse(url, out int _))
        return;
      this.Url = new Uri(string.Format("http://thumbs.reddit.com/{0}_{1}.png", (object) subreddit.Subreddit.FullName, (object) url), UriKind.Absolute);
    }

    public string CssLink { get; set; }

    public string Name { get; set; }

    public Uri Url { get; set; }

    public SubredditStyle SubredditStyle { get; set; }

    public void Delete()
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/delete_sr_img");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        img_name = this.Name,
        uh = this.Reddit.User.Modhash,
        r = this.SubredditStyle.Subreddit.Name
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
      this.SubredditStyle.Images.Remove(this);
    }

    public async Task DeleteAsync()
    {
      SubredditImage subredditImage = this;
      HttpWebRequest request = subredditImage.WebAgent.CreatePost("/api/delete_sr_img");
      Stream requestStreamAsync = await request.GetRequestStreamAsync();
      subredditImage.WebAgent.WritePostBody(requestStreamAsync, (object) new
      {
        img_name = subredditImage.Name,
        uh = subredditImage.Reddit.User.Modhash,
        r = subredditImage.SubredditStyle.Subreddit.Name
      });
      requestStreamAsync.Flush();
      WebResponse responseAsync = await request.GetResponseAsync();
      subredditImage.WebAgent.GetResponseString(responseAsync.GetResponseStream());
      subredditImage.SubredditStyle.Images.Remove(subredditImage);
    }
  }
}
