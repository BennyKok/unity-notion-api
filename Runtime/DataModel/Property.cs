using System;

namespace BennyKok.NotionAPI
{
    [Serializable]
    public class Property
    {
        public string type;
        public string id;
    }

    [Serializable]
    public class OptionEntry
    {
        public string id;
        public string name;
        public string color;
    }

    [Serializable]
    public class Options
    {
        public OptionEntry[] options;
    }

    [Serializable]
    public class MultiSelectProperty : Property
    {
        public Options multi_select;
    }

    [Serializable]
    public class SelectProperty : Property
    {
        public Options select;
    }

    [Serializable]
    public class TitleProperty : Property
    {

    }
}