using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimpleTCP;
using Newtonsoft.Json;

namespace Seer.AGVCom
{
    public class AGVCommucation
    {
        SimpleTcpClient client = new SimpleTcpClient();
        public string Connect(string ip, int port)
        {
            try
            {
                client.Connect(ip, port);
                return "Success";
            }
            catch (Exception e)
            {
                return "Failed," + e.Message;
            }
        }


        /// <summary>
        /// 发送并获取反馈
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="timeout">超时时间，单位ms</param>
        /// <returns></returns>
        public AGVComFrame SendAndGet(AGVComFrame frame, int timeout = 300)
        {
            if (null != frame)
            {
                var msg = client.WriteLineAndGetReply(frame.Pack(), new TimeSpan(0, 0, 0, 0, timeout));
                if (null == msg)
                    return null;

                return new AGVComFrame().Parse(msg.Data);
            }
            else
                return null;
        }


        public void Disconnect()
        {
            client.Disconnect();
        }
    }

    public enum AGVTypes
    {
        状态_查询机器人信息 = 1000,
        状态_查询机器人的运行状态信息=1002,
        状态_查询机器人位置=1004,
        状态_查询机器人速度,
        状态_查询机器人被阻挡状态,
        状态_查询机器人电池状态,
        状态_查询机器人抱闸状态,
        状态_查询机器人激光点云数据,
        状态_查询机器人路径数据,
        状态_查询机器人当前所在区域,
        状态_查询机器人急停状态,
        状态_查询机器人IO数据,
        状态_查询机器人IMU数据,
        状态_查询RFID数据,
        状态_查询机器人的超声传感器数据,
        状态_查询机器人导航状态=1020,
        状态_查询机器人重定位状态,
        状态_查询机器人地图载入状态,
        状态_查询机器人扫图状态=1025,
        状态_查询顶﻿升机构状态=1027,
        状态_查询货叉状态=1028,
        状态_查询辊筒状态,
        状态_查询机器人当前的调度状态,
        状态_查询机器人告警状态=1050,
        状态_查询批量数据1=1100,
        状态_查询批量数据2=1101,
        状态_查询批量数据3=1102,
        状态_查询机器人载入的地图以及储存的地图=1300,
        状态_查询机器人当前载入地图中的站点信息,
        状态_查询机器人参数=1400,


        控制_停止开环运动 = 2000,
        控制_重定位,
        控制_确认定位正确,
        控制_取消重定位,
        控制_开环运动,
        控制_切换载入的地图,

        导航_暂停当前导航 = 3001,
        导航_继续当前导航,
        导航_取消当前导航,
        /// <summary>
        /// 根据地图上的坐标值或站点自由规划路径导航
        /// </summary>
        导航_自由导航 = 3050,
        导航_路径导航,
        导航_获取路径导航的路径 = 3053,

        /// <summary>
        /// 以固定速度直线运动固定距离
        /// </summary>
        导航_平动 = 3055,
        导航_转动,

        导航_钻货架 = 3063,
        导航_目标跟随 = 3070,
        导航_UWB跟随,
        导航_执行预存的任务链 = 3106,

    }



    public enum AGVPort : int
    {
        状态 = 19204,
        控制,
        导航,
        配置,
        其他 = 19210
    }


    public class AGVTaskStatus
    {
        public int task_status;
        public int task_type;
        public string target_id;
        public List<int> target_point;
        public List<string> finished_path;
        public List<string> unfinished_path;
        public int ret_code;
        public string err_msg;
    }
}
