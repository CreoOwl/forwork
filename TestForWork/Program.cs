using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Interface;
using Newtonsoft.Json;
using MailKit.Net.Smtp;
using MimeKit;

namespace TestLibraries
{
    class CResult
    {
        public Dictionary<string, List<Tuple<string, bool>>> Result { get; set; }
        public CResult()
        {
            Result = new Dictionary<string, List<Tuple<string, bool>>>();
        }
    }
     
    class Program
    {
        static void GetAllTests(Dictionary<string, ICheck> tests)
        {
            var config = Assemblies.GetConfig();
            foreach (Test item in config.Tests)
            {
                if (File.Exists(item.Path))
                {
                    Assembly assembly = Assembly.LoadFrom(item.Path);
                    var types = assembly.GetTypes();
                    var checkType = (from type in types
                                     where type.GetInterface("ICheck") != null && !type.IsAbstract
                                     select type).ToList();
                    if (checkType.Count > 0)
                    {
                        tests.Add(item.Check_Name, (ICheck)Activator.CreateInstance(checkType[0]));
                        Console.WriteLine($"Проверка {item.Check_Name} загружена.");
                    }
                }
                else
                    Console.WriteLine($"Библиотека {item.Path} отсутствует!");
            }
        }

        static void CheckEverything(Dictionary<string, ICheck> tests, CResult result)
        {
            var inputs = Data.GetConfig();
            foreach (Input item in inputs.Inputs)
            {
                if (tests.ContainsKey(item.Check_Name))
                {
                    if (result.Result.ContainsKey(item.Check_Name))
                    {
                        result.Result[item.Check_Name].Add(new Tuple<string, bool>(item.Value, tests[item.Check_Name].Check(item.Value)));
                    }
                    else
                    {
                        result.Result.Add(item.Check_Name, new List<Tuple<string, bool>> { new Tuple<string, bool>(item.Value, tests[item.Check_Name].Check(item.Value)) });
                    }
                }
                else
                    Console.WriteLine("Требуемая проверка отсутствует! Проверьте app.config.");
            }
        }

        static void FileWrite(string path, string text)
        {
            using (StreamWriter write = File.CreateText(path))
            {
                write.WriteLine(text);
            }
        }

        static MimeMessage FormMessage(string from, string to, string attachment)
        {
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress(from));
            message.To.Add(new MailboxAddress(to));
            message.Subject = "Проверки";
            var builder = new BodyBuilder();
            builder.TextBody = "В прикрепленном файле результаты недавних проверок.";
            builder.Attachments.Add(attachment);
            message.Body = builder.ToMessageBody();
            message.Body = builder.ToMessageBody();
            return message;
        }

        static bool SendMail(string smtp, int port, string from, string to, string password, string attachment)
        {
            MimeMessage message = FormMessage(from, to, attachment);
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(smtp, port, true);
                    client.Authenticate(from, password);
                    client.Send(message);
                    client.Disconnect(true);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка!\n" + e.Message);
                    return false;
                }
            }
            return true;
        }

        static bool Send(string attachment)
        {
            var mailservice = MailService.GetConfig();
            var logindata = LoginData.GetConfig();
            if (mailservice.SMTP != null && mailservice.SMTP != null && logindata.Login != null && logindata.Password != null)
            {
                var mails = EMail.GetConfig();
                if (mails.Addressees.Count == 0)
                {
                    Console.WriteLine("Адресаты не указаны.");
                    return false;
                }
                foreach (Addressee person in mails.Addressees)
                {
                    if (!SendMail(mailservice.SMTP, int.Parse(mailservice.Port), logindata.Login, person.Mail, logindata.Password, attachment))
                        return false;
                }
                return true;
            }
            else
            {
                Console.WriteLine("Проверьте данные для отправки писем в app.config!");
                return false;
            }
        }

        static void Main(string[] args)
        {
            Dictionary<string, ICheck> tests = new Dictionary<string, ICheck>();
            Console.WriteLine("Загрузка проверок...");
            GetAllTests(tests);
            CResult result = new CResult();
            Console.WriteLine("Прохождение проверок...");
            CheckEverything(tests, result);
            string result_file_name = "result.json";
            string result_text = JsonConvert.SerializeObject(result, Formatting.Indented);
            FileWrite(result_file_name, result_text);
            Console.WriteLine($"Завершено. Результаты хранятся в файле {result_file_name}.");
            if (args.Length > 0 && args[0] == "show")
            {
                Console.WriteLine($"Содержимое {result_file_name}:\n {result_text}");
            }
            else
            {
                Console.WriteLine("Отправка писем...");
                if (!Send(result_file_name))
                {
                    Console.WriteLine($"Письма не отправлены. Содержимое {result_file_name}:");
                    Console.WriteLine(result_text);
                }
                else
                    Console.WriteLine("Письма отправлены!");
            }
            Console.ReadKey();
        }
    }
}
