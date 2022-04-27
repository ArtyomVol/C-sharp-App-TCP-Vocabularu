using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ClientForms
{
    static class Client
    {
        public static TcpClient client = null;
        public static NetworkStream networkStream = null;

        public static void sendDataToServer(string data)
        {
            try
            {
                byte[] byteSend = Encoding.UTF8.GetBytes(data);
                networkStream.Write(byteSend, 0, byteSend.Length);
                networkStream.Flush();
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка передачи данных серверу");
            }
        }

        public static string getDataFromServer()
        {
            try
            {
                byte[] byteRec = new byte[client.ReceiveBufferSize];
                int len = networkStream.Read(byteRec, 0, client.ReceiveBufferSize);
                string dataFromServer = Encoding.UTF8.GetString(byteRec, 0, len);
                return dataFromServer;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка получения данных от сервера");
                return "";
            }
        }
    }
}
