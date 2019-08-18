using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace SLOCK
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string TrueM;
        string FalseM;
        byte[][] EnM;
        byte[][] EnN;
        byte[][] C;
        BigInteger p1;
        BigInteger p2;
        byte[] IV_M;
        byte[] IV_N;
        BigInteger koef1;
        BigInteger koef2;
        BigInteger koef3;
        string StrC;
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog p = new OpenFileDialog();
            p.Filter = "All Files (*txt.x*)|*.txt*";
            if (p.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamReader sr = new StreamReader(p.FileName, System.Text.Encoding.Default))
                    {
                        if (radioButton1.Checked)
                        {
                            TrueM = sr.ReadToEnd(); 
                            for (int i = 0; i < TrueM.Length % 16; i++)
                                TrueM += "*";
                        }
                        else
                        {
                            FalseM = sr.ReadToEnd();
                            for (int i = 0; i < FalseM.Length % 16; i++)
                                FalseM += "*";
                        }
                    }
                    
                  
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void AddStr()
        {
            if (TrueM.Length > FalseM.Length)
            {
                int o = FalseM.Length;
                for (int i = o; i < TrueM.Length; i++)
                {
                    FalseM += " ";
                }
            }
            else 
            {
                int o = TrueM.Length;
                for (int i = o; i < FalseM.Length; i++)
                {
                    TrueM += " ";
                }
            }
        }
        public byte[] ReadKey (string s)
        {
            string[] str = s.Split(' ');
            byte[] res = new byte[4];
            if (str.Length != 4)
                throw new Exception();
            int k = 0;
            for (int i = 0; i < str.Length; i++)
            {
                res[i] = Convert.ToByte(str[i]);
                k += res[i];
            }
            if (k==0)
                throw new Exception();
            return res;
        }
        private void button1_Click(object sender, EventArgs e)
        {
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_MouseMove(object sender, MouseEventArgs e)
        {
            label3.Visible=true;
        }

        private void textBox2_MouseLeave(object sender, EventArgs e)
        {
            label3.Visible = false;
        }

        private void textBox3_MouseMove(object sender, MouseEventArgs e)
        {
            label9.Visible = true;
        }

        private void textBox3_MouseLeave(object sender, EventArgs e)
        {
            label9.Visible = false;
        }

        private void label7_Click(object sender, EventArgs e)
        {
             
        }
       

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SaveFileDialog dd1 = new SaveFileDialog();
            dd1.Filter = "All Files (*txt.x*)|*.txt*";
            if (dd1.ShowDialog() == DialogResult.OK)
            {
                
                if (TrueM == null || FalseM == null)
                {
                    MessageBox.Show("Одно из сообщений пусто");
                    return;
                }
                if (textBox2.Text == "" || textBox3.Text == "")
                {
                    MessageBox.Show("один из ключей не введен");
                    return;
                }
                AddStr();
                if (TrueM.Length!=FalseM.Length)
                {
                    MessageBox.Show("одно из сообщений содержит недопустимые символы");
                    return;
                }
                byte[] key1;
                byte[] key2;
                try
                {
                    key1 = ReadKey(textBox2.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Недопустимый секретный ключ");
                    return;
                }
                try
                {
                    key2 = ReadKey(textBox3.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Недопустимый фиктивный ключ");
                    return;
                }
                key1 = BlockCrypt.CorrectKey(key1);
                key2 = BlockCrypt.CorrectKey(key2);
                p1 = BigArifm.Gnr1();
                p2 = BigArifm.Gnr2(p1);
                textBox4.Text = p1.ToString();
                textBox5.Text = p2.ToString();
                
                using (Aes AesM = Aes.Create())
                {
                    AesM.Key = key1;
                    IV_M = AesM.IV;
                    byte[] h1 = BlockCrypt.EncryptStringToBytes_Aes(TrueM, AesM.Key, AesM.IV);
                    EnM = new byte[h1.Length / 16][];
                    for (int i = 0; i < h1.Length / 16; i++)
                    {
                        EnM[i] = new byte[17];
                        for (int j = 0; j < 16; j++)
                        {
                            EnM[i][j] = h1[i * 16 + j];
                        }
                    }
                    BigInteger IV1 = new BigInteger(IV_M);
                    textBox6.Text = IV1.ToString();
                   
                }
                using (Aes AesN = Aes.Create())
                {
                    AesN.Key = key2;
                    IV_N = AesN.IV;
                    byte[] h2 = BlockCrypt.EncryptStringToBytes_Aes(FalseM, AesN.Key, AesN.IV);
                    EnN = new byte[h2.Length / 16][];
                    for (int i = 0; i < h2.Length / 16; i++)
                    {
                        EnN[i] = new byte[17];
                        for (int j = 0; j < 16; j++)
                        {
                            EnN[i][j] = h2[i * 16 + j];
                        }
                    }
                    BigInteger IV2 = new BigInteger(IV_N);
                    textBox7.Text = IV2.ToString();
                   
                   
                }
                
                koef1 = BigInteger.Multiply(p2, BigArifm.Inverse(p1, p2));
                koef2 = BigInteger.Multiply(p1, BigArifm.Inverse(p2, p1));
                koef3 = BigInteger.Multiply(p1, p2);
                try
                {
                    C = BlockCrypt.CryptC(koef1, koef2, koef3, EnM, EnN);
                }
                catch (Exception)
                {
                    MessageBox.Show("одно из сообщений содержит недопустимые символы");
                    return;
                }
                StrC = BlockCrypt.BytetoSymb(C);
               
                using (StreamWriter sr = new StreamWriter(dd1.FileName,false, System.Text.Encoding.Unicode))
                {
                    sr.Write(StrC);
                }
            
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
