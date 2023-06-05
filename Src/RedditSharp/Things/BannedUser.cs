// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.BannedUser
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace RedditSharp.Things
{
  public class BannedUser : Thing
  {
    [JsonProperty("date")]
    [JsonConverter(typeof (UnixTimestampConverter))]
    public DateTimeOffset? TimeStamp { get; set; }

    [JsonProperty("note")]
    public string Note { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    public BannedUser Init(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.CommonInit(json);
      JsonConvert.PopulateObject(json.ToString(), (object) this, reddit.JsonSerializerSettings);
      return this;
    }

    public async Task<BannedUser> InitAsync(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      BannedUser bannedUser = this;
      bannedUser.CommonInit(json);
      JsonConvert.PopulateObject(json.ToString(), (object) bannedUser, reddit.JsonSerializerSettings);
      return bannedUser;
    }

    private void CommonInit(JToken json) => this.Init(json);
  }
}
