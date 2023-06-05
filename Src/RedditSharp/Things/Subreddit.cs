// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.Subreddit
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
  public class Subreddit : Thing
  {
    private const string SubredditPostUrl = "/r/{0}.json";
    private const string SubredditNewUrl = "/r/{0}/new.json?sort=new";
    private const string SubredditHotUrl = "/r/{0}/hot.json";
    private const string SubredditRisingUrl = "/r/{0}/rising.json";
    private const string SubredditTopUrl = "/r/{0}/top.json?t={1}";
    private const string SubredditControversialUrl = "/r/{0}/controversial.json";
    private const string SubredditGildedUrl = "/r/{0}/gilded.json";
    private const string SubscribeUrl = "/api/subscribe";
    private const string GetSettingsUrl = "/r/{0}/about/edit.json";
    private const string GetReducedSettingsUrl = "/r/{0}/about.json";
    private const string ModqueueUrl = "/r/{0}/about/modqueue.json";
    private const string UnmoderatedUrl = "/r/{0}/about/unmoderated.json";
    private const string SpamUrl = "/r/{0}/about/spam.json";
    private const string EditedUrl = "/r/{0}/about/edited.json";
    private const string FlairTemplateUrl = "/api/flairtemplate";
    private const string ClearFlairTemplatesUrl = "/api/clearflairtemplates";
    private const string SetUserFlairUrl = "/api/flair";
    private const string StylesheetUrl = "/r/{0}/about/stylesheet.json";
    private const string UploadImageUrl = "/api/upload_sr_img";
    private const string FlairSelectorUrl = "/api/flairselector";
    private const string AcceptModeratorInviteUrl = "/api/accept_moderator_invite";
    private const string LeaveModerationUrl = "/api/unfriend";
    private const string BanUserUrl = "/api/friend";
    private const string UnBanUserUrl = "/api/unfriend";
    private const string AddModeratorUrl = "/api/friend";
    private const string AddContributorUrl = "/api/friend";
    private const string ModeratorsUrl = "/r/{0}/about/moderators.json";
    private const string FrontPageUrl = "/.json";
    private const string SubmitLinkUrl = "/api/submit";
    private const string FlairListUrl = "/r/{0}/api/flairlist.json";
    private const string CommentsUrl = "/r/{0}/comments.json";
    private const string SearchUrl = "/r/{0}/search.json?q={1}&restrict_sr=on&sort={2}&t={3}";
    private const string SearchUrlDate = "/r/{0}/search.json?q=timestamp:{1}..{2}&restrict_sr=on&sort={3}&syntax=cloudsearch";
    private const string ModLogUrl = "/r/{0}/about/log.json";
    private const string ContributorsUrl = "/r/{0}/about/contributors.json";
    private const string BannedUsersUrl = "/r/{0}/about/banned.json";
    private const string ModmailUrl = "/r/{0}/message/moderator/inbox.json";

    [JsonIgnore]
    public Wiki Wiki { get; private set; }

    [JsonProperty("spoilers_enabled")]
    public bool SpoilersEnabled { get; set; }

    [JsonProperty("created")]
    [JsonConverter(typeof (UnixTimestampConverter))]
    public DateTimeOffset? Created { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("description_html")]
    public string DescriptionHTML { get; set; }

    [JsonProperty("display_name")]
    public string DisplayName { get; set; }

    [JsonProperty("header_img")]
    public string HeaderImage { get; set; }

    [JsonProperty("header_title")]
    public string HeaderTitle { get; set; }

    [JsonProperty("over_18")]
    public bool NSFW { get; set; }

    [JsonProperty("public_description")]
    public string PublicDescription { get; set; }

    [JsonProperty("subscribers")]
    public int? Subscribers { get; set; }

    [JsonProperty("accounts_active")]
    public int? ActiveUsers { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("url")]
    [JsonConverter(typeof (UrlParser))]
    public Uri Url { get; set; }

    [JsonProperty("user_is_moderator")]
    public bool? UserIsModerator { get; set; }

    [JsonProperty("mod_permissions")]
    [JsonConverter(typeof (ModeratorPermissionConverter))]
    public ModeratorPermission ModPermissions { get; set; }

    [JsonProperty("user_is_banned")]
    public bool? UserIsBanned { get; set; }

    [JsonIgnore]
    public string Name { get; set; }

    public Listing<Post> GetTop(FromTime timePeriod) => this.Name == "/" ? new Listing<Post>(this.Reddit, "/top.json?t=" + System.Enum.GetName(typeof (FromTime), (object) timePeriod).ToLower(), this.WebAgent) : new Listing<Post>(this.Reddit, string.Format("/r/{0}/top.json?t={1}", (object) this.Name, (object) System.Enum.GetName(typeof (FromTime), (object) timePeriod)).ToLower(), this.WebAgent);

    public Listing<Post> Posts => this.Name == "/" ? new Listing<Post>(this.Reddit, "/.json", this.WebAgent) : new Listing<Post>(this.Reddit, string.Format("/r/{0}.json", (object) this.Name), this.WebAgent);

    public Listing<Comment> Comments => this.Name == "/" ? new Listing<Comment>(this.Reddit, "/comments.json", this.WebAgent) : new Listing<Comment>(this.Reddit, string.Format("/r/{0}/comments.json", (object) this.Name), this.WebAgent);

    public Listing<Post> New => this.Name == "/" ? new Listing<Post>(this.Reddit, "/new.json", this.WebAgent) : new Listing<Post>(this.Reddit, string.Format("/r/{0}/new.json?sort=new", (object) this.Name), this.WebAgent);

    public Listing<Post> Hot => this.Name == "/" ? new Listing<Post>(this.Reddit, "/.json", this.WebAgent) : new Listing<Post>(this.Reddit, string.Format("/r/{0}/hot.json", (object) this.Name), this.WebAgent);

    public Listing<Post> Rising => this.Name == "/" ? new Listing<Post>(this.Reddit, "/.json", this.WebAgent) : new Listing<Post>(this.Reddit, string.Format("/r/{0}/rising.json", (object) this.Name), this.WebAgent);

    public Listing<Post> Controversial => this.Name == "/" ? new Listing<Post>(this.Reddit, "/.json", this.WebAgent) : new Listing<Post>(this.Reddit, string.Format("/r/{0}/controversial.json", (object) this.Name), this.WebAgent);

    public Listing<VotableThing> Gilded => this.Name == "/" ? new Listing<VotableThing>(this.Reddit, "/.json", this.WebAgent) : new Listing<VotableThing>(this.Reddit, string.Format("/r/{0}/gilded.json", (object) this.Name), this.WebAgent);

    public Listing<VotableThing> ModQueue => new Listing<VotableThing>(this.Reddit, string.Format("/r/{0}/about/modqueue.json", (object) this.Name), this.WebAgent);

    public Listing<Post> UnmoderatedLinks => new Listing<Post>(this.Reddit, string.Format("/r/{0}/about/unmoderated.json", (object) this.Name), this.WebAgent);

    public Listing<VotableThing> Spam => new Listing<VotableThing>(this.Reddit, string.Format("/r/{0}/about/spam.json", (object) this.Name), this.WebAgent);

    public Listing<VotableThing> Edited => new Listing<VotableThing>(this.Reddit, string.Format("/r/{0}/about/edited.json", (object) this.Name), this.WebAgent);

    public Listing<Post> Search(string terms, Sorting sortE = Sorting.Relevance, TimeSorting timeE = TimeSorting.All)
    {
      string lower1 = sortE.ToString().ToLower();
      string lower2 = timeE.ToString().ToLower();
      return new Listing<Post>(this.Reddit, string.Format("/r/{0}/search.json?q={1}&restrict_sr=on&sort={2}&t={3}", (object) this.Name, (object) Uri.EscapeUriString(terms), (object) lower1, (object) lower2), this.WebAgent);
    }

    public Listing<Post> Search(DateTime from, DateTime to, Sorting sortE = Sorting.New) => this.Search(new DateTimeOffset(from), new DateTimeOffset(to), sortE);

    public Listing<Post> Search(DateTimeOffset from, DateTimeOffset to, Sorting sortE = Sorting.New)
    {
      string lower = sortE.ToString().ToLower();
      return new Listing<Post>(this.Reddit, string.Format("/r/{0}/search.json?q=timestamp:{1}..{2}&restrict_sr=on&sort={3}&syntax=cloudsearch", (object) this.Name, (object) from.ToUnixTimeSeconds(), (object) to.ToUnixTimeSeconds(), (object) lower), this.WebAgent);
    }

    public SubredditSettings Settings
    {
      get
      {
        if (this.Reddit.User == null)
          throw new AuthenticationException("No user logged in.");
        try
        {
          return new SubredditSettings(this, this.Reddit, JObject.Parse(this.WebAgent.GetResponseString(
              this.WebAgent.CreateGet(string.Format("/r/{0}/about/edit.json", 
              (object) this.Name)).GetResponseAsync().Result.GetResponseStream())), this.WebAgent);
        }
        catch
        {
          return new SubredditSettings(this, this.Reddit, JObject.Parse(
              this.WebAgent.GetResponseString(this.WebAgent.CreateGet(string.Format("/r/{0}/about.json", 
              (object) this.Name)).GetResponseAsync().Result.GetResponseStream())), this.WebAgent);
        }
      }
    }

    public UserFlairTemplate[] UserFlairTemplates
    {
      get
      {
        HttpWebRequest post = this.WebAgent.CreatePost("/api/flairselector");
        Stream requestStream = post.GetRequestStreamAsync().Result;
        this.WebAgent.WritePostBody(requestStream, (object) new
        {
          name = this.Reddit.User.Name,
          r = this.Name,
          uh = this.Reddit.User.Modhash
        });
        requestStream.Flush();
        JToken jtoken = JObject.Parse(this.WebAgent.GetResponseString(
            post.GetResponseAsync().Result.GetResponseStream()))["choices"];
        List<UserFlairTemplate> userFlairTemplateList = new List<UserFlairTemplate>();
        foreach (object obj in (IEnumerable<JToken>) jtoken)
        {
          UserFlairTemplate userFlairTemplate 
                        = JsonConvert.DeserializeObject<UserFlairTemplate>(obj.ToString());

          userFlairTemplateList.Add(userFlairTemplate);
        }
        return userFlairTemplateList.ToArray();
      }
    }

    public SubredditStyle Stylesheet => new SubredditStyle(this.Reddit, this, JToken.Parse(this.WebAgent.GetResponseString(
        this.WebAgent.CreateGet(string.Format("/r/{0}/about/stylesheet.json", 
            (object) this.Name)).GetResponseAsync().Result.GetResponseStream())), this.WebAgent);

    public IEnumerable<ModeratorUser> Moderators
    {
      get
      {
        JObject jobject = JObject.Parse(this.WebAgent.GetResponseString(
            this.WebAgent.CreateGet(string.Format("/r/{0}/about/moderators.json", 
            (object) this.Name)).GetResponseAsync().Result.GetResponseStream()));

        JToken[] jtokenArray = !(jobject["kind"].ToString() != "UserList") 
            ? ((IEnumerable<JToken>) jobject["data"][(object) "children"]).ToArray<JToken>()
            : throw new FormatException("Reddit responded with an object that is not a user listing.");
       
        ModeratorUser[] moderators = new ModeratorUser[jtokenArray.Length];
        for (int index = 0; index < jtokenArray.Length; ++index)
        {
          ModeratorUser moderatorUser = new ModeratorUser(this.Reddit, jtokenArray[index]);
          moderators[index] = moderatorUser;
        }
        return (IEnumerable<ModeratorUser>) moderators;
      }
    }

    public IEnumerable<TBUserNote> UserNotes => ToolBoxUserNotes.GetUserNotes(this.WebAgent, this.Name);

    public Listing<Contributor> Contributors => new Listing<Contributor>(this.Reddit, string.Format("/r/{0}/about/contributors.json", (object) this.Name), this.WebAgent);

    public Listing<BannedUser> BannedUsers => new Listing<BannedUser>(this.Reddit, string.Format("/r/{0}/about/banned.json", (object) this.Name), this.WebAgent);

    public Listing<PrivateMessage> Modmail
    {
      get
      {
        if (this.Reddit.User == null)
          throw new AuthenticationException("No user logged in.");
        return new Listing<PrivateMessage>(this.Reddit, string.Format("/r/{0}/message/moderator/inbox.json", (object) this.Name), this.WebAgent);
      }
    }

    public async Task<Subreddit> InitAsync(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      Subreddit subreddit = this;
      subreddit.CommonInit(reddit, json, webAgent);
      JsonConvert.PopulateObject(json[(object) "data"].ToString(), (object) subreddit, reddit.JsonSerializerSettings);
      subreddit.SetName();
      return subreddit;
    }

    public Subreddit Init(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.CommonInit(reddit, json, webAgent);
      JsonConvert.PopulateObject(json[(object) "data"].ToString(), (object) this, reddit.JsonSerializerSettings);
      this.SetName();
      return this;
    }

    private void SetName()
    {
      this.Name = this.Url.ToString();
      if (this.Name.StartsWith("/r/"))
        this.Name = this.Name.Substring(3);
      if (this.Name.StartsWith("r/"))
        this.Name = this.Name.Substring(2);
      this.Name = this.Name.TrimEnd('/');
    }

    private void CommonInit(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.Init(json);
      this.Reddit = reddit;
      this.WebAgent = webAgent;
      this.Wiki = new Wiki(reddit, this, webAgent);
    }

    public static Subreddit GetRSlashAll(Reddit reddit)
    {
      Subreddit rslashAll = new Subreddit();
      rslashAll.DisplayName = "/r/all";
      rslashAll.Title = "/r/all";
      rslashAll.Url = new Uri("/r/all", UriKind.Relative);
      rslashAll.Name = "all";
      rslashAll.Reddit = reddit;
      rslashAll.WebAgent = reddit.WebAgent;
      return rslashAll;
    }

    public static Subreddit GetFrontPage(Reddit reddit)
    {
      Subreddit frontPage = new Subreddit();
      frontPage.DisplayName = "Front Page";
      frontPage.Title = "reddit: the front page of the internet";
      frontPage.Url = new Uri("/", UriKind.Relative);
      frontPage.Name = "/";
      frontPage.Reddit = reddit;
      frontPage.WebAgent = reddit.WebAgent;
      return frontPage;
    }

    public void Subscribe()
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost("/api/subscribe");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        action = "sub",
        sr = this.FullName,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void Unsubscribe()
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost("/api/subscribe");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        action = "unsub",
        sr = this.FullName,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void ClearFlairTemplates(FlairType flairType)
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/clearflairtemplates");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        flair_type = (flairType == FlairType.Link ? "LINK_FLAIR" : "USER_FLAIR"),
        uh = this.Reddit.User.Modhash,
        r = this.Name
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void AddFlairTemplate(
      string cssClass,
      FlairType flairType,
      string text,
      bool userEditable)
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/flairtemplate");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        css_class = cssClass,
        flair_type = (flairType == FlairType.Link ? "LINK_FLAIR" : "USER_FLAIR"),
        text = text,
        text_editable = userEditable,
        uh = this.Reddit.User.Modhash,
        r = this.Name,
        api_type = "json"
      });
      requestStream.Flush();

      JToken.Parse(this.WebAgent.GetResponseString(
          post.GetResponseAsync().Result.GetResponseStream()));
    }

        /// <summary>
        /// Get the text of the specified users flair.
        /// </summary>
        /// <param name="user">reddit username</param>
        /// <returns></returns>
        public string GetFlairText(string user)
        {
            var request = WebAgent.CreateGet(string.Format(FlairListUrl + 
                "?name=" + user, Name));
            var response = request.GetResponseAsync().Result;
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            var json = JToken.Parse(data);
            return (string)json["users"][0]["flair_text"];
        }

        /// <summary>
        /// Get the text of the specified users flair.
        /// </summary>
        /// <param name="user">reddit username</param>
        /// <returns></returns>
        public async Task<string> GetFlairTextAsync(string user)
        {
            var request = WebAgent.CreateGet(string.Format(FlairListUrl + "?name=" + user, Name));
            var response = await request.GetResponseAsync();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            var json = JToken.Parse(data);
            return (string)json["users"][0]["flair_text"];
        }

        /// <summary>
        /// Get the css class of the specified users flair.
        /// </summary>
        /// <param name="user">reddit username</param>
        /// <returns></returns>
        public string GetFlairCssClass(string user) 
        {
            var request = WebAgent.CreateGet(string.Format(FlairListUrl 
                + "?name=" + user, Name));
            var response = request.GetResponseAsync().Result;
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            var json = JToken.Parse(data);
            return (string)json["users"][0]["flair_css_class"];
        }

        /// <summary>
        /// Get the css class of the specified users flair.
        /// </summary>
        /// <param name="user">reddit username</param>
        /// <returns></returns>
        public async Task<string> GetFlairCssClassAsync(string user)
    {
            var request = WebAgent.CreateGet(string.Format(FlairListUrl + "?name=" + user, Name));
            var response = await request.GetResponseAsync();
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            var json = JToken.Parse(data);
            return (string)json["users"][0]["flair_css_class"];
        }

    public void SetUserFlair(string user, string cssClass, string text)
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/flair");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        css_class = cssClass,
        text = text,
        uh = this.Reddit.User.Modhash,
        r = this.Name,
        name = user
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public async Task SetUserFlairAsync(string user, string cssClass, string text)
    {
      Subreddit subreddit = this;
      HttpWebRequest request = subreddit.WebAgent.CreatePost("/api/flair");
      Stream requestStreamAsync = await request.GetRequestStreamAsync();
      subreddit.WebAgent.WritePostBody(requestStreamAsync, (object) new
      {
        css_class = cssClass,
        text = text,
        uh = subreddit.Reddit.User.Modhash,
        r = subreddit.Name,
        name = user
      });
      requestStreamAsync.Flush();
      WebResponse responseAsync = await request.GetResponseAsync();
      subreddit.WebAgent.GetResponseString(responseAsync.GetResponseStream());
    }

    public void UploadHeaderImage(string name, ImageType imageType, byte[] file)
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/upload_sr_img");
      MultipartFormBuilder multipartFormBuilder = new MultipartFormBuilder(post);
      multipartFormBuilder.AddDynamic((object) new
      {
        name = name,
        uh = this.Reddit.User.Modhash,
        r = this.Name,
        formid = "image-upload",
        img_type = (imageType == ImageType.PNG ? "png" : "jpg"),
        upload = "",
        header = 1
      });
      multipartFormBuilder.AddFile(nameof (file), "foo.png", 
          file, imageType == ImageType.PNG ? "image/png" : "image/jpeg");
      multipartFormBuilder.Finish();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public async Task UploadHeaderImageAsync(string name, ImageType imageType, byte[] file)
    {
      Subreddit subreddit = this;
      HttpWebRequest post = subreddit.WebAgent.CreatePost("/api/upload_sr_img");
      MultipartFormBuilder multipartFormBuilder = new MultipartFormBuilder(post);
      multipartFormBuilder.AddDynamic((object) new
      {
        name = name,
        uh = subreddit.Reddit.User.Modhash,
        r = subreddit.Name,
        formid = "image-upload",
        img_type = (imageType == ImageType.PNG ? "png" : "jpg"),
        upload = "",
        header = 1
      });
      multipartFormBuilder.AddFile(nameof (file), "foo.png", file, imageType == ImageType.PNG ? "image/png" : "image/jpeg");
      multipartFormBuilder.Finish();
      WebResponse responseAsync = await post.GetResponseAsync();
      subreddit.WebAgent.GetResponseString(responseAsync.GetResponseStream());
    }

    public void AddModerator(string user)
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/friend");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        api_type = "json",
        uh = this.Reddit.User.Modhash,
        r = this.Name,
        type = "moderator",
        name = user
      });
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void AddModerator(RedditUser user)
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/friend");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        api_type = "json",
        uh = this.Reddit.User.Modhash,
        r = this.Name,
        type = "moderator",
        name = user.Name
      });
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void AcceptModeratorInvite()
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/accept_moderator_invite");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        api_type = "json",
        uh = this.Reddit.User.Modhash,
        r = this.Name
      });
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void RemoveModerator(string id)
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/unfriend");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        api_type = "json",
        uh = this.Reddit.User.Modhash,
        r = this.Name,
        type = "moderator",
        id = id
      });
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public override string ToString() => "/r/" + this.DisplayName;

    public void AddContributor(string user)
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/friend");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        api_type = "json",
        uh = this.Reddit.User.Modhash,
        r = this.Name,
        type = "contributor",
        name = user
      });
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void RemoveContributor(string id)
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/unfriend");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        api_type = "json",
        uh = this.Reddit.User.Modhash,
        r = this.Name,
        type = "contributor",
        id = id
      });
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public async Task RemoveContributorAsync(string id)
    {
      Subreddit subreddit = this;
      HttpWebRequest request = subreddit.WebAgent.CreatePost("/api/unfriend");
      IWebAgent webAgent = subreddit.WebAgent;
      webAgent.WritePostBody(await request.GetRequestStreamAsync(), (object) new
      {
        api_type = "json",
        uh = subreddit.Reddit.User.Modhash,
        r = subreddit.Name,
        type = "contributor",
        id = id
      });
      webAgent = (IWebAgent) null;
      WebResponse response = request.GetResponseAsync().Result;
      subreddit.WebAgent.GetResponseString(response.GetResponseStream());
    }

    public void BanUser(string user, string reason, string note, int duration, string message)
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/friend");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        api_type = "json",
        uh = this.Reddit.User.Modhash,
        r = this.Name,
        container = this.FullName,
        type = "banned",
        name = user,
        ban_reason = reason,
        note = note,
        duration = (duration <= 0 ? "" : duration.ToString()),
        ban_message = message
      });
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public async Task BanUserAsync(
      string user,
      string reason,
      string note,
      int duration,
      string message)
    {
      Subreddit subreddit = this;
      HttpWebRequest request = subreddit.WebAgent.CreatePost("/api/friend");
      IWebAgent webAgent = subreddit.WebAgent;
      webAgent.WritePostBody(await request.GetRequestStreamAsync(), (object) new
      {
        api_type = "json",
        uh = subreddit.Reddit.User.Modhash,
        r = subreddit.Name,
        container = subreddit.FullName,
        type = "banned",
        name = user,
        ban_reason = reason,
        note = note,
        duration = (duration <= 0 ? "" : duration.ToString()),
        ban_message = message
      });
      webAgent = (IWebAgent) null;
      WebResponse responseAsync = await request.GetResponseAsync();
      subreddit.WebAgent.GetResponseString(responseAsync.GetResponseStream());
    }

    public void BanUser(string user, string note) => this.BanUser(user, "", note, 0, "");

    public async Task BanUserAsync(string user, string note) => await this.BanUserAsync(user, "", note, 0, "");

    public void UnBanUser(string user)
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/unfriend");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        uh = this.Reddit.User.Modhash,
        r = this.Name,
        type = "banned",
        container = this.FullName,
        executed = "removed",
        name = user
      });
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public async Task UnBanUserAsync(string user)
    {
      Subreddit subreddit = this;
      HttpWebRequest request = subreddit.WebAgent.CreatePost("/api/unfriend");
      IWebAgent webAgent = subreddit.WebAgent;
      webAgent.WritePostBody(await request.GetRequestStreamAsync(), (object) new
      {
        uh = subreddit.Reddit.User.Modhash,
        r = subreddit.Name,
        type = "banned",
        container = subreddit.FullName,
        executed = "removed",
        name = user
      });
      webAgent = (IWebAgent) null;
      WebResponse responseAsync = await request.GetResponseAsync();
      subreddit.WebAgent.GetResponseString(responseAsync.GetResponseStream());
    }

    private Post Submit(SubmitData data)
    {
      if (this.Reddit.User == null)
        throw new RedditException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost("/api/submit");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) data);
      JToken jtoken = JToken.Parse(this.WebAgent.GetResponseString(
          post.GetResponseAsync().Result.GetResponseStream()));
      ICaptchaSolver captchaSolver = this.Reddit.CaptchaSolver;

      if (((IEnumerable<JToken>) jtoken[(object) "json"][(object) "errors"]).Any<JToken>() 
                && jtoken[(object) "json"][(object) "errors"][(object) 0][(object) 0].ToString() == "BAD_CAPTCHA" && captchaSolver != null)
      {
        data.Iden = jtoken[(object) "json"][(object) "captcha"].ToString();
        CaptchaResponse captchaResponse = captchaSolver.HandleCaptcha(new Captcha(data.Iden));
        data.Captcha = !captchaResponse.Cancel ? captchaResponse.Answer : throw new CaptchaFailedException("Captcha verification failed when submitting " + data.Kind + " post");
        return this.Submit(data);
      }
      if (((IEnumerable<JToken>) jtoken[(object) "json"][(object) "errors"]).Any<JToken>() && jtoken[(object) "json"][(object) "errors"][(object) 0][(object) 0].ToString() == "ALREADY_SUB")
        throw new DuplicateLinkException(string.Format("Post failed when submitting.  The following link has already been submitted: {0}", (object) "/api/submit"));
      if (((IEnumerable<JToken>) jtoken[(object) "json"][(object) "errors"]).Any<JToken>())
        throw new Exception("Error when attempting to submit. Error: " + jtoken[(object) "json"][(object) "errors"][(object) 0][(object) 0].ToString());
      return new Post().Init(this.Reddit, jtoken[(object) "json"], this.WebAgent);
    }

    private async Task<Post> SubmitAsync(SubmitData data)
    {
      Subreddit subreddit = this;
      if (subreddit.Reddit.User == null)
        throw new RedditException("No user logged in.");
      HttpWebRequest request = subreddit.WebAgent.CreatePost("/api/submit");
      IWebAgent webAgent = subreddit.WebAgent;
      webAgent.WritePostBody(await request.GetRequestStreamAsync(), (object) data);
      webAgent = (IWebAgent) null;
      WebResponse responseAsync = await request.GetResponseAsync();
      JToken json = JToken.Parse(subreddit.WebAgent.GetResponseString(responseAsync.GetResponseStream()));
      ICaptchaSolver captchaSolver = subreddit.Reddit.CaptchaSolver;
      if (((IEnumerable<JToken>) json[(object) "json"][(object) "errors"]).Any<JToken>() && json[(object) "json"][(object) "errors"][(object) 0][(object) 0].ToString() == "BAD_CAPTCHA" && captchaSolver != null)
      {
        data.Iden = json[(object) "json"][(object) "captcha"].ToString();
        CaptchaResponse captchaResponse = captchaSolver.HandleCaptcha(new Captcha(data.Iden));
        data.Captcha = !captchaResponse.Cancel ? captchaResponse.Answer : throw new CaptchaFailedException("Captcha verification failed when submitting " + data.Kind + " post");
        return await subreddit.SubmitAsync(data);
      }
      if (((IEnumerable<JToken>) json[(object) "json"][(object) "errors"]).Any<JToken>() && json[(object) "json"][(object) "errors"][(object) 0][(object) 0].ToString() == "ALREADY_SUB")
        throw new DuplicateLinkException(string.Format("Post failed when submitting.  The following link has already been submitted: {0}", (object) "/api/submit"));
      if (((IEnumerable<JToken>) json[(object) "json"][(object) "errors"]).Any<JToken>())
        throw new Exception("Error when attempting to submit. Error: " + json[(object) "json"][(object) "errors"][(object) 0][(object) 0].ToString());
      return new Post().Init(subreddit.Reddit, json[(object) "json"], subreddit.WebAgent);
    }

    public Post SubmitPost(
      string title,
      string url,
      string captchaId = "",
      string captchaAnswer = "",
      bool resubmit = false)
    {
      LinkData data = new LinkData();
      data.Subreddit = this.Name;
      data.UserHash = this.Reddit.User.Modhash;
      data.Title = title;
      data.URL = url;
      data.Resubmit = resubmit;
      data.Iden = captchaId;
      data.Captcha = captchaAnswer;
      return this.Submit((SubmitData) data);
    }

    public async Task<Post> SubmitPostAsync(
      string title,
      string url,
      string captchaId = "",
      string captchaAnswer = "",
      bool resubmit = false)
    {
      Subreddit subreddit1 = this;
      Subreddit subreddit2 = subreddit1;
      LinkData data = new LinkData();
      data.Subreddit = subreddit1.Name;
      data.UserHash = subreddit1.Reddit.User.Modhash;
      data.Title = title;
      data.URL = url;
      data.Resubmit = resubmit;
      data.Iden = captchaId;
      data.Captcha = captchaAnswer;
      return await subreddit2.SubmitAsync((SubmitData) data);
    }

    public Post SubmitTextPost(string title, string text, string captchaId = "", string captchaAnswer = "")
    {
      TextData data = new TextData();
      data.Subreddit = this.Name;
      data.UserHash = this.Reddit.User.Modhash;
      data.Title = title;
      data.Text = text;
      data.Iden = captchaId;
      data.Captcha = captchaAnswer;
      return this.Submit((SubmitData) data);
    }

    public async Task<Post> SubmitTextPostAsync(
      string title,
      string text,
      string captchaId = "",
      string captchaAnswer = "")
    {
      Subreddit subreddit1 = this;
      Subreddit subreddit2 = subreddit1;
      TextData data = new TextData();
      data.Subreddit = subreddit1.Name;
      data.UserHash = subreddit1.Reddit.User.Modhash;
      data.Title = title;
      data.Text = text;
      data.Iden = captchaId;
      data.Captcha = captchaAnswer;
      return await subreddit2.SubmitAsync((SubmitData) data);
    }

    public Listing<ModAction> GetModerationLog() => new Listing<ModAction>(this.Reddit, string.Format("/r/{0}/about/log.json", (object) this.Name), this.WebAgent);

    public Listing<ModAction> GetModerationLog(ModActionType action) => new Listing<ModAction>(this.Reddit, string.Format("/r/{0}/about/log.json?type={1}", (object) this.Name, (object) ModActionTypeConverter.GetRedditParamName(action)), this.WebAgent);

    public Listing<ModAction> GetModerationLog(string[] mods) => new Listing<ModAction>(this.Reddit, string.Format("/r/{0}/about/log.json?mod={1}", (object) this.Name, (object) string.Join(",", mods)), this.WebAgent);

    public Listing<ModAction> GetModerationLog(ModActionType action, string[] mods) => new Listing<ModAction>(this.Reddit, string.Format("/r/{0}/about/log.json?type={1}&mod={2}", (object) this.Name, (object) ModActionTypeConverter.GetRedditParamName(action), (object) string.Join(",", mods)), this.WebAgent);

    public IEnumerable<Comment> CommentStream => this.Name == "/" ? new Listing<Comment>(this.Reddit, "/comments.json", this.WebAgent).GetListingStream() : new Listing<Comment>(this.Reddit, string.Format("/r/{0}/comments.json", (object) this.Name), this.WebAgent).GetListingStream();

    public IEnumerable<Post> SubmissionStream => this.Name == "/" ? new Listing<Post>(this.Reddit, "/new.json", this.WebAgent).GetListingStream() : new Listing<Post>(this.Reddit, string.Format("/r/{0}/new.json?sort=new", (object) this.Name), this.WebAgent).GetListingStream();

    public IEnumerable<ModAction> ModerationLogStream
    {
      get
      {
        int num = this.Name == "/" ? 1 : 0;
        return new Listing<ModAction>(this.Reddit, string.Format("/r/{0}/about/log.json", (object) this.Name), this.WebAgent).GetListingStream();
      }
    }

    [Obsolete("Use Posts property instead")]
    public Listing<Post> GetPosts() => this.Posts;

    [Obsolete("Use New property instead")]
    public Listing<Post> GetNew() => this.New;

    [Obsolete("Use Hot property instead")]
    public Listing<Post> GetHot() => this.Hot;

    [Obsolete("Use ModQueue property instead")]
    public Listing<VotableThing> GetModQueue() => this.ModQueue;

    [Obsolete("Use UnmoderatedLinks property instead")]
    public Listing<Post> GetUnmoderatedLinks() => this.UnmoderatedLinks;

    [Obsolete("Use Settings property instead")]
    public SubredditSettings GetSettings() => this.Settings;

    [Obsolete("Use UserFlairTemplates property instead")]
    public UserFlairTemplate[] GetUserFlairTemplates() => this.UserFlairTemplates;

    [Obsolete("Use Stylesheet property instead")]
    public SubredditStyle GetStylesheet() => this.Stylesheet;

    [Obsolete("Use Moderators property instead")]
    public IEnumerable<ModeratorUser> GetModerators() => this.Moderators;
  }
}
