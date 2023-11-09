using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace BennyKok.NotionAPI
{
    [CreateAssetMenu(fileName = "MyDatabaseSchema", menuName = "Notion API/DatabaseSchema", order = 0)]
    public class DatabaseSchema : ScriptableObject
    {
        public string apiKey;
        public string database_id;
        public List<string> fieldNames;
        public List<string> fieldTypes;
    }
}
