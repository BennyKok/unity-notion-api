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
        public static implicit operator string(OptionEntry option) => option.name;
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
        public static implicit operator OptionEntry(SelectProperty property) => property.select;
    }

    [Serializable]
    public class MultiSelectProperty : Property
    {
        public OptionEntry[] multi_select;
        public static implicit operator OptionEntry[](MultiSelectProperty property) => property.multi_select;
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
        public static implicit operator string(TitleProperty property)
        => (property.title != null && property.title.Length > 0) ? property.title[0].text : null;
    }

    [Serializable]
    public class TextProperty : Property
    {
        public Text[] text;
        public static implicit operator string(TextProperty property)
        => (property.text != null && property.text.Length > 0) ? property.text[0].text : null;
    }

    [Serializable]
    public class NumberProperty : Property
    {
        public float number;
        public static implicit operator float(NumberProperty property) => property.number;
    }

    [Serializable]
    public class CheckboxProperty : Property
    {
        public bool checkbox;
        public static implicit operator bool(CheckboxProperty property) => property.checkbox;
    }

    [Serializable]
    public class DateProperty : Property
    {
        public Date date;
    }
}