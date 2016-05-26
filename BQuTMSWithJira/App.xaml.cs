using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BQuTMSWithJira
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = (TextBox)sender;
            if (tb.Text.Length > 0)
            {
                tb.Text = System.Char.ToUpper(tb.Text[0]) + tb.Text.Substring(1);
            }
        }

        [STAThread]
        public static void Main()
        {
            //System.Threading.Thread.Sleep(100000);
            SplashScreen splashScreen = new SplashScreen("images/SplashScreen.png");
            splashScreen.Show(true);
            var application = new App();
            application.Init();
            application.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            SingleInstance.Make();

            base.OnStartup(e);
        }

        public void Init()
        {
            this.InitializeComponent();
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            throw new NotImplementedException();
        }

          // SingleInstance si = new SingleInstance("BQuTMSApp");
                    
          //  if (si.IsSingleInstance)
          //              {
          //            SplashScreen splashScreen = new SplashScreen("images/SplashScreen.png");
          //          splashScreen.Show(true);
          //          BQuTMSWithJira.App app = new BQuTMSWithJira.App();
          //          app.InitializeComponent();
          //          app.Run();
          //          }

          //                    else
          //{
          // //   MessageBox.Show("Already running");
          //  // Bring the other instance to the front or show a message if that fails

          //  if (!SingleInstance.ShowOtherInstance())
          //  {

          //  }

          //}
        
    }
}
