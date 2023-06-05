// Decompiled with JetBrains decompiler
// Type: RedditSharp.LinkData
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

namespace RedditSharp
{
  internal class LinkData : SubmitData
  {
    [RedditAPIName("extension")]
    internal string Extension { get; set; }

    [RedditAPIName("url")]
    internal string URL { get; set; }

    internal LinkData()
    {
      this.Extension = "json";
      this.Kind = "link";
    }
  }
}
