// Decompiled with JetBrains decompiler
// Type: RedditSharp.Domain
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using RedditSharp.Things;
using System;

namespace RedditSharp
{
  public class Domain
  {
    private const string DomainPostUrl = "/domain/{0}.json";
    private const string DomainNewUrl = "/domain/{0}/new.json?sort=new";
    private const string DomainHotUrl = "/domain/{0}/hot.json";
    private const string FrontPageUrl = "/.json";

    [JsonIgnore]
    private Reddit Reddit { get; set; }

    [JsonIgnore]
    private IWebAgent WebAgent { get; set; }

    [JsonIgnore]
    public string Name { get; set; }

    public Listing<Post> Posts => new Listing<Post>(this.Reddit, string.Format("/domain/{0}.json", (object) this.Name), this.WebAgent);

    public Listing<Post> New => new Listing<Post>(this.Reddit, string.Format("/domain/{0}/new.json?sort=new", (object) this.Name), this.WebAgent);

    public Listing<Post> Hot => new Listing<Post>(this.Reddit, string.Format("/domain/{0}/hot.json", (object) this.Name), this.WebAgent);

    protected internal Domain(Reddit reddit, Uri domain, IWebAgent webAgent)
    {
      this.Reddit = reddit;
      this.WebAgent = webAgent;
      this.Name = domain.Host;
    }

    public override string ToString() => "/domain/" + this.Name;
  }
}
