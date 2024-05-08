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
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using OfficeOpenXml;

namespace GraphDoctors;

public partial class CalendarForm : Window
{
    private List<Doctor> doc;
    private List<Otdel> otd;
    private Scheduler _scheduler;
    private TextBox _textbox;
    private TextBlock _comment;
    private ComboBox _combobox;
    private MySqlConnection _connection;
    private string connectionString = "server=localhost;database=Diplom;port=3306;User Id=root;password=Qwerty_123456";
    private Calendar _calendar;
    private DataGrid _dataGrid;

    private string fullTable =
        "SELECT дежурства.ID_дежурства, персонал.Фамилия, дежурства.Дата_дежурства, отделения.Название_отделения FROM дежурства JOIN персонал on персонал.ID_врача = дежурства.Дежурный_врач JOIN отделения ON отделения.ID_отделения = дежурства.Отделение";

    public CalendarForm()
    {
        _scheduler = new Scheduler();
        _connection = new MySqlConnection(connectionString);
        InitializeComponent();
        CmbViewMovieFill();
    }

    public void ShowTable(string sql)
    {
        doc = new List<Doctor>();
        _connection = new MySqlConnection(connectionString);
        _connection.Open();
        MySqlCommand command = new MySqlCommand(sql, _connection);
        MySqlDataReader reader = command.ExecuteReader();
        while (reader.Read() && reader.HasRows)
        {
            var Docs = new Doctor()
            {
                ID_дежурства = reader.GetInt32("ID_дежурства"),
                ID_врача = reader.GetString("Фамилия"),
                Отделение = reader.GetString("Название_отделения"),
                Дата_дежурства = reader.GetDateTime("Дата_дежурства"),
            };
            doc.Add(Docs);
        }

        _connection.Close();
        _dataGrid.ItemsSource = doc;
        _comment.Text = "Список всех врачей на дежурствах: ";
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _comment = this.FindControl<TextBlock>("Comment");
        _calendar = this.FindControl<Calendar>("Calendar");
        _dataGrid = this.FindControl<DataGrid>("DataGrid");
        _textbox = this.FindControl<TextBox>("Search_Doctor");
        _combobox = this.FindControl<ComboBox>("CmbOtdelen");

        _calendar.SelectedDatesChanged += Calendar_SelectedDateChanged;
    }

    private void SearchDoc(object? sender, TextChangedEventArgs e)
    {
        ShowTable(fullTable);
        var filmName = doc;
        filmName = filmName.Where(x => x.ID_врача.Contains(_textbox.Text))
            .ToList(); //фильтруем список по введенному значению в поле поиска
        _dataGrid.ItemsSource = filmName; //обновление источника данных DataGrid отфильтрованным списком
        _comment.Text = "Список всех врачей на дежурствах: ";
    }

    private void OtdFilter_OnClick(object? sender, SelectionChangedEventArgs e)
    {
        ShowTable(fullTable);
        var ComboBox = (ComboBox)sender;
        var currentOtd = ComboBox.SelectedItem as Otdel;
        var filteredArtist = doc
            .Where(x => x.Отделение == currentOtd.Название_отделения)
            .ToList();
        _dataGrid.ItemsSource = filteredArtist;
        _comment.Text = "Список дежурных по отделениям: ";
    }

    public void CmbViewMovieFill()
    {
        otd = new List<Otdel>();
        _connection = new MySqlConnection(connectionString);
        _connection.Open();
        MySqlCommand
            command = new MySqlCommand("select * from отделения",
                _connection); //создание команды для выполнения запроса
        MySqlDataReader reader = command.ExecuteReader(); //выполнение запроса и получение результата
        while (reader.Read() && reader.HasRows) //чтение результатов запроса
        {
            var Mov = new Otdel()
            {
                //получение результатов столбцов по классу 
                ID_отделения = reader.GetInt32("ID_отделения"),
                Название_отделения = reader.GetString("Название_отделения")
            };
            otd.Add(Mov);
        }

        _connection.Close();
        var GenreCMB = this.Find<ComboBox>("CmbOtdelen");
        GenreCMB.ItemsSource = otd.DistinctBy(x => x.Название_отделения);
    }

    private void AddButton_Click(object? sender, RoutedEventArgs e)
    {
        EditorForm addDutyForm = new EditorForm(_connection); // Создаем новую форму для добавления данных
        addDutyForm.Show(); // Открываем новую форму 
        this.Close();
    }

    private void DeleteData_Click(object? sender, RoutedEventArgs e)
    {
        Doctor bck = _dataGrid.SelectedItem as Doctor;
        if (bck == null)
        {
            return;
        }

        _connection = new MySqlConnection(connectionString);
        _connection.Open();
        string sql = "DELETE FROM Дежурства WHERE ID_дежурства = " + bck.ID_дежурства;
        MySqlCommand cmd = new MySqlCommand(sql, _connection);
        cmd.ExecuteNonQuery();
        _connection.Close();
        ShowTable(fullTable);
    }

    private void Calendar_SelectedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedDate = _calendar.SelectedDate.Value.Date;

        Dispatcher.UIThread.InvokeAsync(() =>
        {
            _dataGrid.ItemsSource = _scheduler.GetDoctorsForDate(selectedDate, _connection);
        });
        _comment.Text = "Дежурные в этот день: ";
    }


    private void EditButton_OnClick(object? sender, RoutedEventArgs e)
    {
        // Получаем выбранный элемент из DataGrid
        var selectedDoctor = _dataGrid.SelectedItem as Doctor;

        if (selectedDoctor != null)
        {
            // Создаем новую форму для редактирования данных
            ForEditForm editDutyForm = new ForEditForm(selectedDoctor, _connection);

            // Открываем новую форму
            editDutyForm.Show();
            this.Close();
        }
    }

    private void ExitButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Environment.Exit(0);
    }

    private void DocumentButton_OnClick(object? sender, RoutedEventArgs e)
    {
        string outputFile = @"C:\Users\VanoP\Desktop\\otchet.xlsx";
        string query = "SELECT дежурства.ID_дежурства, персонал.Фамилия, дежурства.Дата_дежурства, отделения.Название_отделения FROM дежурства JOIN персонал on персонал.ID_врача = дежурства.Дежурный_врач JOIN отделения ON отделения.ID_отделения = дежурства.Отделение WHERE MONTH(дежурства.Дата_дежурства) = MONTH(CURRENT_DATE()) AND YEAR(дежурства.Дата_дежурства) = YEAR(CURRENT_DATE()) ORDER BY Дата_дежурства;";
        MySqlCommand command = new MySqlCommand(query, _connection);
        _connection.Open();
        MySqlDataReader dataReader = command.ExecuteReader();
        using (ExcelPackage excelPackage = new ExcelPackage())
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Дежурства");
            int row = 1;
                
            for (int i = 1; i <= dataReader.FieldCount; i++)
            {
                worksheet.Cells[row, i].Value = dataReader.GetName(i - 1);
            }
        
            while (dataReader.Read())
            {
                row++;
                for (int i = 1; i <= dataReader.FieldCount; i++)
                {
                    if (dataReader.GetName(i - 1) == "Дата_дежурства" && dataReader[i - 1] != DBNull.Value)
                    {
                        // Преобразование даты в строку с заданным форматом
                        worksheet.Cells[row, i].Value = ((DateTime)dataReader[i - 1]).ToString("yyyy-MM-dd");
                    }
                    else
                    {
                        worksheet.Cells[row, i].Value = dataReader[i - 1];
                    }
                }
            }
        
            excelPackage.SaveAs(new FileInfo(outputFile));
        }
        dataReader.Close();
        _connection.Close();
        Console.WriteLine("Данные успешно экспортированы в Excel файл.");
}
}



public class Scheduler
{
    public IEnumerable<Doctor> GetDoctorsForDate(DateTime date, MySqlConnection connection)
    {
        List<Doctor> doctors = new List<Doctor>();
        string queryString =
            "SELECT дежурства.ID_дежурства, персонал.Фамилия, дежурства.Дата_дежурства, отделения.Название_отделения FROM дежурства JOIN персонал on персонал.ID_врача = дежурства.Дежурный_врач JOIN отделения ON отделения.ID_отделения = дежурства.Отделение WHERE Дата_дежурства = @selectedDate";

        using (MySqlCommand cmd = new MySqlCommand(queryString, connection))
        {
            cmd.Parameters.AddWithValue("@selectedDate", date);

            connection.Open();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Doctor doctor = new Doctor
                    {
                        ID_дежурства = reader.GetInt32("ID_дежурства"),
                        ID_врача = reader.GetString("Фамилия"),
                        Отделение = reader.GetString("Название_отделения"),
                        Дата_дежурства = reader.GetDateTime("Дата_дежурства")
                    };
                    doctors.Add(doctor);
                }
            }
            connection.Close();
        }
        return doctors;
    }
}

