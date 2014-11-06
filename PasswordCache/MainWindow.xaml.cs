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
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;

namespace PasswordCache
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Буквы
        /// </summary>
        private string[] lat = new string[] {"a","b","c","d","e","f","j","h","i","g","k","l","m","n",
                                                        "o","p","q","r","s","t","u","v","w","x","y","z"};

        private List<Data> myData = new List<Data>();
        private List<Data> t = new List<Data>();
        private string FName = "my.pac";

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                if (File.Exists(FName))
                {
                    OpenDB(FName);
                }
                
                Table.ItemsSource = myData;
                                    
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
        }

        /// <summary>
        /// Загрузить базу из файла
        /// </summary>
        /// <param name="name"></param>
        private void OpenDB(string name)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (Stream str = File.OpenRead(name))
            {
                myData = (List<Data>)bf.Deserialize(str);
            }
            Table.ItemsSource = myData;
        }

        /// <summary>
        /// Сгенерировать на основе слова
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (World.Visibility == System.Windows.Visibility.Collapsed)
                World.Visibility = System.Windows.Visibility.Visible;
            else
                World.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// Генерация паролей
        /// </summary>
        /// <param name="Count"></param>
        /// <param name="Numb"></param>
        /// <param name="Cas"></param>
        /// <returns></returns>
        private string GeneretionPassword(int Count,bool Numb,bool Cas)
        {
            string result = string.Empty;
            Random r = new Random();
            int cc=0;
            int CountNumb = 0;
          
            if (Numb)
                CountNumb = r.Next(0, r.Next(1, Count - 2));

            if (Cas)
                cc = r.Next(1, r.Next(2, Count));


            for (; result.Length<Count;){
                if(cc>0 && (r.Next(0,10)>3))
                {
                    result += lat[r.Next(0, lat.Length)].ToUpper();
                    cc--;
                }
                else
                {
                    if (CountNumb > 0 && (r.Next(0, 10) > 2))
                    {
                        result += r.Next(0, 9);
                        CountNumb--;
                    }
                    else
                    {
                        result += lat[r.Next(0, lat.Length)];
                    }
                }
            }



            return result;
        }

        /// <summary>
        /// Обработчик кнопки генерации пароля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int c;
                if (!Int32.TryParse(Count.Text, out c))
                    throw new Exception("Количество символов должно быть числом!");

                ResultPass.Visibility = System.Windows.Visibility.Visible;
                ResultPass.Text = GeneretionPassword(c, (bool)Numb.IsChecked, (bool)isCase.IsChecked);
            }
            catch(Exception ex){
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Обновление данных в базе
        /// </summary>
        private void CreateDT(string name)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (Stream str = new FileStream(name, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                bf.Serialize(str, myData);
            }

        }
        /// <summary>
        /// Обработчик закрытия окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CreateDT(FName);
        }

        /// <summary>
        /// Сохранение изминений
        /// </summary>
        private void SaveDB()
        {            
                if (MessageBox.Show("Сохранить изминения?", "Сохранение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    CreateDT(FName);
        }

        /// <summary>
        /// Обработчик кнопки добавить данные
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddPass(object sender, RoutedEventArgs e)
        {
            try{
                if (Name.Text == "")
                    throw new Exception("Вы не ввели имя объекта пароля(сайт или игра)");
                if (Password.Text == "")
                    throw new Exception("Вы не ввели пароль");

                myData.Add(new Data(Name.Text, Login.Text, Password.Text, Comment.Text,Url.Text));
                Name.Text = "";
                Login.Text = "";
                Password.Text = "";
                Comment.Text = "";
                Url.Text = "";

                Table.ItemsSource = t;
                Table.ItemsSource = myData;
            }catch(Exception ex){
                MessageBox.Show(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Начать поиск
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoSearch(object sender, RoutedEventArgs e)
        {
            TableView.ItemsSource = t;
            List<Data> dat = (from c in myData
                                    where
                                          c.Name.ToLower().Contains(Search.Text.ToLower()) || c.Log.ToLower().Contains(Search.Text.ToLower()) || c.Com.ToLower().Contains(Search.Text.ToLower()) || c.Pas.ToLower().Contains(Search.Text.ToLower())
                                    select c).ToList<Data>();
            if (dat != null)
                TableView.ItemsSource = dat;
            else
                MessageBox.Show("Совпадений нет");
        }

        /// <summary>
        /// Кнопка в меня для сохранения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog Sdlg = new SaveFileDialog();
            Sdlg.Filter = "Password data base (.pac)|*.pac";
            if (Sdlg.ShowDialog()==true)
            {
                FName = Sdlg.FileName;
                CreateDT(Sdlg.FileName);
            }
        }

        /// <summary>
        /// Закрыть программу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Открыть справку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            reference refer = new reference();
            refer.Show();
        }

        /// <summary>
        /// Кнопка открыть файл в меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog Odlg = new OpenFileDialog();
            Odlg.Filter = "Password data base (.pac)|*.pac";
            if (Odlg.ShowDialog() == true)
            {
                OpenDB(Odlg.FileName);
                FName = Odlg.FileName;
            }
        }

        private void GroupBox_MouseEnter(object sender, MouseEventArgs e)
        {
            if((sender as GroupBox)!=null)
                StatBar.Content = string.Format("{0} , всего записей - {1}", (sender as GroupBox).Tag.ToString(), myData.Count);
            if((sender as TabItem)!=null)
                StatBar.Content = string.Format("{0} , всего записей - {1}", (sender as TabItem).Tag.ToString(), myData.Count);
        }

        private void GroupBox_MouseLeave(object sender, MouseEventArgs e)
        {
            StatBar.Content = string.Format("Ожидание... , всего записей - {0}", myData.Count);
        }
    }
}
