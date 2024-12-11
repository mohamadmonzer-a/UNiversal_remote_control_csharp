using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mu
{
    internal static class Program
    {
        private static bool form1Shown = false;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create the splash screen form
            SplashScreenForm splashScreen = new SplashScreenForm();

            // Create Form1 but do not show it yet
            Form1 form1 = new Form1();

            // Calculate the location of the splash screen to be in the middle of Form1's display area
            Point form1Center = new Point(form1.Left + form1.Width / 2, form1.Top + form1.Height / 2);
            Point splashScreenLocation = new Point(form1Center.X - splashScreen.Width / 2, form1Center.Y - splashScreen.Height / 2);
            splashScreen.StartPosition = FormStartPosition.Manual;
            splashScreen.Location = splashScreenLocation;

            // Show the splash screen and set it to be always on top
            splashScreen.TopMost = true;
            splashScreen.Show();

            // Handle the KeyDown event for the splash screen
            splashScreen.KeyPreview = true;
            splashScreen.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    splashScreen.Close();
                    form1Shown = true;
                    form1.Show();
                }
            };

            // Start a timer to close the splash screen after 25 seconds and show Form1
            Timer timer = new Timer();
            timer.Interval = 25000; // 25 seconds
            timer.Tick += (sender, e) =>
            {
                // Stop the timer
                timer.Stop();

                // Close the splash screen form
                splashScreen.Close();

                // Show Form1
                form1Shown = true;
                form1.Show();
            };
            timer.Start();

            // Run the application message loop with a hidden Form1
            Application.Run();
        }
    }
}
