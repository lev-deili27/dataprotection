using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
 
namespace skitala
{
    class Program
    {       
        private const int sizeOfBlock = 128; //в DES размер блока 64 бит, но поскольку в unicode символ в два раза длинее, то увеличим блок тоже в два раза
            private const int sizeOfChar = 16; //размер одного символа (in Unicode 16 bit)

            private const int shiftKey = 2; //сдвиг ключа 

            private const int quantityOfRounds = 16; //количество раундов

            static string[]  Blocks; //сами блоки в двоичном формате
            
        static void Main(string[] args)
        {
            

                Console.WriteLine("Введите ключевое слово:");
            string keyword = Console.ReadLine();

            Encrypt(keyword);
        

            Console.WriteLine();


        }

private static string StringToRightLength(string input)
        {
            while (((input.Length * sizeOfChar) % sizeOfBlock) != 0)
                input += "#"; //Размер увеличивается с помощью добавления к исходной строке #

            return input;
        }

        private static void CutStringIntoBlocks(string input) //Метод, разбивающий строку в обычном (символьном) формате на блоки.
        {
            Blocks = new string[(input.Length * sizeOfChar) / sizeOfBlock];

            int lengthOfBlock = input.Length / Blocks.Length;

            for (int i = 0; i < Blocks.Length; i++)
            {
            Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
            Blocks[i] = StringToBinaryFormat(Blocks[i]);
            }
        }

        private static void CutBinaryStringIntoBlocks(string input) //Метод, разбивающий строку в двоичном формате на блоки.
        {
            Blocks = new string[input.Length / sizeOfBlock];

            int lengthOfBlock = input.Length / Blocks.Length;

            for (int i = 0; i < Blocks.Length; i++)
                Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
        }

        private static string StringToBinaryFormat(string input) //Метод, переводящий строку в двоичный формат.
        {
            string output = "";

            for (int i = 0; i < input.Length; i++)
            {
                 string char_binary = Convert.ToString(input[i], 2);

                 while (char_binary.Length < sizeOfChar)
                    char_binary = "0" + char_binary;

                 output += char_binary;
            }

             return output;
        }

        private static string CorrectKeyWord(string input, int lengthKey)//Метод, доводящий длину ключа до нужной длины.
        {
            if (input.Length > lengthKey)
                input = input.Substring(0, lengthKey);
            else
            while (input.Length < lengthKey)
                input = "0" + input;

            return input;
        }

        private static string EncodeDES_One_Round(string input, string key)//Один раунд шифрования алгоритмом DES.
        {
            string L = input.Substring(0, input.Length / 2);
            string R = input.Substring(input.Length / 2, input.Length / 2);

            return (R + XOR(L, f(R, key)));
        }

        private static string DecodeDES_One_Round(string input, string key)//Один раунд расшифровки алгоритмом DES.
        {
            string L = input.Substring(0, input.Length / 2);
            string R = input.Substring(input.Length / 2, input.Length / 2);

            return (XOR(f(L, key), R) + L);
        }

        private static string XOR(string s1, string s2) //XOR двух строк с двоичными данными.
        {
            string result = "";

            for (int i = 0; i < s1.Length; i++)
            {
                bool a = Convert.ToBoolean(Convert.ToInt32(s1[i].ToString()));
                bool b = Convert.ToBoolean(Convert.ToInt32(s2[i].ToString()));

                if (a ^ b) result += "1";
                else result += "0";
            }
            return result;
        }

        private static string f(string s1, string s2) //В качестве шифрующей функции тоже XOR
        {
            return XOR(s1, s2);
        }

        private  static string KeyToNextRound(string key) //Циклический сдвиг зашифрования
        {
            for (int i = 0; i < shiftKey; i++)
            {
                key = key[key.Length - 1] + key;
                key = key.Remove(key.Length - 1);
            }

            return key;
        }

        private static string KeyToPrevRound(string key) //Циклический сдвиг расшифрования
        {
            for (int i = 0; i < shiftKey; i++)
            {
                key = key + key[0];
                key = key.Remove(0, 1);
            }
 
            return key;
        }

        private static string StringFromBinaryToNormalFormat(string input) //Метод, переводящий строку с двоичными данными в символьный формат.
        {
            string output = "";

            while (input.Length > 0)
            {
                string char_binary = input.Substring(0, sizeOfChar);
                input = input.Remove(0, sizeOfChar);

                int a = 0;
                int degree = char_binary.Length - 1;

                foreach (char c in char_binary)
                    a += Convert.ToInt32(c.ToString()) * (int)Math.Pow(2, degree--);

                output += ((char)a).ToString();
            }

            return output;
        }

        private static void Encrypt(string keyword)
        {
            if (keyword.Length > 0)
            {
                string s = "";

                string key = keyword;

                StreamReader sr = new StreamReader("in.txt");

                while (!sr.EndOfStream)
                {
                    s += sr.ReadLine();
                }

                sr.Close();

                s = StringToRightLength(s);

                CutStringIntoBlocks(s);

                key = CorrectKeyWord(key, s.Length / (2 * Blocks.Length));
                
                Console.WriteLine(key);
                key = StringToBinaryFormat(key);

                for (int j = 0; j < quantityOfRounds; j++)
                {
                    for (int i = 0; i < Blocks.Length; i++)
                        Blocks[i] = EncodeDES_One_Round(Blocks[i], key);

                    key = KeyToNextRound(key);
                }

                key = KeyToPrevRound(key);
                
                Console.WriteLine(StringFromBinaryToNormalFormat(key)+" Ключ для расшифровки");
               
                string result = "";

                for (int i = 0; i < Blocks.Length; i++)
                    result += Blocks[i];

                string path = @"out1.txt";
                 if (File.Exists(path))
                {
            // Create a file to write to.
                 File.WriteAllText(path, StringFromBinaryToNormalFormat(result), Encoding.BigEndianUnicode);
                }
               // StreamWriter sw = new StreamWriter("out1.txt", Encoding.Unicode);
               // sw.WriteLine(StringFromBinaryToNormalFormat(result));
               // sw.Close();
               Decipher(key);
            }
            
            else Console.WriteLine("Введите ключевое слово!");
           
        }


        private static void Decipher(string keyword)
        {
        if (keyword.Length > 0)
    {
        string s = "";

        string key = StringToBinaryFormat(keyword);

        StreamReader sr = new StreamReader("out1.txt");

        while (!sr.EndOfStream)
        {
            s += sr.ReadLine();
        }

        sr.Close();
 Console.WriteLine(s);
        s = StringToBinaryFormat(s);
 Console.WriteLine(s);
        CutBinaryStringIntoBlocks(s);

        for (int j = 0; j < quantityOfRounds; j++)
        {
            for (int i = 0; i < Blocks.Length; i++)
                Blocks[i] = DecodeDES_One_Round(Blocks[i], key);

            key = KeyToPrevRound(key);
        }

        key = KeyToNextRound(key);

        Console.WriteLine(StringFromBinaryToNormalFormat(key));

        string result = "";

        for (int i = 0; i < Blocks.Length; i++)
           result += Blocks[i];

        StreamWriter sw = new StreamWriter("out2.txt");
       sw.WriteLine(StringFromBinaryToNormalFormat(result));
        sw.Close();

    }
    else  Console.WriteLine("Введите ключевое слово!");
}

    }
}
