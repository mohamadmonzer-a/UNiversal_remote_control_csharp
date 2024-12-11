using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json;
using System.Security.Cryptography;


namespace Mu
{
    public partial class Form5 : Form
    {

        private double messagePanelWidthPercentage;
        private double messagePanelHeightPercentage;


        public Form5()
        {
            InitializeComponent();
            InitializeForm();

            textBox2.UseSystemPasswordChar = true;

            // Calculate the initial percentage based on the form's size and panel's size
            messagePanelWidthPercentage = (double)messagePanel.Size.Width / this.Size.Width;
            messagePanelHeightPercentage = (double)messagePanel.Size.Height / this.Size.Height;

            //mqtt 
            string broker = "broker.hivemq.com";
            int port = 1883;
            string topic = "Csharp/mqtt-mu-test1";
            string clientId = Guid.NewGuid().ToString();
            // If the broker requires authentication, set the username and password
            string username = "";
            string password = "";
            MqttClient client = ConnectMQTT(broker, port, clientId, username, password);
            if (client != null)
            {
                Subscribe(client, topic);
                Publish(client, topic, "connected");
            }
            else
            {
                Console.WriteLine("Failed to initialize MQTT client");
            }
            Subscribe(client, topic);
            Publish(client, topic, "connected");
            //mqtt 
        }

        private void InitializeForm()
        {
            // Set the form's properties
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Set the form border style to fixed
            this.MaximizeBox = false; // Disable maximizing the form
            this.StartPosition = FormStartPosition.CenterScreen; // Center the form on startup
            this.Resize += MainForm_Resize; // Subscribe to the Resize event
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            // Reset the form size if the user attempts to resize it
            this.Size = new System.Drawing.Size(400, 300); // Set your desired size here
        }

        //mqtt 
        public MqttClient ConnectMQTT(string broker, int port, string clientId, string username, string password)
        {
            try
            {
                MqttClient mqttClient = new MqttClient(broker, port, false, null, null, MqttSslProtocols.None);
                mqttClient.Connect(clientId, username, password);
                return mqttClient;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to MQTT Broker: {ex.Message}");
                return null;
            }
        }

        public void Publish(MqttClient client, string topic, string message)
        {

            System.Threading.Thread.Sleep(1 * 1000);

            string msg = "messages: " + message;


            client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(msg));
            Console.WriteLine("Send `{0}` to topic `{1}`", msg, topic);

        }
        public void Subscribe(MqttClient client, string topic)
        {
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });

        }
        public void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string payload = System.Text.Encoding.Default.GetString(e.Message);
            Console.WriteLine("Received `{0}` from `{1}` topic", payload, e.Topic.ToString());
            if (payload != "messages: connected")
            {
                string receivedMessage = payload.ToString();

                // Check if the handle is created before invoking the UI update
                if (this.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        // Update the panel with the received message
                        UpdateReceivedMessage(receivedMessage);
                    });
                }
                else
                {
                    // Handle the case when the handle is not yet created (defer the UI update or perform other action)
                    // For instance, you could queue the update to be executed later when the handle is created.
                }
            }
        }
       

        private string receivedMessage = string.Empty; // Define a variable to hold the received message

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        // Method to update the received message
        private void UpdateReceivedMessage(string message)
        {
            receivedMessage = message;

            // Check if the message contains a password field
            if (receivedMessage.Contains("password"))
            {
                // Find the index of "password"
                int startIndex = receivedMessage.IndexOf("password");

                if (startIndex != -1) // Check if "password" exists in the message
                {
                    startIndex += "password".Length + 3; // Assuming "password" is followed by ": " in the message

                    // Find the index of the comma after the password value
                    int endIndex = receivedMessage.IndexOf(",", startIndex);

                    if (endIndex == -1) // If comma not found, use the end of the string as endIndex
                    {
                        endIndex = receivedMessage.Length;
                    }

                    // Get the substring representing the password value
                    string passwordValue = receivedMessage.Substring(startIndex, endIndex - startIndex);
                    string hashedPassword = HashPassword(passwordValue);

                    // Replace the password value with its hashed representation
                    receivedMessage = receivedMessage.Replace(passwordValue, hashedPassword);
                }
            }

            // Remove previous message labels from the panel
            messagePanel.Controls.Clear();

            // Create a label to display the modified message text
            Label messageLabel = new Label();
            messageLabel.Text = "Latest message from server: " + receivedMessage;
            messageLabel.AutoSize = false;
            messageLabel.Width = messagePanel.Width - 20; // Adjust the width to fit within the panel
            messageLabel.TextAlign = ContentAlignment.TopLeft; // Align text to the top-left corner
            messageLabel.MaximumSize = new Size(messagePanel.Width, 0); // Allow vertical wrapping
            messageLabel.Height = TextRenderer.MeasureText(messageLabel.Text, messageLabel.Font, messageLabel.MaximumSize).Height; // Calculate label height based on text content

            // Add the label to the scrollable panel
            messagePanel.Controls.Add(messageLabel);

            // Ensure the scroll position is at the bottom to see the latest message
            messagePanel.VerticalScroll.Value = messagePanel.VerticalScroll.Maximum;

            // Invalidate the container to update the scrollbars
            messagePanel.Invalidate();
        }





        private void Sending_msg()
        {

            string broker = "broker.hivemq.com";
            int port = 1883;
            string topic = "Csharp/mqtt-mu-test1";
            string clientId = Guid.NewGuid().ToString();
            string username = ""; // Your MQTT username
            string password = ""; // Your MQTT password

            //MqttClient client = ConnectMQTT(broker, port, clientId, username, password);
            MqttClient client = new MqttClient(broker, port, false, null, null, MqttSslProtocols.None);
            client.Connect(clientId, username, password);

            if (client.IsConnected)
            {
                var message = new
                {
                    username = textBox1.Text,
                    password = textBox2.Text
                };

                string jsonMessage = JsonConvert.SerializeObject(message);
                byte[] byteArrayMessage = Encoding.UTF8.GetBytes(jsonMessage);
                client.Publish(topic, byteArrayMessage, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                //Publish(client, topic, Encoding.UTF8.GetBytes(jsonMessage));
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Sending_msg();
        }

       

        private void Form5_Resize(object sender, EventArgs e)
        {
            int newWidth = (int)(this.Width * messagePanelWidthPercentage);
            int newHeight = (int)(this.Height * messagePanelHeightPercentage);
            // Set the new size for the panel
            messagePanel.Size = new Size(newWidth, newHeight);

        }

       

       


        private void HandleEnterKeyPress()
        {
            // Your logic here when Enter key is pressed in the textbox
            //MessageBox.Show("Enter key was pressed in the textbox!");
            // Add your specific action or method call here
            Sending_msg();
        }

        private void textBox2_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {

                //MessageBox.Show("detect ");
                // Call your method or perform the desired action here
                HandleEnterKeyPress();
            }
        }

        private void messagePanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }

    }
