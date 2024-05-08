using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Avalonia;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;


namespace GraphDoctors
{
    public partial class EditorForm : Window
    {
        private readonly MySqlConnection _connection;
        

        private TextBox _doctorNameTextBox;
        private TextBox _specializationTextBox;
        private DatePicker _workDatePicker;
        private Button _addButton;
        public EditorForm(MySqlConnection connection)
        {
            InitializeComponent();
            
            _connection = connection;
        }
        
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            _doctorNameTextBox = this.FindControl<TextBox>("DoctorNameTextBox");
            _specializationTextBox = this.FindControl<TextBox>("SpecializationTextBox");
            _workDatePicker = this.FindControl<DatePicker>("WorkDatePicker");
            _addButton = this.FindControl<Button>("AddButton");

            _workDatePicker.SelectedDate = DateTime.Today;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string doctorName = _doctorNameTextBox.Text;
            string specialization = _specializationTextBox.Text;
            DateTime workDate = _workDatePicker.SelectedDate.GetValueOrDefault().Date;

            if (!string.IsNullOrEmpty(doctorName) && !string.IsNullOrEmpty(specialization))
            {
                AddDoctorToDatabase(doctorName, specialization, workDate);
                CalendarForm main = new CalendarForm();
                main.Show();
                Close();
            }
            else
            {
                // Вывести сообщение об ошибке, если какие-либо поля не заполнены
            }
        }
        
        private void BackButton_OnClick(object? sender, RoutedEventArgs e)
        {
            CalendarForm main = new CalendarForm();
            main.Show();
            this.Close();
        }

        private void AddDoctorToDatabase(string doctorName, string specialization, DateTime workDate)
        {
            string queryString = "INSERT INTO Дежурства (Дежурный_врач, Отделение, Дата_дежурства) VALUES (@doctorName, @specialization, @workDate)";

            using (MySqlCommand cmd = new MySqlCommand(queryString, _connection))
            {
                cmd.Parameters.AddWithValue("@doctorName", doctorName);
                cmd.Parameters.AddWithValue("@specialization", specialization);
                cmd.Parameters.AddWithValue("@workDate", workDate);

                _connection.Open();
                cmd.ExecuteNonQuery();
                _connection.Close();
            }
        }
    } 
}