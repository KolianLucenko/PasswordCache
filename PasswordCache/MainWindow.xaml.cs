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

        private DataSet ds = new DataSet();
        private List<Data> myData = new List<Data>();
        private List<Data> t = new List<Data>();
        SqlDataAdapter dAdapt;
        private string conString = "Data Source=(local);Initial Catalog=Pass;Integrated Security=True";


        public MainWindow()
        {
            InitializeComponent();
            try
            {
                dAdapt = new SqlDataAdapter("Select * from Ps", conString);
                SqlCommandBuilder builder = new SqlCommandBuilder(dAdapt);

                dAdapt.Fill(ds, "password");


                var dataList = from c in ds.Tables["password"].AsEnumerable()
                                    select new Data(c.Field<string>("Имя(сайт,игра)"), c.Field<string>("Логин"),
                                        c.Field<string>("Пароль"), c.Field<string>("Комментарий"));
                foreach (var c in dataList)
                    myData.Add(c);

                Table.ItemsSource = myData;
                                    
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message);
            }
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


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResultPass.Visibility = System.Windows.Visibility.Visible;
            try
            {
                int c;
                if (!Int32.TryParse(Count.Text, out c))
                    throw new Exception("Количество символов должно быть числом!");

                ResultPass.Text = GeneretionPassword(c, (bool)Numb.IsChecked, (bool)isCase.IsChecked);
            }
            catch(Exception ex){
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Обновление данных в базе
        /// </summary>
        private void CreateDT()
        {
            SqlConnection sqlConnection1 = new SqlConnection(conString);
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "DELETE FROM Ps";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;

            sqlConnection1.Open();
            cmd.ExecuteNonQuery();
            sqlConnection1.Close();

            foreach(var d in myData){
                DataRow row = ds.Tables["password"].NewRow();
                row["Имя(сайт,игра)"] = d.Name;
                row["Логин"] = d.Log;
                row["Пароль"] = d.Pas;
                row["Комментарий"] = d.Com;
                ds.Tables["password"].Rows.Add(row);
            }
            dAdapt.Update(ds, "password");

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CreateDT();
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
                if (Login.Text == "")
                    throw new Exception("Вы не ввели логин");
                if (Password.Text == "")
                    throw new Exception("Вы не ввели пароль");

                myData.Add(new Data(Name.Text, Login.Text, Password.Text, Comment.Text));
                Name.Text = "";
                Login.Text = "";
                Password.Text = "";
                Comment.Text = "";

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
            TableView.ItemsSource = from c in myData
                                    where
                                        c.Com.ToLower() == Search.Text.ToLower() || c.Log.ToLower() == Search.Text.ToLower() || c.Name.ToLower() == Search.Text.ToLower() || c.Pas.ToLower() == Search.Text.ToLower()
                                    select c;
        }
    }
}
