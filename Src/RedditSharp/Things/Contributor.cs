// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.Contributor
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace RedditSharp.Things
{
  public class Contributor : Thing
  {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("date")]
    [JsonConverter(typeof (UnixTimestampConverter))]
    public DateTimeOffset DateAdded { get; set; }

    public Contributor Init(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.CommonInit(json);
      JsonConvert.PopulateObject(json.ToString(), (object) this, reddit.JsonSerializerSettings);
      return this;
    }

    public async Task<Contributor> InitAsync(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      Contributor contributor = this;
      contributor.CommonInit(json);
      JsonConvert.PopulateObject(json.ToString(), (object) contributor, reddit.JsonSerializerSettings);
      return contributor;
    }

    private void CommonInit(JToken json) => this.Init(json);
  }
}
