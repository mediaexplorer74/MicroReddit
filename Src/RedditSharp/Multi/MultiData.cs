// Decompiled with JetBrains decompiler
// Type: RedditSharp.Multi.MultiData
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedditSharp.Multi
{
  public class MultiData
  {
    [JsonProperty("kind")]
    public string Kind { get; set; }

    [JsonIgnore]
    public MData Data { get; set; }

    protected internal MultiData(Reddit reddit, JToken json, IWebAgent webAgent, bool subs = true)
    {
      this.Data = new MData(reddit, json[(object) "data"], webAgent, subs);
      JsonConvert.PopulateObject(json.ToString(), (object) this, reddit.JsonSerializerSettings);
    }
  }
}
