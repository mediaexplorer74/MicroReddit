// Decompiled with JetBrains decompiler
// Type: RedditSharp.UserFlairTemplate
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RedditSharp
{
  public class UserFlairTemplate
  {
    [JsonProperty("flair_text")]
    public string Text { get; set; }

    [JsonProperty("flair_css_class")]
    public string CssClass { get; set; }

    [JsonProperty("flair_template_id")]
    public string TemplateId { get; set; }

    [JsonProperty("flair_text_editable")]
    public bool IsEditable { get; set; }

    [JsonProperty("flair_position")]
    [JsonConverter(typeof (StringEnumConverter))]
    public FlairPosition FlairPosition { get; set; }
  }
}
