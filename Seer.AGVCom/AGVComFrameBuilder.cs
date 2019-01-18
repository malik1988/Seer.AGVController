using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
namespace Seer.AGVCom
{
    public class AGVComFrameBuilder
    {
        public static AGVComFrame NoData_Request(AGVTypes type)
        {
            AGVComFrame frame = new AGVComFrame(new AGVProtocolHeader() { type = (UInt16)type, length = 0 }, null);
            return frame;
        }

        public static AGVComFrame 状态_查询机器人导航状态(bool simple = false)
        {
            return new AGVComFrame(new AGVProtocolHeader()
            {
                type = (UInt16)AGVTypes.状态_查询机器人导航状态
            },
            "{\"simple\":" + simple.ToString().ToLower() + "}");
        }

        public static bool CheckResponse(AGVTypes sendType, ref AGVComFrame response)
        {
            if (null != response && response.header.type == (UInt16)(sendType + AGVProtocolHeader.TypeResponseOffset))
                return true;
            else
                return false;
        }

        public static AGVTaskStatus Response_状态_查询机器人导航状态(ref AGVComFrame response)
        {
            if (CheckResponse(AGVTypes.状态_查询机器人导航状态, ref response))
            {
                AGVTaskStatus status = null;
                try
                {
                    string data = System.Text.ASCIIEncoding.UTF8.GetString(response.data);
                    status = JsonConvert.DeserializeObject<AGVTaskStatus>(data);
                }
                catch
                {

                }
                return status;
            }

            return null;
        }



    }
}
