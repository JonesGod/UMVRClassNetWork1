using System;
using System.Text;

namespace ChatCoreTest
{
    internal class Program
    {
        private static byte[] m_PacketData;
        private static uint m_Pos;
        private static int nowRead;

        public static void Main(string[] args)
        {
            m_PacketData = new byte[1024];
            m_Pos = 0;

            Write(109);
            Write(109.2f);
            Write("Hello!!!");
            Write(2378);
            Write(20456.22f);
            Write("Good Morning!!!");

            Console.Write($"Output Byte array(length:{m_Pos}): ");
            for (var i = 0; i < m_Pos; i++)
            {
                Console.Write(m_PacketData[i] + ", ");
            }
            Console.WriteLine(" ");
            //Console.WriteLine("m_Pos: " + m_Pos);
            
            nowRead = (int)m_Pos - 5;
            //Console.WriteLine("nowRead: " + nowRead);
            while (nowRead > 0)
            {
                Read(m_PacketData, nowRead);
                //Console.WriteLine("nowRead: " + nowRead);
            }

            Console.ReadLine();
        }


        // write an integer into a byte array
        private static bool Write(int i)
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(i);
            byte[] m_IntData = new byte[bytes.Length + 1];
            m_IntData[0] = 0;
            bytes.CopyTo(m_IntData, 1);
            _Write(m_IntData);
            return true;
        }
        // write an integer into a string byte array
        private static bool Write(int i, bool isString)
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(i);
            byte[] m_IntData = new byte[bytes.Length + 1];
            if(isString == true)
            {
                m_IntData[0] = 2;
            }                  
            bytes.CopyTo(m_IntData, 1);
            _Write(m_IntData);
            return true;
        }

        // write a float into a byte array
        private static bool Write(float f)
        {
            // convert int to byte array
            var bytes = BitConverter.GetBytes(f);
            byte[] m_FloatData = new byte[bytes.Length + 1];
            m_FloatData[0] = 1;
            bytes.CopyTo(m_FloatData, 1);
            _Write(m_FloatData);
            return true;
        }

        // write a string into a byte array
        private static bool Write(string s)
        {
            // convert string to byte array
            var bytes = Encoding.Unicode.GetBytes(s);
            
            // write byte array length to packet's byte array
            if (Write(bytes.Length, true) == false)
            {
                return false;
            }

            _Write(bytes);             
            return true;
        }

        // write a byte array into packet's byte array
        private static void _Write(byte[] byteData)
        {
            // converter little-endian to network's big-endian
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteData);
            }

            byteData.CopyTo(m_PacketData, m_Pos);
            m_Pos += (uint)byteData.Length;
        }
        private static void Read(byte[] byteData, int pos)
        {
            byte[] bytes = new byte[m_Pos];
            for (int k = 0; k < m_Pos; k++)
            {
                bytes[k] = byteData[k];
            }

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            switch (bytes[pos])
            {
                case 0:
                    int i = BitConverter.ToInt32(bytes, pos + 1);
                    Console.WriteLine("int: {0}", i);
                    nowRead -= 5;
                    break;
                case 1:
                    float f = BitConverter.ToSingle(bytes, pos + 1);
                    Console.WriteLine("float: {0}", f);
                    nowRead -= 5;
                    break;
                case 2:
                    int stringLength = BitConverter.ToInt32(bytes, pos + 1);
                    string s = Encoding.Unicode.GetString(bytes, pos - stringLength, stringLength);
                    Console.WriteLine("string: {0}", s);
                    nowRead -= (stringLength + 5);
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
        }
    }
}
