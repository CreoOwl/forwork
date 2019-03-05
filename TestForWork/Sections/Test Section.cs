using System.Configuration;


namespace TestLibraries
{
    public class Test : ConfigurationElement
    {

        [ConfigurationProperty("check_name", IsRequired = true)]
        public string Check_Name
        {
            get
            {
                return this["check_name"] as string;
            }
        }
        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get
            {
                return this["path"] as string;
            }
        }
    }
    [ConfigurationCollection(typeof(Test), AddItemName = "")]
    public class Tests
        : ConfigurationElementCollection
    {
        public Test this[int index]
        {
            get
            {
                return base.BaseGet(index) as Test;
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

        public new Test this[string responseString]
        {
            get { return (Test)BaseGet(responseString); }
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
            return new Test();
        }

        protected override object GetElementKey(System.Configuration.ConfigurationElement element)
        {
            return ((Test)element).Check_Name;
        }
    }

    public class Assemblies
        : ConfigurationSection
    {

        public static Assemblies GetConfig()
        {
            return (Assemblies)System.Configuration.ConfigurationManager.GetSection("Assemblies") ?? new Assemblies();
        }

        [System.Configuration.ConfigurationProperty("Tests")]
        [ConfigurationCollection(typeof(Tests), AddItemName = "Test")]
        public Tests Tests
        {
            get
            {
                object o = this["Tests"];
                return o as Tests;
            }
        }

    }
}
