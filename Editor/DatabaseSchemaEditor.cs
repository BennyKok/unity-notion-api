using System.IO;
using System.Text;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using BennyKok.NotionAPI.Editor.SimpleJSON;
using System;
using UnityEditor;

namespace BennyKok.NotionAPI.Editor
{
    [UnityEditor.CustomEditor(typeof(DatabaseSchema))]
    public class DatabaseSchemaEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Fetch Schema"))
            {
                var m_target = target as DatabaseSchema;
                var api = new NotionAPI(m_target.apiKey);

                EditorCoroutineUtility.StartCoroutine(api.GetDatabaseJSON(m_target.database_id, (db) =>
                {
                    Debug.Log(db);
                    UnityEditor.Undo.RecordObject(m_target, "Update Schema");
                    var json = JSON.Parse(db);
                    m_target.fieldNames.Clear();
                    m_target.fieldTypes.Clear();
                    foreach (var node in json["properties"])
                    {
                        m_target.fieldNames.Add(node.Key);
                        m_target.fieldTypes.Add(node.Value["type"]);
                    }
                    EditorUtility.SetDirty(m_target);
                    CreateCodeSchemaFile(m_target);
                }), this);
            }
            if (GUILayout.Button("Re-generate"))
            {
                var m_target = target as DatabaseSchema;
                var api = new NotionAPI(m_target.apiKey);

                CreateCodeSchemaFile(m_target);
            }
        }

        public void CreateCodeSchemaFile(DatabaseSchema target)
        {
            var sb = new StringBuilder();
            sb.Append($"using {typeof(IDObject).Namespace};");
            sb.Append(Environment.NewLine);
            sb.Append("using System;");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("[Serializable]");
            sb.Append(Environment.NewLine);
            sb.Append("public class ");
            sb.Append(target.name);
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
            var scriptPath = Path.Combine(path.FullName, target.name + ".cs");
            using (var w = File.CreateText(scriptPath))
            {
                w.Write(sb);
            }

            scriptPath = "Assets" + scriptPath.Substring(Application.dataPath.Length);
            AssetDatabase.ImportAsset(scriptPath);
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
            }

            return null;
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