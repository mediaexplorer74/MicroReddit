// Decompiled with JetBrains decompiler
// Type: RedditSharp.Multi.MultiSubs
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedditSharp.Multi
{
  public class MultiSubs
  {
    [JsonProperty("name")]
    public string Name { get; set; }

    protected internal MultiSubs(Reddit reddit, JToken json, IWebAgent webAgent) => JsonConvert.PopulateObject(json.ToString(), (object) this, reddit.JsonSerializerSettings);

    public MultiSubs(string name) => this.Name = name;
  }
}
