using System;
using System.Windows.Forms;

namespace ClientForms
{
    public partial class EditTerm : Form
    {
        public EditTerm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            label3.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text.Length > 0)
            {
                if (textBox1.Text.Length > 0)
                {
                    Client.sendDataToServer("Edit term§" + comboBox1.Text + "§" + textBox1.Text);
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

        private void EditTerm_Load(object sender, EventArgs e)
        {
            button2.PerformClick();
        }
        public void UpdateComboBoxItems(ComboBox.ObjectCollection items, int selectedIndex)
        {
            comboBox1.Items.Clear();
            foreach (string item in items)
            {
                comboBox1.Items.Add(item);
            }
            comboBox1.SelectedIndex = selectedIndex;
        }
    }
}
