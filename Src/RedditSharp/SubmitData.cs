// Decompiled with JetBrains decompiler
// Type: RedditSharp.SubmitData
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

namespace RedditSharp
{
  internal abstract class SubmitData
  {
    [RedditAPIName("api_type")]
    internal string APIType { get; set; }

    [RedditAPIName("kind")]
    internal string Kind { get; set; }

    [RedditAPIName("sr")]
    internal string Subreddit { get; set; }

    [RedditAPIName("uh")]
    internal string UserHash { get; set; }

    [RedditAPIName("title")]
    internal string Title { get; set; }

    [RedditAPIName("iden")]
    internal string Iden { get; set; }

    [RedditAPIName("captcha")]
    internal string Captcha { get; set; }

    [RedditAPIName("resubmit")]
    internal bool Resubmit { get; set; }

    protected SubmitData() => this.APIType = "json";
  }
}
