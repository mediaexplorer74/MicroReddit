using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MicroReddit.Entities;
using MicroReddit.Interfaces;

namespace MicroReddit.Services
{
    public class RedditService : IRedditService
    {
        private AuthInfo _authInfo;
        private HttpClient _httpClient;
        private DateTime _lastRequest;

        public RedditService(AuthInfo authInfo)
        {
            _authInfo = authInfo;
            if (_authInfo == null)
            {
                Debug.WriteLine(_authInfo);
                return;
            }

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_authInfo.baseAddress);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(("application/json")));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(_authInfo.tokenType, _authInfo.accessToken);
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Baconit2", "v1"));
        }

        public async Task<List<Post>> GetTopPostsByLimit(int limit)
        {
            var url = $"/top?limit={limit}";
            HttpResponseMessage response = default;

            try
            {
                response = await _httpClient.GetAsync(url);
            }
            catch 
            {
            }

            if (response.IsSuccessStatusCode)
            {
                _lastRequest = DateTime.Now;

                var stringResponse = await response.Content.ReadAsStringAsync();

                List<Post> posts = GetTopPosts(stringResponse);

                return posts;
            }
            else
            {
                throw new HttpRequestException("[HttpRequestException]: GetTopPosts(): Request FAIL");
            }
        }

        private List<Post> GetTopPosts(string response)
        {
            var myJson = JsonConvert.DeserializeObject<dynamic>(response);

            var postsList = new List<Post>();
            
            int initialSize = postsList.Count();

            foreach (var post in myJson.data.children)
            {
                postsList.Add(
                    new Post()
                    {
                        id = initialSize,
                        author = post.data.author,
                        title = post.data.title,
                        subreddit = post.data.subreddit,
                        dateTimeCreated = ConvertFromUnixTimestamp((int)post.data.created_utc),
                        numberOfComments = post.data.num_comments,
                        thumbnail = post.data.thumbnail,
                        mainPicture = post.data.url
                    }
                );

                initialSize++;
            }

            return postsList;
        }

        private DateTime ConvertFromUnixTimestamp(int created)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(created);
        }
    }
}
