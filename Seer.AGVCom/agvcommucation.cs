using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using SimpleTCP;
using Newtonsoft.Json;
using System.Net.Sockets;

namespace Seer.AGVCom
{
    public class AGVCommucation
    {
        //SimpleTcpClient client = new SimpleTcpClient();
        Socket socket=new Socket(SocketType.Dgram,ProtocolType.Tcp);
        public string Connect(string ip, int port)
        {
            try
            {
                socket.Connect(ip, port);
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
            if (null == frame || null == frame.Pack())
                return null;

            int ret = socket.Send(frame.Pack());
            if (ret > 0)
                if (socket.Poll(timeout * 1000, SelectMode.SelectRead))
                {
                    byte[] buf = new byte[socket.Available];
                    socket.Receive(buf);
                    return new AGVComFrame().Parse(buf);
                }
                else
                    return null;
            else
                return null;

        }


        public void Disconnect()
        {
            socket.Close();
        }
    }
    /// <summary>
    /// API类型码
    /// </summary>
    public enum AGVTypes
    {
        状态_查询机器人信息 = 1000,
        状态_查询机器人的运行状态信息 = 1002,
        状态_查询机器人位置 = 1004,
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
        状态_查询机器人导航状态 = 1020,
        状态_查询机器人重定位状态,
        状态_查询机器人地图载入状态,
        状态_查询机器人扫图状态 = 1025,
        状态_查询顶﻿升机构状态 = 1027,
        状态_查询货叉状态 = 1028,
        状态_查询辊筒状态,
        状态_查询机器人当前的调度状态,
        状态_查询机器人告警状态 = 1050,
        状态_查询批量数据1 = 1100,
        状态_查询批量数据2 = 1101,
        状态_查询批量数据3 = 1102,
        状态_查询机器人载入的地图以及储存的地图 = 1300,
        状态_查询机器人当前载入地图中的站点信息,
        状态_查询机器人参数 = 1400,


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


    /// <summary>
    /// API端口
    /// </summary>
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


    public enum AGVErrorCode : int
    {
        请求不可用 = 40000,
        必要的请求参数缺失,
        请求参数类型错误,
        请求参数不合法,
        运行模式错误,
        非法的地图名,
        正在烧写固件,
        烧写固件错误,
        文件名非法,

        关机指令出现错误 = 40010,
        重启指令出现错误,
        调度系统控制中,
        robod错误,
        robod警告,

        地图解析出错 = 40050,
        地图不存在,
        加载地图错误,
        重载地图错误,
        空地图,
        文件不存在,
        地图转换失败,
        当前无可用rawmap文件,
        当前无可用calib文件,

        音频文件不存在 = 40060,
        播放音频出错,
        上传音频文件失败,
        音频正在播放中,

        保存模型文件出错 = 40069,
        模型文件解析错误,
        标定数据解析错误,
        保存标定文件出错,
        清除标定数据出错,

        请求执行超时 = 40100,
        请求被禁止,
        机器人繁忙,

        内部错误,
        解析任务链错误,
        任务链名字非法,
        任务链不存在,
        任务链正在执行中,
        设置参数类型错误,
        设置的参数不存在,
        设置参数出错,
        设置并保存参数类型错误,
        设置并保存参数不存在,
        设置并保存出错,
        重载的参数类型错误,
        重载的参数不存在,
        重载参数出错,

        初始化状态错误 = 41000,
        地图载入状态错误,
        重定位状态错误,
        找不到重定位的,
        置信度过低,
        找不到起点,
        找不到准备点,
        找不到终点,
        充电点不存在,
        速度值非法,

        辊筒或皮带连接错误 = 42000,
        辊筒或皮带类型未知,
        辊筒或皮带不支持该指令,
        顶升机构连接错误,
        顶升机构类型未知,
        顶升机构不支持该指令,
        升降操作出错,
        错误的报文类型,
        未知的报文类型,
        错误的数据区,
        协议版本错误时得到的响应,
        数据区过大
    }

    public class AGVErrorInfo
    {
        public AGVErrorCode ret_code;
        public string err_msg;
    }

}
