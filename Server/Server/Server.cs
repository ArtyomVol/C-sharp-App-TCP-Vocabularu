using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;

namespace Server
{
    static class Server
    {
        private static string defaultGatevay = "0.0.0.0";
        private const int PORT = 10000;
        private const string XML_ADDRESS =
            "../../inform.xml";
        private static XmlDocument xmlDocument = new XmlDocument();
        private static XmlElement xmlElement;

        static void Main()
        {
            try
            {
                Console.WriteLine(" Сервер запущен.");
                Console.WriteLine(" Время запуска: {0}", DateTime.Now);
                TcpListener tcpListener = new TcpListener(IPAddress.Parse(defaultGatevay), PORT);
                tcpListener.Start();
                xmlDocument.Load(XML_ADDRESS);
                xmlElement = xmlDocument.DocumentElement;

                Console.WriteLine(" Сервер ждёт подключений:");

                List<Thread> threads = new List<Thread>();

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    DelThreads(threads);
                    Thread thread = new Thread(() => ClientThread(tcpClient));
                    thread.Start();
                    threads.Add(thread);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(" Error: " + exception.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }

        static void DelThreads(List<Thread> threads)
        {
            List<Thread> threadsToDel = new List<Thread>();
            for (int i = 0; i < threads.Count; i++)
            {
                if (!threads[i].IsAlive)
                {
                    threadsToDel.Add(threads[i]);
                    threads[i].Abort();
                }
            }
            foreach (var thread in threadsToDel)
            {
                threads.Remove(thread);
            }
        }

        static void ClientThread(TcpClient tcpClient)
        {
            try
            {
                NetworkStream networkStream = tcpClient.GetStream();
                Console.WriteLine(" {0} Подключён клиент с IP {1}, на поток с ID {2}",
                    DateTime.Now, ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address,
                    Thread.CurrentThread.ManagedThreadId);
                String data = "";
                byte[] reciveBytes;
                int lenByteReccived;
                String sendData;
                byte[] sendByte;
                while (data != "Close")
                {
                    reciveBytes = new byte[tcpClient.ReceiveBufferSize];
                    lenByteReccived = networkStream.Read(reciveBytes, 0, tcpClient.ReceiveBufferSize);
                    data = Encoding.UTF8.GetString(reciveBytes, 0, lenByteReccived);
                    networkStream.Flush();
                    sendData = DataProcessing(data);
                    sendByte = Encoding.UTF8.GetBytes(sendData);
                    networkStream.Write(sendByte, 0, sendByte.Length);
                }
                Console.WriteLine(" {0} Отключён клиент с IP {1}, на потоке с ID {2}",
                    DateTime.Now, ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address,
                    Thread.CurrentThread.ManagedThreadId);
                networkStream.Close();
                tcpClient.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Error: {0}", ex.ToString());
            }
        }

        static string DataProcessing(String data)
        {
            if (data == "Get terms")
            {
                Console.WriteLine(" {0} Клиент на потоке с ID {1} запросил список терминов",
                    DateTime.Now, Thread.CurrentThread.ManagedThreadId);
                return GetTerms();
            }
            else
            {
                string[] rows = data.Split('§');
                if (rows[0] == "Get term" && rows.Length == 2)
                {
                    Console.WriteLine(" {0} Клиент на потоке с ID {1} запросил определение термина '{2}'",
                        DateTime.Now, Thread.CurrentThread.ManagedThreadId, rows[1]);
                    return GetTerm(rows[1]);
                }
                else if (rows[0] == "Add term" && rows.Length == 3)
                {
                    Console.WriteLine(" {0} Клиент на потоке с ID {1} запросил добавление термина '{2}'," +
                        " с определением {3}",
                        DateTime.Now, Thread.CurrentThread.ManagedThreadId, rows[1], rows[2]);
                    return AddTerm(rows[1], rows[2]);
                }
                else if (rows[0] == "Delete term" && rows.Length == 2)
                {
                    Console.WriteLine(" {0} Клиент на потоке с ID {1} запросил удаление термина '{2}'",
                        DateTime.Now, Thread.CurrentThread.ManagedThreadId, rows[1]);
                    return DeleteTerm(rows[1]);
                }
                else if (rows[0] == "Edit term" && rows.Length == 3)
                {
                    Console.WriteLine(" {0} Клиент на потоке с ID {1} запросил изменение определения термина '{2}' на {3}",
                        DateTime.Now, Thread.CurrentThread.ManagedThreadId, rows[1], rows[2]);
                    return EditTerm(rows[1], rows[2]);
                }
                else if (data != "Close")
                {
                    Console.WriteLine(" {0} Клиент на потоке с ID {1} ввёл неизвестную комманду '{2}'",
                        DateTime.Now, Thread.CurrentThread.ManagedThreadId, data);
                }
            }
            return "Unknown command";
        }

        static string GetTerms()
        {
            xmlDocument.Load(XML_ADDRESS);
            xmlElement = xmlDocument.DocumentElement;
            string terms = "";
            foreach (XmlNode xmlNode in xmlElement)
            {
                if (xmlNode.Attributes.Count > 0)
                {
                    XmlNode attribute = xmlNode.Attributes.GetNamedItem("subject");
                    if (attribute != null)
                    {
                        terms += attribute.Value + "§";
                    }
                }
            }
            if (terms.Length > 0)
            {
                terms = terms.Substring(0, terms.Length - 1);
            }
            return terms;
        }

        static string GetTerm(string subject)
        {
            xmlDocument.Load(XML_ADDRESS);
            xmlElement = xmlDocument.DocumentElement;
            foreach (XmlNode xmlNode in xmlElement)
            {
                if (xmlNode.Attributes.Count > 0)
                {
                    XmlNode attribute = xmlNode.Attributes.GetNamedItem("subject");
                    if (attribute.Value == subject)
                    {
                        foreach (XmlNode childNode in xmlNode.ChildNodes)
                        {
                            if (childNode.Name == "definition")
                            {
                                return childNode.InnerText;
                            }
                        }
                    }
                }
            }
            return "Термин '" + subject + "' не существует.";
        }

        static string AddTerm(string subject, string definition)
        {
            xmlDocument.Load(XML_ADDRESS);
            xmlElement = xmlDocument.DocumentElement;
            foreach (XmlNode xmlNode in xmlElement)
            {
                if (xmlNode.Attributes.Count > 0)
                {
                    XmlNode attribute = xmlNode.Attributes.GetNamedItem("subject");
                    if (attribute.Value == subject)
                    {
                        return "Термин '" + subject + "' уже существует.";
                    }
                }
            }
            xmlDocument = new XmlDocument();
            xmlDocument.Load(XML_ADDRESS);
            xmlElement = xmlDocument.DocumentElement;

            XmlElement term = xmlDocument.CreateElement("term");
            XmlAttribute attributeSubject = xmlDocument.CreateAttribute("subject");
            XmlElement termDefinition = xmlDocument.CreateElement("definition");
            XmlText subjectText = xmlDocument.CreateTextNode(subject);
            XmlText termDefinitionText = xmlDocument.CreateTextNode(definition);
            attributeSubject.AppendChild(subjectText);
            termDefinition.AppendChild(termDefinitionText);
            term.Attributes.Append(attributeSubject);
            term.AppendChild(termDefinition);
            xmlElement.AppendChild(term);
            xmlDocument.Save(XML_ADDRESS);
            return "Термин '" + subject + "' успешно добавлен.";
        }

        static string DeleteTerm(string subject)
        {
            xmlDocument.Load(XML_ADDRESS);
            xmlElement = xmlDocument.DocumentElement;
            foreach (XmlNode xmlNode in xmlElement)
            {
                if (xmlNode.Attributes.Count > 0)
                {
                    XmlNode attribute = xmlNode.Attributes.GetNamedItem("subject");
                    if (attribute.Value == subject)
                    {
                        xmlElement.RemoveChild(xmlNode);
                        xmlDocument.Save(XML_ADDRESS);
                        return "Термин '" + subject + "' успешно удалён.";
                    }
                }
            }
            return "Термин '" + subject + "' не существует.";
        }

        static string EditTerm(string subject, string definition)
        {
            xmlDocument.Load(XML_ADDRESS);
            xmlElement = xmlDocument.DocumentElement;
            foreach (XmlNode xmlNode in xmlElement)
            {
                if (xmlNode.Attributes.Count > 0)
                {
                    XmlNode attribute = xmlNode.Attributes.GetNamedItem("subject");
                    if (attribute.Value == subject)
                    {
                        foreach (XmlNode childNode in xmlNode.ChildNodes)
                        {
                            if (childNode.Name == "definition")
                            {
                                childNode.InnerText = definition;
                                xmlDocument.Save(XML_ADDRESS);
                                return "Определение термина '" + subject + "' успешно изменено.";
                            }
                        }
                    }
                }
            }
            return "Термин '" + subject + "' не существует.";
        }
    }
}
