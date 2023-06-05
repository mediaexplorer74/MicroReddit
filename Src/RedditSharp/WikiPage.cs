// Decompiled with JetBrains decompiler
// Type: RedditSharp.WikiPage
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedditSharp.Things;
using System;

namespace RedditSharp
{
  public class WikiPage
  {
    [JsonProperty("may_revise")]
    public string MayRevise { get; set; }

    [JsonProperty("revision_date")]
    [JsonConverter(typeof (UnixTimestampConverter))]
    public DateTimeOffset? RevisionDate { get; set; }

    [JsonProperty("content_html")]
    public string HtmlContent { get; set; }

    [JsonProperty("content_md")]
    public string MarkdownContent { get; set; }

    [JsonIgnore]
    public RedditUser RevisionBy { get; set; }

    protected internal WikiPage(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      this.RevisionBy = new RedditUser().Init(reddit, json[(object) "revision_by"], webAgent);
      JsonConvert.PopulateObject(json.ToString(), (object) this, reddit.JsonSerializerSettings);
    }
  }
}
