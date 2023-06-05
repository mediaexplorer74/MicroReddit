// Type: RedditSharp.RedditException
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using System;
using System.Runtime.Serialization;

namespace RedditSharp
{
  [Serializable]
  public class RedditException : Exception
  {
    public RedditException()
    {
    }

    public RedditException(string message)
      : base(message)
    {
    }

    public RedditException(string message, Exception inner)
      : base(message, inner)
    {
    }

     //RnD
    protected RedditException(SerializationInfo info, StreamingContext context)
      : base(info.ToString(), /*context*/default)
    {
    }
  }
}
