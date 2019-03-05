using System.Configuration;

namespace TestLibraries
{
    public class Input : ConfigurationElement
    {
        
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get
            {
                return this["value"] as string;
            }
        }
        [ConfigurationProperty("check_name", IsRequired = true)]
        public string Check_Name
        {
            get
            {
                return this["check_name"] as string;
            }
        }
    }
    [ConfigurationCollection(typeof(Input), AddItemName = "Input")]
    public class Inputs
        : ConfigurationElementCollection
    {
        public Input this[int index]
        {
            get
            {
                return base.BaseGet(index) as Input;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new Input this[string responseString]
        {
            get { return (Input)BaseGet(responseString); }
            set
            {
                if (BaseGet(responseString) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(responseString)));
                }
                BaseAdd(value);
            }
        }

        protected override System.Configuration.ConfigurationElement CreateNewElement()
        {
            return new Input();
        }

        protected override object GetElementKey(System.Configuration.ConfigurationElement element)
        {
            return ((Input)element).Value;
        }
    }

    public class Data
        : ConfigurationSection
    {

        public static Data GetConfig()
        {
            return (Data)System.Configuration.ConfigurationManager.GetSection("Data") ?? new Data();
        }

        [System.Configuration.ConfigurationProperty("Inputs")]
        [ConfigurationCollection(typeof(Inputs), AddItemName = "Input")]
        public Inputs Inputs
        {
            get
            {
                object o = this["Inputs"];
                return o as Inputs;
            }
        }

    }
}
