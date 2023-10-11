namespace GraphDB_WindowsForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoginForm loginForm = new LoginForm();

            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                // If you had a main form you wanted to display after successful login:
                // Application.Run(new MainForm());

                // If not, and you just wanted to show the login form and then exit if it closes:
                // Do nothing here, the application will exit after this block.
            }
        }

    }


}