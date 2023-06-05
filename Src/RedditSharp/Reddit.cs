// Decompiled with JetBrains decompiler
// Type: RedditSharp.Reddit
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RedditSharp
{
  public class Reddit
  {
    private const string SslLoginUrl = "https://ssl.reddit.com/api/login";
    private const string LoginUrl = "/api/login/username";
    private const string UserInfoUrl = "/user/{0}/about.json";
    private const string MeUrl = "/api/me.json";
    private const string OAuthMeUrl = "/api/v1/me.json";
    private const string SubredditAboutUrl = "/r/{0}/about.json";
    private const string ComposeMessageUrl = "/api/compose";
    private const string RegisterAccountUrl = "/api/register";
    private const string GetThingUrl = "/api/info.json?id={0}";
    private const string GetCommentUrl = "/r/{0}/comments/{1}/foo/{2}";
    private const string GetPostUrl = "{0}.json";
    private const string DomainUrl = "www.reddit.com";
    private const string OAuthDomainUrl = "oauth.reddit.com";
    private const string SearchUrl = "/search.json?q={0}&restrict_sr=off&sort={1}&t={2}";
    private const string UrlSearchPattern = "url:'{0}'";
    private const string NewSubredditsUrl = "/subreddits/new.json";
    private const string PopularSubredditsUrl = "/subreddits/popular.json";
    private const string GoldSubredditsUrl = "/subreddits/gold.json";
    private const string DefaultSubredditsUrl = "/subreddits/default.json";
    private const string SearchSubredditsUrl = "/subreddits/search.json?q={0}";
    private const string CreateLiveEventUrl = "/api/live/create";
    private const string GetLiveEventUrl = "https://www.reddit.com/live/{0}/about";
    public ICaptchaSolver CaptchaSolver;

    internal IWebAgent WebAgent { get; set; }

    public AuthenticatedUser User { get; set; }

    public RedditSharp.WebAgent.RateLimitMode RateLimit
    {
      get => RedditSharp.WebAgent.RateLimit;
      set => RedditSharp.WebAgent.RateLimit = value;
    }

    internal JsonSerializerSettings JsonSerializerSettings { get; set; }

    public Subreddit FrontPage => Subreddit.GetFrontPage(this);

    public Subreddit RSlashAll => Subreddit.GetRSlashAll(this);

    public Reddit()
      : this(true)
    {
    }

    public Reddit(bool useSsl)
    {
      RedditSharp.WebAgent webAgent = new RedditSharp.WebAgent();
      this.JsonSerializerSettings = new JsonSerializerSettings()
      {
        CheckAdditionalContent = false,
        DefaultValueHandling = (DefaultValueHandling) 1
      };
      RedditSharp.WebAgent.Protocol = useSsl ? "https" : "http";
      this.WebAgent = (IWebAgent) webAgent;
      this.CaptchaSolver = (ICaptchaSolver) new ConsoleCaptchaSolver();
    }

    public Reddit(RedditSharp.WebAgent.RateLimitMode limitMode, bool useSsl = true)
      : this(useSsl)
    {
      RedditSharp.WebAgent.UserAgent = "";
      RedditSharp.WebAgent.RateLimit = limitMode;
      RedditSharp.WebAgent.RootDomain = "www.reddit.com";
    }

    [Obsolete("OAuth is recommended.", false)]
    public Reddit(string username, string password, bool useSsl = true)
      : this(useSsl)
    {
      this.LogIn(username, password, useSsl);
    }

    public Reddit(string accessToken)
      : this(true)
    {
      RedditSharp.WebAgent.RootDomain = "oauth.reddit.com";
      this.WebAgent.AccessToken = accessToken;
      this.InitOrUpdateUser();
    }

    public Reddit(IWebAgent agent)
    {
      this.WebAgent = agent;
      this.JsonSerializerSettings = new JsonSerializerSettings()
      {
        CheckAdditionalContent = false,
        DefaultValueHandling = (DefaultValueHandling) 1
      };
      this.CaptchaSolver = (ICaptchaSolver) new ConsoleCaptchaSolver();
    }

    public Reddit(IWebAgent agent, bool initUser)
    {
      this.WebAgent = agent;
      this.JsonSerializerSettings = new JsonSerializerSettings()
      {
        CheckAdditionalContent = false,
        DefaultValueHandling = (DefaultValueHandling) 1
      };
      this.CaptchaSolver = (ICaptchaSolver) new ConsoleCaptchaSolver();
      if (!initUser)
        return;
      this.InitOrUpdateUser();
    }

    public AuthenticatedUser LogIn(string username, string password, bool useSsl = true)
    {
      //TODO
      //if (Type.GetType("Mono.Runtime") != (Type) null)
      //  ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback) ((s, c, ch, ssl) => true);
      
      this.WebAgent.Cookies = new CookieContainer();
      HttpWebRequest httpWebRequest = !useSsl 
                ? this.WebAgent.CreatePost("/api/login/username")
                : this.WebAgent.CreatePost("https://ssl.reddit.com/api/login");
      Stream requestStream = httpWebRequest.GetRequestStreamAsync().Result;
      if (useSsl)
        this.WebAgent.WritePostBody(requestStream, (object) new
        {
          user = username,
          passwd = password,
          api_type = "json"
        });
      else
        this.WebAgent.WritePostBody(requestStream, (object) new
        {
          user = username,
          passwd = password,
          api_type = "json",
          op = "login"
        });
      requestStream.Flush();
      if (((IEnumerable<JToken>) JObject.Parse(
          this.WebAgent.GetResponseString(
              httpWebRequest.GetResponseAsync().Result.GetResponseStream()))["json"][(object) "errors"]).Count<JToken>() != 0)
        throw new AuthenticationException("Incorrect login.");
      this.InitOrUpdateUser();
      return this.User;
    }

    public RedditUser GetUser(string name) => new RedditUser().Init(this, (JToken) JObject.Parse(this.WebAgent.GetResponseString(this.WebAgent.CreateGet(string.Format("/user/{0}/about.json", 
        (object) name)).GetResponseAsync().Result.GetResponseStream())), this.WebAgent);

    public void InitOrUpdateUser() => this.User = new AuthenticatedUser().Init(this, (JToken) JObject.Parse(this.WebAgent.GetResponseString(this.WebAgent.CreateGet(string.IsNullOrEmpty(this.WebAgent.AccessToken) ? "/api/me.json" : "/api/v1/me.json").GetResponseAsync().Result.GetResponseStream())), this.WebAgent);

    [Obsolete("Use User property instead")]
    public AuthenticatedUser GetMe() => this.User;

    public Subreddit GetSubreddit(string name)
    {
      name = Regex.Replace(name, "(r/|/)", "");
      return this.GetThing<Subreddit>(string.Format("/r/{0}/about.json", (object) name));
    }

    public async Task<Subreddit> GetSubredditAsync(string name)
    {
      name = Regex.Replace(name, "(r/|/)", "");
      return await this.GetThingAsync<Subreddit>(string.Format("/r/{0}/about.json", (object) name));
    }

    public Domain GetDomain(string domain)
    {
      if (!domain.StartsWith("http://") && !domain.StartsWith("https://"))
        domain = "http://" + domain;
      return new Domain(this, new Uri(domain), this.WebAgent);
    }

    public JToken GetToken(Uri uri, bool isLive = false)
    {
      string str = uri.AbsoluteUri;
      if (str.EndsWith("/"))
        str = str.Remove(str.Length - 1);
      JToken jtoken = JToken.Parse(this.WebAgent.GetResponseString(this.WebAgent.CreateGet(string.Format("{0}.json", (object) str)).GetResponseAsync().Result.GetResponseStream()));
      return isLive ? jtoken : jtoken[(object) 0][(object) "data"][(object) "children"].First;
    }

    public Post GetPost(Uri uri)
    {
      if (!string.IsNullOrEmpty(this.WebAgent.AccessToken) && uri.AbsoluteUri.StartsWith("https://www.reddit.com"))
        uri = new Uri(uri.AbsoluteUri.Replace("https://www.reddit.com", "https://oauth.reddit.com"));
      return new Post().Init(this, this.GetToken(uri), this.WebAgent);
    }

    public LiveUpdateEvent CreateLiveEvent(
      string title,
      string description,
      string resources = "",
      bool nsfw = false)
    {
      if (string.IsNullOrEmpty(title))
        throw new ArgumentException(nameof (title));
      if (string.IsNullOrEmpty(description))
        throw new ArgumentException(nameof (description));
      HttpWebRequest post = this.WebAgent.CreatePost("/api/live/create");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        api_type = "json",
        title = title,
        description = description,
        resources = resources,
        nsfw = nsfw
      });
      JObject jobject = JObject.Parse(this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream()));
      return !((IEnumerable<JToken>) jobject["json"][(object) "errors"]).Any<JToken>() ? this.GetLiveEvent(new Uri(string.Format("https://www.reddit.com/live/{0}/about", (object) jobject["json"][(object) "data"][(object) "id"].ToString()))) : throw new Exception(jobject["json"][(object) "errors"][(object) 0][(object) 0].ToString());
    }

    public LiveUpdateEvent GetLiveEvent(Uri uri)
    {
      if (!uri.AbsoluteUri.EndsWith("about"))
        uri = new Uri(uri.AbsoluteUri + "/about");
      return new LiveUpdateEvent().Init(this, this.GetToken(uri, true), this.WebAgent);
    }

    public void ComposePrivateMessage(
      string subject,
      string body,
      string to,
      string fromSubReddit = "",
      string captchaId = "",
      string captchaAnswer = "")
    {
      if (this.User == null)
        throw new Exception("User can not be null.");
      if (!string.IsNullOrWhiteSpace(fromSubReddit))
      {
        Subreddit subreddit = this.GetSubreddit(fromSubReddit);
        if (!subreddit.Moderators.Select<ModeratorUser, string>((Func<ModeratorUser, string>) (b => b.Name)).ToList<string>().Contains(this.User.Name))
          throw new AuthenticationException(string.Format("User {0} is not a moderator of subreddit {1}.", (object) this.User.Name, (object) subreddit.Name));
      }
      HttpWebRequest post = this.WebAgent.CreatePost("/api/compose");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        api_type = "json",
        subject = subject,
        text = body,
        to = to,
        from_sr = fromSubReddit,
        uh = this.User.Modhash,
        iden = captchaId,
        captcha = captchaAnswer
      });
      JObject jobject = JObject.Parse(this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream()));
      ICaptchaSolver captchaSolver = this.CaptchaSolver;
      if (((IEnumerable<JToken>) jobject["json"][(object) "errors"]).Any<JToken>() && jobject["json"][(object) "errors"][(object) 0][(object) 0].ToString() == "BAD_CAPTCHA" && captchaSolver != null)
      {
        captchaId = jobject["json"][(object) "captcha"].ToString();
        CaptchaResponse captchaResponse = captchaSolver.HandleCaptcha(new Captcha(captchaId));
        if (captchaResponse.Cancel)
          return;
        this.ComposePrivateMessage(subject, body, to, fromSubReddit, captchaId, captchaResponse.Answer);
      }
      else if (((IEnumerable<JToken>) jobject["json"][(object) "errors"]).Any<JToken>())
        throw new Exception("Error when composing message. Error: " + jobject["json"][(object) "errors"][(object) 0][(object) 0].ToString());
    }

    public AuthenticatedUser RegisterAccount(string userName, string passwd, string email = "")
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/register");
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        api_type = "json",
        email = email,
        passwd = passwd,
        passwd2 = passwd,
        user = userName
      });
      return new AuthenticatedUser().Init(this, (JToken) JObject.Parse(this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream())), this.WebAgent);
    }

    public Thing GetThingByFullname(string fullname) => Thing.Parse(this, JToken.Parse(this.WebAgent.GetResponseString(this.WebAgent.CreateGet(string.Format("/api/info.json?id={0}", 
        (object) fullname)).GetResponseAsync().Result.GetResponseStream()))[(object) "data"][(object) "children"][(object) 0], this.WebAgent);

    public Comment GetComment(string subreddit, string name, string linkName)
    {
      try
      {
        if (linkName.StartsWith("t3_"))
          linkName = linkName.Substring(3);
        if (name.StartsWith("t1_"))
          name = name.Substring(3);
        return this.GetComment(new Uri(string.Format("/r/{0}/comments/{1}/foo/{2}", (object) subreddit, (object) linkName, (object) name)));
      }
      catch (WebException ex)
      {
        return (Comment) null;
      }
    }

    public Comment GetComment(Uri uri)
    {
      JToken jtoken = JToken.Parse(this.WebAgent.GetResponseString(this.WebAgent.CreateGet(string.Format("{0}.json", 
          (object) uri.AbsoluteUri)).GetResponseAsync().Result.GetResponseStream()));

      Post sender = new Post().Init(this, jtoken[(object) 0][(object) "data"][(object) "children"][(object) 0], this.WebAgent);
      return new Comment().Init(this, jtoken[(object) 1][(object) "data"][(object) "children"][(object) 0], this.WebAgent, (Thing) sender);
    }

    public Listing<T> SearchByUrl<T>(string url) where T : Thing => this.Search<T>(string.Format("url:'{0}'", (object) url));

    public Listing<T> Search<T>(string query, Sorting sortE = Sorting.Relevance, TimeSorting timeE = TimeSorting.All) where T : Thing
    {
      string lower1 = sortE.ToString().ToLower();
      string lower2 = timeE.ToString().ToLower();
      return new Listing<T>(this, string.Format("/search.json?q={0}&restrict_sr=off&sort={1}&t={2}", (object) query, (object) lower1, (object) lower2), this.WebAgent);
    }

    public Listing<T> SearchByTimestamp<T>(
      DateTime from,
      DateTime to,
      string query = "",
      string subreddit = "",
      Sorting sortE = Sorting.Relevance,
      TimeSorting timeE = TimeSorting.All)
      where T : Thing
    {
      return this.SearchByTimestamp<T>(new DateTimeOffset(from), new DateTimeOffset(to), query, subreddit, sortE, timeE);
    }

    public Listing<T> SearchByTimestamp<T>(
      DateTimeOffset from,
      DateTimeOffset to,
      string query = "",
      string subreddit = "",
      Sorting sortE = Sorting.Relevance,
      TimeSorting timeE = TimeSorting.All)
      where T : Thing
    {
      string lower1 = sortE.ToString().ToLower();
      string lower2 = timeE.ToString().ToLower();
      return new Listing<T>(this, string.Format("/search.json?q={0}&restrict_sr=off&sort={1}&t={2}", (object) ("(and+timestamp:" + (object) from.ToUnixTimeSeconds() + ".." + (object) to.ToUnixTimeSeconds() + "+'" + query + "'+subreddit:'" + subreddit + "')&syntax=cloudsearch"), (object) lower1, (object) lower2), this.WebAgent);
    }

    public Listing<Subreddit> GetNewSubreddits() => new Listing<Subreddit>(this, "/subreddits/new.json", this.WebAgent);

    public Listing<Subreddit> GetPopularSubreddits() => new Listing<Subreddit>(this, "/subreddits/popular.json", this.WebAgent);

    public Listing<Subreddit> GetGoldSubreddits() => new Listing<Subreddit>(this, "/subreddits/gold.json", this.WebAgent);

    public Listing<Subreddit> GetDefaultSubreddits() => new Listing<Subreddit>(this, "/subreddits/default.json", this.WebAgent);

    public Listing<Subreddit> SearchSubreddits(string query) => new Listing<Subreddit>(this, string.Format("/subreddits/search.json?q={0}", (object) query), this.WebAgent);

    protected internal async Task<T> GetThingAsync<T>(string url) where T : Thing
    {
      Reddit reddit = this;
      WebResponse responseAsync = await reddit.WebAgent.CreateGet(url).GetResponseAsync();
      JToken json = JToken.Parse(reddit.WebAgent.GetResponseString(responseAsync.GetResponseStream()));
      return (T) await Thing.ParseAsync(reddit, json, reddit.WebAgent);
    }

    protected internal T GetThing<T>(string url) where T : Thing => (T) Thing.Parse(this, JToken.Parse(
        this.WebAgent.GetResponseString(this.WebAgent.CreateGet(url).GetResponseAsync().Result.GetResponseStream())), this.WebAgent);
  }
}
