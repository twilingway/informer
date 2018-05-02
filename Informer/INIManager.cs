using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
//using HardwareMonitor.Hardware;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using OpenHardwareMonitor.Hardware;
using System.IO;
//Класс для чтения/записи INI-файлов
public class INIManager
{
    //Конструктор, принимающий путь к INI-файлу
    public INIManager(string aPath)
    {
        path = aPath;
    }

    //Конструктор без аргументов (путь к INI-файлу нужно будет задать отдельно)
    public INIManager() : this("") { }

    //Возвращает значение из INI-файла (по указанным секции и ключу)
    public string GetPrivateString(string aSection, string aKey)
    {
        //Для получения значения
        StringBuilder buffer = new StringBuilder(SIZE);

        //Получить значение в buffer
        GetPrivateProfileString(aSection, aKey, null, buffer, SIZE, path);

        //Вернуть полученное значение
        return buffer.ToString();
    }

    //Пишет значение в INI-файл (по указанным секции и ключу)
    public void WritePrivateString(string aSection, string aKey, string aValue)
    {
        //Записать значение в INI-файл
        WritePrivateProfileString(aSection, aKey, aValue, path);
    }

    //Возвращает или устанавливает путь к INI файлу
    public string Path { get { return path; } set { path = value; } }

    //Поля класса
    private const int SIZE = 1024; //Максимальный размер (для чтения значения из файла)
    private string path = null; //Для хранения пути к INI-файлу

    //Импорт функции GetPrivateProfileString (для чтения значений) из библиотеки kernel32.dll
    [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString", CharSet=CharSet.Unicode)]
    private static extern uint GetPrivateProfileString(string section, string key, string def, StringBuilder buffer, uint size, string path);

    //Импорт функции WritePrivateProfileString (для записи значений) из библиотеки kernel32.dll
    [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString", SetLastError=true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool WritePrivateProfileString(string section, string key, string str, string path);
}
