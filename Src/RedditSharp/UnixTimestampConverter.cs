// Decompiled with JetBrains decompiler
// Type: RedditSharp.UnixTimestampConverter
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace RedditSharp
{
  public class UnixTimestampConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(double) || objectType == typeof(DateTime);
    }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            return token.Value<long>();//.UnixTimeStampToDateTime();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

    }
}
