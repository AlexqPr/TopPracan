using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MassangerG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void CreateChatBtn_Click(object sender, RoutedEventArgs e) //Окно админа
        {
            if (NicknameTbx.Text == "")
            {
                MessageBox.Show("Имя пользователя не заполнено!");
            }
            else
            {
                ChatOwnerWindow window = new ChatOwnerWindow(NicknameTbx.Text);
                this.Close();
                bool? result = window.ShowDialog();
                if (result == true || result == false || result == null)
                {
                    NewMainWindow window1 = new NewMainWindow();
                    window1.Show();
                }
            }
        }

        private void ConnectChatBtn_Click(object sender, RoutedEventArgs e) //Окно юзера
        {
            if (NicknameTbx.Text == "" || ChatAddressTbx.Text == "")
            {
                MessageBox.Show("Не все поля заполнены!");
            }
            else
            {
                ChatUserWindow window = new ChatUserWindow(ChatAddressTbx.Text, NicknameTbx.Text);
                this.Close();
                bool? result = window.ShowDialog();
                if(result == true || result == false || result == null)
                {
                    NewMainWindow window1 = new NewMainWindow();
                    window1.Show();
                }
            }
        }
    }
}
