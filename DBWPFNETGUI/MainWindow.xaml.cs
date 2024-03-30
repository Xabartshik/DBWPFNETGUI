using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using DBWPFNETGUI;
/* Сделал -- Ошлаков Данил, ИВТ-22
 * Требования
Разделение представления и модели (данных и методов работы с ними).

Одна таблица с 4+ полями === КНИГА ОБИД.

Собственный формат БД или SQL (QSLite или серверная БД)

Добавление, проверка, изменение, удаление данных

Поиск, сортировка (как минимум по одному полю).

Документация (в коде) описывающая формат данных в файле (если используется).

Требования к GUI:

вывод данных в таблицу (но не хранение в этом компоненте!)
меню приложения (menu) DONE
панель инструментов (toolbar) DONE
шрифт и цветовая палитра отличные от задаваемых по умолчанию. DONE
иконка приложения DONE
Всплывающая подсказка или подсказка в строке состояния для элементов интерфейса. ToolTip="Всплывающая подсказка для кнопки" DONE
◦ Информация о разработчике. 
◦ Дополнительно (выполните минимум 3 пункта): 
▪ горячие клавиши +++
▪ цветовое кодирование данных в таблице +++ (если живой, то зеленый, если мертвый, то красный)
▪------------------------------------ использование элементов интерфейса (флажок, числовое поле ввода и тт.п.) в таблице ??? (М.б. в настройках сделать, типа отображения) (Done, MenuItem IsCheckable)
▪ хранение изображений в БД ---
▪ использовать как минимум одно модальное окно +++ 
▪ Автоматическое сохранение БД через заданные интервалы времени +++ 
▪------------------------------------ краткая справка +++ (Типа готово?)
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */
namespace DBWPFNETGUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GreatBookOfGrudges greatBookOfGrudges = new GreatBookOfGrudges("blabla.db");



        public MainWindow()
        {
            InitializeComponent();

            //Настройка
            dataGrid.CanUserAddRows = false;
            //Связывание источника и dataGrid
            dataGrid.ItemsSource = greatBookOfGrudges.Records;
            //Куда сохранить ДБ
            //SQLiteHelper._databasePath = "D:\\SDK\\Projects\\GUI\\VS\\DBWPFNETGUI\\DBWPFNETGUI\\bin\\Debug\\net8.0-windows\\Grudges.db";
            SQLiteHelper._databasePath = "Grudges.db";
            dataGrid.IsReadOnly = false;
        }

        //Обработчик события нажатия на кнопку О разработчике. Выводит информацию о разработчике.
        private void MIAbDeveloper_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Сделал: Ошлаков Данил, ИВТ-22", "О разработчике", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        //Обработчик события нажатия на кнопку О программе. Выводит информацию о программе.
        private void MIAbProgram_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Небольшая База Данных 'Книга Обид'. В ней хранится \nинформация о том, кто, когда и как обидел хозяина этой книги. \nРазработана для дварфов.", "О программе", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        //Процедура для выбора файла для сохранения
        public static string GetSelectedFileName()
        {
            // Создаем экземпляр класса OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Устанавливаем начальный каталог
            openFileDialog.InitialDirectory = "C:\\";

            // Устанавливаем фильтр файлов
            openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";

            // Отображаем диалоговое окно
            if (openFileDialog.ShowDialog() == true)
            {
                // Возвращаем выбранный файл
                return openFileDialog.FileName;
            }

            // Если файл не выбран, возвращаем пустую строку
            return "";
        }
        //Обработчик событий кнопки "Загрузить". Загружает файл базы данных, вызывая диалоговое окно
        private void MIFLoad_Click(object sender, RoutedEventArgs e)
        {
            //Загрузка данных
            greatBookOfGrudges.Records = SQLiteHelper.LoadObservableCollection();
            //Поскольку операция загрузки возвращает новый объект, нужно заново укзать на источник данных
            dataGrid.ItemsSource = greatBookOfGrudges.Records;
            MessageBox.Show("Загружено");
        }
        //Обработчик кнопки Добавить. Добавляет пустую строку
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            greatBookOfGrudges.Records.Add(new GreatBookOfGrudgesRecord());
        }

        private void MIFSave_Click(object sender, RoutedEventArgs e)
        {
            //greatBookOfGrudges.Save();
            //sqliteHelper.Save(greatBookOfGrudges.Records);
            SQLiteHelper.SaveObservableCollection(greatBookOfGrudges.Records);
            MessageBox.Show("Сохранение завершено");
        }

    }
}
