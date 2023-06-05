// Decompiled with JetBrains decompiler
// Type: RedditSharp.ModeratorPermission
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using System;

namespace RedditSharp
{
  [Flags]
  public enum ModeratorPermission
  {
    None = 0,
    Access = 1,
    Config = 2,
    Flair = 4,
    Mail = 8,
    Posts = 16, // 0x00000010
    Wiki = 32, // 0x00000020
    All = Wiki | Posts | Mail | Flair | Config | Access, // 0x0000003F
  }
}
