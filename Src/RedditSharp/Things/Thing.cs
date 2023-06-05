// Decompiled with JetBrains decompiler
// Type: RedditSharp.Things.Thing
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json.Linq;
using RedditSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace RedditSharp.Things
{
  public class Thing
  {
    protected Reddit Reddit { get; set; }

    protected IWebAgent WebAgent { get; set; }

    internal void Init(JToken json)
    {
      if (json == null)
        return;
      JToken jtoken = json[(object) "name"] == null ? json[(object) "data"] : json;
      this.FullName = ((IEnumerable<JToken>) jtoken[(object) "name"]).ValueOrDefault<string>();
      this.Id = ((IEnumerable<JToken>) jtoken[(object) "id"]).ValueOrDefault<string>();
      this.Kind = ((IEnumerable<JToken>) json[(object) "kind"]).ValueOrDefault<string>();
      this.FetchedAt = DateTimeOffset.Now;
    }

    public string Id { get; set; }

    public string FullName { get; set; }

    public string Kind { get; set; }

    public DateTimeOffset FetchedAt { get; private set; }

    public TimeSpan TimeSinceFetch => DateTimeOffset.Now - this.FetchedAt;

    public static async Task<Thing> ParseAsync(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      switch (((IEnumerable<JToken>) json[(object) "kind"]).ValueOrDefault<string>())
      {
        case "LiveUpdate":
          return (Thing) await new LiveUpdate().InitAsync(reddit, json, webAgent);
        case "LiveUpdateEvent":
          return (Thing) await new LiveUpdateEvent().InitAsync(reddit, json, webAgent);
        case "modaction":
          return (Thing) await new ModAction().InitAsync(reddit, json, webAgent);
        case "more":
          return await new More().InitAsync(reddit, json, webAgent);
        case "t1":
          return (Thing) await new Comment().InitAsync(reddit, json, webAgent, (Thing) null);
        case "t2":
          return (Thing) await new RedditUser().InitAsync(reddit, json, webAgent);
        case "t3":
          return (Thing) await new Post().InitAsync(reddit, json, webAgent);
        case "t4":
          return (Thing) await new PrivateMessage().InitAsync(reddit, json, webAgent);
        case "t5":
          return (Thing) await new Subreddit().InitAsync(reddit, json, webAgent);
        default:
          return (Thing) null;
      }
    }

    public static Thing Parse(Reddit reddit, JToken json, IWebAgent webAgent)
    {
      switch (((IEnumerable<JToken>) json[(object) "kind"]).ValueOrDefault<string>())
      {
        case "LiveUpdate":
          return (Thing) new LiveUpdate().Init(reddit, json, webAgent);
        case "LiveUpdateEvent":
          return (Thing) new LiveUpdateEvent().Init(reddit, json, webAgent);
        case "modaction":
          return (Thing) new ModAction().Init(reddit, json, webAgent);
        case "more":
          return (Thing) new More().Init(reddit, json, webAgent);
        case "t1":
          return (Thing) new Comment().Init(reddit, json, webAgent, (Thing) null);
        case "t2":
          return (Thing) new RedditUser().Init(reddit, json, webAgent);
        case "t3":
          return (Thing) new Post().Init(reddit, json, webAgent);
        case "t4":
          return (Thing) new PrivateMessage().Init(reddit, json, webAgent);
        case "t5":
          return (Thing) new Subreddit().Init(reddit, json, webAgent);
        default:
          return (Thing) null;
      }
    }

    public static async Task<Thing> ParseAsync<T>(Reddit reddit, JToken json, IWebAgent webAgent) where T : Thing
    {
      Thing result = await Thing.ParseAsync(reddit, json, webAgent);
      if (result == null)
      {
        if (typeof (T) == typeof (WikiPageRevision))
          return (Thing) await new WikiPageRevision().InitAsync(reddit, json, webAgent);
        if (typeof (T) == typeof (ModAction))
          return (Thing) await new ModAction().InitAsync(reddit, json, webAgent);
        if (typeof (T) == typeof (Contributor))
          return (Thing) await new Contributor().InitAsync(reddit, json, webAgent);
        if (typeof (T) == typeof (BannedUser))
          return (Thing) await new BannedUser().InitAsync(reddit, json, webAgent);
        if (typeof (T) == typeof (More))
          return await new More().InitAsync(reddit, json, webAgent);
        if (typeof (T) == typeof (LiveUpdate))
          return (Thing) await new LiveUpdate().InitAsync(reddit, json, webAgent);
        if (typeof (T) == typeof (LiveUpdateEvent))
          return (Thing) await new LiveUpdateEvent().InitAsync(reddit, json, webAgent);
      }
      return result;
    }

    public static Thing Parse<T>(Reddit reddit, JToken json, IWebAgent webAgent) where T : Thing
    {
      Thing thing = Thing.Parse(reddit, json, webAgent);
      if (thing == null)
      {
        if (typeof (T) == typeof (WikiPageRevision))
          return (Thing) new WikiPageRevision().Init(reddit, json, webAgent);
        if (typeof (T) == typeof (ModAction))
          return (Thing) new ModAction().Init(reddit, json, webAgent);
        if (typeof (T) == typeof (Contributor))
          return (Thing) new Contributor().Init(reddit, json, webAgent);
        if (typeof (T) == typeof (BannedUser))
          return (Thing) new BannedUser().Init(reddit, json, webAgent);
        if (typeof (T) == typeof (More))
          return (Thing) new More().Init(reddit, json, webAgent);
        if (typeof (T) == typeof (LiveUpdate))
          return (Thing) new LiveUpdate().Init(reddit, json, webAgent);
        if (typeof (T) == typeof (LiveUpdateEvent))
          return (Thing) new LiveUpdateEvent().Init(reddit, json, webAgent);
      }
      return thing;
    }

    protected virtual string SimpleAction(string endpoint)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost(endpoint);
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        id = this.FullName,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      return this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }
  }
}
