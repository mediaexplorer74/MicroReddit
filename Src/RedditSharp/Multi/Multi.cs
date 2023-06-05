// Decompiled with JetBrains decompiler
// Type: RedditSharp.Multi.Multi
// Assembly: RedditSharp, Version=1.1.14.0, Culture=neutral, PublicKeyToken=null
// MVID: 5AA3A237-2C47-4831-9B65-C0500259A1AD
// Assembly location: C:\Users\Admin\Desktop\re\RedditSharp.dll

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Authentication;

namespace RedditSharp.Multi
{
  public class Multi
  {
    private const string GetCurrentUserMultiUrl = "/api/multi/mine";
    private const string GetPublicUserMultiUrl = "/api/multi/user/{0}";
    private const string GetMultiPathUrl = "/api/multi/{0}";
    private const string GetMultiDescriptionPathUrl = "/api/multi/{0}/description";
    private const string GetMultiSubUrl = "/api/multi/{0}/r/{1}";
    private const string PostMultiRenameUrl = "/api/multi/rename";
    private const string PutSubMultiUrl = "/api/multi/{0}/r/{1}";
    private const string CopyMultiUrl = "/api/multi/copy";

    private Reddit Reddit { get; set; }

    private IWebAgent WebAgent { get; set; }

    public Multi(Reddit reddit, IWebAgent webAgent)
    {
      this.Reddit = reddit;
      this.WebAgent = webAgent;
    }

    public List<MultiData> GetCurrentUsersMultis()
    {
      JToken jtoken = JToken.Parse(
          this.WebAgent.GetResponseString(this.WebAgent.CreateGet("/api/multi/mine")
          .GetResponseAsync().Result.GetResponseStream()));

      List<MultiData> currentUsersMultis = new List<MultiData>();
      foreach (JToken json in (IEnumerable<JToken>) jtoken)
        currentUsersMultis.Add(new MultiData(this.Reddit, json, this.WebAgent));
      return currentUsersMultis;
    }

    public List<MultiData> GetPublicUserMultis(string username)
    {
      JToken jtoken = JToken.Parse(this.WebAgent.GetResponseString(
          this.WebAgent.CreateGet(string.Format("/api/multi/user/{0}", 
          (object) username)).GetResponseAsync().Result.GetResponseStream()));

      List<MultiData> publicUserMultis = new List<MultiData>();
      foreach (JToken json in (IEnumerable<JToken>) jtoken)
        publicUserMultis.Add(new MultiData(this.Reddit, json, this.WebAgent));
      return publicUserMultis;
    }

        public MultiData GetMultiByPath(string path)
        {
            return new MultiData(this.Reddit, JToken.Parse(this.WebAgent.GetResponseString(
                this.WebAgent.CreateGet(string.Format("/api/multi/{0}", (object)path)).GetResponseAsync().Result.GetResponseStream())), this.WebAgent);
        }

        public MultiData GetMultiDescription(string path)
        {
            return new MultiData(this.Reddit, JToken.Parse(this.WebAgent.GetResponseString(this.WebAgent.CreateGet(string.Format("/api/multi/{0}/description", (object)path)).GetResponseAsync().Result.GetResponseStream())), this.WebAgent, false);
        }

        public MultiSubs GetSubInformation(string path, string subreddit)
        {
            return new MultiSubs(this.Reddit, JToken.Parse(this.WebAgent.GetResponseString(this.WebAgent.CreateGet(string.Format("/api/multi/{0}/r/{1}", (object)path, (object)subreddit)).GetResponseAsync().Result.GetResponseStream())), this.WebAgent);
        }

        public string RenameMulti(string displayName, string pathFrom, string pathTo)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost("/api/multi/rename");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        display_name = displayName,
        from = pathFrom,
        to = pathTo,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      return this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

        /// <summary>
        /// Adds a Subreddit to the given Multi
        /// </summary>
        /// <param name="path">URL Path of the Multi to update</param>
        /// <param name="subName">Name of the subreddit to add</param>
        /// <returns>A String containing the information of the updated Multi</returns>
        public string PutSubMulti(string path, string subName)
        {
            if (Reddit.User == null)
            {
                throw new AuthenticationException("No user logged in.");
            }
            var request = WebAgent.CreatePut(string.Format(PutSubMultiUrl, path, subName));
            var stream = request.GetRequestStreamAsync().Result;
            JObject modelData = new JObject();
            modelData.Add("name", subName);
            WebAgent.WritePostBody(stream, new
            {
                model = modelData,
                multipath = path,
                srname = subName,
                uh = Reddit.User.Modhash
            });
            stream.Flush();
            var response = request.GetResponseAsync().Result;
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            return data;

        }
        /// <summary>
        /// Updates the description for a given Multi
        /// </summary>
        /// <param name="path">URL path of the Multi to update</param>
        /// <param name="description">New description for the Multi</param>
        /// <returns>A string containing the updated information of the Multi</returns>
        public string PutMultiDescription(string path, string description)
        {
            if (Reddit.User == null)
            {
                throw new AuthenticationException("No user logged in.");
            }
            var request = WebAgent.CreatePut(string.Format(GetMultiDescriptionPathUrl, path));
            var stream = request.GetRequestStreamAsync().Result;
            JObject modelData = new JObject();
            modelData.Add("body_md", description);
            WebAgent.WritePostBody(stream, new
            {
                model = modelData,
                multipath = path,
                uh = Reddit.User.Modhash
            });
            stream.Flush();
            var response = request.GetResponseAsync().Result;
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            return data;
        }


        public string CopyMulti(string displayName, string pathFrom, string pathTo)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest post = this.WebAgent.CreatePost("/api/multi/copy");
      Stream requestStream = post.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        display_name = displayName,
        from = pathFrom,
        to = pathTo,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      return this.WebAgent.GetResponseString(post.GetResponseAsync().Result.GetResponseStream());
    }

    public string DeleteSub(string path, string subname)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest delete = this.WebAgent.CreateDelete(string.Format("/api/multi/{0}/r/{1}", (object) path, (object) subname));
      Stream requestStream = delete.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        multipath = path,
        srname = subname,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      return this.WebAgent.GetResponseString(delete.GetResponseAsync().Result.GetResponseStream());
    }

    public string DeleteMulti(string path)
    {
      if (this.Reddit.User == null)
        throw new AuthenticationException("No user logged in.");
      HttpWebRequest delete = this.WebAgent.CreateDelete(string.Format("/api/multi/{0}", (object) path));
      Stream requestStream = delete.GetRequestStreamAsync().Result;
      this.WebAgent.WritePostBody(requestStream, (object) new
      {
        multipath = path,
        uh = this.Reddit.User.Modhash
      });
      requestStream.Flush();
      return this.WebAgent.GetResponseString(delete.GetResponseAsync().Result.GetResponseStream());
    }

        /// <summary>
        /// Create a new Multi for the authenticated user
        /// </summary>
        /// <param name="description">Multi Description</param>
        /// <param name="displayname">Multi Display Name</param>
        /// <param name="iconname">Icon Name (must be one of the default values)</param>
        /// <param name="keycolor">Hex Code for the desired color</param>
        /// <param name="subreddits">Array of Subreddit names to add</param>
        /// <param name="visibility">Visibility state for the Multi</param>
        /// <param name="weightingscheme">Weighting Scheme for the Multi</param>
        /// <param name="path">Desired URL path for the Multi</param>
        /// <returns>A string containing the information for the newly created Multi or a status of (409) if the Multi already exists</returns>
        public string PostMulti(MData m, string path)
    {
            if (Reddit.User == null)
            {
                throw new AuthenticationException("No user logged in");
            }
            var request = WebAgent.CreatePost(string.Format(GetMultiPathUrl, path));
            var stream = request.GetRequestStreamAsync().Result;
            JObject modelData = new JObject();
            modelData.Add("description_md", m.DescriptionMD);
            modelData.Add("display_name", m.DisplayName);
            modelData.Add("icon_name", m.IconName);
            modelData.Add("key_color", m.KeyColor);
            JArray subData = new JArray();
            foreach (var s in m.Subreddits)
            {
                JObject sub = new JObject();
                sub.Add("name", s.Name);
                subData.Add(sub);
            }
            modelData.Add("subreddits", subData);
            modelData.Add("visibility", m.Visibility);
            modelData.Add("weighting_scheme", m.WeightingScheme);
            WebAgent.WritePostBody(stream, new
            {
                model = modelData,
                multipath = path,
                uh = Reddit.User.Modhash
            });
            stream.Flush();
            var response = request.GetResponseAsync().Result;
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            return data;

        }

        /// <summary>
        /// Create or update a  Multi for the authenticated user
        /// </summary>
        /// <param name="description">Multi Description</param>
        /// <param name="displayname">Multi Display Name</param>
        /// <param name="iconname">Icon Name (must be one of the default values)</param>
        /// <param name="keycolor">Hex Code for the desired color</param>
        /// <param name="subreddits">Array of Subreddit names to add</param>
        /// <param name="visibility">Visibility state for the Multi</param>
        /// <param name="weightingscheme">Weighting Scheme for the Multi</param>
        /// <param name="path">Desired URL path for the Multi</param>
        /// <returns>A string containing the information for the newly created or updated Multi or a status of (409) if the Multi already exists</returns>
        public string PutMulti(MData m, string path)
        {
            if (Reddit.User == null)
            {
                throw new AuthenticationException("No user logged in");
            }
            var request = WebAgent.CreatePut(string.Format(GetMultiPathUrl, path));
            var stream = request.GetRequestStreamAsync().Result;
            JObject modelData = new JObject();
            modelData.Add("description_md", m.DescriptionMD);
            modelData.Add("display_name", m.DisplayName);
            modelData.Add("icon_name", m.IconName);
            modelData.Add("key_color", m.KeyColor);
            JArray subData = new JArray();
            foreach (var s in m.Subreddits)
            {
                JObject sub = new JObject();
                sub.Add("name", s.Name);
                subData.Add(sub);
            }
            modelData.Add("subreddits", subData);
            modelData.Add("visibility", m.Visibility);
            modelData.Add("weighting_scheme", m.WeightingScheme);
            WebAgent.WritePostBody(stream, new
            {
                model = modelData,
                multipath = path,
                uh = Reddit.User.Modhash
            });
            stream.Flush();
            var response = request.GetResponseAsync().Result;
            var data = WebAgent.GetResponseString(response.GetResponseStream());
            return data;
        }
    }
}
