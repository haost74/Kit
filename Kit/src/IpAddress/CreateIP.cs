using System.Collections.Generic;

namespace Kit.src.IpAddress
{

    public  class IpAddress
    {
        public int ID { get; set; } = 0;
        public string Address { get; set; } = "";
    }
    public class CreateIP
    {
        public List<IpAddress> ipAddresses = new List<IpAddress>();
        public void Creste(int[] address)
        {
            if (address != null)
            {

            }
            else
            {
                address = new int[] { 0, 0, 0, 1 };

                while (address[0] < 999)
                {
                    if (address[3] < 999)
                    {
                        ++address[3];
                    }
                    else if (address[2] < 999)
                    {
                        ++address[2];
                        address[3] = 0;
                    }
                    else if (address[1] < 999)
                    {
                        ++address[1];
                        address[2] = 0;
                    }
                    else if (address[0] < 999)
                    {
                        ++address[0];
                        address[1] = 0;
                    }
                    ipAddresses.Add(new IpAddress() { ID = ipAddresses.Count + 1, Address = $"{address[0] + "." + address[1] + "." + address[2] + "." + address[3]}" });

                    if(ipAddresses.Count > 999999)
                    {
                        break;
                    }
                }
            }
        }
    }
}
