using System.IO;
using System.Text;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using BennyKok.NotionAPI.Editor.SimpleJSON;
using System.Text.RegularExpressions;
using System;
using UnityEditor;

namespace BennyKok.NotionAPI.Editor
{
    [UnityEditor.CustomEditor(typeof(DatabaseSchema))]
    public class DatabaseSchemaEditor : UnityEditor.Editor
    {
        private DatabaseSchema m_target
        {
            get { return (DatabaseSchema)target; }
        }

        private bool busy;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginDisabledGroup(busy || string.IsNullOrEmpty(m_target.apiKey) || string.IsNullOrEmpty(m_target.database_id));

            if (GUILayout.Button("Fetch Schema"))
            {
                var api = new NotionAPI(m_target.apiKey);

                busy = true;

                EditorCoroutineUtility.StartCoroutine(api.GetDatabaseJSON(m_target.database_id, (db) =>
                {
                    Debug.Log(db);
                    Undo.RecordObject(m_target, "Update Schema");
                    var json = JSON.Parse(db);
                    m_target.fieldNames.Clear();
                    m_target.fieldTypes.Clear();
                    foreach (var node in json["properties"])
                    {
                        m_target.fieldNames.Add(node.Key);
                        m_target.fieldTypes.Add(node.Value["type"]);
                    }
                    EditorUtility.SetDirty(m_target);
                    busy = false;
                }), this);
            }

            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(busy || m_target.fieldNames == null || m_target.fieldNames.Count == 0);

            if (GUILayout.Button("Create Schema Class"))
            {
                CreateCodeSchemaFile(m_target);
            }

            if(GUILayout.Button("Create Serialized Database Asset"))
            {
                CreateSerializedDatabaseFile(m_target);
            }

            EditorGUI.EndDisabledGroup();
        }

        public void CreateCodeSchemaFile(DatabaseSchema target)
        {
            var sb = new StringBuilder();
            string className = RemoveWhitespace(target.name);

            sb.Append($"using {typeof(IDObject).Namespace};");
            sb.Append(Environment.NewLine);
            sb.Append("using System;");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("[Serializable]");
            sb.Append(Environment.NewLine);
            sb.Append("public class ");
            sb.Append(className);
            sb.Append(Environment.NewLine);
            sb.Append("{");
            sb.Append(Environment.NewLine);

            for (int i = 0; i < target.fieldNames.Count; i++)
            {
                var notionType = GetPropertyTypeFromNotionType(target.fieldTypes[i]);

                if (notionType == null) continue;

                var field = target.fieldNames[i];
                sb.Append("    ");
                sb.Append("public ");
                sb.Append(notionType.Name);
                sb.Append(" ");
                sb.Append(field);
                sb.Append(";");
                sb.Append(Environment.NewLine);
            }

            sb.Append("}");

            var path = Directory.GetParent(AssetDatabase.GetAssetPath(target));
            var scriptPath = Path.Combine(path.FullName, className + ".cs");
            using (var w = File.CreateText(scriptPath))
            {
                w.Write(sb);
            }

            scriptPath = "Assets" + scriptPath.Substring(Application.dataPath.Length);
            AssetDatabase.ImportAsset(scriptPath);
        }

        public void CreateSerializedDatabaseFile(DatabaseSchema target)
        {
            var sb = new StringBuilder();
            int indentLevel = 0;
            string className = RemoveWhitespace(target.name) + "Database";

            void NewLine()
            {
                sb.Append(Environment.NewLine);
                if (indentLevel > 0) sb.Append(" ".PadLeft(indentLevel * 4));
            }

            sb.Append($"using {typeof(IDObject).Namespace};");
            NewLine();
            sb.Append("using UnityEngine;");
            NewLine();
            sb.Append("#if UNITY_EDITOR");
            NewLine();
            sb.Append("using Unity.EditorCoroutines.Editor;");
            NewLine();
            sb.Append("#endif");
            NewLine();
            NewLine();
            //sb.Append(string.Format("[CreateAssetMenu(fileName = \"{0}\", menuName = \"Notion API/Databases/{1}\")]", className, className));
            //NewLine();
            sb.Append("public class ");
            sb.Append(className);
            sb.Append(" : ScriptableObject");
            NewLine();
            sb.Append("{");

            indentLevel++;

            NewLine();

            //PROPERTIES
            sb.Append("[System.Serializable]");
            NewLine();
            sb.Append("public class Definition");
            NewLine();
            sb.Append("{");

            indentLevel++;

            NewLine();

            //TITLE
            sb.Append("public TitleProperty Name;");

            for (int i = 0; i < target.fieldNames.Count; i++)
            {
                var notionType = GetPropertyDefinitionFromNotionType(target.fieldTypes[i]);

                if (notionType == null) continue;

                var field = target.fieldNames[i];
                NewLine();
                sb.Append("public ");
                sb.Append(notionType.Name);
                sb.Append(" ");
                sb.Append(field);
                sb.Append(";");
            }

            indentLevel--;

            NewLine();
            sb.Append("}");

            NewLine();
            NewLine();

            //PAGES
            sb.Append("[System.Serializable]");
            NewLine();
            sb.Append("public class Properties");
            NewLine();
            sb.Append("{");

            indentLevel++;

            bool hasPeopleProperty = false;

            for (int i = 0; i < target.fieldNames.Count; i++)
            {
                var notionType = GetPropertyTypeFromNotionType(target.fieldTypes[i]);

                if (notionType == null) continue;
                if (notionType == typeof(PeopleProperty)) hasPeopleProperty = true;

                var field = target.fieldNames[i];
                NewLine();
                sb.Append("public ");
                sb.Append(notionType.Name);
                sb.Append(" ");
                sb.Append(field);
                sb.Append(";");
            }

            indentLevel--;

            NewLine();
            sb.Append("}");

            NewLine();

            NewLine();
            sb.Append("public DatabaseSchema databaseSchema;");

            NewLine();
            sb.Append("public Database<Definition> database;");

            NewLine();
            sb.Append("public Page<Properties>[] pages;");

            if (hasPeopleProperty)
            {
                NewLine();
                sb.Append("public DatabaseUsers users;");
            }

            NewLine();
            NewLine();
            sb.Append("#if UNITY_EDITOR");
            NewLine();
            sb.Append("[EditorButton(\"Fetch Data From Notion\", \"SyncEditor\")]");
            NewLine();
            sb.Append("public bool doSync;");
            NewLine();
            NewLine();
            sb.Append("public void SyncEditor()");
            NewLine();
            sb.Append("{");
            indentLevel++;
            NewLine();
            sb.Append("var api = new NotionAPI(databaseSchema.apiKey);");
            NewLine();
            sb.Append("EditorCoroutineUtility.StartCoroutine(api.GetDatabase<Definition>(databaseSchema.database_id, (db) => { database = db; }), this);");
            NewLine();
            sb.Append("EditorCoroutineUtility.StartCoroutine(api.QueryDatabase<Properties>(databaseSchema.database_id, (pages) => { this.pages = pages.results; }), this);");

            if (hasPeopleProperty)
            {
                NewLine();
                sb.Append("EditorCoroutineUtility.StartCoroutine(api.GetUsers((users) => { this.users = users; }), this);");
            }

            NewLine();
            sb.Append("UnityEditor.EditorUtility.SetDirty(this);");

            indentLevel--;
            NewLine();
            sb.Append("}");
            NewLine();
            sb.Append("#endif");

            indentLevel--;
            NewLine();
            sb.Append("}");


            var path = Directory.GetParent(AssetDatabase.GetAssetPath(target));
            var scriptPath = Path.Combine(path.FullName, className + ".cs");
            using (var w = File.CreateText(scriptPath))
            {
                w.Write(sb);
            }

            scriptPath = "Assets" + scriptPath.Substring(Application.dataPath.Length);
            AssetDatabase.ImportAsset(scriptPath);

            EditorPrefs.SetString("NotionAPI_DatabaseName", className);
            EditorPrefs.SetString("NotionAPI_DatabasePath", Path.Combine(path.ToString(), className));
            EditorPrefs.SetInt("NotionAPI_DatabaseSchemaId", target.GetInstanceID());
        }

        public Type GetPropertyTypeFromNotionType(string notionType)
        {
            switch (notionType)
            {
                case "number": return typeof(NumberProperty);
                case "title": return typeof(TitleProperty);
                case "rich_text": return typeof(TextProperty);
                case "multi_select": return typeof(MultiSelectProperty);
                case "select": return typeof(SelectProperty);
                case "checkbox": return typeof(CheckboxProperty);
                case "date": return typeof(DateProperty);
                case "formula": return typeof(FormulaStringProperty);
                case "people": return typeof(PeopleProperty);
            }

            return null;
        }

        public Type GetPropertyDefinitionFromNotionType(string notionType)
        {
            switch (notionType)
            {
                case "multi_select": return typeof(MultiSelectPropertyDefinition);
                case "select": return typeof(SelectPropertyDefinition);
            }

            return null;
        }

        private string RemoveWhitespace(string text)
        {
            return Regex.Replace(text, @"\s", "");
        }

        [UnityEditor.Callbacks.DidReloadScripts(100)]
        private static void OnScriptsReload()
        {
            if (!EditorPrefs.HasKey("NotionAPI_DatabaseName")) return;

            string className = EditorPrefs.GetString("NotionAPI_DatabaseName");
            string path = EditorPrefs.GetString("NotionAPI_DatabasePath");
            int schemaInstanceId = EditorPrefs.GetInt("NotionAPI_DatabaseSchemaId");

            EditorPrefs.DeleteKey("NotionAPI_DatabaseName");
            EditorPrefs.DeleteKey("NotionAPI_DatabasePath");
            EditorPrefs.DeleteKey("NotionAPI_DatabaseSchemaId");

            var schema = EditorUtility.InstanceIDToObject(schemaInstanceId);

            ScriptableObject so = CreateInstance(className);
            so.name = className;
            so.GetType().GetField("databaseSchema").SetValue(so,schema);

            // Convert absolute path to relative path
            if (path.StartsWith(Path.GetFullPath(Application.dataPath))) {
                path = "Assets" + path.Substring(Application.dataPath.Length);
            }

            AssetDatabase.CreateAsset(so, path + ".asset");
            AssetDatabase.SaveAssets();
        }

        // Escape regex https://www.freeformatter.com/java-dotnet-escape.html#ad-output

        // https://stackoverflow.com/a/60556735
        // https://stackoverflow.com/a/35129815

        // Regex extractPropertiesGroupRgx = new Regex("(?:\\\"|\\')(?:properties)(?:\\\"|\\')(?=:)(?:\\:\\s*)(?:\\\"|\\')?(\\{(?:(?>[^{}\"'\\/]+)|(?>\"(?:(?>[^\\\\\"]+)|\\\\.)*\")|(?>'(?:(?>[^\\\\']+)|\\\\.)*')|(?>\\/\\/.*\\n)|(?>\\/\\*.*?\\*\\/)|(?-1))*\\})", RegexOptions.Multiline);
        // Regex extractPropertiesRgx = new Regex("(?:\\\"|\\')(?:properties)(?:\\\"|\\')(?=:)(?:\\:\\s*)(?:\\\"|\\')?(\\{(?:(?>[^{}\"'\\/]+)|(?>\"(?:(?>[^\\\\\"]+)|\\\\.)*\")|(?>'(?:(?>[^\\\\']+)|\\\\.)*')|(?>\\/\\/.*\\n)|(?>\\/\\*.*?\\*\\/)|(?-1))*\\})", RegexOptions.Multiline);

        // public string ExtractPropertiesGroupJSON(string rawDbJSON)
        // {
        //     var match = extractPropertiesGroupRgx.Match(rawDbJSON);
        //     Debug.Log(match.Groups[0].Value);
        //     return match.Groups[0].Value;
        // }

        // public void ExtractPropertiesJSON(string rawDbJSON)
        // {
        //     var match = extractPropertiesGroupRgx.Match(rawDbJSON);
        // }
    }
}
