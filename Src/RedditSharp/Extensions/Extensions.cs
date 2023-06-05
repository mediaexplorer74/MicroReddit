// Decompiled with JetBrains decompiler
// Type: RedditSharp.Extensions.Extensions
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace RedditSharp.Extensions
{
  public static class Extensions
  {
    public static T ValueOrDefault<T>(this IEnumerable<JToken> enumerable) => enumerable == null ? default (T) : Newtonsoft.Json.Linq.Extensions.Value<T>(enumerable);
  }
}
