// Decompiled with JetBrains decompiler
// Type: RedditSharp.SubredditSettings
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedditSharp.Extensions;
using RedditSharp.Things;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace RedditSharp
{
  public class SubredditSettings
  {
    private const string SiteAdminUrl = "/api/site_admin";
    private const string DeleteHeaderImageUrl = "/api/delete_sr_header";

    private Reddit Reddit { get; set; }

    private IWebAgent WebAgent { get; set; }

    [JsonIgnore]
    public Subreddit Subreddit { get; set; }

    public SubredditSettings(Reddit reddit, Subreddit subreddit, IWebAgent webAgent)
    {
      this.Subreddit = subreddit;
      this.Reddit = reddit;
      this.WebAgent = webAgent;
      this.AllowAsDefault = true;
      this.AllowImages = false;
      this.ExcludeBannedUsersFromModqueue = false;
      this.Domain = (string) null;
      this.Sidebar = string.Empty;
      this.Language = "en";
      this.Title = this.Subreddit.DisplayName;
      this.WikiEditKarma = 100;
      this.WikiEditAge = 10;
      this.UseDomainCss = false;
      this.UseDomainSidebar = false;
      this.HeaderHoverText = string.Empty;
      this.NSFW = false;
      this.PublicDescription = string.Empty;
      this.WikiEditMode = WikiEditMode.None;
      this.SubredditType = SubredditType.Public;
      this.ShowThumbnails = true;
      this.ContentOptions = ContentOptions.All;
      this.SpamFilter = new SpamFilterSettings();
    }

    public SubredditSettings(Subreddit subreddit, Reddit reddit, JObject json, IWebAgent webAgent)
      : this(reddit, subreddit, webAgent)
    {
      JToken jtoken = json["data"];
      this.AllowAsDefault = ((IEnumerable<JToken>) jtoken[(object) "default_set"]).ValueOrDefault<bool>();
      this.AllowImages = ((IEnumerable<JToken>) jtoken[(object) "allow_images"]).ValueOrDefault<bool>();
      this.ExcludeBannedUsersFromModqueue = ((IEnumerable<JToken>) jtoken[(object) "exclude_banned_modqueue"]).ValueOrDefault<bool>();
      this.Domain = ((IEnumerable<JToken>) jtoken[(object) "domain"]).ValueOrDefault<string>();
      this.Sidebar = HttpUtility.HtmlDecode(((IEnumerable<JToken>) jtoken[(object) "description"]).ValueOrDefault<string>() ?? string.Empty);
      this.Language = ((IEnumerable<JToken>) jtoken[(object) "language"]).ValueOrDefault<string>();
      this.Title = ((IEnumerable<JToken>) jtoken[(object) "title"]).ValueOrDefault<string>();
      this.WikiEditKarma = ((IEnumerable<JToken>) jtoken[(object) "wiki_edit_karma"]).ValueOrDefault<int>();
      this.UseDomainCss = ((IEnumerable<JToken>) jtoken[(object) "domain_css"]).ValueOrDefault<bool>();
      this.UseDomainSidebar = ((IEnumerable<JToken>) jtoken[(object) "domain_sidebar"]).ValueOrDefault<bool>();
      this.HeaderHoverText = ((IEnumerable<JToken>) jtoken[(object) "header_hover_text"]).ValueOrDefault<string>();
      this.NSFW = ((IEnumerable<JToken>) jtoken[(object) "over_18"]).ValueOrDefault<bool>();
      this.PublicDescription = HttpUtility.HtmlDecode(((IEnumerable<JToken>) jtoken[(object) "public_description"]).ValueOrDefault<string>() ?? string.Empty);
      this.SpamFilter = new SpamFilterSettings()
      {
        LinkPostStrength = this.GetSpamFilterStrength(((IEnumerable<JToken>) jtoken[(object) "spam_links"]).ValueOrDefault<string>()),
        SelfPostStrength = this.GetSpamFilterStrength(((IEnumerable<JToken>) jtoken[(object) "spam_selfposts"]).ValueOrDefault<string>()),
        CommentStrength = this.GetSpamFilterStrength(((IEnumerable<JToken>) jtoken[(object) "spam_comments"]).ValueOrDefault<string>())
      };
      if (jtoken[(object) "wikimode"] != null)
      {
        switch (((IEnumerable<JToken>) jtoken[(object) "wikimode"]).ValueOrDefault<string>())
        {
          case "disabled":
            this.WikiEditMode = WikiEditMode.None;
            break;
          case "modonly":
            this.WikiEditMode = WikiEditMode.Moderators;
            break;
          case "anyone":
            this.WikiEditMode = WikiEditMode.All;
            break;
        }
      }
      if (jtoken[(object) "subreddit_type"] != null)
      {
        switch (((IEnumerable<JToken>) jtoken[(object) "subreddit_type"]).ValueOrDefault<string>())
        {
          case "public":
            this.SubredditType = SubredditType.Public;
            break;
          case "private":
            this.SubredditType = SubredditType.Private;
            break;
          case "restricted":
            this.SubredditType = SubredditType.Restricted;
            break;
        }
      }
      this.ShowThumbnails = ((IEnumerable<JToken>) jtoken[(object) "show_media"]).ValueOrDefault<bool>();
      this.WikiEditAge = ((IEnumerable<JToken>) jtoken[(object) "wiki_edit_age"]).ValueOrDefault<int>();
      if (jtoken[(object) "content_options"] == null)
        return;
      switch (((IEnumerable<JToken>) jtoken[(object) "content_options"]).ValueOrDefault<string>())
      {
        case "any":
          this.ContentOptions = ContentOptions.All;
          break;
        case "link":
          this.ContentOptions = ContentOptions.LinkOnly;
          break;
        case "self":
          this.ContentOptions = ContentOptions.SelfOnly;
          break;
      }
    }

    public bool AllowAsDefault { get; set; }

    public string Domain { get; set; }

    public string Sidebar { get; set; }

    public string Language { get; set; }

    public string Title { get; set; }

    public int WikiEditKarma { get; set; }

    public bool UseDomainCss { get; set; }

    public bool UseDomainSidebar { get; set; }

    public string HeaderHoverText { get; set; }

    public bool NSFW { get; set; }

    public string PublicDescription { get; set; }

    public WikiEditMode WikiEditMode { get; set; }

    public SubredditType SubredditType { get; set; }

    public bool ShowThumbnails { get; set; }

    public int WikiEditAge { get; set; }

    public ContentOptions ContentOptions { get; set; }

    public SpamFilterSettings SpamFilter { get; set; }

    public bool AllowImages { get; set; }

    public bool ExcludeBannedUsersFromModqueue { get; set; }

    public void UpdateSettings()
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/site_admin");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      string str1;
      switch (this.ContentOptions)
      {
        case ContentOptions.All:
          str1 = "any";
          break;
        case ContentOptions.LinkOnly:
          str1 = "link";
          break;
        default:
          str1 = "self";
          break;
      }
      string str2;
      switch (this.SubredditType)
      {
        case SubredditType.Public:
          str2 = "public";
          break;
        case SubredditType.Private:
          str2 = "private";
          break;
        default:
          str2 = "restricted";
          break;
      }
      string str3;
      switch (this.WikiEditMode)
      {
        case WikiEditMode.Moderators:
          str3 = "modonly";
          break;
        case WikiEditMode.All:
          str3 = "anyone";
          break;
        default:
          str3 = "disabled";
          break;
      }
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        allow_top = this.AllowAsDefault,
        allow_images = this.AllowImages,
        description = this.Sidebar,
        domain = this.Domain,
        exclude_banned_modqueue = this.ExcludeBannedUsersFromModqueue,
        lang = this.Language,
        link_type = str1,
        over_18 = this.NSFW,
        public_description = this.PublicDescription,
        show_media = this.ShowThumbnails,
        sr = this.Subreddit.FullName,
        title = this.Title,
        type = str2,
        uh = this.Reddit.User.Modhash,
        wiki_edit_age = this.WikiEditAge,
        wiki_edit_karma = this.WikiEditKarma,
        wikimode = str3,
        spam_links = this.SpamFilter?.LinkPostStrength.ToString().ToLowerInvariant(),
        spam_selfposts = this.SpamFilter?.SelfPostStrength.ToString().ToLowerInvariant(),
        spam_comments = this.SpamFilter?.CommentStrength.ToString().ToLowerInvariant(),
        api_type = "json"
      }, "header-title", this.HeaderHoverText);
      requestStream.Flush();//.Close();

      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void ResetHeaderImage()
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/delete_sr_header");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        uh = this.Reddit.User.Modhash,
        r = this.Subreddit.Name
      });
            requestStream.Flush();//.Close();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    private SpamFilterStrength GetSpamFilterStrength(string rawValue)
    {
      switch (rawValue)
      {
        case "low":
          return SpamFilterStrength.Low;
        case "high":
          return SpamFilterStrength.High;
        case "all":
          return SpamFilterStrength.All;
        default:
          return SpamFilterStrength.High;
      }
    }
  }
}
