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

namespace GraphDoctors;

public class Doctor
{
    public int ID_дежурства { get; set; }
    public string ID_врача { get; set; }
    public string Отделение { get; set; }
    public DateTime Дата_дежурства { get; set; }
}

public class Otdel
{
    public int ID_отделения { get; set; }
    public string Название_отделения { get; set; }
}