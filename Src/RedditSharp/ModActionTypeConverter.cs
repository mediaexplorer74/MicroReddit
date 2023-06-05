// Decompiled with JetBrains decompiler
// Type: RedditSharp.ModActionTypeConverter
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using System;

namespace RedditSharp
{
  public class ModActionTypeConverter : JsonConverter
  {
        public static string GetRedditParamName(ModActionType action)
        {
            return action == ModActionType.LockPost ? "lock" : action.ToString("g").ToLower();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ModActionType);
        }

        public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      string str = reader.Value.ToString();
      return str.ToLower() == "lock" ? (object) ModActionType.LockPost : Enum.Parse(typeof (ModActionType), str, true);
    }

       

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      if (value == null)
        writer.WriteNull();
      else
        writer.WriteValue(ModActionTypeConverter.GetRedditParamName((ModActionType) value));
    }

      
    }
}
