// Type: RedditSharp.Wiki
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json.Linq;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;

namespace RedditSharp
{
  public class Wiki
  {
    private const string GetWikiPageUrl = "/r/{0}/wiki/{1}.json?v={2}";
    private const string GetWikiPagesUrl = "/r/{0}/wiki/pages.json";
    private const string WikiPageEditUrl = "/r/{0}/api/wiki/edit";
    private const string HideWikiPageUrl = "/r/{0}/api/wiki/hide";
    private const string RevertWikiPageUrl = "/r/{0}/api/wiki/revert";
    private const string WikiPageAllowEditorAddUrl = "/r/{0}/api/wiki/alloweditor/add";
    private const string WikiPageAllowEditorDelUrl = "/r/{0}/api/wiki/alloweditor/del";
    private const string WikiPageSettingsUrl = "/r/{0}/wiki/settings/{1}.json";
    private const string WikiRevisionsUrl = "/r/{0}/wiki/revisions.json";
    private const string WikiPageRevisionsUrl = "/r/{0}/wiki/revisions/{1}.json";
    private const string WikiPageDiscussionsUrl = "/r/{0}/wiki/discussions/{1}.json";

    private Reddit Reddit { get; set; }

    private Subreddit Subreddit { get; set; }

    private IWebAgent WebAgent { get; set; }

        /// <summary>
        /// Get a list of wiki page names for this subreddit.
        /// </summary>
        public IEnumerable<string> PageNames
        {
            get
            {
                //return JObject.Parse(
                //    this.WebAgent.GetResponseString(
                //        this.WebAgent.CreateGet(
                //            string.Format("/r/{0}/wiki/pages.json", (object)this.Subreddit.Name)).GetResponse().GetResponseStream()))["data"].Values<string>();
                var request = WebAgent.CreateGet(string.Format(GetWikiPagesUrl, Subreddit.Name));
                System.Threading.Tasks.Task<WebResponse> responseT = request.GetResponseAsync();//.GetResponse();

                WebResponse response = responseT.Result;
                Stream stream = response.GetResponseStream();
                string json = WebAgent.GetResponseString(stream);
                return JObject.Parse(json)["data"].Values<string>();
            }
        }

        public Listing<WikiPageRevision> Revisions => new Listing<WikiPageRevision>(this.Reddit, string.Format("/r/{0}/wiki/revisions.json", (object) this.Subreddit.Name), this.WebAgent);

    protected internal Wiki(Reddit reddit, Subreddit subreddit, IWebAgent webAgent)
    {
      this.Reddit = reddit;
      this.Subreddit = subreddit;
      this.WebAgent = webAgent;
    }

        public WikiPage GetPage(string page, string version = null)
        {
            return new WikiPage(this.Reddit, 
                JObject.Parse(this.WebAgent.GetResponseString(
                    this.WebAgent.CreateGet(string.Format("/r/{0}/wiki/{1}.json?v={2}", 
                (object)this.Subreddit.Name, (object)page,
                (object)version)).GetResponseAsync().Result.GetResponseStream()))["data"], 
                this.WebAgent);
        }

        public WikiPageSettings GetPageSettings(string name)
        {
            return new WikiPageSettings(this.Reddit, 
                JObject.Parse(this.WebAgent.GetResponseString(
                    this.WebAgent.CreateGet(string.Format("/r/{0}/wiki/settings/{1}.json", 
                (object)this.Subreddit.Name,
                (object)name)).GetResponseAsync().Result.GetResponseStream()))["data"], this.WebAgent);
        }

        public void SetPageSettings(string name, WikiPageSettings settings)
    {
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/r/{0}/wiki/settings/{1}.json", (object) this.Subreddit.Name, (object) name));

      System.Threading.Tasks.Task<Stream> ttt = post.GetRequestStreamAsync();

      Stream rrr = ttt.Result;


      this.WebAgent.WritePostBody(rrr, (object) new
      {
        page = name,
        permlevel = settings.PermLevel,
        listed = settings.Listed,
        uh = this.Reddit.User.Modhash
      });

      System.Threading.Tasks.Task<WebResponse> ppp = post.GetResponseAsync();
      WebResponse r = ppp.Result;
      this.WebAgent.GetResponseString(r.GetResponseStream()); 
    }

    public Listing<WikiPageRevision> GetPageRevisions(string page)
    {
        return new Listing<WikiPageRevision>(this.Reddit, string.Format("/r/{0}/wiki/revisions/{1}.json", (object)this.Subreddit.Name, (object)page), this.WebAgent);
    }

    /// <summary>
    /// Get a list of discussions about this wiki page.
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public Listing<Post> GetPageDiscussions(string page)
    {
        return new Listing<Post>(this.Reddit, string.Format("/r/{0}/wiki/discussions/{1}.json", (object)this.Subreddit.Name, (object)page), this.WebAgent);
    }

    /// <summary>
    /// Edit a wiki page.
    /// </summary>
    /// <param name="page">wiki page</param>
    /// <param name="content">new content</param>
    /// <param name="previous">previous</param>
    /// <param name="reason">reason for edit</param>
    public void EditPage(string page, string content, string previous = null, string reason = null)
    {
            /*
             HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/r/{0}/api/wiki/edit", (object) this.Subreddit.Name));
             object obj = (object) new
             {
               content = content,
               page = page,
               uh = this.Reddit.User.Modhash
             };
             List<string> stringList = new List<string>();
             if (previous != null)
             {
               stringList.Add(nameof (previous));
               stringList.Add(previous);
             }
             if (reason != null)
             {
               stringList.Add(nameof (reason));
               stringList.Add(reason);
             }
             // ISSUE: reference to a compiler-generated field
             if (Wiki.\u003C\u003Eo__33.\u003C\u003Ep__0 == null)
             {
               // ISSUE: reference to a compiler-generated field
               Wiki.\u003C\u003Eo__33.\u003C\u003Ep__0 = CallSite<Action<CallSite, IWebAgent, Stream, object, string[]>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "WritePostBody", (IEnumerable<Type>) null, typeof (Wiki), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
               {
                 CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                 CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
                 CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
                 CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
               }));
             }
             // ISSUE: reference to a compiler-generated field
             // ISSUE: reference to a compiler-generated field
             Wiki.\u003C\u003Eo__33.\u003C\u003Ep__0.Target((CallSite) Wiki.\u003C\u003Eo__33.\u003C\u003Ep__0, this.WebAgent, post.GetRequestStream(), obj, stringList.ToArray());
             this.WebAgent.GetResponseString(post.GetResponse().GetResponseStream());
              */
            var request = WebAgent.CreatePost(string.Format(WikiPageEditUrl, Subreddit.Name));
            dynamic param = new
            {
                content = content,
                page = page,
                uh = Reddit.User.Modhash
            };
            List<string> addParams = new List<string>();
            if (previous != null)
            {
                addParams.Add("previous");
                addParams.Add(previous);
            }
            if (reason != null)
            {
                addParams.Add("reason");
                addParams.Add(reason);
            }

            System.Threading.Tasks.Task<Stream> rrr = request.GetRequestStreamAsync();
            Stream ttt = rrr.Result;
            WebAgent.WritePostBody(ttt, param, addParams.ToArray());
            var responseT = request.GetResponseAsync();
            var response = responseT.Result;
            var data = WebAgent.GetResponseString(response.GetResponseStream());
        }

        /// <summary>
        /// Hide the specified wiki page.
        /// </summary>
        /// <param name="page">wiki page.</param>
        /// <param name="revision">reason for revision.</param>
        public void HidePage(string page, string revision)
    {
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/r/{0}/api/wiki/hide", (object) this.Subreddit.Name));

      System.Threading.Tasks.Task<Stream> requestT = post.GetRequestStreamAsync();
      Stream stream = requestT.Result;
      this.WebAgent.WritePostBody(stream, (object) new
      {
        page = page,
        revision = revision,
        uh = this.Reddit.User.Modhash
      });
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void RevertPage(string page, string revision)
    {
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format("/r/{0}/api/wiki/revert", (object) this.Subreddit.Name));
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        page = page,
        revision = revision,
        uh = this.Reddit.User.Modhash
      });
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public void SetPageEditor(string page, string username, bool allow)
    {
      HttpWebRequest post = this.WebAgent.CreatePost(string.Format(allow ? "/r/{0}/api/wiki/alloweditor/add" : "/r/{0}/api/wiki/alloweditor/del", (object) this.Subreddit.Name));
      this.WebAgent.WritePostBody(post.GetRequestStreamAsync().Result, (object) new
      {
        page = page,
        username = username,
        uh = this.Reddit.User.Modhash
      });
      this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    [Obsolete("Use PageNames property instead")]
    public IEnumerable<string> GetPageNames() => this.PageNames;

    [Obsolete("Use Revisions property instead")]
    public Listing<WikiPageRevision> GetRevisions() => this.Revisions;
  }
}
