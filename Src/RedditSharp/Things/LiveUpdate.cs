// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.LiveUpdate
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace RedditSharp.Things
{
  public class LiveUpdate : CreatedThing
  {
    private const string StrikeUpdateUrl = "/api/live/{0}/strike_update";
    private const string DeleteUpdateUrl = "/api/live/{0}/delete_update";

    [JsonProperty("body")]
    public string Body { get; set; }

    [JsonProperty("body_html")]
    public string BodyHtml { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("mobile_embeds")]
    public ICollection<LiveUpdate.MobileEmbed> MobileEmbeds { get; set; }

    [JsonProperty("author")]
    public string Author { get; set; }

    [JsonProperty("embeds")]
    public ICollection<LiveUpdate.Embed> Embeds { get; set; }

    [JsonProperty("stricken")]
    public bool IsStricken { get; set; }

    public void Strike() => this.SimpleAction("/api/live/{0}/strike_update");

    public void Delete() => this.SimpleAction("/api/live/{0}/delete_update");

    public async Task<LiveUpdate> InitAsync(Reddit reddit, JToken post, IWebAgent webAgent)
    {
      LiveUpdate liveUpdate = this;
      liveUpdate.CommonInit(reddit, post, webAgent);
      JsonConvert.PopulateObject(post[(object) "data"].ToString(), (object) liveUpdate, reddit.JsonSerializerSettings);
      return liveUpdate;
    }

    public LiveUpdate Init(Reddit reddit, JToken post, IWebAgent webAgent)
    {
      this.CommonInit(reddit, post, webAgent);
      JsonConvert.PopulateObject(post[(object) "data"].ToString(), (object) this, reddit.JsonSerializerSettings);
      return this;
    }

    private void CommonInit(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.Init(json);
      this.Reddit = reddit;
      this.WebAgent = webAgent;
    }

    public void SimpleAction(string url)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format(url, (object) this.Name));
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        api_type = "json",
        id = this.Name,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public class MobileEmbed
    {
      [JsonProperty("provider_url")]
      public string ProviderUrl { get; set; }

      [JsonProperty("description")]
      public string Description { get; set; }

      [JsonProperty("original_url")]
      public string Original_Url { get; set; }

      [JsonProperty("url")]
      public string Url { get; set; }

      [JsonProperty("title")]
      public string Title { get; set; }

      [JsonProperty("thumbnail_width")]
      public int ThumbnailWidth { get; set; }

      [JsonProperty("thumbnail_height")]
      public int ThumbnailHeight { get; set; }

      [JsonProperty("thumbnail_url")]
      public string ThumbnailUrl { get; set; }

      [JsonProperty("author_name")]
      public string AuthorName { get; set; }

      [JsonProperty("version")]
      public string Version { get; set; }

      [JsonProperty("provider_name")]
      public string ProviderName { get; set; }

      [JsonProperty("type")]
      public string Type { get; set; }

      [JsonProperty("author_url")]
      public string AuthorUrl { get; set; }
    }

    public class Embed
    {
      [JsonProperty("url")]
      public string AuthorUrl { get; set; }

      [JsonProperty("width")]
      public int Width { get; set; }

      [JsonProperty("height")]
      public int Height { get; set; }
    }
  }
}
