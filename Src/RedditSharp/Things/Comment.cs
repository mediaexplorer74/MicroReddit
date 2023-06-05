// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.Comment
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedditSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace RedditSharp.Things
{
  public class Comment : VotableThing
  {
    private const string CommentUrl = "/api/comment";
    private const string EditUserTextUrl = "/api/editusertext";
    private const string SetAsReadUrl = "/api/read_message";

    public async Task<Comment> InitAsync(
      Reddit reddit,
      JToken json,
      IWebAgent webAgent,
      Thing sender)
    {
      Comment comment = this;
      JToken data = await comment.CommonInitAsync(reddit, json, webAgent, sender);
      await comment.ParseCommentsAsync(reddit, json, webAgent, sender);
      JsonConvert.PopulateObject(data.ToString(), (object) comment, reddit.JsonSerializerSettings);
      return comment;
    }

    public Comment Init(Reddit reddit, JToken json, IWebAgent webAgent, Thing sender)
    {
      JToken jtoken = this.CommonInit(reddit, json, webAgent, sender);
      this.ParseComments(reddit, json, webAgent, sender);
      JsonConvert.PopulateObject(jtoken.ToString(), (object) this, reddit.JsonSerializerSettings);
      return this;
    }

    public Comment PopulateComments(IEnumerator<Thing> things)
    {
      Thing current = things.Current;
      Dictionary<string, Tuple<Comment, List<Comment>>> dictionary = new Dictionary<string, Tuple<Comment, List<Comment>>>()
      {
        [this.FullName] = Tuple.Create<Comment, List<Comment>>(this, new List<Comment>())
      };
      while (things.MoveNext())
      {
        switch (current)
        {
          case Comment _:
          case More _:
            current = things.Current;
            if (current is Comment)
            {
              Comment comment = (Comment) current;
              dictionary[comment.FullName] = Tuple.Create<Comment, List<Comment>>(comment, new List<Comment>());
              if (dictionary.ContainsKey(comment.ParentId))
              {
                dictionary[comment.ParentId].Item2.Add(comment);
                continue;
              }
              if (!(comment.ParentId == this.ParentId))
                continue;
              goto label_11;
            }
            else if (current is More)
            {
              More more = (More) current;
              if (dictionary.ContainsKey(more.ParentId))
              {
                dictionary[more.ParentId].Item1.More = more;
                continue;
              }
              if (more.ParentId == this.ParentId)
                goto label_11;
              else
                continue;
            }
            else
              continue;
          default:
            goto label_11;
        }
      }
label_11:
      foreach (KeyValuePair<string, Tuple<Comment, List<Comment>>> keyValuePair in dictionary)
        keyValuePair.Value.Item1.Comments = (IList<Comment>) keyValuePair.Value.Item2.ToArray();
      return this;
    }

    private JToken CommonInit(Reddit reddit, JToken json, IWebAgent webAgent, Thing sender)
    {
      this.Init(reddit, webAgent, json);
      JToken jtoken = json[(object) "data"];
      this.Reddit = reddit;
      this.WebAgent = webAgent;
      this.Parent = sender;
      if (jtoken[(object) "context"] != null)
        this.LinkId = Newtonsoft.Json.Linq.Extensions.Value<string>((IEnumerable<JToken>) jtoken[(object) "context"]).Split('/')[4];
      return jtoken;
    }

    private async Task<JToken> CommonInitAsync(
      Reddit reddit,
      JToken json,
      IWebAgent webAgent,
      Thing sender)
    {
      Comment comment = this;
      VotableThing votableThing = await comment.InitAsync(reddit, webAgent, json);
      JToken jtoken = json[(object) "data"];
      comment.Reddit = reddit;
      comment.WebAgent = webAgent;
      comment.Parent = sender;
      if (jtoken[(object) "context"] != null)
      {
        string str = Newtonsoft.Json.Linq.Extensions.Value<string>((IEnumerable<JToken>) jtoken[(object) "context"]);
        comment.LinkId = str.Split('/')[4];
      }
      return jtoken;
    }

    private void ParseComments(Reddit reddit, JToken data, IWebAgent webAgent, Thing sender)
    {
      JToken source = data[(object) nameof (data)][(object) "replies"];
      List<Comment> commentList = new List<Comment>();
      if (source != null && ((IEnumerable<JToken>) source).Count<JToken>() > 0)
      {
        foreach (JToken json in (IEnumerable<JToken>) source[(object) nameof (data)][(object) "children"])
          commentList.Add(new Comment().Init(reddit, json, webAgent, sender));
      }
      this.Comments = (IList<Comment>) commentList.ToArray();
    }

    private async Task ParseCommentsAsync(
      Reddit reddit,
      JToken data,
      IWebAgent webAgent,
      Thing sender)
    {
      JToken source = data[(object) nameof (data)][(object) "replies"];
      List<Comment> subComments = new List<Comment>();
      if (source != null && ((IEnumerable<JToken>) source).Count<JToken>() > 0)
      {
        foreach (JToken json in (IEnumerable<JToken>) source[(object) nameof (data)][(object) "children"])
        {
          List<Comment> commentList = subComments;
          commentList.Add(await new Comment().InitAsync(reddit, json, webAgent, sender));
          commentList = (List<Comment>) null;
        }
      }
      this.Comments = (IList<Comment>) subComments.ToArray();
    }

    [JsonIgnore]
    [Obsolete("Use AuthorName instead.", false)]
    public string Author => this.AuthorName;

    [JsonProperty("body")]
    public string Body { get; set; }

    [JsonProperty("body_html")]
    public string BodyHtml { get; set; }

    [JsonProperty("parent_id")]
    public string ParentId { get; set; }

    [JsonProperty("subreddit")]
    public string Subreddit { get; set; }

    [JsonProperty("link_id")]
    public string LinkId { get; set; }

    [JsonProperty("link_title")]
    public string LinkTitle { get; set; }

    [JsonIgnore]
    public More More { get; set; }

    [JsonIgnore]
    public IList<Comment> Comments { get; private set; }

    [JsonIgnore]
    public Thing Parent { get; internal set; }

    public override string Shortlink
    {
      get
      {
        string str = "";
        int num = this.LinkId.IndexOf('_');
        if (num > -1)
          str = this.LinkId.Substring(num + 1);
        return string.Format("{0}://{1}/r/{2}/comments/{3}/_/{4}", (object) RedditSharp.WebAgent.Protocol, (object) RedditSharp.WebAgent.RootDomain, (object) this.Subreddit, this.Parent != null ? (object) this.Parent.Id : (object) str, (object) this.Id);
      }
    }

    public Comment Reply(string message)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost("/api/comment");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        text = message,
        thing_id = this.FullName,
        uh = this.Reddit.User.Modhash,
        api_type = "json"
      });
      requestStream.Flush();
      try
      {
        JObject jobject = JObject.Parse(this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream()));
        if (jobject["json"][(object) "ratelimit"] != null)
          throw new RateLimitException(TimeSpan.FromSeconds(((IEnumerable<JToken>) jobject["json"][(object) "ratelimit"]).ValueOrDefault<double>()));
        return new Comment().Init(this.Reddit, jobject["json"][(object) "data"][(object) "things"][(object) 0], this.WebAgent, (Thing) this);
      }
      catch (WebException ex)
      {
        new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
        return (Comment) null;
      }
    }

    public void EditText(string newText)
    {
      if (this.Reddit.User == null)
        throw new Exception("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost("/api/editusertext");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        api_type = "json",
        text = newText,
        thing_id = this.FullName,
        uh = this.Reddit.User.Modhash
      });
      JToken jtoken = JToken.Parse(this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream()));
      if (!jtoken[(object) "json"].ToString().Contains("\"errors\": []"))
        throw new Exception("Error editing text. Error: " + jtoken[(object) "json"][(object) "errors"][(object) 0][(object) 0].ToString());
      this.Body = newText;
    }

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
  }
}
