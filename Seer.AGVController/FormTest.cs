using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Seer.AGVCom;

namespace Seer.AGVController
{
    public partial class FormTest : Form
    {
        AGVController agv1;
        public FormTest()
        {
            InitializeComponent();
            agv1 = new AGVController("192.168.10.107");
            agv1.OnStatusUpdate += OnAgvStatusUpdate;
            //string result= agv1.Connect();

            foreach(AGVTypes t in Enum.GetValues(typeof(AGVTypes)))
            {
                string name=t.ToString();
                if (name.StartsWith("状态_"))
                    this.comboBox1.Items.Add(t);
            }

            this.comboBox1.SelectedItem = this.comboBox1.Items[0];

        }


        private void button连接_Click(object sender, EventArgs e)
        {
            string ip = this.textBox_Ip.Text;
            if(string.IsNullOrEmpty(ip))
            {
                MessageBox.Show("无效IP地址！", "错误");
                return;
            }


            string name = this.button连接.Text;
            switch (name)
            {
                case "连接":
                    string result = agv1.Connect(ip,false);
                    if (result.Contains("Failed"))
                        this.label1.ForeColor = Color.Red;
                    else
                        this.label1.ForeColor = Color.Green;
                    this.label1.Text = result;
                    this.button连接.Text = "断开";
                    break;
                case "断开":
                    agv1.Disconnect();
                    this.label1.Text = "断开";
                    this.label1.ForeColor = Color.Red;
                    this.button连接.Text = "连接";
                    break;
            }

        }

        void OnAgvStatusUpdate(object sender, string e)
        {
            this.Invoke(new Action(() =>
            {
                this.richTextBox2.Text = e;
                this.richTextBox1.AppendText(DateTime.Now.ToString() + e + "\n");
            }));
        }

        private void button状态_Click(object sender, EventArgs e)
        {

            AGVTypes value = (AGVTypes)this.comboBox1.SelectedItem;

            if (agv1.IsConnected)

                agv1.AddMessage(AGVComFrameBuilder.NoData_Request(value));
            else
                MessageBox.Show("设备未连接，请先连接设备！", "错误");

        }
    }
}
