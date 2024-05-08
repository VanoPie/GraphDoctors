using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MySql.Data.MySqlClient;
using System;
using System.Linq;

namespace GraphDoctors
{
    public partial class ForEditForm : Window
    {
        private MySqlConnection _connection;
        private Doctor _selectedDoctor;

        public ForEditForm(Doctor selectedDoctor, MySqlConnection connection)
        {
            _connection = connection;
            _selectedDoctor = selectedDoctor;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            
            ID_dezh = this.FindControl<TextBox>("ID_dezh");
            DoctorNameTextBox = this.FindControl<TextBox>("DoctorNameTextBox");
            SpecializationTextBox = this.FindControl<TextBox>("SpecializationTextBox");
            WorkDatePicker = this.FindControl<DatePicker>("WorkDatePicker");
            
            LoadDoctorData();
        }
        
        private void LoadDoctorData()
        {
            ID_dezh.Text = _selectedDoctor.ID_дежурства.ToString();
            DoctorNameTextBox.Text = _selectedDoctor.ID_врача.ToString();
            SpecializationTextBox.Text = _selectedDoctor.Отделение.ToString();
            WorkDatePicker.SelectedDate = _selectedDoctor.Дата_дежурства.Date;
        }

        private void EditButton_Click(object? sender, RoutedEventArgs e)
        {
            // Создаем SQL-запрос для обновления данных о дежурстве
            string queryString = "UPDATE Дежурства SET Дежурный_врач = @id_врача, Отделение = @отделение, Дата_дежурства = @дата_дежурства WHERE ID_дежурства = @id_дежурства";

            using (MySqlCommand cmd = new MySqlCommand(queryString, _connection))
            {
                cmd.Parameters.AddWithValue("@id_врача", Convert.ToInt32(DoctorNameTextBox.Text));
                cmd.Parameters.AddWithValue("@отделение", Convert.ToInt32(SpecializationTextBox.Text));
                cmd.Parameters.AddWithValue("@дата_дежурства", WorkDatePicker.SelectedDate.GetValueOrDefault().Date);
                cmd.Parameters.AddWithValue("@id_дежурства", Convert.ToInt32(ID_dezh.Text));

                _connection.Open();
                cmd.ExecuteNonQuery();
                _connection.Close();
            }

            CalendarForm main = new CalendarForm();
            main.Show();
            this.Close();
        }

        private void BackButton_OnClick(object? sender, RoutedEventArgs e)
        {
            CalendarForm main = new CalendarForm();
            main.Show();
            this.Close();
        }
    }
}