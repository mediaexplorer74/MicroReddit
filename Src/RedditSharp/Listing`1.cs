// Decompiled with JetBrains decompiler
// Type: RedditSharp.Listing`1
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json.Linq;
using RedditSharp.Extensions;
using RedditSharp.Things;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace RedditSharp
{
  public class Listing<T> : IEnumerable<T>, IEnumerable where T : Thing
  {
    internal const int DefaultListingPerRequest = 25;

    private IWebAgent WebAgent { get; set; }

    private Reddit Reddit { get; set; }

    private string Url { get; set; }

    internal Listing(Reddit reddit, string url, IWebAgent webAgent)
    {
      this.WebAgent = webAgent;
      this.Reddit = reddit;
      this.Url = url;
    }

    public IEnumerator<T> GetEnumerator(int limitPerRequest, int maximumLimit = -1, bool stream = false) => (IEnumerator<T>) new Listing<T>.ListingEnumerator<T>(this, limitPerRequest, maximumLimit, stream);

    public IEnumerator<T> GetEnumerator() => this.GetEnumerator(25);

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public IEnumerable<T> GetListing(int maximumLimit) => this.GetListing(maximumLimit, 25);

    public IEnumerable<T> GetListing(int maximumLimit, int limitPerRequest) => Listing<T>.GetEnumerator(this.GetEnumerator(limitPerRequest, maximumLimit));

    public IEnumerable<T> GetListingStream(int limitPerRequest = -1, int maximumLimit = -1) => Listing<T>.GetEnumerator(this.GetEnumerator(limitPerRequest, maximumLimit, true));

    private static IEnumerable<T> GetEnumerator(IEnumerator<T> enumerator)
    {
      while (enumerator.MoveNext())
        yield return enumerator.Current;
    }

    private class ListingEnumerator<T> : IEnumerator<T>, IDisposable, IEnumerator where T : Thing
    {
      private bool stream;
      private List<string> done;

      private Listing<T> Listing { get; set; }

      private int CurrentPageIndex { get; set; }

      private string After { get; set; }

      private string Before { get; set; }

      private Thing[] CurrentPage { get; set; }

      private int Count { get; set; }

      private int LimitPerRequest { get; set; }

      private int MaximumLimit { get; set; }

      public ListingEnumerator(
        Listing<T> listing,
        int limitPerRequest,
        int maximumLimit,
        bool stream = false)
      {
        this.Listing = listing;
        this.CurrentPageIndex = -1;
        this.CurrentPage = new Thing[0];
        this.done = new List<string>();
        this.stream = stream;
        this.LimitPerRequest = limitPerRequest <= 0 ? 25 : limitPerRequest;
        this.MaximumLimit = maximumLimit;
      }

      public T Current => (T) this.CurrentPage[this.CurrentPageIndex];

      private void FetchNextPage()
      {
        if (this.stream)
          this.PageForward();
        else
          this.PageBack();
      }

      private void PageBack()
      {
        string url = this.Listing.Url;
        if (this.After != null)
          url = url + (url.Contains("?") ? "&" : "?") + "after=" + this.After;
        if (this.LimitPerRequest != -1)
        {
          int num = this.LimitPerRequest;
          if (this.MaximumLimit != -1)
          {
            if (num > this.MaximumLimit)
              num = this.MaximumLimit;
            else if (this.Count + num > this.MaximumLimit)
              num = this.MaximumLimit - this.Count;
          }
          if (num > 0)
            url = url + (url.Contains("?") ? (object) "&" : (object) "?") + "limit=" + (object) num;
        }
        if (this.Count > 0)
          url = url + (url.Contains("?") ? (object) "&" : (object) "?") + "count=" + (object) this.Count;
        JToken json = JToken.Parse(
            this.Listing.WebAgent.GetResponseString(this.Listing.WebAgent.CreateGet(url).GetResponseAsync().Result.GetResponseStream()));
        if (((IEnumerable<JToken>) json[(object) "kind"]).ValueOrDefault<string>() != nameof (Listing<T>))
          throw new FormatException("Reddit responded with an object that is not a listing.");
        this.Parse(json);
      }

      private void PageForward()
      {
        string url = this.Listing.Url;
        if (this.Before != null)
          url = url + (url.Contains("?") ? "&" : "?") + "before=" + this.Before;
        if (this.LimitPerRequest != -1)
        {
          int num = this.LimitPerRequest;
          if (num > this.MaximumLimit && this.MaximumLimit != -1)
            num = this.MaximumLimit;
          else if (this.Count + num > this.MaximumLimit && this.MaximumLimit != -1)
            num = this.MaximumLimit - this.Count;
          if (num > 0)
            url = url + (url.Contains("?") ? (object) "&" : (object) "?") + "limit=" + (object) num;
        }

        if (this.Count > 0)
          url = url + (url.Contains("?") ? (object) "&" : (object) "?") + "count=" + (object) this.Count;
       
        JToken json = JToken.Parse(this.Listing.WebAgent.GetResponseString(
            this.Listing.WebAgent.CreateGet(url).GetResponseAsync().Result.GetResponseStream()));

        if (((IEnumerable<JToken>) json[(object) "kind"]).ValueOrDefault<string>() != nameof (Listing<T>))
          throw new FormatException("Reddit responded with an object that is not a listingStream.");
        this.Parse(json);
      }

      private void Parse(JToken json)
      {
        JArray jarray = json[(object) "data"][(object) "children"] as JArray;
        List<Thing> thingList = new List<Thing>();
        for (int index = 0; index < ((JContainer) jarray).Count; ++index)
        {
          if (!this.stream)
          {
            thingList.Add(Thing.Parse<T>(this.Listing.Reddit, jarray[index], this.Listing.WebAgent));
          }
          else
          {
            string str1 = ((IEnumerable<JToken>) jarray[index][(object) "kind"]).ValueOrDefault<string>();
            string str2 = ((IEnumerable<JToken>) jarray[index][(object) "data"][(object) "id"]).ValueOrDefault<string>();
            if (str1 == "t4" && jarray[index][(object) "data"][(object) "replies"].HasValues)
            {
              foreach (JToken json1 in (IEnumerable<JToken>) (jarray[index][(object) "data"][(object) "replies"][(object) "data"][(object) "children"] as JArray))
              {
                string str3 = ((IEnumerable<JToken>) json1[(object) "data"][(object) "id"]).ValueOrDefault<string>();
                if (!this.done.Contains(str3))
                {
                  thingList.Add(Thing.Parse<T>(this.Listing.Reddit, json1, this.Listing.WebAgent));
                  this.done.Add(str3);
                }
              }
            }
            if (!string.IsNullOrEmpty(str2) && !this.done.Contains(str2))
            {
              thingList.Add(Thing.Parse<T>(this.Listing.Reddit, jarray[index], this.Listing.WebAgent));
              this.done.Add(str2);
            }
          }
        }
        if (this.stream)
          thingList.Reverse();
        this.CurrentPage = thingList.ToArray();
        this.Count += this.CurrentPage.Length;
        this.After = Newtonsoft.Json.Linq.Extensions.Value<string>((IEnumerable<JToken>) json[(object) "data"][(object) "after"]);
        this.Before = Newtonsoft.Json.Linq.Extensions.Value<string>((IEnumerable<JToken>) json[(object) "data"][(object) "before"]);
      }

      public void Dispose()
      {
      }

      object IEnumerator.Current => (object) this.Current;

      public bool MoveNext() => this.stream ? this.MoveNextForward() : this.MoveNextBack();

      private bool MoveNextBack()
      {
        ++this.CurrentPageIndex;
        if (this.CurrentPageIndex == this.CurrentPage.Length)
        {
          if (this.After == null && this.CurrentPageIndex != 0 || this.MaximumLimit != -1 && this.Count >= this.MaximumLimit)
            return false;
          this.FetchNextPage();
          this.CurrentPageIndex = 0;
          if (this.CurrentPage.Length == 0)
            return false;
        }
        return true;
      }

      private bool MoveNextForward()
      {
        ++this.CurrentPageIndex;
        if (this.CurrentPageIndex == this.CurrentPage.Length)
        {
          int tries = 0;
          while (this.MaximumLimit == -1 || this.Count < this.MaximumLimit)
          {
            ++tries;
            try
            {
              this.FetchNextPage();
            }
            catch (Exception ex)
            {
              this.Sleep(tries, ex);
            }
            this.CurrentPageIndex = 0;
            if (this.CurrentPage.Length == 0)
              this.Sleep(tries);
            else
              goto label_10;
          }
          return false;
        }
label_10:
        return true;
      }

      private void Sleep(int tries, Exception ex = null)
      {
        int num = 180;
        if (tries > 36)
        {
          if (ex != null)
            throw ex;
        }
        else
          num = tries * 5;
        Thread.Sleep(num * 1000);
      }

      public void Reset()
      {
        this.After = this.Before = (string) null;
        this.CurrentPageIndex = -1;
        this.CurrentPage = new Thing[0];
      }
    }
  }
}
