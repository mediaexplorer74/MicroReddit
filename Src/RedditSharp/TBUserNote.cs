// Decompiled with JetBrains decompiler
// Type: RedditSharp.TBUserNote
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using System;

namespace RedditSharp
{
  public class TBUserNote
  {
    public int NoteTypeIndex { get; set; }

    public string NoteType { get; set; }

    public string SubName { get; set; }

    public string Submitter { get; set; }

    public int SubmitterIndex { get; set; }

    public string Message { get; set; }

    public string AppliesToUsername { get; set; }

    public string Url { get; set; }

    public DateTimeOffset Timestamp { get; set; }
  }
}
