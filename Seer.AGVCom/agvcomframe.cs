using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Seer.AGVCom
{
    public class AGVProtocolHeader
    {
        /// <summary>
        /// 报文同步字节，固定为0x5A
        /// </summary>
        public readonly byte sync = 0x5A;
        /// <summary>
        /// 版本号，默认为1
        /// </summary>
        public byte version = 1;
        public UInt16 number;
        public UInt32 length;
        public UInt16 type;
        public byte[] reserved = new byte[6];

        public readonly int SelfLength = 1 + 1 + 2 + 4 + 2 + 6;
        public static UInt16 TypeResponseOffset { get { return 10000; } }
        public byte[] Pack()
        {
            byte[] buf = new byte[SelfLength];
            int index = 0;
            buf[index] = sync;
            index += 1;

            buf[index] = version;
            index += 1;

            Array.Copy(BitConverter.GetBytes(number).Reverse().ToArray(), 0, buf, index, 2);
            index += 2;

            Array.Copy(BitConverter.GetBytes(length).Reverse().ToArray(), 0, buf, index, 4);
            index += 4;

            Array.Copy(BitConverter.GetBytes(type).Reverse().ToArray(), 0, buf, index, 2);
            index += 2;


            Array.Copy(reserved, 0, buf, index, reserved.Length);
            index += reserved.Length;

            return buf;
        }

        public AGVProtocolHeader Parse(byte[] buf)
        {
            if (null == buf || buf.Length < SelfLength || buf[0] != sync) return null;
            int index = 1;
            this.version = buf[index];
            index += 1;

            byte[] u16 = new byte[2];
            Array.Copy(buf, index, u16, 0, u16.Length);
            this.number = BitConverter.ToUInt16(u16.Reverse().ToArray(), 0);
            index += u16.Length;

            byte[] u32 = new byte[4];
            Array.Copy(buf, index, u32, 0, u32.Length);
            this.length = BitConverter.ToUInt16(u32.Reverse().ToArray(), 0);
            index += u32.Length;

            Array.Copy(buf, index, u16, 0, u16.Length);
            this.type = BitConverter.ToUInt16(u16.Reverse().ToArray(), 0);
            index += u16.Length;


            Array.Copy(buf, index, reserved, 0, reserved.Length);
            index += u16.Length;
            return this;
        }
    }
    public class AGVComFrameBase
    {
        public AGVProtocolHeader header;
        public byte[] data;

        public byte[] Pack()
        {
            if (null != this.data && this.header.length == this.data.Length)
            {
                byte[] buf = new byte[this.header.length + this.header.SelfLength];
                Array.Copy(this.header.Pack(), buf, this.header.SelfLength);
                Array.Copy(this.data, 0, buf, header.SelfLength, this.data.Length);

                return buf;
            }
            else
                return this.header.Pack();
        }

        public AGVComFrameBase Parse(byte[] buf)
        {
            if (null == buf)
                return null;

            AGVProtocolHeader header = new AGVProtocolHeader().Parse(buf);

            this.header = header;
            if (null == header)
                return null;
            if (header.length > 0 && buf.Length >= header.SelfLength + header.length)
            {
                byte[] data = new byte[header.length];
                Array.Copy(buf, header.SelfLength, data, 0, header.length);
                this.data = data;
            }
            return this;
        }
    }

    public class AGVComFrame : AGVComFrameBase
    {
        public AGVComFrame() : base() { }
        public AGVComFrame(AGVProtocolHeader header, string json)
            : base()
        {
            this.header = header;
            if (null != json)
                this.data = System.Text.ASCIIEncoding.UTF8.GetBytes(json);
            if (null != this.data && this.data.Length > 0)
                this.header.length = (uint)this.data.Length;
        }
        public AGVComFrame(AGVProtocolHeader header, object serialObj)
            : base()
        {
            this.header = header;
            if (null != serialObj)
            {
                try
                {
                    string d = JsonConvert.SerializeObject(serialObj, Formatting.None);
                    this.data = System.Text.ASCIIEncoding.UTF8.GetBytes(d);
                    this.header.length = (uint)this.data.Length;
                }
                catch
                {
                    this.data = null;
                }
            }
        }
        public AGVComFrame(AGVTypes type, object serialObj)
            : base()
        {
            this.header = new AGVProtocolHeader()
            {
                type = (UInt16)type,
                number = (UInt16)new Random(new Guid().GetHashCode()).Next()

            };
            if (null != serialObj)
            {
                try
                {
                    string d = JsonConvert.SerializeObject(serialObj, Formatting.None);
                    this.data = System.Text.ASCIIEncoding.UTF8.GetBytes(d);
                    this.header.length = (uint)this.data.Length;
                }
                catch
                {
                    this.data = null;
                }
            }
        }


        public AGVComFrame Parse(byte[] buf)
        {
            if (null == base.Parse(buf))
                return null;

            return this;
        }


    }
}
