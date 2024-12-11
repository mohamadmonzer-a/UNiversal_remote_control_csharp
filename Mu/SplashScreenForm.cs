﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Media;

namespace Mu
{
    public partial class SplashScreenForm : Form
    {
        // Define an event to notify when the splash screen is closed
        public event EventHandler SplashScreenClosed;
        private SoundPlayer soundPlayer;

        public SplashScreenForm()
        {
            InitializeComponent();
            this.FormClosed += SplashScreenForm_FormClosed;
            this.Load += SplashScreenForm_Load;
        }


        private void SplashScreenForm_Load(object sender, EventArgs e)
        {
            string imagePath = @"C:\Users\USER\Desktop\c# app\Mu.Project2\Mu.Project2\icons\logo123.gif";
            string audioPath = @"C:\Users\USER\Desktop\c# app\Mu.Project2\Mu.Project2\audio\audio_track.wav";

            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            //this.TopMost = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;


            try
            {
                // Load the image
                pictureBox1.Image = Image.FromFile(imagePath);

                // Calculate the form size based on the image size
                this.Size = pictureBox1.Image.Size;
                // Center the PictureBox in the form
                pictureBox1.Location = new Point(
                    (this.ClientSize.Width - pictureBox1.Width) / 2,
                    (this.ClientSize.Height - pictureBox1.Height) / 2);



                // Load and play the audio
                soundPlayer = new SoundPlayer(audioPath);
                soundPlayer.Play();


                // Start a timer to close the splash screen after 7 seconds
                Timer timer = new Timer();
                timer.Interval = 25000; // 7 seconds
                timer.Tick += Timer_Tick;
                timer.Start();
            }
            catch (Exception ex)
            {
                // Display an error message if the image fails to load
                MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Close the splash screen after the timer expires
            this.Close();
        }


        private void SplashScreenForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            soundPlayer?.Stop();
            // Raise the event when the form is closed
            SplashScreenClosed?.Invoke(this, EventArgs.Empty);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}