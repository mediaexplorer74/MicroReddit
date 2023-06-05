// Decompiled with JetBrains decompiler
// Type: RedditSharp.RateLimitException
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using System;

namespace RedditSharp
{
  public class RateLimitException : Exception
  {
    public TimeSpan TimeToReset { get; set; }

    public RateLimitException(TimeSpan timeToReset) => this.TimeToReset = timeToReset;
  }
}
