// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.AuthenticatedUser
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace RedditSharp.Things
{
  public class AuthenticatedUser : RedditUser
  {
    private const string ModeratorUrl = "/reddits/mine/moderator.json";
    private const string UnreadMessagesUrl = "/message/unread.json?mark=true&limit=25";
    private const string ModQueueUrl = "/r/mod/about/modqueue.json";
    private const string UnmoderatedUrl = "/r/mod/about/unmoderated.json";
    private const string ModMailUrl = "/message/moderator.json";
    private const string MessagesUrl = "/message/messages.json";
    private const string InboxUrl = "/message/inbox.json";
    private const string SentUrl = "/message/sent.json";

    public async Task<AuthenticatedUser> InitAsync(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      AuthenticatedUser authenticatedUser = this;
      await authenticatedUser.CommonInitAsync(reddit, json, webAgent);
      JsonConvert.PopulateObject(json[(object) "name"] == null ? json[(object) "data"].ToString() : json.ToString(), (object) authenticatedUser, reddit.JsonSerializerSettings);
      return authenticatedUser;
    }

    public AuthenticatedUser Init(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.CommonInit(reddit, json, webAgent);
      JsonConvert.PopulateObject(json[(object) "name"] == null ? json[(object) "data"].ToString() : json.ToString(), (object) this, reddit.JsonSerializerSettings);
      return this;
    }

    private void CommonInit(Reddit reddit, JToken json, IWebAgent webAgent) => base.Init(reddit, json, webAgent);

    private async Task CommonInitAsync(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      RedditUser redditUser = await base.InitAsync(reddit, json, webAgent).ConfigureAwait(false);
    }

    public Listing<Subreddit> ModeratorSubreddits => new Listing<Subreddit>(this.Reddit, "/reddits/mine/moderator.json", this.WebAgent);

    public Listing<Thing> UnreadMessages => new Listing<Thing>(this.Reddit, "/message/unread.json?mark=true&limit=25", this.WebAgent);

    public Listing<VotableThing> ModerationQueue => new Listing<VotableThing>(this.Reddit, "/r/mod/about/modqueue.json", this.WebAgent);

    public Listing<Post> UnmoderatedLinks => new Listing<Post>(this.Reddit, "/r/mod/about/unmoderated.json", this.WebAgent);

    public Listing<PrivateMessage> ModMail => new Listing<PrivateMessage>(this.Reddit, "/message/moderator.json", this.WebAgent);

    public Listing<PrivateMessage> PrivateMessages => new Listing<PrivateMessage>(this.Reddit, "/message/messages.json", this.WebAgent);

    public Listing<Thing> Inbox => new Listing<Thing>(this.Reddit, "/message/inbox.json", this.WebAgent);

    public Listing<PrivateMessage> Sent => new Listing<PrivateMessage>(this.Reddit, "/message/sent.json", this.WebAgent);

    public Listing<Post> GetUnmoderatedLinks() => new Listing<Post>(this.Reddit, "/r/mod/about/unmoderated.json", this.WebAgent);

    [Obsolete("Use ModeratorSubreddits property instead")]
    public Listing<Subreddit> GetModeratorReddits() => this.ModeratorSubreddits;

    [Obsolete("Use UnreadMessages property instead")]
    public Listing<Thing> GetUnreadMessages() => this.UnreadMessages;

    [Obsolete("Use ModerationQueue property instead")]
    public Listing<VotableThing> GetModerationQueue() => new Listing<VotableThing>(this.Reddit, "/r/mod/about/modqueue.json", this.WebAgent);

    [Obsolete("Use ModMail property instead")]
    public Listing<PrivateMessage> GetModMail() => new Listing<PrivateMessage>(this.Reddit, "/message/moderator.json", this.WebAgent);

    [Obsolete("Use PrivateMessages property instead")]
    public Listing<PrivateMessage> GetPrivateMessages() => new Listing<PrivateMessage>(this.Reddit, "/message/messages.json", this.WebAgent);

    [Obsolete("Use Inbox property instead")]
    public Listing<PrivateMessage> GetInbox() => new Listing<PrivateMessage>(this.Reddit, "/message/inbox.json", this.WebAgent);

    [JsonProperty("modhash")]
    public string Modhash { get; set; }

    [JsonProperty("has_mail")]
    public bool HasMail { get; set; }

    [JsonProperty("has_mod_mail")]
    public bool HasModMail { get; set; }
  }
}
