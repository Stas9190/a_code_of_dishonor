using System;
using System.Windows.Forms;

namespace Vigenere
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label2.Text = string.Empty;
        }

        private void CipherButton_Click(object sender, EventArgs e)
        {
            if (textBox_input.Text == string.Empty || textBox_key.Text == string.Empty)
            {
                MessageBox.Show("Необходимо заполнить поле с исходным текстом и поле ключа!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            textBox_output.Text = string.Empty;

            string input = textBox_input.Text;
            string key = textBox_key.Text;

            string result = Vigenere.vigenereEncrypt(input, key);

            textBox_output.Text = result;

            label2.Text = string.Empty;
        }

        private void DecipherButton_Click(object sender, EventArgs e)
        {
            if (textBox_input.Text == string.Empty)
            {
                MessageBox.Show("Необходимо заполнить поле с исходным текстом!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            textBox_output.Text = string.Empty;

            string input = textBox_input.Text.Replace("\r\n", string.Empty).Replace(" ", string.Empty);

            string result = Vigenere.vigenereDecrypt(input, out int KeyLength);

            label2.Text = $"Длина ключа: {KeyLength}";

            textBox_output.Text = result;
        }
    }
}
