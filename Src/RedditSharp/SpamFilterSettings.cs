// Decompiled with JetBrains decompiler
// Type: RedditSharp.SpamFilterSettings
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

namespace RedditSharp
{
  public class SpamFilterSettings
  {
    public SpamFilterStrength LinkPostStrength { get; set; }

    public SpamFilterStrength SelfPostStrength { get; set; }

    public SpamFilterStrength CommentStrength { get; set; }

    public SpamFilterSettings()
    {
      this.LinkPostStrength = SpamFilterStrength.High;
      this.SelfPostStrength = SpamFilterStrength.High;
      this.CommentStrength = SpamFilterStrength.High;
    }
  }
}
