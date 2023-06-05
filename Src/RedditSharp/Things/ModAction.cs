// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.ModAction
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace RedditSharp.Things
{
  public class ModAction : Thing
  {
    [JsonProperty("action")]
    [JsonConverter(typeof (ModActionTypeConverter))]
    public ModActionType Action { get; set; }

    [JsonProperty("created_utc")]
    [JsonConverter(typeof (UnixTimestampConverter))]
    public DateTimeOffset? TimeStamp { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("details")]
    public string Details { get; set; }

    [JsonProperty("mod_id36")]
    public string ModeratorId { get; set; }

    [JsonProperty("mod")]
    public string ModeratorName { get; set; }

    [JsonProperty("target_author")]
    public string TargetAuthorName { get; set; }

    [JsonProperty("target_fullname")]
    public string TargetThingFullname { get; set; }

    [JsonProperty("target_permalink")]
    public string TargetThingPermalink { get; set; }

    [JsonProperty("sr_id36")]
    public string SubredditId { get; set; }

    [JsonProperty("subreddit")]
    public string SubredditName { get; set; }

    [JsonProperty("target_body")]
    public string TargetBody { get; set; }

    [JsonProperty("target_title")]
    public string TargetTitle { get; set; }

    [JsonIgnore]
    public RedditUser TargetAuthor => this.Reddit.GetUser(this.TargetAuthorName);

    [JsonIgnore]
    public Thing TargetThing => this.Reddit.GetThingByFullname(this.TargetThingFullname);

    public async Task<ModAction> InitAsync(Reddit reddit, JToken post, IWebAgent webAgent)
    {
      ModAction modAction = this;
      modAction.CommonInit(reddit, post, webAgent);
      JsonConvert.PopulateObject(post[(object) "data"].ToString(), (object) modAction, reddit.JsonSerializerSettings);
      return modAction;
    }

    public ModAction Init(Reddit reddit, JToken post, IWebAgent webAgent)
    {
      this.CommonInit(reddit, post, webAgent);
      JsonConvert.PopulateObject(post[(object) "data"].ToString(), (object) this, reddit.JsonSerializerSettings);
      return this;
    }

    private void CommonInit(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.Init(json);
      this.Reddit = reddit;
      this.WebAgent = webAgent;
    }
  }
}
