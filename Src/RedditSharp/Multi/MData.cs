// Decompiled with JetBrains decompiler
// Type: RedditSharp.Multi.MData
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedditSharp.Multi
{
  public class MData
  {
    [JsonProperty("can_edit")]
    public bool CanEdit { get; set; }

    [JsonProperty("display_name")]
    public string DisplayName { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("description_html")]
    public string DescriptionHTML { get; set; }

    [JsonProperty("created")]
    [JsonConverter(typeof (UnixTimestampConverter))]
    public DateTimeOffset? Created { get; set; }

    [JsonProperty("copied_from")]
    public string CopiedFrom { get; set; }

    [JsonProperty("icon_url")]
    public string IconUrl { get; set; }

    [JsonIgnore]
    public List<MultiSubs> Subreddits { get; set; }

    [JsonProperty("created_utc")]
    [JsonConverter(typeof (UnixTimestampConverter))]
    public DateTimeOffset? CreatedUTC { get; set; }

    [JsonProperty("key_color")]
    public string KeyColor { get; set; }

    [JsonProperty("visibility")]
    public string Visibility { get; set; }

    [JsonProperty("icon_name")]
    public string IconName { get; set; }

    [JsonProperty("weighting_scheme")]
    public string WeightingScheme { get; set; }

    [JsonProperty("path")]
    public string Path { get; set; }

    [JsonProperty("description_md")]
    public string DescriptionMD { get; set; }

    protected internal MData(Reddit reddit, JToken json, IWebAgent webAgent, bool subs)
    {
      this.Subreddits = new List<MultiSubs>();
      if (subs)
      {
        for (int index = 0; index < ((IEnumerable<JToken>) json[(object) "subreddits"]).Count<JToken>(); ++index)
          this.Subreddits.Add(new MultiSubs(reddit, json[(object) "subreddits"][(object) index], webAgent));
      }
      JsonConvert.PopulateObject(json.ToString(), (object) this, reddit.JsonSerializerSettings);
    }

    public MData()
    {
    }
  }
}
