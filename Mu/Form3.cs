using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using System.Net;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;




using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;






namespace Mu
{
    public partial class Form3 : Form
    {
        
        private DataTable dataTable = new DataTable();
        private DataGridView dataGridView1;
        private readonly HttpListener listener = new HttpListener();
        private MqttClient mqttClient;

        public Form3()
        {
            
            InitializeComponent();
            InitializeMqttClient();
            SubscribeToTopic();
            SetupDataGridView();
            InitializeForm();
            //string pythonExePath = @"C:\Users\Silver\Desktop\c# app\Mu.Project2\Mu.Project2\python_maria_db_remotes\get_remote_code.py";

        }


        private void InitializeMqttClient()
        {
            try
            {
                mqttClient = new MqttClient("broker.hivemq.com");

                // register to message received
                mqttClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

                string clientId = Guid.NewGuid().ToString();
                mqttClient.Connect(clientId);
            }
            catch (uPLibrary.Networking.M2Mqtt.Exceptions.MqttConnectionException ex)
            {
                // Display the exception message in a message box
                MessageBox.Show("MQTT Connection Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SubscribeToTopic()
        {
            mqttClient.Subscribe(new string[] { "Csharp/mqtt-mu-test2" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Message);
            UpdateLabel(message);
        }

        private void UpdateLabel(string message)
        {
            if (label9.InvokeRequired)
            {
                label9.Invoke((MethodInvoker)delegate
                {
                    label9.Text = message;
                });
            }
            else
            {
                label9.Text = message;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                mqttClient.Disconnect();
            }
            catch (uPLibrary.Networking.M2Mqtt.Exceptions.MqttCommunicationException ex)
            {
                // Display the exception message in a message box
                MessageBox.Show("MQTT Communication Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            base.OnFormClosing(e);
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

        private void button1_Click(object sender, EventArgs e)
        {
            string manufacturer = textBox1Database.Text;
            if (!manufacturer.Contains("_"))
            {
                // If it doesn't contain an underscore, add an underscore to it
                manufacturer += "_";
                textBox1Database.Text = manufacturer;
                label3.Text = "please enter manufacturer_type as <Brand>_<type> '\n' ex : lg_dvd ";
                label3.Visible = true;
                // MessageBox.Show("please enter manufacturer_type as <Brand>_<type> '\n' ex : lg_dvd ");
            }
            else
            {
                label3.Visible = false;
            }
                string buttonKey = button_key.Text;

            ExecutePythonScript(manufacturer, buttonKey);
        }

        private void ExecutePythonScript(string manufacturer, string buttonKey)
        {
            try
            {
                ProcessStartInfo start = new ProcessStartInfo();
                string ipAddress_raspi = Form1.GlobalVariables.ReceivedIpAddress;

                start.FileName = "python"; // Assumes python is in PATH environment variable
                string pythonScriptPath = @"C:\Users\USER\Desktop\c# app\Mu.Project2\Mu.Project2\python_maria_db_remotes\get_remote_code2.py";
                string arguments = $"\"{pythonScriptPath}\" \"{ipAddress_raspi}\" \"{manufacturer}\" \"{buttonKey}\"";
                // Print the Python script path and arguments
                Console.WriteLine("Python Script Path: " + pythonScriptPath);
                Console.WriteLine("Arguments: " + arguments);

                start.Arguments = arguments;

                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                start.CreateNoWindow = true;

                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        // Parse the Python script output
                        ParsePythonOutput(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error executing Python script: " + ex.Message);
            }
        }
        /*
        private void ParsePythonOutput(string output)
        {
            // Split the output into lines
            string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            // Iterate through each line
            foreach (string line in lines)
            {
                // Print each line to the console
                Console.WriteLine(line);
            }
        }
        */

        private void SetupDataGridView()
        {
            // Add the "Save" button column to the DataGridView
            DataGridViewButtonColumn saveButtonColumn = new DataGridViewButtonColumn();
            saveButtonColumn.HeaderText = "Save";
            saveButtonColumn.Name = "SaveButtonColumn";
            saveButtonColumn.Text = "Save";
            saveButtonColumn.UseColumnTextForButtonValue = true;
            dataGridView2.Columns.Add(saveButtonColumn);

            // Subscribe to the CellContentClick event
            dataGridView2.CellContentClick += DataGridView_CellContentClick;
        }
        public static class InputBox
        {
            public static string Show(string prompt)
            {
                Form promptForm = new Form();
                promptForm.Width = 500;
                promptForm.Height = 150;
                promptForm.Text = prompt;
                Label textLabel = new Label() { Left = 50, Top = 20, Text = prompt };
                TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
                Button confirmation = new Button() { Text = "OK", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
                confirmation.Click += (sender, e) => { promptForm.Close(); };
                promptForm.Controls.Add(textBox);
                promptForm.Controls.Add(confirmation);
                promptForm.Controls.Add(textLabel);
                promptForm.AcceptButton = confirmation;

                return promptForm.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            }
        }
        private void DataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is in the "Save" button column
            if (e.ColumnIndex == dataGridView2.Columns["SaveButtonColumn"].Index && e.RowIndex >= 0)
            {
                // Prompt the user to enter the command
                string userCommand = InputBox.Show("Enter the command:");

                // If the user entered a command, construct the URL with the command and row data
                if (!string.IsNullOrEmpty(userCommand))
                {
                    try
                    {
                        // Get the data from the clicked row
                        string remoteName = dataGridView2.Rows[e.RowIndex].Cells["Remote"].Value.ToString();
                        string button = dataGridView2.Rows[e.RowIndex].Cells["Button"].Value.ToString();
                        string protocol = dataGridView2.Rows[e.RowIndex].Cells["Protocol"].Value.ToString();
                        string parameter1 = dataGridView2.Rows[e.RowIndex].Cells["Parameter1"].Value.ToString();
                        string parameter2 = dataGridView2.Rows[e.RowIndex].Cells["Parameter2"].Value.ToString();
                        string parameter3 = dataGridView2.Rows[e.RowIndex].Cells["Parameter3"].Value.ToString();

                        // Get the globally stored IP address
                        //string ipAddress = GlobalVariables.ReceivedIpAddress;


                        string ipAddress_raspi = Form1.GlobalVariables.ReceivedIpAddress;
                        // Construct the URL string with the IP address
                        string url = $"http://{ipAddress_raspi}:1880/save_command?remote={remoteName}&button={button}&protocol={protocol}&param1={parameter1}&param2={parameter2}&param3={parameter3}&userCommand={userCommand}";

                        // Send a request to the URL
                        using (var client = new WebClient())
                        {
                            try
                            {
                                string response = client.DownloadString(url);
                                // Optionally handle the response if needed
                                MessageBox.Show("Command saved successfully.");
                            }
                            catch (WebException ex)
                            {
                                // Handle any exceptions
                                MessageBox.Show($"Error saving command: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                    }
                }
            }
        }
        private void ParsePythonOutput(string output)
        {
            // Split the output into lines
            string[] lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                // Print each line to the console
                Console.WriteLine(line);
            }
            // Create a new DataTable
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Remote", typeof(string));
            dataTable.Columns.Add("Button", typeof(string));
            dataTable.Columns.Add("Protocol", typeof(string));
            dataTable.Columns.Add("Parameter1", typeof(string));
            dataTable.Columns.Add("Parameter2", typeof(string));
            dataTable.Columns.Add("Parameter3", typeof(string));

            // Iterate through each line
            foreach (string line in lines)
            {
                // Split the line by colon (:) to separate remote name and button info
                string[] parts = line.Split(':');

                // Ensure that parts has at least two elements before accessing parts[1]
                if (parts.Length >= 2)
                {
                    string remoteName = parts[0].Trim();
                    string buttonInfo = parts[1].Trim();

                    // Split the button info by comma (,) to extract button details
                    string[] buttonDetails = buttonInfo.Split(',');

                    // Ensure that buttonDetails array has at least five elements
                    if (buttonDetails.Length >= 5)
                    {
                        // Remove parentheses from the parsed strings
                        remoteName = remoteName.Replace("(", "").Replace(")", "");
                        for (int i = 0; i < 5; i++)
                        {
                            buttonDetails[i] = buttonDetails[i].Replace("(", "").Replace(")", "");
                        }
                        dataTable.Rows.Add(remoteName, buttonDetails[0].Trim(), buttonDetails[1].Trim(), buttonDetails[2].Trim(), buttonDetails[3].Trim(), buttonDetails[4].Trim());
                    }
                    else
                    {
                        // Log or handle the case where buttonDetails array doesn't have enough elements
                        Console.WriteLine($"Unexpected button info format: {buttonInfo}");
                    }
                }
                else
                {
                    // Log or handle the case where the line doesn't contain a colon
                    Console.WriteLine($"Line does not contain expected format: {line}");
                }
            }

            // Bind the DataTable to the DataGridView
            dataGridView2.DataSource = dataTable;
        }

        

        private void dataGridView2_SelectionChanged_1(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];

                string remoteName = selectedRow.Cells["Remote"].Value.ToString();
                string button = selectedRow.Cells["Button"].Value.ToString();
                string protocol = selectedRow.Cells["Protocol"].Value.ToString();
                string parameter1 = selectedRow.Cells["Parameter1"].Value.ToString();
                string parameter2 = selectedRow.Cells["Parameter2"].Value.ToString();
                string parameter3 = selectedRow.Cells["Parameter3"].Value.ToString();

                // Construct the URL with the row data
                string ipAddress_raspi = Form1.GlobalVariables.ReceivedIpAddress;
                //string url = $"http://yourapi.com?remote={remoteName}&button={button}&protocol={protocol}&param1={parameter1}&param2={parameter2}&param3={parameter3}";
                var url = $"http://{ipAddress_raspi}:1880/code_ir?remote={remoteName}&button={button}&protocol={protocol}&param1={parameter1}&param2={parameter2}&param3={parameter3}";
            


            // Send a request to the URL
            using (var client = new WebClient())
                {
                    try
                    {
                        string response = client.DownloadString(url);
                        // Optionally handle the response if needed
                    }
                    catch (WebException ex)
                    {
                        // Handle any exceptions
                        MessageBox.Show($"Error sending request: {ex.Message}");
                    }
                }
            }
        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is in the "Save" button column
            if (e.ColumnIndex == dataGridView2.Columns["SaveButtonColumn"].Index && e.RowIndex >= 0)
            {
                // Get the data from the clicked row
                string remoteName = dataGridView2.Rows[e.RowIndex].Cells["Remote"].Value.ToString();
                string button = dataGridView2.Rows[e.RowIndex].Cells["Button"].Value.ToString();
                string protocol = dataGridView2.Rows[e.RowIndex].Cells["Protocol"].Value.ToString();
                string parameter1 = dataGridView2.Rows[e.RowIndex].Cells["Parameter1"].Value.ToString();
                string parameter2 = dataGridView2.Rows[e.RowIndex].Cells["Parameter2"].Value.ToString();
                string parameter3 = dataGridView2.Rows[e.RowIndex].Cells["Parameter3"].Value.ToString();

                // Save the row's data (implement your saving logic here)

                // Create a new button dynamically with the same functionality as the row
                Button newButton = new Button();
                newButton.Text = $"{remoteName}: {button}";
                newButton.Click += (s, _) =>
                {
                    // Implement the functionality to perform the same action as the row
                    // You can use the data retrieved from the clicked row here
                    Console.WriteLine($"Button {remoteName}: {button} clicked");
                };

                // Add the new button to the form
                // You may need to adjust the location and layout of the button as needed
                Controls.Add(newButton);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshButtons();
        }
        Dictionary<string, string> scriptDictionary = new Dictionary<string, string>();





        private void RefreshButtons()
        {
            try
            {
                // Call the Python script to get saved buttons data
                string ipAddress = Form1.GlobalVariables.ReceivedIpAddress;
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "python"; // Set the Python interpreter
                string pythonScriptPath = @"C:\Users\USER\Desktop\c# app\Mu.Project2\Mu.Project2\pythonforsavedbuttons\get_savesd_buttons.py";
                string arguments = $"\"{pythonScriptPath}\" \"{ipAddress}\""; // Pass ipAddress instead of ipAddress_raspi

                psi.Arguments = arguments; // Set the script path and arguments
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true; // Hide the terminal window

                using (Process process = Process.Start(psi))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        Console.WriteLine("Received data from Python script:");
                        Console.WriteLine(result); // Debug print to see the received data

                        // Parse the JSON data
                        JArray jsonDataArray;
                        try
                        {
                            jsonDataArray = JArray.Parse(result);
                        }
                        catch (JsonReaderException)
                        {
                            // Handle the case where the return is not a valid JSON array
                            JObject jsonDataObject;
                            try
                            {
                                jsonDataObject = JObject.Parse(result);
                                CreateButtonFromObject(jsonDataObject);
                            }
                            catch (JsonReaderException)
                            {
                                // Handle the case where the return is not a valid JSON object
                                Console.WriteLine("Received data is not in valid JSON format.");
                            }
                            return;
                        }

                        // Clear existing buttons before adding new ones
                        flowLayoutPanel1.Controls.Clear();

                        // Loop through each object in the array
                        foreach (JObject jsonData in jsonDataArray)
                        {
                            CreateButtonFromObject(jsonData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing buttons: {ex.Message}");
            }
        }

        private void CreateButtonFromObject(JObject jsonData)
        {
            // Extract user_command_csharp and script values
            string userCommand = jsonData.Value<string>("user_command_csharp");
            string script = jsonData.Value<string>("script");

            // Create a new button
            Button button = new Button();
            button.Text = userCommand;
            button.Click += (sender, e) => Button_Click(sender, e, userCommand, script); // Set the event handler

            // Add the button to the panel
            flowLayoutPanel1.Controls.Add(button);
        }



        private async void Button_Click(object sender, EventArgs e, string userCommand, string script)
        {
            // Send the script to Node-RED
            await SendScriptToRaspi(script);
        }

        private async Task SendScriptToRaspi(string script)
        {
            string url = $"http://{Form1.GlobalVariables.ReceivedIpAddress}:1880/buttontriggered";
            // Construct the URL to send the HTTP request
            Console.WriteLine(url);
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(script);

                try
                {
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Script sent to Node-RED successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send script to Node-RED. Status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            RefreshButtons();
        }

        private async  void button3_Click(object sender, EventArgs e)
        {
        

        }

        private async void button4_Click(object sender, EventArgs e)
        {
           

        }

        private async void button3_Click_1(object sender, EventArgs e)
        {
            try
            {
                string manufacture = textBox1.Text;
                string button = textBox2.Text;

                // Check if textbox1 and textbox2 are not empty
                if (string.IsNullOrEmpty(manufacture) || string.IsNullOrEmpty(button))
                {
                    MessageBox.Show("Please fill in both textboxes before sending the request. like  manufacture : LGTV , button : powerON");
                    return; // Exit the method without sending the request
                }

                string nodeRedUrl = $"http://{Form1.GlobalVariables.ReceivedIpAddress}:1880/learnIR";

                using (HttpClient client = new HttpClient())
                {
                    var jsonContent = new StringContent($"{{\"manufacture\":\"{manufacture}\",\"button\":\"{button}\"}}", Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(nodeRedUrl, jsonContent);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Request sent successfully!");
                    }
                    else
                    {
                        MessageBox.Show($"Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }


        }

        private async void button4_Click_1(object sender, EventArgs e)
        {
            try
            {
                string manufacture = textBox1.Text;
                string button = textBox2.Text;

                // Check if textbox1 and textbox2 are not empty
                if (string.IsNullOrEmpty(manufacture) || string.IsNullOrEmpty(button))
                {
                    MessageBox.Show("Please fill in both textboxes before sending the request. like  manufacture : LGTV , button : powerON");
                    return; // Exit the method without sending the request
                }

                string nodeRedUrl = $"http://{Form1.GlobalVariables.ReceivedIpAddress}:1880/learnRF";

                using (HttpClient client = new HttpClient())
                {
                    var jsonContent = new StringContent($"{{\"manufacture\":\"{manufacture}\",\"button\":\"{button}\"}}", Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(nodeRedUrl, jsonContent);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Request sent successfully!");
                    }
                    else
                    {
                        MessageBox.Show($"Error: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click_1(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1Database_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button_key_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}
