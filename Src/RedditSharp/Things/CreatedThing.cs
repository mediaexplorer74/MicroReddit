// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.CreatedThing
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace RedditSharp.Things
{
  public class CreatedThing : Thing
  {
    protected CreatedThing Init(Reddit reddit, JToken json)
    {
      this.CommonInit(reddit, json);
      JsonConvert.PopulateObject(json[(object) "data"].ToString(), (object) this, reddit.JsonSerializerSettings);
      return this;
    }

    protected async Task<CreatedThing> InitAsync(Reddit reddit, JToken json)
    {
      CreatedThing createdThing = this;
      createdThing.CommonInit(reddit, json);
      JsonConvert.PopulateObject(json[(object) "data"].ToString(), (object) createdThing, reddit.JsonSerializerSettings);
      return createdThing;
    }

    private void CommonInit(Reddit reddit, JToken json)
    {
      this.Init(json);
      this.Reddit = reddit;
    }

    [JsonProperty("created")]
    [JsonConverter(typeof (UnixTimestampConverter))]
    public DateTimeOffset Created { get; set; }

    [JsonProperty("created_utc")]
    [JsonConverter(typeof (UnixTimestampConverter))]
    public DateTimeOffset CreatedUTC { get; set; }
  }
}
