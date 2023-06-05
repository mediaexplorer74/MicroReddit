// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.PrivateMessage
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace RedditSharp.Things
{
  public class PrivateMessage : Thing
  {
    private const string SetAsReadUrl = "/api/read_message";
    private const string CommentUrl = "/api/comment";

    [JsonProperty("body")]
    public string Body { get; set; }

    [JsonProperty("body_html")]
    public string BodyHtml { get; set; }

    [JsonProperty("was_comment")]
    public bool IsComment { get; set; }

    [JsonProperty("created")]
    [JsonConverter(typeof (UnixTimestampConverter))]
    public DateTimeOffset Sent { get; set; }

    [JsonProperty("created_utc")]
    [JsonConverter(typeof (UnixTimestampConverter))]
    public DateTimeOffset SentUTC { get; set; }

    [JsonProperty("dest")]
    public string Destination { get; set; }

    [JsonProperty("author")]
    public string Author { get; set; }

    [JsonProperty("subreddit")]
    public string Subreddit { get; set; }

    [JsonProperty("new")]
    public bool Unread { get; set; }

    [JsonProperty("subject")]
    public string Subject { get; set; }

    [JsonProperty("parent_id")]
    public string ParentID { get; set; }

    [JsonProperty("first_message_name")]
    public string FirstMessageName { get; set; }

    [JsonIgnore]
    public PrivateMessage[] Replies { get; set; }

    [JsonIgnore]
    public PrivateMessage Parent
    {
      get
      {
        if (string.IsNullOrEmpty(this.ParentID))
          return (PrivateMessage) null;
        Listing<PrivateMessage> source = new Listing<PrivateMessage>(this.Reddit, "/message/messages/" + this.ParentID.Remove(0, 3) + ".json", this.WebAgent);
        PrivateMessage privateMessage = source.First<PrivateMessage>();
        return privateMessage.FullName == this.ParentID ? source.First<PrivateMessage>() : ((IEnumerable<PrivateMessage>) privateMessage.Replies).First<PrivateMessage>((Func<PrivateMessage, bool>) (x => x.FullName == this.ParentID));
      }
    }

    public Listing<PrivateMessage> Thread => string.IsNullOrEmpty(this.ParentID) ? (Listing<PrivateMessage>) null : new Listing<PrivateMessage>(this.Reddit, "/message/messages/" + this.ParentID.Remove(0, 3) + ".json", this.WebAgent);

    public async Task<PrivateMessage> InitAsync(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      PrivateMessage privateMessage = this;
      privateMessage.CommonInit(reddit, json, webAgent);
      JsonConvert.PopulateObject(json[(object) "data"].ToString(), (object) privateMessage, reddit.JsonSerializerSettings);
      return privateMessage;
    }

    public PrivateMessage Init(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.CommonInit(reddit, json, webAgent);
      JsonConvert.PopulateObject(json[(object) "data"].ToString(), (object) this, reddit.JsonSerializerSettings);
      return this;
    }

    private void CommonInit(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.Init(json);
      this.Reddit = reddit;
      this.WebAgent = webAgent;
      JToken jtoken = json[(object) "data"];
      if (jtoken[(object) "replies"] == null || !((IEnumerable<JToken>) jtoken[(object) "replies"]).Any<JToken>() || jtoken[(object) "replies"][(object) "data"] == null || jtoken[(object) "replies"][(object) "data"][(object) "children"] == null)
        return;
      List<PrivateMessage> privateMessageList = new List<PrivateMessage>();
      foreach (JToken json1 in (IEnumerable<JToken>) jtoken[(object) "replies"][(object) "data"][(object) "children"])
        privateMessageList.Add(new PrivateMessage().Init(reddit, json1, webAgent));
      this.Replies = privateMessageList.ToArray();
    }

    [Obsolete("Use Thread property instead")]
    public Listing<PrivateMessage> GetThread() => this.Thread;

    public void SetAsRead()
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/read_message");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        id = this.FullName,
        uh = this.Reddit.User.Modhash,
        api_type = "json"
      });
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public async Task SetAsReadAsync()
    {
      PrivateMessage privateMessage = this;
      HttpWebRequest request = privateMessage.WebAgent.CreatePost("/api/read_message");
      IWebAgent webAgent = privateMessage.WebAgent;
      webAgent.WritePostBody(await request.GetRequestStreamAsync(), (object) new
      {
        id = privateMessage.FullName,
        uh = privateMessage.Reddit.User.Modhash,
        api_type = "json"
      });
      webAgent = (IWebAgent) null;
      WebResponse responseAsync = await request.GetResponseAsync();
      privateMessage.WebAgent.GetResponseString(responseAsync.GetResponseStream());
    }

    public void Reply(string message)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost("/api/comment");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        text = message,
        thing_id = this.FullName,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      JObject.Parse(this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream()));
    }

    public async Task ReplyAsync(string message)
    {
      PrivateMessage privateMessage = this;
      if (privateMessage.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest request = privateMessage.WebAgent.CreatePost("/api/comment");
      Stream requestStreamAsync = await request.GetRequestStreamAsync();
      privateMessage.WebAgent.WritePostBody(requestStreamAsync, (object) new
      {
        text = message,
        thing_id = privateMessage.FullName,
        uh = privateMessage.Reddit.User.Modhash
      });
      requestStreamAsync.Flush();
      WebResponse responseAsync = await request.GetResponseAsync();
      JObject.Parse(privateMessage.WebAgent.GetResponseString(responseAsync.GetResponseStream()));
    }
  }
}
