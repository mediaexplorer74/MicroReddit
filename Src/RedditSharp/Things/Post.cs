// Type: RedditSharp.Things.Post
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedditSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web;

namespace RedditSharp.Things
{
  public class Post : VotableThing
  {
    private const string CommentUrl = "/api/comment";
    private const string GetCommentsUrl = "/comments/{0}.json";
    private const string EditUserTextUrl = "/api/editusertext";
    private const string HideUrl = "/api/hide";
    private const string UnhideUrl = "/api/unhide";
    private const string SetFlairUrl = "/r/{0}/api/flair";
    private const string MarkNSFWUrl = "/api/marknsfw";
    private const string UnmarkNSFWUrl = "/api/unmarknsfw";
    private const string ContestModeUrl = "/api/set_contest_mode";
    private const string StickyModeUrl = "/api/set_subreddit_sticky";
    private const string SpoilerUrl = "/api/spoiler";
    private const string UnSpoilerUrl = "/api/unspoiler";

    public async Task<Post> InitAsync(Reddit reddit, JToken post, IWebAgent webAgent)
    {
      Post post1 = this;
      await post1.CommonInitAsync(reddit, post, webAgent);
      JsonConvert.PopulateObject(post[(object) "data"].ToString(), 
          (object) post1, reddit.JsonSerializerSettings);
      return post1;
    }

    public Post Init(Reddit reddit, JToken post, IWebAgent webAgent)
    {
      this.CommonInit(reddit, post, webAgent);
      JsonConvert.PopulateObject(post[(object) "data"].ToString(), 
          (object) this, reddit.JsonSerializerSettings);
      return this;
    }

    private void CommonInit(Reddit reddit, JToken post, IWebAgent webAgent)
    {
      this.Init(reddit, webAgent, post);
      this.Reddit = reddit;
      this.WebAgent = webAgent;
    }

    private async Task CommonInitAsync(Reddit reddit, JToken post, IWebAgent webAgent)
    {
      Post post1 = this;
      VotableThing votableThing = await post1.InitAsync(reddit, webAgent, post);
      post1.Reddit = reddit;
      post1.WebAgent = webAgent;
    }

    [JsonIgnore]
    public RedditUser Author => this.Reddit.GetUser(this.AuthorName);

        public RedditSharp.Things.Comment[] Comments
        {
            get
            {
                return this.ListComments().ToArray();
            }
        }

        [JsonProperty("spoiler")]
    public bool IsSpoiler { get; set; }

    [JsonProperty("domain")]
    public string Domain { get; set; }

    [JsonProperty("is_self")]
    public bool IsSelfPost { get; set; }

    [JsonProperty("link_flair_css_class")]
    public string LinkFlairCssClass { get; set; }

    [JsonProperty("link_flair_text")]
    public string LinkFlairText { get; set; }

    [JsonProperty("num_comments")]
    public int CommentCount { get; set; }

    [JsonProperty("over_18")]
    public bool NSFW { get; set; }

    [JsonProperty("permalink")]
    [JsonConverter(typeof (UrlParser))]
    public Uri Permalink { get; set; }

    [JsonProperty("selftext")]
    public string SelfText { get; set; }

    [JsonProperty("selftext_html")]
    public string SelfTextHtml { get; set; }

    [JsonProperty("thumbnail")]
    [JsonConverter(typeof (UrlParser))]
    public Uri Thumbnail { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("subreddit")]
    public string SubredditName { get; set; }

    [JsonIgnore]
    public Subreddit Subreddit => this.Reddit.GetSubreddit("/r/" + this.SubredditName);

    [JsonProperty("url")]
    [JsonConverter(typeof (UrlParser))]
    public Uri Url { get; set; }

    public RedditSharp.Things.Comment Comment(string message)
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
      requestStream.Flush();//.Close();

      JObject jobject = JObject.Parse(this.WebAgent.GetResponseString(
          post.GetResponseAsync().Result.GetResponseStream()));

      if (jobject["json"][(object) "ratelimit"] != null)
        throw new RateLimitException(
            TimeSpan.FromSeconds(((IEnumerable<JToken>) jobject["json"][(object) "ratelimit"])
            .ValueOrDefault<double>()));
      return new RedditSharp.Things.Comment()
                .Init(this.Reddit, jobject["json"][(object) "data"][(object) "things"][(object) 0],
                this.WebAgent, (Thing) this);
    }

    public void Spoiler() => this.SimpleAction("/api/spoiler");

    public void UnSpoiler() => this.SimpleAction("/api/unspoiler");

    private string SimpleActionToggle(string endpoint, bool value, bool requiresModAction = false)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      List<string> list = this.Subreddit.Moderators.Select<ModeratorUser, string>((Func<ModeratorUser, string>) (b => b.Name)).ToList<string>();
      if (requiresModAction && !list.Contains(this.Reddit.User.Name))
        throw new AuthenticationException(string.Format("User {0} is not a moderator of subreddit {1}.", (object) this.Reddit.User.Name, (object) this.Subreddit.Name));
      HttpWebRequest post = this.WebAgent.CreatePost(endpoint);
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        id = this.FullName,
        state = value,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      return this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void Hide() => this.SimpleAction("/api/hide");

    public void Unhide() => this.SimpleAction("/api/unhide");

    public void MarkNSFW() => this.SimpleAction("/api/marknsfw");

    public void UnmarkNSFW() => this.SimpleAction("/api/unmarknsfw");

    public void ContestMode(bool state) => this.SimpleActionToggle("/api/set_contest_mode", state);

    public void StickyMode(bool state) => this.SimpleActionToggle("/api/set_subreddit_sticky", state, true);

    [Obsolete("Use Comments property instead")]
    public RedditSharp.Things.Comment[] GetComments() => this.Comments;

    public void EditText(string newText)
    {
      if (this.Reddit.User == null)
        throw new Exception("No user logged in.");
      if (!this.IsSelfPost)
        throw new Exception("Submission to edit is not a self-post.");
      HttpWebRequest post = this.WebAgent.CreatePost("/api/editusertext");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        api_type = "json",
        text = newText,
        thing_id = this.FullName,
        uh = this.Reddit.User.Modhash
      });
      JToken jtoken = JToken.Parse(this.WebAgent.GetResponseString(
          post.GetResponseAsync().Result.GetResponseStream()));
      if (!jtoken[(object) "json"].ToString().Contains("\"errors\": []"))
        throw new Exception("Error editing text. Error: " + jtoken[(object) "json"][(object) "errors"][(object) 0][(object) 0].ToString());
      this.SelfText = newText;
    }

    public void Update() => JsonConvert.PopulateObject(this.Reddit.GetToken(this.Url)[(object) "data"].ToString(), (object) this, this.Reddit.JsonSerializerSettings);

    public void SetFlair(string flairText, string flairClass)
    {
      if (this.Reddit.User == null)
        throw new Exception("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/r/{0}/api/flair", (object) this.SubredditName));
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        api_type = "json",
        css_class = flairClass,
        link = this.FullName,
        name = this.Reddit.User.Name,
        text = flairText,
        uh = this.Reddit.User.Modhash
      });
      JToken.Parse(this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream()));
      this.LinkFlairText = flairText;
    }

    public List<RedditSharp.Things.Comment> ListComments(int? limit = null)
    {
      string url = string.Format("/comments/{0}.json", (object) this.Id);
      if (limit.HasValue)
      {
        NameValueCollection queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString.Add(nameof (limit), limit.Value.ToString());
        url = string.Format("{0}?{1}", (object) url, (object) queryString);
      }
      JToken jtoken = ((IEnumerable<JToken>) JArray.Parse(this.WebAgent.GetResponseString(this.WebAgent.CreateGet(url).GetResponseAsync().Result.GetResponseStream()))).Last<JToken>()[(object) "data"][(object) "children"];
      List<RedditSharp.Things.Comment> commentList = new List<RedditSharp.Things.Comment>();
      foreach (JToken json in (IEnumerable<JToken>) jtoken)
      {
        RedditSharp.Things.Comment comment = new RedditSharp.Things.Comment().Init(this.Reddit, json, this.WebAgent, (Thing) this);
        if (!(comment.Kind == "more"))
          commentList.Add(comment);
      }
      return commentList;
    }

    public IEnumerable<RedditSharp.Things.Comment> EnumerateComments()
    {
      Post sender = this;
      string url = string.Format("/comments/{0}.json", (object) sender.Id);
      WebResponse response = sender.WebAgent.CreateGet(url).GetResponseAsync().Result;
      JToken jtoken1 = ((IEnumerable<JToken>) JArray.Parse(sender.WebAgent.GetResponseString(response.GetResponseStream()))).Last<JToken>()[(object) "data"][(object) "children"];
      More moreComments = (More) null;
      foreach (JToken jtoken2 in (IEnumerable<JToken>) jtoken1)
      {
        RedditSharp.Things.Comment comment = new RedditSharp.Things.Comment().Init(sender.Reddit, jtoken2, sender.WebAgent, (Thing) sender);
        if (comment.Kind == "more")
          moreComments = new More().Init(sender.Reddit, jtoken2, sender.WebAgent);
        else
          yield return comment;
      }
      if (moreComments != null)
      {
        IEnumerator<Thing> things = moreComments.Things().GetEnumerator();
        things.MoveNext();
        Thing currentThing = (Thing) null;
        while (currentThing != things.Current)
        {
          currentThing = things.Current;
          if (things.Current is RedditSharp.Things.Comment)
            yield return ((RedditSharp.Things.Comment) things.Current).PopulateComments(things);
          if (things.Current is More)
          {
            More current = (More) things.Current;
            if (!(current.ParentId != sender.FullName))
            {
              things = current.Things().GetEnumerator();
              things.MoveNext();
            }
            else
              break;
          }
        }
        things = (IEnumerator<Thing>) null;
        currentThing = (Thing) null;
      }
    }
  }
}
