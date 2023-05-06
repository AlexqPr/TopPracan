using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Логика взаимодействия для ChatOwnerWindow.xaml
    /// </summary>
    public partial class ChatOwnerWindow : Window
    {
        private Socket socket;
        bool forlogs = true;
        private string NameAdmin;
        private List<Socket> clients = new List<Socket>();
        private List<string> newlist1 = new();//Лист с именами
        private List<string> newlist2 = new();//Лист с подключенными(для логов)
        struct MESIMEN
        {
            public string Name;
            public string Message;
        }
        public ChatOwnerWindow(string admin)
        {
            InitializeComponent();
            NameAdmin = admin;
            newlist1.Add(admin);
            UsersListBox.ItemsSource = newlist1;
            ForLogs(admin);
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 8888);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipPoint);
            socket.Listen(100);

            ListenToClients();
        }

        private async Task ListenToClients()
        {
            while (true)
            {
                var client = await socket.AcceptAsync();
                clients.Add(client);
                RecieveMessage(client);
                SendMessage(client, $"/username{NameAdmin}");
                

            }
        }

        private async Task RecieveMessage(Socket client)
        {
            while (true)
            {
                byte[] bytes = new byte[1024];
                await client.ReceiveAsync(bytes, SocketFlags.None);
                string message = Encoding.UTF8.GetString(bytes);
                MESIMEN newtemp = IMENA(message, client);
                if (newtemp.Message == "exception")
                {

                }
                else
                {
                    //MessageLbx.Items.Add($"[Сообщение от{client.RemoteEndPoint}]: {message}");
                    ChatListBox.Items.Add($"Дата: {DateTime.Now} Сообщение от {newtemp.Name}: {newtemp.Message}");
                    foreach (var item in clients)
                    {
                        SendMessage(item, message);
                    }
                }




            }
        }

        private async Task SendMessage(Socket client, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(bytes, SocketFlags.None);
        }

        private async Task SendMessage1(string message)
        {
            MESIMEN newtemp = IMENA(message, socket);
                //MessageLbx.Items.Add($"[Сообщение от{client.RemoteEndPoint}]: {message}");
                ChatListBox.Items.Add($"Дата: {DateTime.Now} Сообщение от {newtemp.Name}: {newtemp.Message}");
                foreach (var item in clients)
                {
                    SendMessage(item, message);
                }
            
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            //TestWindow window5 = new TestWindow();
            //window5.Logi.ItemsSource = newlist2;
            //window5.Show();
            //MessageBox.Show(clients.Count.ToString());
            //MessageBox.Show(newlist1.Count.ToString());
            //SendMessage1(MessageTxt.Text);
        }

        private MESIMEN IMENA(string stroka, Socket chmo)
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

            string message = new(sim);

            MESIMEN temp = new MESIMEN();

            string name = stroka.Replace("/username", " ");

            int newname = name.LastIndexOf(" ");
            name = name.Substring(newname + 1);//Вытягивание имени
            temp.Name = name;
            temp.Message = message;
            string proverka = message.Replace(" ", "");
            if (proverka.CompareTo("") == 0)//Проверка на пустую строку
            {
                if (newlist1.Contains(name) == true)
                {

                }
                else
                {
                    newlist1.Add(name);
                    ForLogs(name);
                    UsersListBox.ItemsSource = null;
                    UsersListBox.ItemsSource = newlist1;
                }
                MESIMEN lol = new MESIMEN();
                lol.Name = "";
                lol.Message = "exception"; //Как по другому это реализовать я хз ( можно было бы использовать throw new ArgumentException(""); но дальше как будто клиент отключается)
                return lol;
            }
            if (message.CompareTo("/disconnect") == 0)
            {
                //Обновление листа с пользователями
                Removing(temp.Name);
                clients.Remove(chmo);//Удаляем клиента из листа
                throw new ArgumentException("");
            }
            else
            {
                return temp;

            }

        }
        private void Removing(string polz)
        {
            for (int i = 0; i < newlist1.Count; i++)
            {
                if (newlist1[i].CompareTo(polz) == 0)
                {
                    newlist1.RemoveAt(i); //Я хз почему не рабоает newlist1.Remove(polz) я потратил 15 минут в никуда (0_0)
                }
            }
            UsersListBox.ItemsSource = null;
            UsersListBox.ItemsSource = newlist1;
        }

        private void ForLogs(string names)
        {
            newlist2.Add("Дата и время подключения: " + DateTime.Now + " " + names);

        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            SendMessage1(MessageTXT.Text+$"/username{NameAdmin}");
            MessageTXT.Text = "";
        }

        private void LogsBtn_Click(object sender, RoutedEventArgs e)
        {
            if(forlogs == true)
            {
                LogsPage page = new LogsPage();
                page.igw(newlist2);
                PageFrame.Content = new LogsPage();

                
                forlogs = false;
            }
            else
            {
                LogsPage page = new LogsPage();
                forlogs = true;
                PageFrame.Content = null;
            }
         
        }
    }
}
