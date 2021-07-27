using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Reflection;
using System.Text;

namespace Kit
{
    class Program
    {

        public static void AppendToFile(string fileToWrite, byte[] DT)
        {
            using (FileStream FS = new FileStream(fileToWrite, File.Exists(fileToWrite) ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write))
            {
                FS.Write(DT, 0, DT.Length);
                FS.Close();
            }
        }

        class Dop
        {
            public string Grt { get; set; } = "qwert";
        }

        class Flesh
        {
            public string Str { get; set; } = "data";
            public int NumRow { get; set; } = 12;
            //public Dop Dp { get; set; } = new Dop(); 
        }

        static void Main(string[] args)
        {





            //BinarySave<List<Test>> bs = new BinarySave<List<Test>>();
            //Test test = new Test();
            //Test test1 = new Test() { DataStr = "123456789qwertyuiopasdfghjklzxcvbnm" };
            //List<Test> list = new List<Test>();
            //list.Add(test);
            //list.Add(test1);
            //bs.Serialize(list);
            //return;

            //---------------------------------------------------------

            //BinarySave<Test> bs = new BinarySave<Test>();

            //bs.Serialize(new Test() { Num = 1000000000});

            //----------------------------------------------------------

            BinarySave<List<Test>> bsb = new BinarySave<List<Test>>();

            bsb.Reader<Test>(1);


        }


        private static void Serialize<T>(T obj, List<int> size = null, List<byte> allbytes = null)
        {
            var type = obj.GetType();
            var all_p = type.GetProperties();

            if(allbytes == null)
             allbytes = new List<byte>();
            if(size == null)
             size = new List<int>();

            for(int i = 0; i < all_p.Length; ++i)
            {
                var p = all_p[i];
                if(p.GetIndexParameters().Length == 0)
                {
                    //Console.WriteLine(p.GetValue(obj));
                    var name = p.PropertyType.Name;
                    byte[] buff = null;
                    var avl = p.GetValue(obj).ToString();
                    double dp = 0;
                    if (name == "String")
                    {
                       buff =  Encoding.UTF8.GetBytes(Convert.ToString(p.GetValue(obj)));
                    }
                    else if(double.TryParse(avl, out dp))
                    {
                        buff = BitConverter.GetBytes(dp);
                        //if (BitConverter.IsLittleEndian)
                        //    Array.Reverse(buff);

                        var dou = BitConverter.ToDouble(buff);
                    }
                    else
                    {
                        var tm = p.GetValue(obj);
                        Serialize(tm,size, allbytes);
                    }
                    if (buff != null)
                    {
                        size.Add(buff.Length);
                        allbytes.AddRange(buff);
                    }
                }
            }

            string fileName = "tmps.dat";

            if (File.Exists(fileName)) File.Delete(fileName);

            File.WriteAllBytes(fileName, allbytes.ToArray());
            int z = 0;
            foreach(var sz in size)
            {
                byte[] bytes = new byte[sz];
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    fs.Seek(z, SeekOrigin.Begin);
                    fs.Read(bytes, 0, bytes.Length);
                }

                try
                {
                    var res_s = BitConverter.ToDouble(bytes);
                    Console.WriteLine(res_s);
                }
                catch (Exception ex)
                {

                    var res = Encoding.UTF8.GetString(bytes);
                    Console.WriteLine(res);
                }

                z = sz;
            }
        }

        /// <summary>
        /// строго преобразовывать объекты в байты
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        static byte[] MBytes<T>(T obj)
        {
            var size = Marshal.SizeOf(obj);
            // Требуются как управляемые, так и неуправляемые буферы
            var bytes = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            // Побайтовое копирование объекта в неуправляемую память.
            Marshal.StructureToPtr(obj, ptr, false);
            // Скопируйте данные из неуправляемой памяти в управляемый буфер.
            Marshal.Copy(ptr, bytes, 0, size);
            // Освободите неуправляемую память.
            Marshal.FreeHGlobal(ptr);

            return bytes;
        }

        /// <summary>
        ///  преобразовать байты в объект:
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="size"></param>
        /// <returns></returns>
        static T MFBytes<T>(int size)
        {
            var bytes = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, ptr, size);
            T your_object = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);

            return your_object;
        }

        static int n = 0;
        private static string GenefateIp(int[] arrIp)
        {
            if (arrIp[3] < 999)
            {
                ++arrIp[3];
            }
            else if (arrIp[2] < 999)
            {
                ++arrIp[2];
                arrIp[3] = 0;
            }
            else if (arrIp[1] < 999)
            {
                ++arrIp[1];

                arrIp[2] = arrIp[3] = 0;
            }
            else if (arrIp[0] < 999)
            {
                ++arrIp[0];
                arrIp[2] = arrIp[3] = arrIp[1] = 0;
            }

            return $"{arrIp[0]}.{arrIp[1]}.{arrIp[2]}.{arrIp[3]}";
        }
    }

    [Serializable]
    public class Data
    {
        public long Id { get; set; }
        public string Address { get; set; }

    }
}
