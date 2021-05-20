using System;

namespace BennyKok.NotionAPI
{
    [Serializable]
    public class Block
    {
        public string type;
        public string id;
    }

    [Serializable]
    public class TextBlock : Block
    {
        public Text text;
        public Annotations annotations;
        public string plain_text;
        public string href;
    }
}