using System.Windows;

namespace BQuTMSWithJira
{
    public partial class AboutTMS : Window
    {
        public AboutTMS()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

        private void ok_but_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
