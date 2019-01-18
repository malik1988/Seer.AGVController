using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Seer.AGVCom;
using System.Timers;

namespace Seer.AGVController
{
    public class AGVController
    {
        string ip = "127.0.0.1";
        int port = (int)AGVPort.导航;
        AGVCommucation comStatus = new AGVCommucation();
        AGVCommucation comTask = new AGVCommucation();

        object locker = new object();
        Queue<AGVComFrame> msgList = new Queue<AGVComFrame>();

        Timer timerStatus;
        Timer timerTask;

        public EventHandler<string> OnStatusUpdate;
        bool _isConnected = false;
        public bool IsConnected { get { return this._isConnected; } }

        public AGVController(string ip = "127.0.0.1", int period = 500)
        {
            this.ip = ip;
            timerStatus = new Timer(period);
            timerStatus.Elapsed += timerStatus_Elapsed;

            timerTask = new Timer(period);
            timerTask.Elapsed += timerTask_Elapsed;
        }



        public string Connect(string ip = "127.0.0.1", bool enableTask = true)
        {
            this.ip = ip;
            string result = "";
            string resultStatus = comStatus.Connect(ip, (int)AGVPort.状态);
            if (resultStatus == "Success")
            {
                timerStatus.Start();
                this._isConnected = true;
            }
            result += resultStatus;

            if (enableTask)
            {
                string resultTask = comTask.Connect(ip, (int)AGVPort.导航);
                if (resultTask == "Success")
                {
                    //timerTask.Start();
                    this._isConnected = true;
                }
                result += ":" + resultTask;
            }
            return result;
        }


        public AGVComFrame Send(AGVComFrame msg)
        {
            AGVComFrame frame = null;
            //导航
            if (msg.header.type >= 3001 && msg.header.type < 3106)
            {
                frame = comTask.SendAndGet(msg, 200);
            }

            return frame;
        }

        public void AddMessage(AGVComFrame msg)
        {
            lock (locker)
            {
                msgList.Enqueue(msg);
            }
        }

        /// <summary>
        /// 定时获取AGV导航状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timerStatus_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (locker)
            {
                if (msgList.Count > 0)
                {
                    AGVComFrame response = comStatus.SendAndGet(msgList.Dequeue(), 300);
                    if (null != response && null != response.data && null != this.OnStatusUpdate)
                    {
                        string data = System.Text.ASCIIEncoding.UTF8.GetString(response.data);
                        AGVTypes _type = (AGVTypes)(response.header.type - AGVProtocolHeader.TypeResponseOffset);
                        string respData = _type.ToString() + ":" + data;
                        this.OnStatusUpdate.BeginInvoke(this, respData, null, null);
                    }
                }

                //AGVComFrame response = comStatus.SendAndGet(AGVComFrameBuilder.状态_查询机器人导航状态(), 300);
                //this.status = AGVComFrameBuilder.Response_状态_查询机器人导航状态(ref response);
                //if (null != this.status && null != this.OnStatusUpdate)
                //{
                //    string data = System.Text.ASCIIEncoding.UTF8.GetString(response.data);
                //    this.OnStatusUpdate.BeginInvoke(this, data, null, null);
                //}
            }
        }

        private void timerTask_Elapsed(object sender, ElapsedEventArgs e)
        {
        }

        public void Disconnect()
        {
            timerStatus.Stop();
            comStatus.Disconnect();
            comTask.Disconnect();
            this._isConnected = false;
        }
    }
}
