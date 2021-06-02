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
        public OptionEntry Value => select;
    }

    [Serializable]
    public class MultiSelectProperty : Property
    {
        public OptionEntry[] multi_select;
        public OptionEntry[] Value => multi_select;
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
        public string Value
        => (title != null && title.Length > 0) ? title[0].text : null;
    }

    [Serializable]
    public class TextPropertyDefinition : Property
    {
        public Text[] text;
        public string Value
        => (text != null && text.Length > 0) ? text[0].plain_text : null;
    }

    [Serializable]
    public class TextProperty : Property
    {
        public Text[] rich_text;
        public string Value
        => (rich_text != null && rich_text.Length > 0) ? rich_text[0].plain_text : null;
    }

    [Serializable]
    public class FormulaStringProperty : Property
    {
        public FormulaString formula;
        public string Value
        => formula.@string;

        [Serializable]
        public class FormulaString
        {
            public string type;
            public string @string;
        }
    }

    [Serializable]
    public class NumberProperty : Property
    {
        public float number;
        public float Value => number;

        public override string ToString() => number.ToString();
    }

    [Serializable]
    public class CheckboxProperty : Property
    {
        public bool checkbox;
        public bool Value => checkbox;

        public override string ToString() => checkbox.ToString();
    }

    [Serializable]
    public class DateProperty : Property
    {
        public Date date;
    }

    [Serializable]
    public class Person
    {
        public string email;
    }

    [Serializable]
    public class UserObject : Property
    {
        public string name;
        public string avatar_url;
        public Person person;
    }

    [Serializable]
    public class PeopleProperty : Property
    {
        public UserObject[] people;
        public UserObject[] Value => people;
    }
}