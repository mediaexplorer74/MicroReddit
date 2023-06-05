// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.LiveUpdateEvent
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace RedditSharp.Things
{
  public class LiveUpdateEvent : CreatedThing
  {
    private const string AcceptContributorInviteUrl = "/api/live/{0}/accept_contribtor_invite";
    private const string CloseThreadUrl = "/api/live/{0}/close_thread";
    private const string EditUrl = "/api/live/{0}/edit";
    private const string InviteContributorUrl = "/api/live/{0}/invite_contributor";
    private const string LeaveContributorUrl = "/api/live/{0}/leave_contributor";
    private const string RemoveContributorUrl = "/api/live/{0}/rm_contributor";
    private const string RevokeContributorInviteUrl = "/api/live/{0}/rm_contributor_invite";
    private const string SetContributorPermissionUrl = "/api/live/{0}/set_contributor_permissions";
    private const string UpdateUrl = "/api/live/{0}/update";
    private const string StrikeUpdateUrl = "/api/live/{0}/strike_update";
    private const string DeleteUpdateUrl = "/api/live/{0}/delete_update";
    private const string GetUrl = "/live/{0}";
    private const string ContributorsUrl = "/live/{0}/contributors.json";
    private const string DiscussionsUrl = "/live/{0}/discussions";
    private const string ReportUrl = "/api/live/{0}/report";

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("description_html")]
    public string DescriptionHtml { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("websocket_uri")]
    public Uri WebsocketUri { get; set; }

    [JsonProperty("state")]
    public string State { get; set; }

    [JsonProperty("nsfw")]
    public bool NSFW { get; set; }

    [JsonProperty("viewer_count")]
    public int? ViewerCount { get; set; }

    [JsonProperty("viewer_count_fuzzed")]
    public bool ViewerCountFuzzed { get; set; }

    [JsonProperty("resources")]
    public string Resources { get; set; }

    [JsonProperty]
    public string Name { get; set; }

    public void AcceptContributorInvite() => this.SimpleAction(string.Format("/api/live/{0}/accept_contribtor_invite", (object) this.Name));

    public void Close() => this.SimpleAction("/api/live/{0}/close_thread");

    public void DeleteUpdate(LiveUpdate update)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/api/live/{0}/delete_update", (object) this.Name));
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        api_type = "json",
        id = update.FullName,
        uh = this.Reddit.User.Modhash
      });
            requestStream.Flush();//.Close();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void DeleteUpdate(string fullName)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/api/live/{0}/delete_update", (object) this.Name));
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        api_type = "json",
        id = fullName,
        uh = this.Reddit.User.Modhash
      });
      
      requestStream.Flush();//.Close();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void Edit(string title, string description, string resources, bool? nsfw)
    {
      IDictionary<string, object> data = (IDictionary<string, object>) new ExpandoObject();
      if (title != null)
        data.Add(new KeyValuePair<string, object>(nameof (title), (object) title));
      if (description != null)
        data.Add(new KeyValuePair<string, object>(nameof (description), (object) description));
      if (resources != null)
        data.Add(new KeyValuePair<string, object>(nameof (resources), (object) resources));
      if (nsfw.HasValue)
        data.Add(new KeyValuePair<string, object>(nameof (nsfw), (object) nsfw.Value));
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/api/live/{0}/edit", (object) this.Id));
      
            this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) data);

      JToken jtoken = JToken.Parse(
          this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream()));

      if (!jtoken[(object) "json"].ToString().Contains("\"errors\": []"))
        throw new Exception(
            "Error editing text. Error: " + jtoken[(object) "json"][(object) "errors"][(object) 0][(object) 0].ToString());
      this.Title = title ?? "";
      this.Description = description ?? "";
      this.Resources = resources ?? "";
      if (!nsfw.HasValue)
        return;
      this.NSFW = nsfw.Value;
    }

    public ICollection<LiveUpdateEvent.LiveUpdateEventUser> GetContributors()
    {
      List<LiveUpdateEvent.LiveUpdateEventUser> contributors = new List<LiveUpdateEvent.LiveUpdateEventUser>();
      JToken jtoken1 = JToken.Parse(this.WebAgent.GetResponseString(this.WebAgent.CreateGet(string.Format("/live/{0}/contributors.json", 
          (object) this.Name)).GetResponseAsync().Result.GetResponseStream()));
      foreach (JToken jtoken2 in jtoken1.Type != (JTokenType)2 ? (IEnumerable<JToken>) jtoken1[(object) "data"][(object) "children"] 
                : (IEnumerable<JToken>) jtoken1[(object) 1][(object) "data"][(object) "children"])
        contributors.Add(jtoken2.ToObject<LiveUpdateEvent.LiveUpdateEventUser>());
      return (ICollection<LiveUpdateEvent.LiveUpdateEventUser>) contributors;
    }

    public Listing<Post> GetDiscussions() => new Listing<Post>(this.Reddit, string.Format("/live/{0}/discussions", (object) this.Name), this.WebAgent);

    public Listing<LiveUpdate> GetThread() => new Listing<LiveUpdate>(this.Reddit, string.Format("/live/{0}", (object) this.Name), this.WebAgent);

    public ICollection<LiveUpdateEvent.LiveUpdateEventUser> GetInvitedContributors()
    {
      List<LiveUpdateEvent.LiveUpdateEventUser> invitedContributors = new List<LiveUpdateEvent.LiveUpdateEventUser>();
            
      foreach (JToken jtoken in (IEnumerable<JToken>)JToken.Parse(this.WebAgent.GetResponseString(
        this.WebAgent.CreateGet(string.Format("/live/{0}/contributors.json", (object)this.Name))
        .GetResponseAsync().Result.GetResponseStream()))[(object)1][(object)"data"][(object)"children"])
      {
        invitedContributors.Add((LiveUpdateEvent.LiveUpdateEventUser)JsonConvert.DeserializeObject(jtoken.ToString()));
      }

      return (ICollection<LiveUpdateEvent.LiveUpdateEventUser>) invitedContributors;
    }

    public void InviteContributor(
      string userName,
      LiveUpdateEvent.LiveUpdateEventPermission permissions)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/api/live/{0}/invite_contributor", (object) this.Name));
      Stream requestStream = post.GetRequestStreamAsync().Result;
      string permissionsString = this.GetPermissionsString(permissions);
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        api_type = "json",
        name = userName,
        permissions = permissionsString,
        type = "liveupdate_contributor_invite",
        uh = this.Reddit.User.Modhash
      });
      
      requestStream.Flush();//.Close();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void LeaveContributor() => this.SimpleAction("/api/live/{0}/leave_contributor");

    public void RemoveContributor(RedditUser user)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/api/live/{0}/rm_contributor", (object) this.Name));
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        api_type = "json",
        id = (user.Kind + "_" + user.Id),
        uh = this.Reddit.User.Modhash
      });
            requestStream.Flush();//.Close();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void RemoveContributor(string userName) => this.RemoveContributor(this.Reddit.GetUser(userName));

    public void Report(string reason = "spam")
    {
      List<string> stringList = new List<string>()
      {
        "spam",
        "vote-manipulation",
        "personal-information",
        "sexualizing-minors",
        "site-breaking"
      };
      if (!stringList.Contains(reason))
      {
        string message = "Invalid report type.  Valid types are : ";
        for (int index = 0; index < stringList.Count; ++index)
        {
          message = message + "'" + stringList[index] + "'";
          if (index != stringList.Count - 1)
            message += ", ";
        }
        throw new InvalidOperationException(message);
      }
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/api/live/{0}/report", (object) this.Name));
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        api_type = "json",
        type = reason,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void RevokeContributorInvite(RedditUser user)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/api/live/{0}/rm_contributor_invite", (object) this.Name));
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        api_type = "json",
        id = (user.Kind + "_" + user.Id),
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void RevokeContributorInvite(string userName) => this.RevokeContributorInvite(this.Reddit.GetUser(userName));

    public void SetContributorPermissions(
      RedditUser user,
      LiveUpdateEvent.LiveUpdateEventPermission permissions)
    {
      this.SetContributorPermissions(user.Name, permissions);
    }

    public void SetContributorPermissions(
      string userName,
      LiveUpdateEvent.LiveUpdateEventPermission permissions)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/api/live/{0}/set_contributor_permissions", (object) this.Name));
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        api_type = "json",
        name = userName,
        type = "liveupdate_contributor",
        permissions = this.GetPermissionsString(permissions),
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void SetInvitedContributorPermissions(
      RedditUser user,
      LiveUpdateEvent.LiveUpdateEventPermission permissions)
    {
      this.SetInvitedContributorPermissions(user.Name, permissions);
    }

    public void SetInvitedContributorPermissions(
      string userName,
      LiveUpdateEvent.LiveUpdateEventPermission permissions)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/api/live/{0}/set_contributor_permissions", (object) this.Name));
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        api_type = "json",
        name = userName,
        type = "liveupdate_contributor_invite",
        permissions = this.GetPermissionsString(permissions),
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void StrikeUpdate(LiveUpdate update) => this.StrikeUpdate(update.FullName);

    public void StrikeUpdate(string fullName)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/api/live/{0}/strike_update", (object) this.Name));
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        api_type = "json",
        id = fullName,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void Update(string markdown)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/api/live/{0}/update", (object) this.Name));
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        api_type = "json",
        body = markdown,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public async Task<LiveUpdateEvent> InitAsync(Reddit reddit, JToken post, IWebAgent webAgent)
    {
      LiveUpdateEvent liveUpdateEvent = this;
      liveUpdateEvent.CommonInit(reddit, post, webAgent);
      JsonConvert.PopulateObject(post[(object) "data"].ToString(), (object) liveUpdateEvent, reddit.JsonSerializerSettings);
      liveUpdateEvent.FullName = liveUpdateEvent.Name;
      liveUpdateEvent.Name = liveUpdateEvent.Name.Replace("LiveUpdateEvent_", "");
      return liveUpdateEvent;
    }

    public LiveUpdateEvent Init(Reddit reddit, JToken post, IWebAgent webAgent)
    {
      this.CommonInit(reddit, post, webAgent);
      JsonConvert.PopulateObject(post[(object) "data"].ToString(), (object) this, reddit.JsonSerializerSettings);
      this.FullName = this.Name;
      this.Name = this.Name.Replace("LiveUpdateEvent_", "");
      return this;
    }

    private void CommonInit(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.Init(json);
      this.Reddit = reddit;
      this.WebAgent = webAgent;
    }

    private void SimpleAction(string url)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format(url, (object) this.Name));
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        api_type = "json",
        uh = this.Reddit.User.Modhash
      });
            requestStream.Flush();//.Close();
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    private string GetPermissionsString(LiveUpdateEvent.LiveUpdateEventPermission input)
    {
      if (input == LiveUpdateEvent.LiveUpdateEventPermission.All)
        return "+all";
      if (input == LiveUpdateEvent.LiveUpdateEventPermission.None)
        return "-all,-close,-edit,-manage,-settings,-update";
      string str1 = "-all,";
      string str2 = !input.HasFlag((System.Enum) LiveUpdateEvent.LiveUpdateEventPermission.Close) ? str1 + "-close," : str1 + "+close,";
      string str3 = !input.HasFlag((System.Enum) LiveUpdateEvent.LiveUpdateEventPermission.Edit) ? str2 + "-edit," : str2 + "+edit,";
      string str4 = !input.HasFlag((System.Enum) LiveUpdateEvent.LiveUpdateEventPermission.Manage) ? str3 + "-manage," : str3 + "+manage,";
      string str5 = !input.HasFlag((System.Enum) LiveUpdateEvent.LiveUpdateEventPermission.Settings) ? str4 + "-settings," : str4 + "+settings,";
      string permissionsString = !input.HasFlag((System.Enum) LiveUpdateEvent.LiveUpdateEventPermission.Update) ? str5 + "-update," : str5 + "+update,";
      if (permissionsString.EndsWith(","))
        permissionsString = permissionsString.Remove(permissionsString.Length - 1, 1);
      return permissionsString;
    }

    [Flags]
    public enum LiveUpdateEventPermission
    {
      None = 0,
      Update = 1,
      Manage = 2,
      Settings = 4,
      Edit = 8,
      Close = 16, // 0x00000010
      All = Close | Edit | Settings | Manage | Update, // 0x0000001F
    }

    public class LiveUpdateEventUser
    {
      [JsonConverter(typeof (LiveUpdateEvent.PermissionsConverter))]
      public LiveUpdateEvent.LiveUpdateEventPermission Permissions { get; set; }

      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("id")]
      public string Id { get; set; }
    }

    private class PermissionsConverter : JsonConverter
    {
      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        LiveUpdateEvent.LiveUpdateEventPermission updateEventPermission = (LiveUpdateEvent.LiveUpdateEventPermission) value;
        writer.WriteStartArray();
        if (updateEventPermission.HasFlag((System.Enum) LiveUpdateEvent.LiveUpdateEventPermission.All))
        {
          writer.WriteValue("all");
          writer.WriteEndArray();
        }
        else if (updateEventPermission.HasFlag((System.Enum) LiveUpdateEvent.LiveUpdateEventPermission.None))
        {
          writer.WriteValue("none");
          writer.WriteEndArray();
        }
        else
        {
          if (updateEventPermission.HasFlag((System.Enum) LiveUpdateEvent.LiveUpdateEventPermission.Edit))
            writer.WriteValue("edit");
          if (updateEventPermission.HasFlag((System.Enum) LiveUpdateEvent.LiveUpdateEventPermission.Close))
            writer.WriteValue("close");
          if (updateEventPermission.HasFlag((System.Enum) LiveUpdateEvent.LiveUpdateEventPermission.Manage))
            writer.WriteValue("manage");
          if (updateEventPermission.HasFlag((System.Enum) LiveUpdateEvent.LiveUpdateEventPermission.Settings))
            writer.WriteValue("settings");
          if (updateEventPermission.HasFlag((System.Enum) LiveUpdateEvent.LiveUpdateEventPermission.Update))
            writer.WriteValue("update");
          writer.WriteEndArray();
        }
      }

      public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        string[] strArray = ((JToken) JArray.Load(reader)).ToObject<string[]>();
        LiveUpdateEvent.LiveUpdateEventPermission updateEventPermission = LiveUpdateEvent.LiveUpdateEventPermission.None;
        bool flag = false;
        foreach (string str in strArray)
        {
          switch (str)
          {
            case "all":
              updateEventPermission = LiveUpdateEvent.LiveUpdateEventPermission.All;
              flag = true;
              break;
            case "close":
              updateEventPermission |= LiveUpdateEvent.LiveUpdateEventPermission.Close;
              break;
            case "edit":
              updateEventPermission |= LiveUpdateEvent.LiveUpdateEventPermission.Edit;
              break;
            case "manage":
              updateEventPermission |= LiveUpdateEvent.LiveUpdateEventPermission.Manage;
              break;
            case "none":
              updateEventPermission = LiveUpdateEvent.LiveUpdateEventPermission.None;
              flag = true;
              break;
            case "settings":
              updateEventPermission |= LiveUpdateEvent.LiveUpdateEventPermission.Settings;
              break;
            case "update":
              updateEventPermission |= LiveUpdateEvent.LiveUpdateEventPermission.Update;
              break;
          }
          if (flag)
            break;
        }
        return (object) updateEventPermission;
      }

            public override bool CanConvert(Type objectType)
            {
                return true;
            }
        }
  }
}
