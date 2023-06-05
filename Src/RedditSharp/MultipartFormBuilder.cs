// Decompiled with JetBrains decompiler
// Type: RedditSharp.MultipartFormBuilder
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace RedditSharp
{
  public class MultipartFormBuilder
  {
    public HttpWebRequest Request { get; set; }

    private string Boundary { get; set; }

    private MemoryStream Buffer { get; set; }

    private TextWriter TextBuffer { get; set; }

    public MultipartFormBuilder(HttpWebRequest request)
    {
      this.Request = request;
      Random random = new Random();
      this.Boundary = "----------" + this.CreateRandomBoundary();
      request.ContentType = "multipart/form-data; boundary=" + this.Boundary;
      this.Buffer = new MemoryStream();
      this.TextBuffer = (TextWriter) new StreamWriter((Stream) this.Buffer);
    }

    private string CreateRandomBoundary()
    {
      string randomBoundary = "";
      Random random = new Random();
      for (int index = 0; index < 10; ++index)
        randomBoundary += "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"[random.Next("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".Length)].ToString();
      return randomBoundary;
    }

    public void AddDynamic(object data)
    {
      foreach (PropertyInfo property in data.GetType().GetProperties())
      {
        string str = Convert.ToString(property.GetValue(data, (object[]) null));
        this.AddString(property.Name, str);
      }
    }

    public void AddString(string name, string value)
    {
      this.TextBuffer.Write("{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n", (object) ("--" + this.Boundary), (object) name, (object) value);
      this.TextBuffer.Flush();
    }

    public void AddFile(string name, string filename, byte[] value, string contentType)
    {
      this.TextBuffer.Write("{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n", (object) ("--" + this.Boundary), (object) name, (object) filename, (object) contentType);
      this.TextBuffer.Flush();
      this.Buffer.Write(value, 0, value.Length);
      this.Buffer.Flush();
      this.TextBuffer.Write("\r\n");
      this.TextBuffer.Flush();
    }

    public void Finish()
    {
      this.TextBuffer.Write("--" + this.Boundary + "--");
      this.TextBuffer.Flush();
      Stream requestStream = this.Request.GetRequestStreamAsync().Result;
      this.Buffer.Seek(0L, SeekOrigin.Begin);
      this.Buffer.WriteTo(requestStream);
      requestStream.Flush();
    }
  }
}
