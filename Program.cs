namespace PcPartPrices
{
    using System;
    using System.Windows.Forms;

    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form pp = new PPP_GUI();
            pp.Text = "PC Part Price Estimator";
            pp.Size = new System.Drawing.Size(600, 735);
            Application.Run(pp);
        }
    }
}
