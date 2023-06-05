// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.More
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace RedditSharp.Things
{
  public class More : Thing
  {
    private const string MoreUrl = "/api/morechildren.json?link_id={0}&children={1}&api_type=json";

    [JsonProperty("children")]
    public string[] Children { get; set; }

    [JsonProperty("parent_id")]
    public string ParentId { get; set; }

    public More Init(Reddit reddit, JToken more, IWebAgent webAgent)
    {
      this.CommonInit(reddit, more, webAgent);
      JsonConvert.PopulateObject(more[(object) "data"].ToString(), (object) this, reddit.JsonSerializerSettings);
      return this;
    }

    private void CommonInit(Reddit reddit, JToken more, IWebAgent webAgent)
    {
      this.Init(more);
      this.Reddit = reddit;
      this.WebAgent = webAgent;
    }

    public IEnumerable<Thing> Things()
    {
      More more = this;
      string url = string.Format(
          "/api/morechildren.json?link_id={0}&children={1}&api_type=json", 
          (object) more.ParentId, (object) string.Join(",", more.Children));

      WebResponse response = more.WebAgent.CreateGet(url).GetResponseAsync().Result;

      JToken jtoken = JObject.Parse(more.WebAgent.GetResponseString(
          response.GetResponseStream()))["json"];

      if (((IEnumerable<JToken>) jtoken[(object) "errors"]).Count<JToken>() != 0)
        throw new AuthenticationException("Incorrect login.");
      foreach (JToken json in (IEnumerable<JToken>) jtoken[(object) "data"][(object) "things"])
        yield return Thing.Parse(more.Reddit, json, more.WebAgent);
    }

    internal async Task<Thing> InitAsync(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      More more = this;
      more.CommonInit(reddit, json, webAgent);
      JsonConvert.PopulateObject(json[(object) "data"].ToString(), (object) more, reddit.JsonSerializerSettings);
      return (Thing) more;
    }
  }
}
