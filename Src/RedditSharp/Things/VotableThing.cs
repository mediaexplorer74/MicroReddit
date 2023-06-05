// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.VotableThing
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace RedditSharp.Things
{
  public class VotableThing : CreatedThing
  {
    private const string VoteUrl = "/api/vote";
    private const string SaveUrl = "/api/save";
    private const string UnsaveUrl = "/api/unsave";
    private const string ReportUrl = "/api/report";
    private const string DistinguishUrl = "/api/distinguish";
    private const string ApproveUrl = "/api/approve";
    private const string DelUrl = "/api/del";
    private const string RemoveUrl = "/api/remove";
    private const string IgnoreReportsUrl = "/api/ignore_reports";
    private const string UnIgnoreReportsUrl = "/api/unignore_reports";

    protected async Task<VotableThing> InitAsync(Reddit reddit, IWebAgent webAgent, JToken json)
    {
      VotableThing votableThing = this;
      await votableThing.CommonInitAsync(reddit, webAgent, json);
      JsonConvert.PopulateObject(json[(object) "data"].ToString(), (object) votableThing, votableThing.Reddit.JsonSerializerSettings);
      return votableThing;
    }

    protected VotableThing Init(Reddit reddit, IWebAgent webAgent, JToken json)
    {
      this.CommonInit(reddit, webAgent, json);
      JsonConvert.PopulateObject(json[(object) "data"].ToString(), (object) this, this.Reddit.JsonSerializerSettings);
      return this;
    }

    private void CommonInit(Reddit reddit, IWebAgent webAgent, JToken json)
    {
      this.Init(reddit, json);
      this.Reddit = reddit;
      this.WebAgent = webAgent;
    }

    private async Task CommonInitAsync(Reddit reddit, IWebAgent webAgent, JToken json)
    {
      VotableThing votableThing = this;
      CreatedThing createdThing = await votableThing.InitAsync(reddit, json);
      votableThing.Reddit = reddit;
      votableThing.WebAgent = webAgent;
    }

    protected virtual void RemoveImpl(bool spam)
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/remove");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        id = this.FullName,
        spam = spam,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    [JsonProperty("approved_by")]
    public string ApprovedBy { get; set; }

    [JsonProperty("author")]
    public string AuthorName { get; set; }

    [JsonProperty("author_flair_css_class")]
    public string AuthorFlairCssClass { get; set; }

    [JsonProperty("author_flair_text")]
    public string AuthorFlairText { get; set; }

    [JsonProperty("banned_by")]
    public string BannedBy { get; set; }

    [JsonProperty("downs")]
    public int Downvotes { get; set; }

    [JsonProperty("edited")]
    public bool Edited { get; set; }

    [JsonProperty("archived")]
    public bool IsArchived { get; set; }

    [JsonProperty("approved")]
    public bool? IsApproved { get; set; }

    [JsonProperty("removed")]
    public bool? IsRemoved { get; set; }

    [JsonProperty("ups")]
    public int Upvotes { get; set; }

    [JsonProperty("score")]
    public int Score { get; set; }

    [JsonProperty("saved")]
    public bool Saved { get; set; }

    public virtual string Shortlink => "http://redd.it/" + this.Id;

    [JsonProperty("stickied")]
    public bool IsStickied { get; set; }

    [JsonIgnore]
    [Obsolete("Use ReportCount instead.", false)]
    public int? NumReports => this.ReportCount;

    [JsonProperty("num_reports")]
    public int? ReportCount { get; set; }

    [JsonProperty("distinguished")]
    [JsonConverter(typeof (VotableThing.DistinguishConverter))]
    public VotableThing.DistinguishType Distinguished { get; set; }

    [JsonProperty("likes")]
    public bool? Liked { get; set; }

    [JsonProperty("mod_reports")]
    [JsonConverter(typeof (VotableThing.ReportCollectionConverter))]
    public ICollection<RedditSharp.Things.Report> ModReports { get; set; }

    [JsonProperty("user_reports")]
    [JsonConverter(typeof (VotableThing.ReportCollectionConverter))]
    public ICollection<RedditSharp.Things.Report> UserReports { get; set; }

    [JsonProperty("gilded")]
    public int Gilded { get; set; }

    [JsonIgnore]
    public VotableThing.VoteType Vote
    {
      get
      {
        bool? liked = this.Liked;
        if (liked.HasValue)
        {
          bool valueOrDefault = liked.GetValueOrDefault();
          if (!valueOrDefault)
            return VotableThing.VoteType.Downvote;
          if (valueOrDefault)
            return VotableThing.VoteType.Upvote;
        }
        return VotableThing.VoteType.None;
      }
      set => this.SetVote(value);
    }

    public void Upvote() => this.SetVote(VotableThing.VoteType.Upvote);

    public void Downvote() => this.SetVote(VotableThing.VoteType.Downvote);

    public void SetVote(VotableThing.VoteType type)
    {
      if (this.Vote == type)
        return;
      HttpWebRequest post = this.WebAgent.CreatePost("/api/vote");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        dir = (int) type,
        id = this.FullName,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
      bool? nullable = this.Liked;
      bool flag1 = true;
      if ((nullable.GetValueOrDefault() == flag1 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
        --this.Upvotes;
      nullable = this.Liked;
      bool flag2 = false;
      if ((nullable.GetValueOrDefault() == flag2 ? (nullable.HasValue ? 1 : 0) : 0) != 0)
        --this.Downvotes;
      switch (type)
      {
        case VotableThing.VoteType.Downvote:
          this.Liked = new bool?(false);
          ++this.Downvotes;
          break;
        case VotableThing.VoteType.None:
          nullable = new bool?();
          this.Liked = nullable;
          break;
        case VotableThing.VoteType.Upvote:
          this.Liked = new bool?(true);
          ++this.Upvotes;
          break;
      }
    }

    public void Save()
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/save");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        id = this.FullName,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
      this.Saved = true;
    }

    public void Unsave()
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/unsave");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        id = this.FullName,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
      this.Saved = false;
    }

    public void ClearVote()
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/vote");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        dir = 0,
        id = this.FullName,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void Report(VotableThing.ReportType reportType, string otherReason = null)
    {
      HttpWebRequest post = this.WebAgent.CreatePost("/api/report");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      string str;
      switch (reportType)
      {
        case VotableThing.ReportType.Spam:
          str = "spam";
          break;
        case VotableThing.ReportType.VoteManipulation:
          str = "vote manipulation";
          break;
        case VotableThing.ReportType.PersonalInformation:
          str = "personal information";
          break;
        case VotableThing.ReportType.SexualizingMinors:
          str = "sexualizing minors";
          break;
        case VotableThing.ReportType.BreakingReddit:
          str = "breaking reddit";
          break;
        default:
          str = "other";
          break;
      }
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        api_type = "json",
        reason = str,
        other_reason = (otherReason ?? ""),
        thing_id = this.FullName,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void Distinguish(VotableThing.DistinguishType distinguishType)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost("/api/distinguish");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      string str;
      switch (distinguishType)
      {
        case VotableThing.DistinguishType.Moderator:
          str = "yes";
          break;
        case VotableThing.DistinguishType.Admin:
          str = "admin";
          break;
        case VotableThing.DistinguishType.None:
          str = "no";
          break;
        default:
          str = "special";
          break;
      }
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        how = str,
        id = this.Id,
        uh = this.Reddit.User.Modhash
      });
      
        requestStream.Flush();

        JToken jtoken = default;

        if ((jtoken = JObject.Parse(this.WebAgent.GetResponseString(
            post.GetResponseAsync().Result.GetResponseStream()))["jquery"]).Count<JToken>(
            (Func<JToken, bool>)(i => jtoken.Value<int>((IEnumerable<JToken>)i[(object)0]) == 11
            && jtoken.Value<int>((IEnumerable<JToken>)i[(object)1]) == 12)) == 0)
        {
            throw new AuthenticationException("You are not permitted to distinguish this item.");
        }
    }

    public void Approve() => this.SimpleAction("/api/approve");

    public void Remove() => this.RemoveImpl(false);

    public void RemoveSpam() => this.RemoveImpl(true);

    public void Del() => this.SimpleAction("/api/del");

    public void IgnoreReports() => this.SimpleAction("/api/ignore_reports");

    public void UnIgnoreReports() => this.SimpleAction("/api/unignore_reports");

    public enum VoteType
    {
      Downvote = -1, // 0xFFFFFFFF
      None = 0,
      Upvote = 1,
    }

    public enum ReportType
    {
      Spam,
      VoteManipulation,
      PersonalInformation,
      SexualizingMinors,
      BreakingReddit,
      Other,
    }

    public enum DistinguishType
    {
      Moderator,
      Admin,
      Special,
      None,
    }

    internal class DistinguishConverter : JsonConverter
    {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(VotableThing.DistinguishType) || objectType == typeof(string);
            }

      public override object ReadJson
      (
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer
      )
      {
            var token = JToken.Load(reader);
            var value = token.Value<string>();
            if (value == null)
                return DistinguishType.None;
            switch (value)
            {
                case "moderator": return DistinguishType.Moderator;
                case "admin": return DistinguishType.Admin;
                case "special": return DistinguishType.Special;
                default: return DistinguishType.None;
            }
        }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        VotableThing.DistinguishType distinguishType = (VotableThing.DistinguishType) value;
        if (distinguishType == VotableThing.DistinguishType.None)
          writer.WriteNull();
        else
          writer.WriteValue(distinguishType.ToString().ToLower());
      }
    }

    internal class ReportCollectionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ICollection<RedditSharp.Things.Report>) || objectType == typeof(object);
        }

        public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        JToken jtoken = JToken.Load(reader);
        if (jtoken.Type != (JTokenType)2 || ((IEnumerable<JToken>) jtoken.Children()).Count<JToken>() == 0)
          return (object) new Collection<RedditSharp.Things.Report>();
        Collection<RedditSharp.Things.Report> collection = new Collection<RedditSharp.Things.Report>();
        foreach (JToken child in jtoken.Children())
        {
          if (child.Type == (JTokenType)2 && ((IEnumerable<JToken>) child.Children()).Count<JToken>() == 2)
          {
            RedditSharp.Things.Report report = new RedditSharp.Things.Report()
            {
              Reason = jtoken.Value<string>((IEnumerable<JToken>) child.First)
            };
            if (child.Last.Type == (JTokenType)8)
            {
              report.ModeratorName = jtoken.Value<string>((IEnumerable<JToken>) child.Last);
              report.Count = 1;
            }
            else
            {
              report.ModeratorName = "";
              report.Count = jtoken.Value<int>((IEnumerable<JToken>) child.Last);
            }
            collection.Add(report);
          }
        }
        return (object) collection;
      }

      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        if (!(value is ICollection<RedditSharp.Things.Report> reports) || reports.Count == 0)
        {
          writer.WriteStartArray();
          writer.WriteEndArray();
        }
        else
        {
          writer.WriteStartArray();
          foreach (RedditSharp.Things.Report report in (IEnumerable<RedditSharp.Things.Report>) reports)
          {
            writer.WriteStartArray();
            writer.WriteValue(report.Reason);
            if (string.IsNullOrEmpty(report.ModeratorName))
              writer.WriteValue(report.Count);
            else
              writer.WriteValue(report.ModeratorName);
            writer.WriteEndArray();
          }
          writer.WriteEndArray();
        }
      }
    }
  }
}
