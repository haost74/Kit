using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Kit
{

    public class Test
    {
        public int Num { get; set; } = 10;
        public string DataStr { get; set; } = "hello78";

        public double DNum { get; set; } = 1015441.25025788;
    }

    public class MetaClass
    {
        string nameType { get; set; } = "";
        public MetaClass(string nameType)
        {
            this.nameType = nameType;
        }

        public MetaClass() { }
        public int Num { get; set; } = 0;
        public int AllSize { get; set; }
        public int CountField { get; set; } = -1;
        public List<byte> AllBytes = new List<byte>();
        public List<int> CellSize { get; set; } = new List<int>();

        public List<string> Types { get; set; } = new List<string>();


        public void SerializeJson()
        {
            if (File.Exists(NameFile)) File.Delete(NameFile);
            File.WriteAllText(NameFile, JsonSerializer.Serialize(this));
        }

        public MetaClass Deserialize()
        {
            if (File.Exists(NameFile))
            {
                return (MetaClass)JsonSerializer.Deserialize<MetaClass>(File.ReadAllText(NameFile));
            }
            return null;
        }

        private string NameFile
        {
            get { return nameType.Replace('.', '_') + "_MetaClass.json"; }
        }
    }

    public class BinarySave<T>
    {
        private MetaClass mc;
        public void Serialize(T obj)
        {
            if (obj is IList && obj.GetType().IsGenericType)
            {
                foreach (var el in (IEnumerable)obj)
                {
                    SerealizeObj(el);
                }
            }
            else
            {
                SerealizeObj(obj);
            }

        }

        private void SerealizeObj<R>(R obj, bool isAdd = false)
        {
            var type = obj.GetType();
            var all_p = type.GetProperties();
            string fileName = type.FullName + ".dat";
            if (mc == null)
                mc = new MetaClass(fileName);
            mc.CountField = all_p.Length;
            for (int i = 0; i < all_p.Length; ++i)
            {
                var p = all_p[i];
                var name = p.PropertyType.Name;
                byte[] buff = null;
                var avl = p.GetValue(obj).ToString();
                double dp = 0;

                if (name == "String")
                {
                    buff = Encoding.UTF8.GetBytes(Convert.ToString(p.GetValue(obj)));
                    mc.CellSize.Add(buff.Length);
                    if (mc.CountField > mc.Types.Count)
                        mc.Types.Add(name);
                }
                else if (double.TryParse(avl, out dp))
                {
                    buff = BitConverter.GetBytes(dp);
                    mc.CellSize.Add(buff.Length);
                    if (mc.CountField > mc.Types.Count)
                        mc.Types.Add(name);
                }
                else
                {

                }

                if (buff != null)
                {
                    mc.AllBytes.AddRange(buff);
                }
            }


            if (File.Exists(fileName)) File.Delete(fileName);
            File.WriteAllBytes(fileName, mc.AllBytes.ToArray());
            mc.AllSize = mc.CellSize.Sum(x => x);
            mc.SerializeJson();
        }

        public bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public void Reader<R>(int numType)
        {
            var type = typeof(R);
            string fileName = type.FullName + ".dat";
            if (File.Exists(fileName))
            {
                mc = new MetaClass(fileName);
                mc = mc.Deserialize();

                int x = mc.CountField; int y = mc.CountField;

                var gdata = mc.CellSize.GroupBy(_ => x++ / y).Select(n => n.ToList());

                if (numType < gdata.Count())
                {
                    var el = gdata.ElementAt(numType);

                    var m = 0;
                    int sum = 0;
                    while (m < numType)
                    {
                        sum += gdata.ElementAt(m).Sum();
                        ++m;
                    }

                    var sz = el.Sum();
                    byte[] buff = new byte[sz];
                    using (FileStream fs = new FileStream(fileName, FileMode.Open))
                    {
                        fs.Seek(sum, SeekOrigin.Begin);
                        fs.Read(buff, 0, buff.Length);
                    }

                    int n = 0;
                    int i = 0;
                    foreach (var gel in el)
                    {
                        var nBuff = Frez(n, gel, buff);

                        var dType = mc.Types[i];

                        switch(dType)
                        {
                            case "String":
                                var res = Encoding.UTF8.GetString(nBuff);
                                Console.WriteLine(res);
                                break;
                            default:
                                var res_s1 = BitConverter.ToDouble(nBuff);
                                Console.WriteLine(res_s1);
                                break;
                        }


                        n += gel;
                        ++i;
                    }

                    //buff = buff.Take(8).ToArray();

                    //var nBuff = Frez(0, 8, buff);
                    //var nBuff = Frez(8, 35, buff);
                    //var nBuff = Frez(35+8, 8, buff);


                }
            }

        }


        private byte[] Frez(int strat, int step, byte[] arr)
        {
            byte[] res = new byte[step];
            using (MemoryStream ms = new MemoryStream(arr))
            {
                ms.Seek(strat, SeekOrigin.Begin);
                ms.Read(res, 0, step);
            }
            return res;
        }

    }
}
