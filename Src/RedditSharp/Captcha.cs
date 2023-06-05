// Decompiled with JetBrains decompiler
// Type: RedditSharp.Captcha
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using System;

namespace RedditSharp
{
  public struct Captcha
  {
    private const string UrlFormat = "http://www.reddit.com/captcha/{0}";
    public readonly string Id;
    public readonly Uri Url;

    internal Captcha(string id)
    {
      this.Id = id;
      this.Url = new Uri(string.Format("http://www.reddit.com/captcha/{0}", (object) this.Id), UriKind.Absolute);
    }
  }
}
