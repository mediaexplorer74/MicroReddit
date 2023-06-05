// Decompiled with JetBrains decompiler
// Type: RedditSharp.ModeratorUser
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedditSharp
{
  public class ModeratorUser
  {
    public ModeratorUser(Reddit reddit, JToken json) => JsonConvert.PopulateObject(json.ToString(), (object) this, reddit.JsonSerializerSettings);

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("mod_permissions")]
    [JsonConverter(typeof (ModeratorPermissionConverter))]
    public ModeratorPermission Permissions { get; set; }

    public override string ToString() => this.Name;
  }
}
