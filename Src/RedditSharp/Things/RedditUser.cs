// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.RedditUser
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace RedditSharp.Things
{
  public class RedditUser : Thing
  {
    private const string OverviewUrl = "/user/{0}.json";
    private const string CommentsUrl = "/user/{0}/comments.json";
    private const string LinksUrl = "/user/{0}/submitted.json";
    private const string SubscribedSubredditsUrl = "/subreddits/mine.json";
    private const string LikedUrl = "/user/{0}/liked.json";
    private const string DislikedUrl = "/user/{0}/disliked.json";
    private const string SavedUrl = "/user/{0}/saved.json";
    private const int MAX_LIMIT = 100;

    public async Task<RedditUser> InitAsync(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      RedditUser redditUser = this;
      redditUser.CommonInit(reddit, json, webAgent);
      JsonConvert.PopulateObject(json[(object) "name"] == null ? json[(object) "data"].ToString() : json.ToString(), (object) redditUser, reddit.JsonSerializerSettings);
      return redditUser;
    }

    public RedditUser Init(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.CommonInit(reddit, json, webAgent);
      JsonConvert.PopulateObject(json[(object) "name"] == null ? json[(object) "data"].ToString() : json.ToString(), (object) this, reddit.JsonSerializerSettings);
      return this;
    }

    private void CommonInit(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.Init(json);
      this.Reddit = reddit;
      this.WebAgent = webAgent;
    }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("is_gold")]
    public bool HasGold { get; set; }

    [JsonProperty("is_mod")]
    public bool IsModerator { get; set; }

    [JsonProperty("link_karma")]
    public int LinkKarma { get; set; }

    [JsonProperty("comment_karma")]
    public int CommentKarma { get; set; }

    [JsonProperty("created")]
    [JsonConverter(typeof (UnixTimestampConverter))]
    public DateTimeOffset Created { get; set; }

    public Listing<VotableThing> Overview => new Listing<VotableThing>(this.Reddit, string.Format("/user/{0}.json", (object) this.Name), this.WebAgent);

    public Listing<Post> LikedPosts => new Listing<Post>(this.Reddit, string.Format("/user/{0}/liked.json", (object) this.Name), this.WebAgent);

    public Listing<Post> DislikedPosts => new Listing<Post>(this.Reddit, string.Format("/user/{0}/disliked.json", (object) this.Name), this.WebAgent);

    public Listing<Comment> Comments => new Listing<Comment>(this.Reddit, string.Format("/user/{0}/comments.json", (object) this.Name), this.WebAgent);

    public Listing<Post> Posts => new Listing<Post>(this.Reddit, string.Format("/user/{0}/submitted.json", (object) this.Name), this.WebAgent);

    public Listing<Subreddit> SubscribedSubreddits => new Listing<Subreddit>(this.Reddit, "/subreddits/mine.json", this.WebAgent);

    public Listing<VotableThing> GetOverview(Sort sorting = Sort.New, int limit = 25, FromTime fromTime = FromTime.All)
    {
      if (limit < 1 || limit > 100)
        throw new ArgumentOutOfRangeException(nameof (limit), "Valid range: [1," + (object) 100 + "]");
      return new Listing<VotableThing>(this.Reddit, string.Format("/user/{0}.json", (object) this.Name) + string.Format("?sort={0}&limit={1}&t={2}", (object) Enum.GetName(typeof (Sort), (object) sorting), (object) limit, (object) Enum.GetName(typeof (FromTime), (object) fromTime)), this.WebAgent);
    }

    public Listing<Comment> GetComments(Sort sorting = Sort.New, int limit = 25, FromTime fromTime = FromTime.All)
    {
      if (limit < 1 || limit > 100)
        throw new ArgumentOutOfRangeException(nameof (limit), "Valid range: [1," + (object) 100 + "]");
      return new Listing<Comment>(this.Reddit, string.Format("/user/{0}/comments.json", (object) this.Name) + string.Format("?sort={0}&limit={1}&t={2}", (object) Enum.GetName(typeof (Sort), (object) sorting), (object) limit, (object) Enum.GetName(typeof (FromTime), (object) fromTime)), this.WebAgent);
    }

    public Listing<Post> GetPosts(Sort sorting = Sort.New, int limit = 25, FromTime fromTime = FromTime.All)
    {
      if (limit < 1 || limit > 100)
        throw new ArgumentOutOfRangeException(nameof (limit), "Valid range: [1,100]");
      return new Listing<Post>(this.Reddit, string.Format("/user/{0}/submitted.json", (object) this.Name) + string.Format("?sort={0}&limit={1}&t={2}", (object) Enum.GetName(typeof (Sort), (object) sorting), (object) limit, (object) Enum.GetName(typeof (FromTime), (object) fromTime)), this.WebAgent);
    }

    public Listing<VotableThing> GetSaved(Sort sorting = Sort.New, int limit = 25, FromTime fromTime = FromTime.All)
    {
      if (limit < 1 || limit > 100)
        throw new ArgumentOutOfRangeException(nameof (limit), "Valid range: [1," + (object) 100 + "]");
      return new Listing<VotableThing>(this.Reddit, string.Format("/user/{0}/saved.json", (object) this.Name) + string.Format("?sort={0}&limit={1}&t={2}", (object) Enum.GetName(typeof (Sort), (object) sorting), (object) limit, (object) Enum.GetName(typeof (FromTime), (object) fromTime)), this.WebAgent);
    }

    public override string ToString() => this.Name;

    [Obsolete("Use Overview property instead")]
    public Listing<VotableThing> GetOverview() => this.Overview;

    [Obsolete("Use Comments property instead")]
    public Listing<Comment> GetComments() => this.Comments;

    [Obsolete("Use Posts property instead")]
    public Listing<Post> GetPosts() => this.Posts;

    [Obsolete("Use SubscribedSubreddits property instead")]
    public Listing<Subreddit> GetSubscribedSubreddits() => this.SubscribedSubreddits;
  }
}
