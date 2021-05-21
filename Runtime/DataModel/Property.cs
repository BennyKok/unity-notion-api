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
    public class MultiSelectPropertyDefinition : Property
    {
        public Options multi_select;
    }

    [Serializable]
    public class SelectProperty : Property
    {
        public OptionEntry select;
    }

    [Serializable]
    public class MultiSelectProperty : Property
    {
        public OptionEntry[] multi_select;
    }

    [Serializable]
    public class SelectPropertyDefinition : Property
    {
        public Options select;
    }

    [Serializable]
    public class TitleProperty : Property
    {
        public Text[] title;
    }

    [Serializable]
    public class TextProperty : Property
    {
        public Text[] text;
    }

    [Serializable]
    public class NumberProperty : Property
    {
        public float number;
    }

    [Serializable]
    public class CheckboxProperty : Property
    {
        public bool checkbox;
    }

    [Serializable]
    public class DateProperty : Property
    {
        public Date date;
    }
}