// Decompiled with JetBrains decompiler
// Type: RedditSharp.UrlParser
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace RedditSharp
{
  class UrlParser : JsonConverter
  {
    public override bool CanConvert(Type objectType) 
            => objectType == typeof (string) || objectType == typeof (Uri);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JToken token = JToken.Load(reader);
      if (token.Type != JTokenType.String)
        return (object) token.Value<Uri>((IEnumerable<JToken>) token);

      if (Type.GetType("Mono.Runtime") == (Type) null)
        return (object) new Uri(token.Value<string>((IEnumerable<JToken>) token), 
            UriKind.RelativeOrAbsolute);
      
      return token.Value<string>((IEnumerable<JToken>) token).StartsWith("/") 
                ? (object) new Uri(token.Value<string>((IEnumerable<JToken>) token), 
                UriKind.Relative) 
                : (object) new Uri(token.Value<string>((IEnumerable<JToken>) token), 
                UriKind.RelativeOrAbsolute);
    }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
