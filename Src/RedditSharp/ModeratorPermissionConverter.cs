// Decompiled with JetBrains decompiler
// Type: RedditSharp.ModeratorPermissionConverter
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RedditSharp
{
  internal class ModeratorPermissionConverter : JsonConverter
  {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson
        (
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer
        )
        {
          ModeratorPermission result;
          if (!Enum.TryParse<ModeratorPermission>(string.Join(",",
              ((IEnumerable<JToken>) JArray.Load(reader)).Select<JToken, string>(
                  (Func<JToken, string>) (t => t.ToString()))), true, out result))
            result = ModeratorPermission.None;

          return (object) result;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ModeratorPermission);
        }
    }
}
