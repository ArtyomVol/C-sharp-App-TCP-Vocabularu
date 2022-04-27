using System;
using System.Windows.Forms;

namespace ClientForms
{
    public partial class DeleteTerm : Form
    {
        public DeleteTerm()
        {
            InitializeComponent();
        }

        private void DeleteTerm_Shown(object sender, EventArgs e)
        {
            label2.Text = "";
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text.Length > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Вы действительно хотите удалить '"
                    + comboBox1.Text + "'?", "Предупреждение", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Client.sendDataToServer("Delete term§" + comboBox1.Text);
                    label2.Text = Client.getDataFromServer();
                    if (comboBox1.Items.Contains(comboBox1.Text))
                    {
                        comboBox1.Items.Remove(comboBox1.Text);
                    }
                }
            }
            else
            {
                label2.Text = "Ошибка: Вы не написали название термина.";
            }
        }
    }
}
