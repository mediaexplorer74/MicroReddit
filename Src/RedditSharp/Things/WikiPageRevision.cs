// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.WikiPageRevision
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace RedditSharp.Things
{
  public class WikiPageRevision : Thing
  {
    [JsonProperty("id")]
    public new string Id { get; private set; }

    [JsonProperty("timestamp")]
    [JsonConverter(typeof (UnixTimestampConverter))]
    public DateTimeOffset? TimeStamp { get; set; }

    [JsonProperty("reason")]
    public string Reason { get; private set; }

    [JsonProperty("page")]
    public string Page { get; private set; }

    [JsonIgnore]
    public RedditUser Author { get; set; }

    protected internal WikiPageRevision()
    {
    }

    internal async Task<WikiPageRevision> InitAsync(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      WikiPageRevision wikiPageRevision = this;
      wikiPageRevision.CommonInit(reddit, json, webAgent);
      JsonConvert.PopulateObject(json.ToString(), (object) wikiPageRevision, reddit.JsonSerializerSettings);
      return wikiPageRevision;
    }

    internal WikiPageRevision Init(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.CommonInit(reddit, json, webAgent);
      JsonConvert.PopulateObject(json.ToString(), (object) this, reddit.JsonSerializerSettings);
      return this;
    }

    private void CommonInit(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.Init(json);
      this.Author = new RedditUser().Init(reddit, json[(object) "author"], webAgent);
    }
  }
}
