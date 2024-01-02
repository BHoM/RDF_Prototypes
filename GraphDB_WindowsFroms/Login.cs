/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GraphDB_WindowsForms
{
    public class LoginForm : Form
    {
        // Public properties to access the user input data from the component
        public string ServerAddress { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }


        public LoginForm()
        {

            // Set up the form properties
            this.Text = "GraphDB Login";
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

            Label serverAddressLabel = new Label()
            {
                Text = "Server address:",
                Left = 20,
                Top = 80,
                Width = 100
            };

            TextBox serverAddressTextBox = new TextBox()
            {
                Left = 120,
                Top = 80,
                Width = 200
            };

            Button button = new Button()
            {
                Text = "OK",
                Left = 120,
                Top = 110,
                Width = 100
            };

            // Add the controls to the form
            this.Controls.Add(usernameLabel);
            this.Controls.Add(usernameTextBox);
            this.Controls.Add(passwordLabel);
            this.Controls.Add(passwordTextBox);
            this.Controls.Add(serverAddressLabel);
            this.Controls.Add(serverAddressTextBox);
            this.Controls.Add(button);

            // Attach the button's click event handler
            button.Click += (sender, e) =>
            {
                // Access the user's input data here
                Username = usernameTextBox.Text;
                Password = passwordTextBox.Text;
                ServerAddress = serverAddressTextBox.Text;

                // Close the form with a result indicating the "OK" button was clicked
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
        }
    }

}