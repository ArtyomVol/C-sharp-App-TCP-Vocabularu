using System;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ClientForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Text = "";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClientClose();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = listBox1.SelectedIndex;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string dataToServer = "Get terms";
            Client.sendDataToServer(dataToServer);
            string dataFromServer = Client.getDataFromServer();

            string[] rows = dataFromServer.Split('§');
            listBox1.Items.Clear();
            comboBox1.Items.Clear();
            for (int i = 0; i < rows.Length; i++)
            {
                listBox1.Items.Add(rows[i]);
                comboBox1.Items.Add(rows[i]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Client.sendDataToServer("Get term§" + comboBox1.Text);
            textBox2.Text = Client.getDataFromServer();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AddingTerm addingTerm = new AddingTerm();
            addingTerm.ShowDialog();
            button3.PerformClick();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DeleteTerm deleteTerm = new DeleteTerm();
            deleteTerm.UpdateComboBoxItems(comboBox1.Items, listBox1.SelectedIndex);
            deleteTerm.ShowDialog();
            button3.PerformClick();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ClientClose();
                Client.client = new TcpClient();
                int port;
                int.TryParse(textBox5.Text, out port);
                Client.client.Connect(textBox4.Text, port);
                Client.networkStream = Client.client.GetStream();
                button1.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
                comboBox1.Enabled = true;
                textBox2.Enabled = true;
                button3.PerformClick();
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка соединения с сервером");
                button1.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                comboBox1.Text = "";
                comboBox1.Enabled = false;
                textBox2.Text = "";
                textBox2.Enabled = false;
                listBox1.Items.Clear();
                comboBox1.Items.Clear();
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            EditTerm editTerm = new EditTerm();
            editTerm.UpdateComboBoxItems(comboBox1.Items, listBox1.SelectedIndex);
            editTerm.ShowDialog();
            button3.PerformClick();
        }

        private void ClientClose()
        {
            if (Client.networkStream != null && Client.networkStream.CanWrite)
            {
                try
                {
                    byte[] byteSend = Encoding.UTF8.GetBytes("Close");
                    Client.networkStream.Write(byteSend, 0, byteSend.Length);
                    Client.networkStream.Close();
                }
                catch (Exception)
                {
                    Client.client.Close();
                }
            }
            if (Client.client != null && Client.client.Connected)
            {
                Client.client.Close();
            }
        }
    }
}
