// Decompiled with JetBrains decompiler
// Type: RedditSharp.WikiPageSettings
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedditSharp
{
  public class WikiPageSettings
  {
    [JsonProperty("listed")]
    public bool Listed { get; set; }

    [JsonProperty("permlevel")]
    public int PermLevel { get; set; }

    [JsonIgnore]
    public IEnumerable<RedditUser> Editors { get; set; }

    public WikiPageSettings()
    {
    }

    protected internal WikiPageSettings(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.Editors = ((IEnumerable<JToken>) ((IEnumerable<JToken>) json[(object) "editors"]).ToArray<JToken>()).Select<JToken, RedditUser>((Func<JToken, RedditUser>) (x => new RedditUser().Init(reddit, x, webAgent)));
      JsonConvert.PopulateObject(json.ToString(), (object) this, reddit.JsonSerializerSettings);
    }
  }
}
