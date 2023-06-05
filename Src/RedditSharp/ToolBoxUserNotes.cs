// Decompiled with JetBrains decompiler
// Type: RedditSharp.ToolBoxUserNotes
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json.Linq;
using RedditSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;

namespace RedditSharp
{
  public static class ToolBoxUserNotes
  {
    private const string ToolBoxUserNotesWiki = "/r/{0}/wiki/usernotes";

    public static IEnumerable<TBUserNote> GetUserNotes(IWebAgent webAgent, string subName)
    {
      HttpWebRequest get = webAgent.CreateGet(string.Format("/r/{0}/wiki/usernotes", (object) subName));
      JObject jobject1 = JObject.Parse(Newtonsoft.Json.Linq.Extensions.Value<string>((IEnumerable<JToken>) webAgent.ExecuteRequest(get)[(object) "data"][(object) "content_md"]));
      int num = Newtonsoft.Json.Linq.Extensions.Value<int>((IEnumerable<JToken>) jobject1["ver"]);
      string[] array1 = jobject1["constants"][(object) "users"].Values<string>().ToArray<string>();
      string[] array2 = jobject1["constants"][(object) "warnings"].Values<string>().ToArray<string>();
      if (num < 6)
        throw new ToolBoxUserNotesException("Unsupported ToolBox version");
      try
      {
        string end;
        using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(Newtonsoft.Json.Linq.Extensions.Value<string>((IEnumerable<JToken>) jobject1["blob"]))))
        {
          memoryStream.ReadByte();
          memoryStream.ReadByte();
          using (DeflateStream deflateStream = new DeflateStream((Stream) memoryStream, CompressionMode.Decompress))
          {
            using (StreamReader streamReader = new StreamReader((Stream) deflateStream))
              end = streamReader.ReadToEnd();
          }
        }
        JObject jobject2 = JObject.Parse(end);
        List<TBUserNote> userNotes = new List<TBUserNote>();
        foreach (KeyValuePair<string, JToken> keyValuePair in jobject2)
        {
          foreach (JToken child in keyValuePair.Value[(object) "ns"].Children())
          {
            TBUserNote tbUserNote = new TBUserNote()
            {
              AppliesToUsername = keyValuePair.Key,
              SubName = subName,
              SubmitterIndex = Newtonsoft.Json.Linq.Extensions.Value<int>((IEnumerable<JToken>) child[(object) "m"]),
              Submitter = array1[Newtonsoft.Json.Linq.Extensions.Value<int>((IEnumerable<JToken>) child[(object) "m"])],
              NoteTypeIndex = Newtonsoft.Json.Linq.Extensions.Value<int>((IEnumerable<JToken>) child[(object) "w"]),
              NoteType = array2[Newtonsoft.Json.Linq.Extensions.Value<int>((IEnumerable<JToken>) child[(object) "w"])],
              Message = Newtonsoft.Json.Linq.Extensions.Value<string>((IEnumerable<JToken>) child[(object) "n"]),
              Timestamp = DateTimeOffset.FromUnixTimeSeconds(
                  Newtonsoft.Json.Linq.Extensions.Value<long>((IEnumerable<JToken>) child[(object) "t"])),
              Url = ToolBoxUserNotes.UnsquashLink(subName, ((IEnumerable<JToken>) child[(object) "l"]).ValueOrDefault<string>())
            };
            userNotes.Add(tbUserNote);
          }
        }
        return (IEnumerable<TBUserNote>) userNotes;
      }
      catch (Exception ex)
      {
        throw new ToolBoxUserNotesException("An error occured while processing Usernotes wiki. See inner exception for details", ex);
      }
    }

    public static string UnsquashLink(string subreddit, string permalink)
    {
      string str = "https://reddit.com/r/" + subreddit + "/";
      if (string.IsNullOrEmpty(permalink))
        return str;
      string[] strArray = permalink.Split(',');
      if (strArray[0] == "l")
      {
        str = str + "comments/" + strArray[1] + "/";
        if (strArray.Length > 2)
          str = str + "-/" + strArray[2] + "/";
      }
      else if (strArray[0] == "m")
        str = str + "message/messages/" + strArray[1];
      return str;
    }
  }
}
