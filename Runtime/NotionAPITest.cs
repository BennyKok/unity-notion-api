using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace BennyKok.NotionAPI
{
    public class NotionAPITest : MonoBehaviour
    {
        public string apiKey;
        public string database_id;

        private IEnumerator Start()
        {
            var api = new NotionAPI(apiKey);

            yield return api.GetDatabase<CardDatabaseProperties>(database_id, (db) =>
            {
                Debug.Log(db.id);
                Debug.Log(db.created_time);
                Debug.Log(db.title.First().text.content);

                Debug.Log(JsonUtility.ToJson(db));
            });

            yield return api.QueryDatabaseJSON(database_id, (db) =>
            {
                Debug.Log(db);
            });
        }

        [Serializable]
        public class CardDatabaseProperties
        {
            public MultiSelectProperty Tags;
            public TitleProperty Name;
        }
    }
}