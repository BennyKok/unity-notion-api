using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace BennyKok.NotionAPI
{
    public class NotionAPI
    {
        private bool debug = true;
        private string apiKey;
        private readonly static string version = "v1";
        private readonly static string rootUrl = $"https://api.notion.com/{version}";
        private readonly static string urlDB = rootUrl + "/databases";
        private readonly static string urlUsers = rootUrl + "/users";

        public NotionAPI(string apiKey)
        {
            this.apiKey = apiKey;
        }

        enum RequestType
        {
            GET, POST
        }

        private UnityWebRequest WebRequestWithAuth(string url, RequestType requestType, WWWForm form = null)
        {
            UnityWebRequest request = null;
            switch (requestType)
            {
                case RequestType.GET:
                    request = UnityWebRequest.Get(url);
                    break;
                case RequestType.POST:
                    request = UnityWebRequest.Post(url, form);
                    break;
            }
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Notion-Version", "2021-05-13");
            return request;
        }

        public IEnumerator GetJSON(string url, Action<string> callback)
        {
            if (debug) Debug.Log("GET Requesting: " + url);
            using (var request = WebRequestWithAuth(url, RequestType.GET))
            {
                yield return request.SendWebRequest();
                var data = request.downloadHandler.text;
                callback(data);
            }
        }

        public IEnumerator PostJSON(string url, Action<string> callback, WWWForm form)
        {
            if (debug) Debug.Log("POST Requesting: " + url);
            using (var request = WebRequestWithAuth(url, RequestType.POST, form))
            {
                yield return request.SendWebRequest();
                var data = request.downloadHandler.text;
                callback(data);
            }
        }

        /// <summary>
        /// Get the Notion Database JSON object parsed with Unity's JsonUtility
        /// </summary>
        /// <param name="database_id">Database Id</param>
        /// <param name="callback"></param>
        /// <typeparam name="T">An serializable class containing all Property field for the Json parsing</typeparam>
        /// <returns></returns>
        public IEnumerator GetDatabase<T>(string database_id, Action<Database<T>> callback)
        {
            yield return GetDatabaseJSON(database_id, (json) =>
            {
                if (debug) Debug.Log(json);
                callback(JsonUtility.FromJson<Database<T>>(json));
            });
        }

        /// <summary>
        /// Return the entire Notion Database schema in raw JSON string
        /// </summary>
        /// <param name="database_id">Database Id</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public IEnumerator GetDatabaseJSON(string database_id, Action<string> callback)
        {
            var url = $"{urlDB}/{database_id}";
            yield return GetJSON(url, callback);
        }

        public IEnumerator QueryDatabase<T>(string database_id, Action<DatabaseQueryResponse<T>> callback)
        {
            yield return QueryDatabaseJSON(database_id, (json) =>
            {
                if (debug) Debug.Log(json);
                callback(JsonUtility.FromJson<DatabaseQueryResponse<T>>(json));
            });
        }

        public IEnumerator QueryDatabaseJSON(string database_id, Action<string> callback)
        {
            var url = $"{urlDB}/{database_id}/query";
            yield return PostJSON(url, callback, null);
        }

        public IEnumerator GetUsers(Action<DatabaseUsers> callback)
        {
            var url = $"{urlUsers}/";

            yield return GetJSON(url, (json) =>
            {
                if (debug) Debug.Log(json);
                callback(JsonUtility.FromJson<DatabaseUsers>(json));
            });
        }
    }
}
