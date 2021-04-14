using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Vigenere
{
    class Helper
    {

        public static bool isEnglishLetter(char c) => (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я');


        public static int indexOfLetter(char c) => char.IsUpper(c) ? 1040 : 1072;

        public static int[] countFreq(string text)
        {
            int[] observedFreq = new int[32];
            foreach (char c in text)
                if (isEnglishLetter(c))
                    observedFreq[c - indexOfLetter(c)]++;

            return observedFreq;
        }
    }


    class Caesar : Helper
    {
        static double chiSquared(int[] observedFreq, double[] expectedFreq)
        {
            double chi = 0;
            int sum = 0;
            foreach (int freq in observedFreq)
                sum += freq;
            for (int i = 0; i < 32; i++)
                chi += Math.Pow(((1.0 * observedFreq[i] / sum) - (expectedFreq[i] / 100.0)), 2.0) / (expectedFreq[i] / 100.0);

            return chi;
        }

        public static string caesarDecrypt(string cipherText)
        {
            double[] expectedFreq = { 7.5, 1.7, 4.6, 1.6, 3.0, 8.8, 0.8, 1.9, 7.5, 1.2, 3.4, 4.2, 3.2, 6.4, 10.9, 2.8, 4.8, 5.4, 6.4, 2.6, 0.2, 1.1, 0.5, 1.5, 0.7, 0.4, 1.7, 1.9, 1.7, 0.4, 0.7, 2.2 };

            int[] observedFreq = countFreq(cipherText);
            double[] chiStatistics = new double[32];

            for (int i = 0; i < 32; i++)
                chiStatistics[i] = chiSquared(countFreq(caesarEncrypt(cipherText, i)), expectedFreq);

            double min = chiStatistics[0];
            int index = 0;
            for (int i = 1; i < chiStatistics.Length; i++)
            {
                if (min > chiStatistics[i])
                {
                    min = chiStatistics[i];
                    index = i;
                }
            }
            return caesarEncrypt(cipherText, index);
        }

        public static string caesarEncrypt(string plainText, int factor)
        {
            StringBuilder cipherText = new StringBuilder();
            foreach (char c in plainText)
            {
                if (!isEnglishLetter(c))
                    cipherText.Append(c);
                else
                    cipherText.Append((char)((c + factor - indexOfLetter(c)) % 32 + indexOfLetter(c)));
            }
            return cipherText.ToString();
        }
    }


    class Vigenere : Helper
    {

        static public string vigenereEncrypt(string plainText, string key)
        {
            string[] cipherParts = split(plainText, key.Length);
            for (int i = 0; i < key.Length; i++)
                cipherParts[i] = Caesar.caesarEncrypt(cipherParts[i], key[i] - indexOfLetter(key[i]));

            return mergeParts(cipherParts);
        }

        static double indexOfCoincidence(string column)
        {
            int[] freq = countFreq(column);
            int total = 0;
            int sum = 0;
            for (int i = 0; i < 32; i++)
            {
                sum += freq[i] * (freq[i] - 1);
                total += freq[i];
            }
            return (double)sum / ((total * (total - 1)) / 32);
        }

        static int estimateKeyLength(string cipherText)
        {
            double[] delta = new double[11];
            int len = 1;
            for (int keyLength = 1; keyLength <= 10; keyLength++)
            {
                string[] parts = split(cipherText, keyLength);
                double sum = 0;
                for (int k = 0; k < keyLength; k++)
                    sum += indexOfCoincidence(parts[k]);
                delta[keyLength] = (double)sum / keyLength;
            }
            for (int keyLength = 1; keyLength <= 10; keyLength++)
            {
                if (Math.Abs(delta[keyLength] - 1.73) <= 0.25)
                {
                    len = keyLength;
                    break;
                }
            }
            return len;
        }

        static string[] split(string plainText, int keyLength)
        {
            StringBuilder[] parts = new StringBuilder[keyLength];
            for (int i = 0; i < keyLength; i++)
                parts[i] = new StringBuilder();
            int counter = 0;
            int index = 0;
            while (counter < plainText.Length)
            {
                char c = plainText[counter];
                parts[index % keyLength].Append(plainText[counter]);
                if (isEnglishLetter(c))
                    index++;
                counter++;
            }
            string[] partsStr = new string[keyLength];
            for (int i = 0; i < keyLength; i++)
                partsStr[i] = parts[i].ToString();

            return partsStr;
        }

        static string mergeParts(string[] parts)
        {
            StringBuilder plainText = new StringBuilder();
            int empty = 0;
            int index = 0;

            while (true)
            {
                plainText.Append(parts[index % parts.Length][0]);
                char c = parts[index % parts.Length][0];
                parts[index % parts.Length] = parts[index % parts.Length].Substring(1);

                if (parts[index % parts.Length].Equals("")) empty++;
                if (empty == parts.Length) break;

                if (isEnglishLetter(c))
                {
                    while (true)
                    {
                        index++;
                        if (!parts[index % parts.Length].Equals("")) break;
                    }
                }
            }
            return plainText.ToString();
        }

        static string alphabet = "АБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";

        static public string vigenereDecrypt(string cipherText, out int KeyLength)
        {

            int k = estimateKeyLength(cipherText);
            KeyLength = k;
            string[] parts = split(cipherText, k);

            List<int[]> G = new List<int[]>(k);

            for (int i = 0; i < parts.Length; i++)
            {
                int[] temp = new int[32];
                for (int j = 0; j < parts[i].Length; j++)
                {
                    int index = GetIndex(parts[i][j].ToString());
                    if (index == -1)
                        continue;
                    temp[index]++;
                }
                G.Add(temp);
            }

            List<double[]> Gf = new List<double[]>(k);

            string s = "";
            for (int i = 0; i < G.Count; i++)
            {
                for (int j = 0; j < G[i].Length; j++)
                {
                    s += GetLetter((double)G[i][j] / parts[i].Length) + " ";
                }
                Console.WriteLine(s);
                s = string.Empty;
            }



            for (int i = 0; i < parts.Length; i++)
                parts[i] = Caesar.caesarDecrypt(parts[i]);

            return mergeParts(parts);
        }

        static int GetIndex(string letter)
        {
            return alphabet.IndexOf(letter);
        }

        static string GetLetter(double freq)
        {
            string ret = "";

            List<double> fr = new List<double>
            {
                /*а*/   0.075,
                /*б*/   0.017,
                /*в*/   0.046,
                /*г*/   0.016,
                /*д*/   0.030,
                /*е*/   0.088,
                /*ж*/   0.008,
                /*з*/   0.019,
                /*и*/   0.075, 
                /*й*/   0.012,
                /*к*/   0.034,
                /*л*/   0.042,
                /*м*/   0.032,
                /*н*/   0.064,
                /*о*/   0.109,
                /*п*/   0.028,
                /*р*/   0.048,
                /*с*/   0.054,
                /*т*/   0.064,
                /*у*/   0.026,
                /*ф*/   0.002,
                /*х*/   0.011,
                /*ц*/   0.005,
                /*ч*/   0.015,
                /*ш*/   0.007,
                /*щ*/   0.004,
                /*ъ*/   0.017,
                /*ы*/   0.019,
                /*ь*/   0.017,
                /*э*/   0.004,
                /*ю*/   0.007,
                /*я*/   0.022
            };

            double closest = fr.OrderBy(item => Math.Abs(freq - item)).First();

            ret = ((char)(1040 + fr.FindIndex(e => e == closest))).ToString();

            return ret;
        }
    }
}