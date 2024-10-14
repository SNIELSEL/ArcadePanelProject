using System.Windows;
using System.Windows.Input;

namespace WPFAdminControlApp
{
    /// <summary>
    /// Interaction logic for AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        public AddUserWindow()
        {
            InitializeComponent();

            MoveBar.MouseLeftButtonDown += TitleBar_MouseLeftButtonDown;
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Start dragging the window
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
