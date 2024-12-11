using System;
using System.Collections.Generic;

using System.Drawing;

using System.IO;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Speech.Synthesis;
using Microsoft.Win32;
using System.Text.RegularExpressions;

using YourNamespace;
using System.Net.Sockets;
using System.Net;

using LiveCharts;

using LiveCharts.Wpf;


namespace Mu
{


    public partial class Form1 : Form
    {
        private readonly string logFilePath = @"C:\Users\USER\Desktop\c# app\Mu.Project2\Mu.Project2\logger\looger.log";
        private FileSystemWatcher fileWatcher;
        private NotifyIcon notifyIcon;
        Form2 secondForm;
        Form3 thirdForm;
        Form4 forthForm;
        Form5 fifthForm;
        private UdpClient udpListener;

        public Form1()
        {
            InitializeComponent();
            InitializeNotifyIcon();
            InitializeForm();
            InitializeFileWatcher();
            DisplayPieChart();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        


            // Subscribe to the FormClosing event
            this.FormClosing += Form1_FormClosing;

            button3.Visible = false;
            AttachButtonEventHandlers(this);
            try
            {
                udpListener = new UdpClient(9091);
                udpListener.EnableBroadcast = true;
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Error: Port 9091 is already in use. {ex.Message}");
                // Handle the error or choose a different port
            }


        }


        /////////////////////chart Start 
        private void InitializeFileWatcher()
        {
            string directoryPath = Path.GetDirectoryName(logFilePath);
            fileWatcher = new FileSystemWatcher(directoryPath)
            {
                Filter = Path.GetFileName(logFilePath),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size
            };

            fileWatcher.Changed += (s, e) => DisplayPieChart();
            fileWatcher.EnableRaisingEvents = true;
        }

        private void DisplayPieChart()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(DisplayPieChart));
                return;
            }

            var buttonUsage = GetButtonUsage();

            pieChart1.Series = new SeriesCollection();

            foreach (var button in buttonUsage)
            {
                pieChart1.Series.Add(new PieSeries
                {
                    Title = button.Key,
                    Values = new ChartValues<int> { button.Value },
                    DataLabels = true
                });
            }
        }

        private Dictionary<string, int> GetButtonUsage()
        {
            var buttonUsage = new Dictionary<string, int>();

            if (File.Exists(logFilePath))
            {
                var logLines = File.ReadAllLines(logFilePath);

                foreach (var line in logLines)
                {
                    var parts = line.Split(',');

                    string buttonText = null;
                    foreach (var part in parts)
                    {
                        if (part.Contains("Button_Text:"))
                        {
                            buttonText = part.Split(':').Last().Trim();
                            break;
                        }
                    }

                    if (buttonText != null)
                    {
                        if (buttonUsage.ContainsKey(buttonText))
                        {
                            buttonUsage[buttonText]++;
                        }
                        else
                        {
                            buttonUsage[buttonText] = 1;
                        }
                    }
                }
            }

            return buttonUsage;
        }











        /// <summary>
        /// /////////start of log with adding AttachButtonEventHandlers(this); to public
        /// </summary>
        /// <param name="parent"></param>
        private void AttachButtonEventHandlers(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Button button)
                {
                    button.Click += new EventHandler(Button_Click);
                }
                else if (control.HasChildren)
                {
                    AttachButtonEventHandlers(control); // Recursively attach to child controls
                }
            }
        }



        private void Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                Logger.Log($"Button_Name: {button.Name},Button_Text: {button.Text} clicked.");
            }
        }

        /// <summary>
        /// ////////end of log
        /// </summary>
        /// 
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Dispose of the UDP listener when the form is closing
            if (udpListener != null)
            {
                udpListener.Close();
                udpListener.Dispose();
            }

            // Terminate the application process when the form is closing
            //Environment.Exit(0);
        }

        private void InitializeForm()
        {
            // Set the form's properties
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // Set the form border style to fixed
            this.MaximizeBox = false; // Disable maximizing the form
            //this.StartPosition = FormStartPosition.CenterScreen; // Center the form on startup
            //this.Resize += MainForm_Resize; // Subscribe to the Resize event
           // pictureBox2.Image = Image.FromFile(@"C:\Users\USER\Desktop\c# app\Mu.Project2\Mu.Project2\icons\logo2.gif");
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            // Check if the event source is the form itself
            if (sender == this)
            {
                // Reset the form size if the user attempts to resize it
                this.Size = new System.Drawing.Size(400, 300); // Set your desired size here
            }
        }
        private void InitializeNotifyIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Information;
            notifyIcon.Visible = true;
            notifyIcon.Click += NotifyIcon_Click;
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            // Handle click event
            // You can open your application or perform other actions here
        }

        private void ShowNotification(string message)
        {
            notifyIcon.BalloonTipTitle = "Notification";
            notifyIcon.BalloonTipText = message;
            notifyIcon.ShowBalloonTip(3000); // Display the notification for 3 seconds
        }

        new int count = 0;
        private void button1_Click(object sender, EventArgs e)
        {


            count++;
            if (count % 2 != 0)
            {
                button3.Text = "Clicked ODD";
                button3.ForeColor = System.Drawing.Color.Red;
                button3.BackColor = System.Drawing.Color.Pink;
                secondForm = new Form2();
                secondForm.Show();
                System.Threading.Thread.Sleep(1000);
                this.Hide();


                CreateDynamicButton("Test 1");




            }
            else
            {
                button3.Text = "Clicked EVEN";
                button3.ForeColor = System.Drawing.Color.Blue;
                button3.BackColor = System.Drawing.Color.White;
                this.Show();
                //SplashScreenManager.ShowForm(typeof(WaitForm1));
                //this.Shown += new System.EventHandler(this.Show());
                secondForm = new Form2();
                System.Threading.Thread.Sleep(1000);
                secondForm.Hide();




            }
        }

        public static class GlobalVariables
        {
            public static string ReceivedIpAddress { get; set; }
        }








        int @boot = 0;

        private async void Form1_MouseEnter(object sender, EventArgs e)
        {

            if (@boot == 0)
            {


                //CreateDynamicButton("Test 2");
                @boot++;
                foreach (var button in this.Controls.OfType<Button>())
                {
                    button.Show();
                    //await button.Scale(220, 680);
                    //button.BackgroundImage = imageList1.Images[0];

                    //button.Size = button.BackgroundImage.Size;



                    using (SpeechSynthesizer synth = new SpeechSynthesizer())
                    {
                        synth.SelectVoice("Microsoft Zira Desktop");
                        //synth.Speak(button.Name+ "has been resized");
                        synth.SpeakAsync(button.Name + "has been resized");




                    }

                    //button.MouseLeave += createButton_MouseEnter;
                }

                foreach (var picturebox in this.Controls.OfType<PictureBox>())
                {
                    picturebox.BackgroundImage = imageList2.Images[0];
                    picturebox.Size = picturebox.BackgroundImage.Size;

                }
            }

        }




        //***************************************************************** build Dynamic Button
        private void CreateDynamicButton(string Text)
        {

            // Create a Button object  
            Button dynamicButton = new Button();

            // Set Button properties  
            dynamicButton.Height = 40;
            dynamicButton.Width = 300;
            dynamicButton.BackColor = System.Drawing.Color.Red;
            dynamicButton.ForeColor = System.Drawing.Color.Blue;
            dynamicButton.Location = new Point(20, 150);
            //dynamicButton.Text = "I am Dynamic Button";
            dynamicButton.Text = Text;
            dynamicButton.Name = "DynamicButton";
            dynamicButton.Font = new Font("Georgia", 16);

            // Add a Button Click Event handler  
            dynamicButton.Click += new EventHandler(DynamicButton_Click);
            dynamicButton.BackgroundImage = imageList1.Images[0];

            dynamicButton.Size = dynamicButton.BackgroundImage.Size;

            // Add Button to the Form. Placement of the Button  
            // will be based on the Location and Size of button  
            Controls.Add(dynamicButton);
            if (@boot == 0)
            {
                using (SpeechSynthesizer synth = new SpeechSynthesizer())
                {
                    //synth.SelectVoice("Microsoft Zira Desktop");
                    //synth.Speak("Event detected , Button 2 is generated");

                }
            }

        }
      
        private void DynamicButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Dynamic button is clicked");
        }



        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Load the GIF into PictureBox
           


            //pictureBox2.Image = Image.FromFile(@"C:\Users\USER\Desktop\c# app\Mu.Project2\Mu.Project2\icons\logo2.gif");
            //AnimateLogoFullScreen();
        }

        private void AnimateLogoFullScreen()
        {
            // Hide all other controls
            foreach (Control control in Controls)
            {
                if (control != pictureBox2)
                {
                    control.Visible = false;
                }
            }

            // Set pictureBox2 to full screen
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Location = new Point(0, 0);
            pictureBox2.Size = ClientSize;
            

            // Start animation
            Timer timer = new Timer();
            timer.Interval = 20; // Adjust interval for smoother animation
            timer.Tick += (sender, e) =>
            {
                int targetWidth = ClientSize.Width;
                int targetHeight = ClientSize.Height;
                if (pictureBox2.Width < targetWidth)
                {
                    pictureBox2.Width += Math.Max(1, (targetWidth - pictureBox2.Width) / 10);
                }
                if (pictureBox2.Height < targetHeight)
                {
                    pictureBox2.Height += Math.Max(1, (targetHeight - pictureBox2.Height) / 10);
                }
                if (pictureBox2.Width >= targetWidth && pictureBox2.Height >= targetHeight)
                {
                    // Animation complete
                    timer.Stop();
                    // Reset after 5 seconds
                    Task.Delay(5000).ContinueWith(_ =>
                    {
                        
                        // Reset to default state
                        ResetToDefault();
                    }, TaskScheduler.FromCurrentSynchronizationContext());

                }
            };
            timer.Start();
        }

        private void ResetToDefault()
        {
            // Show all controls
            foreach (Control control in Controls)
            {
                control.Visible = true;
            }
            // Reset PictureBox size and location
            pictureBox2.Size = new Size(100, 100); // Adjust to the original size
            pictureBox2.Location = new Point(10, 10); // Adjust to the original location
        }
















        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            secondForm.Show();
            System.Threading.Thread.Sleep(1000);
            this.Hide();
        }




            private void SetStartup()
        {
            //regedit
            //Computer\HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run

            RegistryKey rk = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            string AppName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            if (checkBox1.Checked)
            {
                // rk.SetValue(AppName, Application.ExecutablePath);
                MessageBox.Show(AppName + '\n' + (Application.ExecutablePath).ToString());
                //string pattern = @"[^%](?<target>\/+)";
                string pattern = @"(?<target>\/+)";
                string replacement = "\\";
                //string input = "$16.32 12.19 £16.29 €18.29  €18,29";
                string input = Application.ExecutablePath;
                string result = "\"" + Regex.Replace(input, pattern, replacement) + "\"";
                Console.WriteLine(result);
                rk.SetValue(AppName, result);

                //How do I copy the contents of a String to the clipboard in C#? [duplicate]
                System.Windows.Forms.Clipboard.SetText("Computer\\HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");


                //Run Command Prompt Commands
                string strCmdText;
                strCmdText = "/K regedit"; //use /K to type in cmd
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);




                using (SpeechSynthesizer synth = new SpeechSynthesizer())
                {
                    synth.SelectVoice("Microsoft Zira Desktop");
                    synth.Speak("using Perl Regex to change slash to Backslash");
                    synth.Speak("your destination has been copied to your clipboard, please paste it in directory, and search for the a register called : " + AppName);
                }
            }
            else
                rk.DeleteValue(AppName, false);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SetStartup();
        }

        private void Form1_MouseHover(object sender, EventArgs e)
        {

        }




        //if mouse houver a button add event and activate void
        private void button3_MouseHover(object sender, EventArgs e)
        {
            this.button3.MouseLeave += new System.EventHandler(this.allButtons_MouseLeave);
            this.button3.MouseEnter += new System.EventHandler(this.allButtons_MouseEnter);
        }
        // enter handler
        private void allButtons_MouseEnter(object sender, System.EventArgs e)
        {
            Button btn = (Button)sender;
            btn.BackColor = System.Drawing.Color.Cyan;
            btn.Font = new Font(btn.Font.Name, btn.Font.Size, FontStyle.Bold);
        }

        // leave handler
        private void allButtons_MouseLeave(object sender, System.EventArgs e)
        {
            Button btn = (Button)sender;
            btn.BackColor = System.Drawing.Color.DeepPink; // whatever your original color was
            btn.Font = new Font(btn.Font.Name, btn.Font.Size, FontStyle.Regular);
        }

        private void buttonOpenForm3_Click_1(object sender, EventArgs e)
        {
            // Open Form3 when the button is clicked

            thirdForm = new Form3();

            //thirdForm.FetchDataFromMySQL(); // Call method to fetch data from MySQL
            thirdForm.Show();
        }

        private void toForm4_Click(object sender, EventArgs e)
        {
            // Open Form3 when the button is clicked

            forthForm = new Form4();

            //thirdForm.FetchDataFromMySQL(); // Call method to fetch data from MySQL
            forthForm.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            fifthForm = new Form5();

            //thirdForm.FetchDataFromMySQL(); // Call method to fetch data from MySQL
            fifthForm.Show();
        }



        ///////////////////////////////////////////////////////////////////////////
        ///




















        private int posX;
        private int destX = 1283;
        private int posY;
        private int destY = 26;
        private int pos1Y;
        private int dest2Y = 91;
        private int pos2Y;
        private int dest3Y = 179;
        private int pos3Y;
        private int dest4Y = 243;

        private int button3PosX;
        private int button3PosY;
        private int button2PosX;
        private int button2PosY;
        private int toForm4PosX;
        private int toForm4PosY;
        private int buttonOpenForm3PosX;
        private int buttonOpenForm3PosY;

        public Timer timerButton3;
        public Timer timerButtonOpenForm3;
        public Timer timerButton2;
        public Timer timerToForm4;


        private void MoveButton(Button button, int destinationX, int destinationY, ref Timer timer)
        {
            Timer localTimer = new Timer();
            timer = localTimer; // Update the reference with the new Timer instance
            localTimer.Interval = 10; // Change the interval as needed for desired animation speed

            int posX = button.Location.X;
            int posY = button.Location.Y;

            localTimer.Tick += (sender, e) =>
            {
                int moveX = destinationX - posX;
                int moveY = destinationY - posY;

                posX += (moveX > 0) ? Math.Min(125, moveX) : Math.Max(-125, moveX);
                posY += (moveY > 0) ? Math.Min(125, moveY) : Math.Max(-125, moveY);

                button.Location = new Point(posX, posY);

                if (posX == destinationX && posY == destinationY)
                {
                    localTimer.Stop();
                }
            };
            localTimer.Start();
        }





        private void SetInitialPositions()
        {
            buttonOpenForm3PosX = buttonOpenForm3.Location.X;
            // MessageBox.Show("the buttonOpenForm3PosX position X is " + buttonOpenForm3PosX);
            buttonOpenForm3PosY = buttonOpenForm3.Location.Y;

            button2PosX = button2.Location.X;
            button2PosY = button2.Location.Y;

            button3PosX = button3.Location.X;
            button3PosY = button3.Location.Y;
            toForm4PosX = toForm4.Location.X;
            toForm4PosY = toForm4.Location.Y;
        }


        private void ResetButtonPositions()
        {

            timerButton3?.Stop();
            timerButtonOpenForm3?.Stop();
            timerButton2?.Stop();
            timerToForm4?.Stop();
            // Reset the positions of the buttons to their initial positions
            button3.Location = new Point(button3PosX, button3PosY);
            buttonOpenForm3.Location = new Point(buttonOpenForm3PosX, buttonOpenForm3PosY);
            button2.Location = new Point(button2PosX, button2PosY);
            toForm4.Location = new Point(toForm4PosX, toForm4PosY);
            // Reset positions for other buttons...

        }





        private bool isButtonClicked = false;
        private void button4_Click_1(object sender, EventArgs e)
        {
            if (!isButtonClicked)
            {

                isButtonClicked = true;
                SetInitialPositions();
                MoveButton(button3, destX, destY, ref timerButton3);
                MoveButton(buttonOpenForm3, destX, dest2Y, ref timerButtonOpenForm3);
                MoveButton(button2, destX, dest3Y, ref timerButton2);
                MoveButton(toForm4, destX, dest4Y, ref timerToForm4);
                //MessageBox.Show("clicked");
            }
            else
            {
                ResetButtonPositions();
                isButtonClicked = false;
                //MessageBox.Show("not clicked");
            }
        }

     

        private async Task StartUdpListener()
        {
            try
            {
                Console.WriteLine("Waiting for broadcast messages...");

                // Listen for broadcast messages
                while (true)
                {
                    // Receive the broadcast message
                    UdpReceiveResult result = await udpListener.ReceiveAsync();
                    string receivedMessage = Encoding.ASCII.GetString(result.Buffer);

                    // Print the received message
                    Console.WriteLine("Received broadcast message: " + receivedMessage);

                    // Update UI with the received message (if needed)
                    UpdateUIWithReceivedMessage(receivedMessage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        private void UpdateUIWithReceivedMessage(string message)
        {
            // Update the UI with the received message
            // For example, display it in a label or textbox
            // Here you can add your logic to update the UI as needed
            // For simplicity, let's just display it in a message box
            //MessageBox.Show("Received broadcast message: " + message);
            ShowNotification("Received broadcast message: " + message);
            string receivedIpAddress = message; // Example IP address, replace with actual value
            GlobalVariables.ReceivedIpAddress = receivedIpAddress;
        }



        static void SendBroadcastMessage()
        {
            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.EnableBroadcast = true;

                // Define the port number to send broadcast messages
                int port = 9090;

                // Define the broadcast IP address and port
                IPAddress broadcastAddress = IPAddress.Parse("255.255.255.255");
                IPEndPoint broadcastEndPoint = new IPEndPoint(broadcastAddress, port);

                // Get local IP address
                string ipAddress = GetLocalIPAddress();

                try
                {
                    // Create the message to send (include IP address)
                    string messageToSend = "Broadcast message from " + ipAddress;
                    byte[] sendData = Encoding.ASCII.GetBytes(messageToSend);

                    // Send the broadcast message
                    udpClient.Send(sendData, sendData.Length, broadcastEndPoint);
                    Console.WriteLine("Broadcast message sent.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
            }
        }

        static string GetLocalIPAddress()
        {
            // Get local IP address
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP address not found.");
        }


        private async void button1_Click_2(object sender, EventArgs e)
        {
            try
            {
                SendBroadcastMessage();

                // Start listening for broadcast messages asynchronously
                await StartUdpListener(); // Use await to asynchronously wait for StartUdpListener to finish
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}

