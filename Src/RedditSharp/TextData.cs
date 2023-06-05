// Decompiled with JetBrains decompiler
// Type: RedditSharp.TextData
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

namespace RedditSharp
{
  internal class TextData : SubmitData
  {
    [RedditAPIName("text")]
    internal string Text { get; set; }

    internal TextData() => this.Kind = "self";
  }
}
