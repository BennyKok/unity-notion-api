using System;

namespace BennyKok.NotionAPI
{
    [Serializable]
    public class IDObject
    {
        public string id;
        public string created_time;
        public string last_edited_time;
    }

    [Serializable]
    public class Database<T> : IDObject
    {
        public Text[] title;
        public T properties;
    }

    [Serializable]
    public class Page<T> : IDObject
    {
        public T properties;
    }

    [Serializable]
    public class DatabaseQueryResponse<T>
    {
        public Page<T>[] results;
    }

    [Serializable]
    public class DatabaseUsers
    {
        public UserObject[] results;
    }
}