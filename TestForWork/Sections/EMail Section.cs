using System.Configuration;

namespace TestLibraries
{
    public class MailService : ConfigurationSection
    {
        public static MailService GetConfig()
        {
            return (MailService)System.Configuration.ConfigurationManager.GetSection("MailService") ?? new MailService();
        }

        [ConfigurationProperty("smtp", IsRequired = true)]
        public string SMTP
        {
            get
            {
                return this["smtp"] as string;
            }
        }
        [ConfigurationProperty("port", IsRequired = true)]
        public string Port
        {
            get
            {
                return this["port"] as string;
            }
        }
    }

    public class LoginData : ConfigurationSection
    {
        public static LoginData GetConfig()
        {
            return (LoginData)System.Configuration.ConfigurationManager.GetSection("LoginData") ?? new LoginData();
        }

        [ConfigurationProperty("login", IsRequired = true)]
        public string Login
        {
            get
            {
                return this["login"] as string;
            }
        }
        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get
            {
                return this["password"] as string;
            }
        }
    }

    public class Addressee : ConfigurationElement
    {
        [ConfigurationProperty("mail", IsRequired = true)]
        public string Mail
        {
            get
            {
                return this["mail"] as string;
            }
        }
    }

    [ConfigurationCollection(typeof(Addressee), AddItemName = "Addressee")]
    public class Addressees
        : ConfigurationElementCollection
    {
        public Addressee this[int index]
        {
            get
            {
                return base.BaseGet(index) as Addressee;
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

        public new Addressee this[string responseString]
        {
            get { return (Addressee)BaseGet(responseString); }
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
            return new Addressee();
        }

        protected override object GetElementKey(System.Configuration.ConfigurationElement element)
        {
            return ((Addressee)element).Mail;
        }
    }

    public class EMail
        : ConfigurationSection
    {

        public static EMail GetConfig()
        {
            return (EMail)System.Configuration.ConfigurationManager.GetSection("EMail") ?? new EMail();
        }

        [System.Configuration.ConfigurationProperty("Addressees")]
        [ConfigurationCollection(typeof(Addressees), AddItemName = "Addressee")]
        public Addressees Addressees
        {
            get
            {
                object o = this["Addressees"];
                return o as Addressees;
            }
        }
    }
}
