// Decompiled with JetBrains decompiler
// Type: RedditSharp.SubredditStyle
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json.Linq;
using RedditSharp.Things;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace RedditSharp
{
  public class SubredditStyle
  {
    private const string UploadImageUrl = "/api/upload_sr_img";
    private const string UpdateCssUrl = "/api/subreddit_stylesheet";

    private Reddit Reddit { get; set; }

    private IWebAgent WebAgent { get; set; }

    public SubredditStyle(Reddit reddit, Subreddit subreddit, IWebAgent webAgent)
    {
      this.Reddit = reddit;
      this.Subreddit = subreddit;
      this.WebAgent = webAgent;
    }

        public SubredditStyle(Reddit reddit, Subreddit subreddit, JToken json, IWebAgent webAgent)
          : this(reddit, subreddit, webAgent)
        {
            Images = new List<SubredditImage>();
            var data = json["data"];
            CSS = HttpUtility.HtmlDecode(data["stylesheet"].Value<string>());
            foreach (var image in data["images"])
            {
                Images.Add(new SubredditImage(
                    Reddit, this, image["link"].Value<string>(),
                    image["name"].Value<string>(), image["url"].Value<string>(), WebAgent));

            }
        }

    public string CSS { get; set; }

    public List<SubredditImage> Images { get; set; }

    public Subreddit Subreddit { get; set; }

    public void UpdateCss()
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/subreddit_stylesheet");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        op = "save",
        stylesheet_contents = this.CSS,
        uh = this.Reddit.User.Modhash,
        api_type = "json",
        r = this.Subreddit.Name
      });
            requestStream.Flush();//.Close();
      JToken.Parse(this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream()));
    }

    public void UploadImage(string name, ImageType imageType, byte[] file)
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/upload_sr_img");
      MultipartFormBuilder multipartFormBuilder = new MultipartFormBuilder(post);
      multipartFormBuilder.AddDynamic((object) new
      {
        name = name,
        uh = this.Reddit.User.Modhash,
        r = this.Subreddit.Name,
        formid = "image-upload",
        img_type = (imageType == ImageType.PNG ? "png" : "jpg"),
        upload = ""
      });
      multipartFormBuilder.AddFile(nameof (file), "foo.png", file, imageType == ImageType.PNG ? "image/png" : "image/jpeg");
      multipartFormBuilder.Finish();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }
  }
}
