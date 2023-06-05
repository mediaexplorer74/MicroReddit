// Decompiled with JetBrains decompiler
// Type: RedditSharp.ToolBoxUserNotesException
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using System;

namespace RedditSharp
{
  internal class ToolBoxUserNotesException : Exception
  {
    public ToolBoxUserNotesException()
    {
    }

    public ToolBoxUserNotesException(string message)
      : base(message)
    {
    }

    public ToolBoxUserNotesException(string message, Exception inner)
      : base(message, inner)
    {
    }
  }
}
