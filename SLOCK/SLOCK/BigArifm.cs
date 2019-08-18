using System;
using System.Numerics;

namespace SLOCK
{
    class BigArifm
    {
        static Random rnd = new Random();
        public static BigInteger Gnr1()
        {
            byte[] y = new byte[17];
            y[16] = 1;
            for (int i = 0; i < 16; i++)
            {
                y[i] = (byte)rnd.Next(0, 255);
            }
            BigInteger a = new BigInteger(y);
            return a;
        }
       
        public static BigInteger Gnr2(BigInteger r1)
        {
            byte[] maxVb = new byte[17];
            maxVb[16] = 1;
            for (int i = 0; i < 16; i++)
            {
                maxVb[i] = 255;
            }
            byte[] minVb = new byte[17];
            minVb[16] = 1;
           
            BigInteger MaxV = new BigInteger(maxVb);
            BigInteger minV = new BigInteger(minVb);
            BigInteger r2 = Gnr1();
            while (true)
            {
                if (BigInteger.GreatestCommonDivisor(r1, r2) != 1)
                {
                    if (BigInteger.Compare(r2, MaxV) < 0)
                    {
                        r2++;
                    }
                    else
                    {
                        r2 = minV;
                    }
                }
                else
                    return r2;
            }

        }
        /// <summary>
        /// поиск обратного элемента в кольце по модулю
        /// </summary>
        /// <param name="modn"> модуль </param>
        /// <param name="a">число к которому ищем обратный</param>
        /// <returns></returns>
        public static BigInteger Inverse(BigInteger modn, BigInteger a)
        {
            BigInteger z;
            BigInteger b = modn, x = BigInteger.Zero, d = BigInteger.One;
            while (a.CompareTo(BigInteger.Zero) == 1)
            {
                BigInteger q = BigInteger.DivRem(b, a, out z);
                BigInteger k = a;
                a = z;
                b = k;
                k = d;
                d = BigInteger.Subtract(x, BigInteger.Multiply(q, d));
                x = k;
            }
            x = BigInteger.Remainder(x, modn);
            if (x.CompareTo(BigInteger.Zero) == -1)

            { x = BigInteger.Remainder(BigInteger.Add(x, modn), modn); }
            return x;
        }

    }
}
