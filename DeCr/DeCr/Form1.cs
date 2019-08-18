using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace DeCr
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        BigInteger p;
        BigInteger IV;
        byte[] key;
        string Mes;
        byte[][] C1;
        byte[][] C2;
        byte[] forCr;
        byte[] ByteMes;
        string strC;
        
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
                OpenFileDialog pp = new OpenFileDialog();
                pp.Filter = "All Files (*txt.x*)|*.txt*";
                if (pp.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamReader sr = new StreamReader(pp.FileName, System.Text.Encoding.Unicode))
                        {          
                               Mes = sr.ReadToEnd();
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            
        }

        public byte[] MestoByte(string s)
        {
            if (s==null)
                throw new ArgumentException("Файл пуст");
            byte[] a = new byte[s.Length];
            if ((s.Length) % 34 != 0)
                throw new ArgumentException("Неверный текст для декодирования");
            for (int i = 0; i < s.Length; i++)
            {
                a[i] = (byte)s[i];
            }
            return a;
        }
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog pp = new SaveFileDialog();
            pp.Filter = "All Files (*txt.x*)|*.txt*";
            if (pp.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ByteMes = MestoByte(Mes);

                }
                catch(ArgumentException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }


                try
                {
                    key = ReadKey(textBox1.Text);
                }
                catch (Exception)
                {
                    MessageBox.Show("Недопустимый ключ");
                    return;
                }



                if (!BigInteger.TryParse(textBox2.Text, out p))
                {
                    MessageBox.Show("параметр Р - число");
                    return;
                }
                


                if (!ProvP(p))
                {
                    MessageBox.Show("недопустимое значение параметра P");
                    return;
                }


                using (Aes AesMes = Aes.Create())
                { 
                    try
                    {
                        IV=BigInteger.Parse(textBox3.Text);
                        AesMes.IV = IV.ToByteArray();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("недопустимый IV(вектор инициализации)");
                        return;
                    }
                    AesMes.Key = CorrectKey(key);
                    C1 = toBlocks(ByteMes);
                    C2 = toNewBlocks(C1);
                    forCr = DecryptYY.BytetoOneArray(C2);
                   
                    try
                    {
                        strC = DecryptYY.DecryptStringFromBytes_Aes(forCr, AesMes.Key, AesMes.IV);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("файл поврежден");
                        return;
                    }
                }
                
                using (StreamWriter sr = new StreamWriter(pp.FileName, false, System.Text.Encoding.Default))
                {
                    sr.Write(strC);
                }
                
            }
        }
        public byte[] ReadKey(string s)
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
            if (k == 0)
                throw new Exception();
            return res;
        }
        public bool ProvP(BigInteger p)
        {
            byte[] k = p.ToByteArray();
            if (k.Length != 17 || k[16] != 1)
                return false;
            else
                return true;
        }

        public static byte[] CorrectKey(byte[] key)
        {
            byte[] res = new byte[32];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    res[j + i * 4] = key[j];
                }
            }
            return res;
        }

        public byte [][] toBlocks(byte[] s)
        {

            byte[][] E = new byte[s.Length / 34][];
            for (int i = 0; i < s.Length / 34; i++)
            {
                E[i] = new byte[34];
                for (int j = 0; j < 34; j++)
                {
                    E[i][j] = s[i * 34 + j];
                }
            }
            return E;
        }
        public byte[][] toNewBlocks (byte[][] s)
        {
            byte[][] res = new byte[s.Length][];
            for (int i = 0; i < s.Length; i++)
            {
                byte[] ch = new byte[s[i][33]];
                for (int j = 0; j < ch.Length; j++)
                {
                    ch[j] = s[i][j];
                }
                BigInteger t = new BigInteger(ch);
                BigInteger newC = BigInteger.Remainder(t, p);
                res[i] = newC.ToByteArray();
            }
            return res;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
