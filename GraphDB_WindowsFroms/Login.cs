using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GraphDB_WindowsForms
{
    public class LoginForm : Form
    {
        // Public properties to access the user input data from the component
        public string Username { get; private set; }
        public string Password { get; private set; }

        public LoginForm()
        {


            // Set up the form properties
            this.Text = "DesFab database Login";
            this.Width = 400;
            this.Height = 200;
            // Form initialization and controls setup as before...

            // Load the logo image from file
            // change this to locate yout icon (it has to be a bmp file)

            //System.Drawing.Bitmap bmp = DesFab.GH.Properties.Resources.DesFab1;
            //this.Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon());

            // Set the logo as the form's icon
            //this.Icon = new System.Drawing.Icon(logoFilePath);

            // Create the controls for username and password input
            Label usernameLabel = new Label()
            {
                Text = "Username:",
                Left = 20,
                Top = 20,
                Width = 100
            };

            TextBox usernameTextBox = new TextBox()
            {
                Left = 120,
                Top = 20,
                Width = 200
            };

            Label passwordLabel = new Label()
            {
                Text = "Password:",
                Left = 20,
                Top = 50,
                Width = 100
            };

            TextBox passwordTextBox = new TextBox()
            {
                Left = 120,
                Top = 50,
                Width = 200,
                PasswordChar = '*' // Show asterisks for password input
            };

            Button button = new Button()
            {
                Text = "OK",
                Left = 120,
                Top = 80,
                Width = 100
            };

            // Add the controls to the form
            this.Controls.Add(usernameLabel);
            this.Controls.Add(usernameTextBox);
            this.Controls.Add(passwordLabel);
            this.Controls.Add(passwordTextBox);
            this.Controls.Add(button);

            // Attach the button's click event handler
            button.Click += (sender, e) =>
            {
                // Access the user's input data here
                Username = usernameTextBox.Text;
                Password = passwordTextBox.Text;

                // Close the form with a result indicating the "OK" button was clicked
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
        }
    }

}