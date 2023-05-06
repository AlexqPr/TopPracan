using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MassangerG
{
    /// <summary>
    /// Логика взаимодействия для ChatUserWindow.xaml
    /// </summary>
    public partial class ChatUserWindow : Window
    {
        private Socket server;
        public string IMA;
        private List<string> users = new();
        struct MESIMEN
        {
            public string Name;
            public string Message;
        }
        public ChatUserWindow(string IPADDRESS, string IMIA)
        {
            InitializeComponent();
            users.Add(IMIA);
            UsersListBox.ItemsSource = users;
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.ConnectAsync(IPADDRESS, 8888);

            IMA = IMIA;
            SendMessage($"/username{IMA}");
            RecieveMessage();
        }
        private async Task RecieveMessage()
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                await server.ReceiveAsync(bytes, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);
                MESIMEN newtemp = IMENA(message);
                if(newtemp.Message == "exception")
                {

                }
                else
                {
                    ChatListBox.Items.Add($"Дата: {DateTime.Now} Сообщение от {newtemp.Name}: {newtemp.Message}");
                }



            }
        }

        private async Task SendMessage(string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await server.SendAsync(bytes, SocketFlags.None);
        }


        private MESIMEN IMENA(string stroka)
        {

            int prikol = stroka.LastIndexOf("/username");
            char[] sim = new char[stroka.Length];
            for (int i = 0; i < stroka.Length; i++)
            {
                if (i == prikol)
                {
                    break;
                }
                else
                {

                    sim[i] = stroka[i];
                }
            }

            string message = new string(sim);

            MESIMEN temp = new MESIMEN();

            string name = stroka.Replace("/username", " ");

            int newname = name.LastIndexOf(" ");
            name = name.Substring(newname + 1);//Вытягивание имени
            temp.Name = name;
            temp.Message = message;
            //Console.WriteLine($"Сообщение от {name}: {message}");
          
            if(users.Contains(temp.Name) == false)
            {
                users.Add(name);//НЕ РАБОТАЕТ ЭТО СРАНОЕ УСЛОВИЕ Я ХОЧУ УМЕРЕТЬ ЭТОЙ НОЧЬЮ
                UsersListBox.ItemsSource = null;
                UsersListBox.ItemsSource = users;
            }
            if (message.CompareTo("") == 0)
            {
                temp.Message = "exception";
                return temp;
            }
            if (message.CompareTo("/disconnect") == 0)
            {
                //Обновление листа с пользователями
                Removing(temp.Name);
                throw new ArgumentException("");
            }
            else
            {
                return temp;

            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("/disconnect" + $"/username{IMA}");
            server.Close();
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageTxt.Text == "/disconnect")
            {
                SendMessage("/disconnect" + $"/username{IMA}");
                server.Close();
            }
            SendMessage(MessageTxt.Text + $"/username{IMA}");
            MessageTxt.Text = "";
        }

        private void Removing(string polz)
        {
            MessageBox.Show("АДЦЬУАДЦЛа");
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].CompareTo(polz) == 0)
                {
                    users.RemoveAt(i); //Я хз почему не рабоает newlist1.Remove(polz) я потратил 15 минут в никуда (0_0)
                }
            }
            UsersListBox.ItemsSource = null;
            UsersListBox.ItemsSource = users;
        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            SendMessage("/disconnect" + $"/username{IMA}");
            MessageTxt.Text = "";
            DialogResult = false;
        }
    }
}
