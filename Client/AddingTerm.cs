using System;
using System.Windows.Forms;

namespace ClientForms
{
    public partial class AddingTerm : Form
    {
        public AddingTerm()
        {
            InitializeComponent();
        }

        private void AddingTerm_Load(object sender, EventArgs e)
        {
            label3.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                if (textBox2.Text.Length > 0)
                {
                    Client.sendDataToServer("Add term§" + textBox1.Text + "§" + textBox2.Text);
                    label3.Text = Client.getDataFromServer();
                }
                else
                {
                    label3.Text = "Ошибка: Вы не написали определение термина";
                }
            }
            else
            {
                label3.Text = "Ошибка: Вы не написали название термина";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            label3.Text = "";
        }

        private void AddingTerm_Shown(object sender, EventArgs e)
        {
            label3.Text = "";
        }
    }
}
