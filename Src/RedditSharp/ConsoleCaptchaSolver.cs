// Decompiled with JetBrains decompiler
// Type: RedditSharp.ConsoleCaptchaSolver
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using System;

namespace RedditSharp
{
  public class ConsoleCaptchaSolver : ICaptchaSolver
  {
    public CaptchaResponse HandleCaptcha(Captcha captcha)
    {
      Console.WriteLine("Captcha required! The captcha ID is {0}", (object) captcha.Id);
      Console.WriteLine("You can find the captcha image at this url: {0}", (object) captcha.Url);
      Console.WriteLine("Please input your captcha response or empty string to cancel:");
      string str = Console.ReadLine();
      return new CaptchaResponse(string.IsNullOrEmpty(str) ? (string) null : str);
    }
  }
}
