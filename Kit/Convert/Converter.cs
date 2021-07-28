using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kit.Convert
{

    public struct ConvertData
    {
        /// <summary>
        /// данные класса 
        /// </summary>
        public List<byte> buff;
        /// <summary>
        /// имя файла
        /// </summary>
        public string NameFile;
        /// <summary>
        /// имена типов в классе
        /// </summary>
        public List<string> Types;

        public string GetNameFile()
        {
            if (string.IsNullOrEmpty(NameFile))
                throw new System.ArgumentException($"Не инициализировано поле {NameFile}");
            return NameFile + ".dat";
        }

        public string GetNameFileMetaData()
        {
            if (string.IsNullOrEmpty(NameFile))
                throw new System.ArgumentException($"Не инициализировано поле {NameFile}");
            return NameFile + "Meta.json";
        }

        public bool SerializeBytes()
        {
            bool isRes = false;


            return isRes;
        }

    }
    public class Converter
    {
        public static ConvertData SerializeObj<T>(T obj)
        {
            ConvertData cd = new ConvertData();
            cd.Types = new List<string>();
            cd.buff = new List<byte>();
            byte[] res = new byte[0];

            var type = obj.GetType();
            var all_p = type.GetProperties();
            cd.NameFile = type.FullName;

            for (int i = 0; i < all_p.Length; ++i)
            {
                var p = all_p[i];
                var name = p.PropertyType.Name;
                cd.Types.Add(name);
                cd.buff.AddRange(GetBytes(name, p.GetValue(obj)));
            }

            return cd;
        }

        private static byte[] GetBytes<R>(string nameType, R obj)
        {
            byte[] res = null;

            switch (nameType)
            {
                case "Int32":
                   return BitConverter.GetBytes(int.Parse(System.Convert.ToString(obj)));
                case "IntPtr":
                    return BitConverter.GetBytes(nint.Parse(System.Convert.ToString(obj)));
                case "UIntPtr":
                    return BitConverter.GetBytes(nuint.Parse(System.Convert.ToString(obj)));
                case "Int64":
                    return BitConverter.GetBytes(long.Parse(System.Convert.ToString(obj)));
                case "UInt64":
                    return BitConverter.GetBytes(ulong.Parse(System.Convert.ToString(obj)));
                case "UInt32":
                    return BitConverter.GetBytes(uint.Parse(System.Convert.ToString(obj)));
                case "Int16":
                    return BitConverter.GetBytes(ushort.Parse(System.Convert.ToString(obj)));
                case "SByte":
                    return BitConverter.GetBytes(int.Parse(System.Convert.ToString(obj)));
                case "Byte":
                    return BitConverter.GetBytes(byte.Parse(System.Convert.ToString(obj)));
                case "Double":
                    return BitConverter.GetBytes(sbyte.Parse(System.Convert.ToString(obj)));
                case "Single":
                    return BitConverter.GetBytes(float.Parse(System.Convert.ToString(obj)));
                case "Decimal":
                    return BitconverterExt.GetBytes(decimal.Parse(System.Convert.ToString(obj)));
                case "String":
                    return Encoding.UTF8.GetBytes(System.Convert.ToString(obj));
            }

            return res;
        }
    }

    /*
     decimal testDecimal = 987.123456m;
         //Get the bytes of the decimal
         byte[] decimalBytes = BitconverterExt.GetBytes(testDecimal);
         //Create a decimal from those bytes
         decimal fromBytes = BitconverterExt.ToDecimal(decimalBytes);
     */
    public class BitconverterExt
    {
        public static byte[] GetBytes(decimal dec)
        {
            //Load four 32 bit integers from the Decimal.GetBits function
            Int32[] bits = decimal.GetBits(dec);
            //Create a temporary list to hold the bytes
            List<byte> bytes = new List<byte>();
            //iterate each 32 bit integer
            foreach (Int32 i in bits)
            {
                //add the bytes of the current 32bit integer
                //to the bytes list
                bytes.AddRange(BitConverter.GetBytes(i));
            }
            //return the bytes list as an array
            return bytes.ToArray();
        }
        public static decimal ToDecimal(byte[] bytes)
        {
            //check that it is even possible to convert the array
            if (bytes.Count() != 16)
                throw new Exception("A decimal must be created from exactly 16 bytes");
            //make an array to convert back to int32's
            Int32[] bits = new Int32[4];
            for (int i = 0; i <= 15; i += 4)
            {
                //convert every 4 bytes into an int32
                bits[i / 4] = BitConverter.ToInt32(bytes, i);
            }
            //Use the decimal's new constructor to
            //create an instance of decimal
            return new decimal(bits);
        }
    }
}
