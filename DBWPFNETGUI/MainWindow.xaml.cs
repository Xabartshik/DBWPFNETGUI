using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DBWPFNETGUI;
using SQLitePCL;
using DataGridExtensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Xml.Linq;

/* Сделал -- Ошлаков Данил, ИВТ-22
 * Требования
Разделение представления и модели (данных и методов работы с ними).

Одна таблица с 4+ полями === КНИГА ОБИД.

----------------------------------------------Собственный формат БД или SQL (QSLite или серверная БД)

проверка

------------------------------------------------Поиск, сортировка (как минимум по одному полю).

---------------------------------------------------Документация (в коде) описывающая формат данных в файле (если используется).

Требования к GUI:

-------------------------------------вывод данных в таблицу (но не хранение в этом компоненте!)
-------------------------------------меню приложения (menu) DONE
-------------------------------------панель инструментов (toolbar) DONE
-------------------------------------шрифт и цветовая палитра отличные от задаваемых по умолчанию. DONE
Всплывающая подсказка или подсказка в строке состояния для элементов интерфейса. ToolTip="Всплывающая подсказка для кнопки" DONE
◦ Дополнительно (выполните минимум 3 пункта): 
▪ ---------------------------------------------горячие клавиши +++
▪ цветовое кодирование данных в таблице +++ (если живой, то зеленый, если мертвый, то красный)
▪------------------------------------ использование элементов интерфейса (флажок, числовое поле ввода и тт.п.) в таблице ??? (М.б. в настройках сделать, типа отображения) (Done, MenuItem IsCheckable)
▪--------------------------------------------------------------------- использовать как минимум одно модальное окно +++ 
▪------------------------------------Автоматическое сохранение БД через заданные интервалы времени +++ 
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
        private GreatBookOfGrudges greatBookOfGrudges = new GreatBookOfGrudges("Grudges.db");
        //Стили для изменения цвета ячеек
        private Style styleRed = new Style(typeof(DataGridCell));
        private Style styleGreen = new Style(typeof(DataGridCell));
        //Токен отмены задачи. Используется для автосохранения
        private CancellationTokenSource _cts;
        //Настройка автосохранения
        private bool _setting_as;
        //Горячая клавиша для Add Button
        public static RoutedCommand HKAdd = new RoutedCommand();
        //Горячая клавиша для Delete Button
        public static RoutedCommand HKDelete = new RoutedCommand();
        //Горячая клавиша для Save Button
        public static RoutedCommand HKSave = new RoutedCommand();
        //Горячая клавиша для Load Button
        public static RoutedCommand HKLoad = new RoutedCommand();
        //Горячая клавиша для Clear Button
        public static RoutedCommand HKClear = new RoutedCommand();
        //Горячая клавиша для Search Button
        public static RoutedCommand HKSearch = new RoutedCommand();
        //Запрет на сохранение, пока ячейки редактируются
        public static bool SaveCancel = false;
        public MainWindow()
        {
            InitializeComponent();

            //Настройка
            dataGrid.CanUserAddRows = false;
            //Связывание источника и dataGrid
            dataGrid.ItemsSource = greatBookOfGrudges.Records;
            //Куда сохранить ДБ
            //SQLiteHelper._databasePath = "D:\\SDK\\Projects\\GUI\\VS\\DBWPFNETGUI\\DBWPFNETGUI\\bin\\Debug\\net8.0-windows\\Grudges.db";
            //SQLiteHelper._databasePath = "Grudges.db";
            dataGrid.IsReadOnly = false;
            // Устанавливаем значение логической переменной автосохранения
            _setting_as = false;

            // Создаем источник токена отмены
            _cts = new CancellationTokenSource();

            // Запускаем задачу, которая будет вызывать процедуру сохранения БД
            Task.Run(() => RunFunc(_cts.Token));
            //Добавляю комбинации горячих клавиш. За что они отвечают видно по названию
            HKAdd.InputGestures.Add(new KeyGesture(System.Windows.Input.Key.Enter, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(HKAdd, addButton_Click));
            HKDelete.InputGestures.Add(new KeyGesture(System.Windows.Input.Key.Back, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(HKDelete, removeButton_Click));
            HKSave.InputGestures.Add(new KeyGesture(System.Windows.Input.Key.S, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(HKSave, MIFSave_Click));
            HKLoad.InputGestures.Add(new KeyGesture(System.Windows.Input.Key.L, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(HKLoad, MIFLoad_Click));
            HKClear.InputGestures.Add(new KeyGesture(System.Windows.Input.Key.Delete, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(HKClear, clearButton_Click));
            HKSearch.InputGestures.Add(new KeyGesture(System.Windows.Input.Key.F, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(HKSearch, searchButton_Click));
        }
        //Запуск функции автосохранения
        private void RunFunc(CancellationToken token)
        {
            // Пока токен отмены не установлен, вызываем процедуру сохранения БД
            while (!token.IsCancellationRequested)
            {
                if (_setting_as)
                {
                    if (SaveCancel)
                    {
                        MessageBox.Show("Нельзя сохранить, пока редактирование не завершено!", "Ошибка!");
                    }
                    else
                    {
                        greatBookOfGrudges.Save();
                        TBWarning.Text += "Сохранение завершено в файл " + greatBookOfGrudges.Path() + "\n";
                        TBWarning.ScrollToEnd();
                    }
                }

                // Ожидаем 300 секунд
                Thread.Sleep(300000);
            }
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



        //Обработчик событий кнопки "Загрузить". Загружает файл базы данных, вызывая диалоговое окно
        private void MIFLoad_Click(object sender, RoutedEventArgs e)
        {
            //Проверка на загрузку файла
            try
            {
                //Загрузка данных
                greatBookOfGrudges.Load();
                //Поскольку операция загрузки возвращает новый объект, нужно заново укзать на источник данных
                dataGrid.ItemsSource = greatBookOfGrudges.Records;
                TBWarning.Text += "Загрузка завершена из файла " + greatBookOfGrudges.Path() + "\n";
                TBWarning.ScrollToEnd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!");
            }
        }
        //Обработчик кнопки Добавить. Добавляет пустую строку
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveCancel)
            {
                MessageBox.Show("Нельзя добавить строку, пока редактирование не завершено!", "Ошибка!");
            }
            int size = greatBookOfGrudges.Records.Count;
            if (size == 0)
            {
                greatBookOfGrudges.Records.Add(new GreatBookOfGrudgesRecord());
            }
            else
            {
                uint last_id = greatBookOfGrudges.Records[size-1].GrudgeNumber;
                greatBookOfGrudges.Records.Add(new GreatBookOfGrudgesRecord((uint)(last_id + 1)));
                TBWarning.Text += "Добавлена строка\n";
                TBWarning.ScrollToEnd();

            }
        }
        //Обработчик кнопки Сохранить. Сохраняет БД
        private void MIFSave_Click(object sender, RoutedEventArgs e)
        {
            if (SaveCancel)
            {
                MessageBox.Show("Нельзя сохранить, пока редактирование не завершено!", "Ошибка!");
            }
            else 
            {
                greatBookOfGrudges.Save();
                TBWarning.Text += "Сохранение завершено в файл "+ greatBookOfGrudges.Path() +"\n";
                TBWarning.ScrollToEnd();
            }
        }




        private void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //int row = e.Row.GetIndex();
            //DataGridRow test = new DataGridRow();
            //MessageBox.Show(e.Row.Item.ToString());
                //dataGrid.Columns[1].CellStyle = styleRed;
        }

        private void UpdateColors(int index, string[] colors, string[] values)
        {

        }
        /// <summary>
        /// Отмена запрета на сохранение после завершения редактирования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SaveCancel = false;

        }
        /// <summary>
        /// Присваивание переменной автосохранения значения Истина
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MISAutosave_Checked(object sender, RoutedEventArgs e)
        {
            _setting_as = true;
            TBWarning.Text += "Автосохранение включено\n";
            TBWarning.ScrollToEnd();

        }
        /// <summary>
        /// Присваивание переменной автосохранения значения Ложь
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MISAutosave_Unchecked(object sender, RoutedEventArgs e)
        {
            _setting_as = false;
            TBWarning.Text += "Автосохранение отключено\n";
            TBWarning.ScrollToEnd();
        }
        /// <summary>
        /// Отображение модального окна поиска
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            var windowSearch = new WindowSearch(greatBookOfGrudges);
            windowSearch.Owner = this;
            windowSearch.ShowDialog();
        }
        /// <summary>
        /// Кнопка удаления одной строки. Удаляет одну строку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            int sIndex = dataGrid.SelectedIndex;
            if (sIndex != -1)
            {
                greatBookOfGrudges.Records.RemoveAt(sIndex);
                sIndex++;
                TBWarning.Text += "Строка №" + sIndex.ToString() + "удалена\n";
                TBWarning.ScrollToEnd();

            }
            else
                MessageBox.Show("Нажми на строку для удаления!", "Ошибка");
        }
        /// <summary>
        /// Очищает всё. Очищает как коллекцию, так и таблицу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            greatBookOfGrudges.Records.Clear();
        }
        /// <summary>
        /// Настройка добавления автоматического строк. Включает 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MISAutoadd_Unchecked(object sender, RoutedEventArgs e)
        {
            dataGrid.CanUserAddRows = false;
            TBWarning.Text += "Автодобавление строки отключено\n";
            TBWarning.ScrollToEnd();
        }
        /// <summary>
        /// Настройка добавления автоматического строк. Выключает 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MISAutoadd_Checked(object sender, RoutedEventArgs e)
        {
            dataGrid.CanUserAddRows = true;
            TBWarning.Text += "Автодобавление строки включено\n";
            TBWarning.ScrollToEnd();
        }
        /// <summary>
        /// Включение запрета на сохранение ячеек, пока не закончится сохранения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            SaveCancel = true;
        }
        //Выбор пути для сохранения
        private void MISSavePath_Click(object sender, RoutedEventArgs e)
        {
            string newpath;
            var windowEditPath = new WindowPath();
            windowEditPath.Owner = this;
            windowEditPath.ShowDialog();
            if (windowEditPath.chosen_path != null)
            {
                newpath = windowEditPath.chosen_path;
                greatBookOfGrudges.UpdatePath(newpath);
                TBWarning.Text += "Выбран новый путь по умолчанию: " + greatBookOfGrudges.Path() + "\n";
            }
        }
    }
}
