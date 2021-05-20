using System;

namespace BennyKok.NotionAPI
{
    [Serializable]
    public class Database<T>
    {
        public string id;
        public string created_time;
        public string last_edited_time;
        public TextBlock[] title;
        public T properties;
    }

    [Serializable]
    public class Text
    {
        public string content;
        public string link;
    }

    [Serializable]
    public class Annotations
    {
        public bool bold;
        public bool italic;
        public bool strikethrough;
        public bool underline;
        public bool code;
        public string color;
    }
}