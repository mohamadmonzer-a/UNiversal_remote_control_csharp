using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using Mu;
using Nest;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;


namespace YourNamespace
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
            InitializeForm();
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

        private void Form4_Load(object sender, EventArgs e)
        {
            // Load instructors' names when the form loads
            RetrieveInstructors();
        }



        //start.Arguments = @"""c:\Users\Silver\Desktop\c# app\Mu.Project2\Mu.Project2\python_to_get_names_and_set_to_data_mysqlWorkbanch\python-to-send-name-image-to-cSharp.py""";
        private void RetrieveInstructors()
        {
            //got phpmyadmin to sql to grant permissions:
            //GRANT ALL PRIVILEGES ON seniorproject.* TO 'root'@'%';
            //GRANT ALL PRIVILEGES ON seniorproject.ir_remote TO 'root'@'%';

            ProcessStartInfo start = new ProcessStartInfo();
            string ipAddress_raspi = Form1.GlobalVariables.ReceivedIpAddress;
            //MessageBox.Show(ipAddress_raspi);
            string pythonScriptPath = @"C:\Users\USER\Desktop\c# app\Mu.Project2\Mu.Project2\python_to_get_names_and_set_to_data_phpmyadmin\python-to-send-name-image-to-cSharp.py";

            start.FileName = "python.exe"; // Path to Python interpreter
            start.Arguments = $"\"{pythonScriptPath}\" \"{ipAddress_raspi}\""; // Pass the IP address as an argument

            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            start.CreateNoWindow = true;

            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine("Result from Python script:");
                    Console.WriteLine(result); // Output the result to console for debugging
                    //MessageBox.Show(result);

                    string[] instructors = result.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    Console.WriteLine("Instructors obtained:");
                    foreach (string instructor in instructors)
                    {
                        Console.WriteLine(instructor); // Output each instructor to console for debugging
                    }

                    comboBoxInstructors.Items.Clear();
                    comboBoxInstructors.Items.AddRange(instructors);
                }
            }
        }


        private void DisplayTextBoxContent()
        {
            // Get the selected item from the ComboBox
            string selectedText = comboBoxInstructors.SelectedItem.ToString();

            if (comboBoxInstructors.SelectedItem != null)
            {
                string selectedInstructor = comboBoxInstructors.SelectedItem.ToString();
                string basePath = @"C:\Users\USER\Desktop\c# app\Mu.Project2\Mu.Project2\python_to_get_names_and_set_to_data_phpmyadmin\images";
                //string imagePath = Path.Combine(basePath, selectedInstructor + ".jpg");
                //pictureBox.ImageLocation = imagePath;

                string infoPath = Path.Combine(basePath, selectedInstructor + ".txt");

                if (File.Exists(infoPath))
                {
                    string instructorInfo = File.ReadAllText(infoPath);

                    // Create a TextBox to display the text content
                    TextBox textBox = new TextBox();
                    textBox.Multiline = true;
                    textBox.ReadOnly = true;
                    textBox.ScrollBars = ScrollBars.Vertical;
                    textBox.Size = panel1.ClientSize;

                    textBox.Text = instructorInfo;

                    // Clear the panel and add the TextBox to the panel
                    panel1.Controls.Clear();
                    panel1.Controls.Add(textBox);
                    panel1.Visible = true;
                }
                else
                {
                    MessageBox.Show("Information file not found.");
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            RetrieveInstructors();
        }
        private string selectedInstructor;
        private void comboBoxInstructors_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            // Get the selected item from the ComboBox
            selectedInstructor = comboBoxInstructors.SelectedItem?.ToString();

            // Check if an instructor is selected
            if (!string.IsNullOrEmpty(selectedInstructor))
            {
                string basePath = @"C:\Users\USER\Desktop\c# app\Mu.Project2\Mu.Project2\python_to_get_names_and_set_to_data_phpmyadmin\images";
                string imagePath = Path.Combine(basePath, selectedInstructor + ".jpg");

                // Load and display the image in PictureBox
                pictureBox.ImageLocation = imagePath;
            }
        }








        private int posX;
        private int posY;
        private int destX = 58; // Destination X position
        private int destY = 192; // Destination Y position
        private int comboBoxPosX;
        private int comboBoxPosY;
        private int buttonPosX;
        private int buttonPosY;
        private int destComboBoxX = 75; // Destination X position for ComboBox
        private int destComboBoxY = 22; // Destination Y position for ComboBox
        private int destButtonX = 121; // Destination X position for Button
        private int destButtonY = 71; // Destination Y position for Button

        private void SetInitialPosition(int initialX, int initialY)
        {
            comboBoxPosX = comboBoxInstructors.Location.X;
            comboBoxPosY = comboBoxInstructors.Location.Y;

            buttonPosX = button1.Location.X;
            buttonPosY = button1.Location.Y;


            posX = initialX;
            posY = initialY;
        }

        private void MoveComboBoxToDestination()
        {
            Timer timer = new Timer();
            timer.Interval = 50; // Change the interval as needed for desired animation speed
            timer.Tick += (sender, e) =>
            {
                int moveX = destComboBoxX - comboBoxPosX;
                int moveY = destComboBoxY - comboBoxPosY;

                comboBoxPosX += (moveX > 0) ? Math.Min(5, moveX) : Math.Max(-5, moveX);
                comboBoxPosY += (moveY > 0) ? Math.Min(5, moveY) : Math.Max(-5, moveY);

                comboBoxInstructors.Location = new Point(comboBoxPosX, comboBoxPosY);

                if (comboBoxPosX == destComboBoxX && comboBoxPosY == destComboBoxY)
                {
                    timer.Stop();
                }
            };
            timer.Start();
        }

        private void MoveButtonToDestination()
        {
            Timer timer = new Timer();
            timer.Interval = 50; // Change the interval as needed for desired animation speed
            timer.Tick += (sender, e) =>
            {
                int moveX = destButtonX - buttonPosX;
                int moveY = destButtonY - buttonPosY;

                buttonPosX += (moveX > 0) ? Math.Min(5, moveX) : Math.Max(-5, moveX);
                buttonPosY += (moveY > 0) ? Math.Min(5, moveY) : Math.Max(-5, moveY);

                button1.Location = new Point(buttonPosX, buttonPosY);

                if (buttonPosX == destButtonX && buttonPosY == destButtonY)
                {
                    timer.Stop();
                }
            };
            timer.Start();
        }








        private void MovePictureBox(int destinationX, int destinationY)
        {
            Timer timer = new Timer();
            timer.Interval = 50; // Change the interval as needed for desired animation speed
            timer.Tick += (sender, e) =>
            {
                int moveX = destinationX - posX;
                int moveY = destinationY - posY;

                posX += (moveX > 0) ? Math.Min(5, moveX) : Math.Max(-5, moveX);
                posY += (moveY > 0) ? Math.Min(5, moveY) : Math.Max(-5, moveY);

                pictureBox.Location = new Point(posX, posY);

                if (posX == destinationX && posY == destinationY)
                {
                    timer.Stop();
                }
            };
            timer.Start();
        }






        private void pictureBox_Click(object sender, EventArgs e)
        {
            SetInitialPosition(pictureBox.Location.X, pictureBox.Location.Y);
            MovePictureBox(destX, destY); // Move PictureBox
            MoveComboBoxToDestination(); // Move ComboBox
            MoveButtonToDestination(); // Move Button
            DisplayTextBoxContent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show(selectedInstructor);
            if (!string.IsNullOrEmpty(selectedInstructor))
            {
                
                // Open a web browser with the LinkedIn search URL for the selected instructor
                System.Diagnostics.Process.Start("https://www.google.com/search?q=LinkedIn " + selectedInstructor);
            }
        }



    }
}
