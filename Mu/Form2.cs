using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using CefSharp;
using CefSharp.WinForms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;



using System.Windows.Media.Imaging;
using System.Windows.Shapes;
//using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Input;
using System.Security.Policy;
using System.Diagnostics;
using Aspose.Html;
using System.Xml;


// including the M2Mqtt Library
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Windows.Threading;
using System.Drawing.Drawing2D;
using System.Speech.Synthesis;
using Nest;



namespace Mu
{


    public partial class Form2 : Form

    {

        private ChromiumWebBrowser browser;
        //mqtt 
        public Form2()
        {
            InitializeComponent();
            // Initialize ChromiumWebBrowser control
            browser = new ChromiumWebBrowser();
            SetBrowserSize(0, 0);
            //browser.Visible = false;
            //browser.Size = new System.Drawing.Size(800, 600); // Set your desired size here

            // Add the browser control to your form
            Controls.Add(browser);


            // Subscribe to SizeChanged event
            browser.SizeChanged += Browser_SizeChanged;


            // Subscribe to LoadingStateChanged event
            browser.LoadingStateChanged += Browser_LoadingStateChanged;


            // this.WindowState = FormWindowState.Maximized;


            //mqtt 
            string broker = "broker.hivemq.com";
            int port = 1883;
            string topic = "Csharp/mqtt-mu-test_3";
            string clientId = Guid.NewGuid().ToString();
            // If the broker requires authentication, set the username and password
            string username = "";
            string password = "";
            MqttClient client = ConnectMQTT(broker, port, clientId, username, password);
            Subscribe(client, topic);
            Publish(client, topic, "connected");
            //mqtt 




        }
        String[] myArrayInstrucursName = new string[0];



        public void circularButton_Effect(string color)
        {
            //string color = "red";
            if (color == "green")
            {
                circularButton1.BackColor = Color.Green;
            }
            else
            {
                circularButton1.BackColor = Color.Red;
            }
        }


        int connect = 0;

        //mqtt 
        public MqttClient ConnectMQTT(string broker, int port, string clientId, string username, string password)
        {

            MqttClient client = new MqttClient(broker, port, false, MqttSslProtocols.None, null, null);
            client.Connect(clientId, username, password);
            if (client.IsConnected)
            {
                //MessageBox.Show("Connected to MQTT Broker");
                Console.WriteLine("Connected to MQTT Broker");
                circularButton_Effect("green");
                label1.Text = "connected to MQTT";
                if (connect == 0)
                {
                    using (SpeechSynthesizer synth = new SpeechSynthesizer())
                    {
                        synth.SelectVoice("Microsoft Zira Desktop");
                        synth.Speak("The server has been reached, you are now connected");
                    }
                    connect = 1;
                }

            }
            else
            {
                Console.WriteLine("Failed to connect");
                circularButton_Effect("red");
                label1.Text = "disconnected ";
            }
            return client;
        }
        public void Publish(MqttClient client, string topic, string message)
        {

            System.Threading.Thread.Sleep(1 * 1000);

            //string msg = "messages: " + message;
            string msg;
            if (message == "map" || message == "find")
            {
                msg = message;
            }
            else
            {
                msg = "messages: " + message;
            }



            client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(msg));
            Console.WriteLine("Send `{0}` to topic `{1}`", msg, topic);

        }
        public void Subscribe(MqttClient client, string topic)
        {
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

        }

        //private void SetBrowserSize(int width, int height)
        private void SetBrowserSize(double percentWidth, double percentHeight)
        {
            // Set the size of the browser control
            int width = (int)(this.Width * percentWidth / 100);
            int height = (int)(this.Height * percentHeight / 100);


            browser.Size = new System.Drawing.Size(width, height);

            browser.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
        }
        public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string payload = System.Text.Encoding.Default.GetString(e.Message);
            Console.WriteLine("Received `{0}` from `{1}` topic", payload, e.Topic.ToString());

            if (payload != "messages: connected")
            {
                // Check if the form's handle has been created
                if (this.IsHandleCreated)
                {
                    // Invoke UI updates on the UI thread
                    this.Invoke(new Action(() =>
                    {
                        label3.Text = "lat msg from server : " + payload.ToString();
                        textBox2.Text = payload.ToString();
                        label3.Visible = true;


                        if (textBox2.Text.StartsWith("https://www.google.com/map"))
                        {
                            try
                            {
                                // Load the URL specified in textBox1
                                browser.Load(textBox2.Text);
                                SetBrowserSize(80, 90);
                            }
                            catch (System.UriFormatException)
                            {
                                return;
                            }
                        }
                    }));
                }
                else
                {
                    Console.WriteLine("Form handle has not been created yet. UI updates postponed.");
                }
            }
        }

        //mqtt 


        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();//this creates new instance, which is wrong
            form1.Show();//the method defined in Form1
            System.Threading.Thread.Sleep(1000);
            this.Close();
        }




        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)13)
            {

                // Enter key pressed
                if (!textBox1.Text.StartsWith("https://www.google.com/search?q="))
                {
                    textBox1.Text = @"https://www.google.com/search?q=" + textBox1.Text;
                    //MessageBox.Show(textBox1.Text);
                    try
                    {
                        // Load the URL specified in textBox1
                        browser.Load(textBox1.Text);
                        SetBrowserSize(80, 90);
                    }
                    catch (System.UriFormatException)
                    {
                        return;
                    }
                }



            }
        }
        private void Browser_SizeChanged(object sender, EventArgs e)
        {
            // Adjust zoom level to fit content when size changes
            FitContent();
        }

        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                // Browser finished loading, set initial zoom level to fit content
                FitContent();
            }
        }

        private void FitContent()
        {
            // Adjust zoom level to fit content
            browser.SetZoomLevel(0);
        }


        private void textBox2_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Console.WriteLine("enter key is pressed");

                //mqtt 
                string broker = "broker.hivemq.com";
                int port = 1883;
                string topic = "Csharp/mqtt-mu-test_3";
                string clientId = Guid.NewGuid().ToString();
                // If the broker requires authentication, set the username and password
                string username = "";
                string password = "";
                MqttClient client = ConnectMQTT(broker, port, clientId, username, password);
                Publish(client, topic, textBox2.Text);


            }
            else
            {
                Console.WriteLine(e.KeyChar);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
