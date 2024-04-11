using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Security.Policy;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Xml;
namespace DBWPFNETGUI
{

    //https://waha.fandom.com/ru/wiki/%D0%9A%D0%BD%D0%B8%D0%B3%D0%B8_%D0%9E%D0%B1%D0%B8%D0%B4
    // Класс, представляющий модель данных для одной записи в Великой Книге Обид
    public class GreatBookOfGrudgesRecord
    {
        // Приватное поле для хранения номера обиды
        private uint _grudgeNumber;

        // Приватное поле для хранения описания обиды
        private string _grudge;

        // Приватное поле для хранения даты нанесения обиды
        private string _dateOfWrongdoing;

        // Приватное поле для хранения имени обидчика
        private string _foolName;

        // Приватное поле для хранения статуса искупления обиды
        private string _redemptionStatus;

        // Приватное поле для хранения имени свидетеля обиды
        private string _witness;

        // Приватное поле для хранения доказательств обиды
        private string _evidence;

        // Приватное поле для хранения уровня обиды
        private string _grudgeLevel;

        ////Новое событие
        //public event PropertyChangedEventHandler PropertyChanged;

        //public void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
        // Конструктор по умолчанию
        public GreatBookOfGrudgesRecord()
        {
            _grudgeNumber = 0;
            _grudge = "";
            _dateOfWrongdoing = "";
            _foolName = "";
            _redemptionStatus = "";
            _witness = "";
            _evidence = "";
            _grudgeLevel = "";
        }

        // Конструктор по умолчанию c указанием id
        public GreatBookOfGrudgesRecord(uint grudgeNumber)
        {
            _grudgeNumber = grudgeNumber;
            _grudge = "";
            _dateOfWrongdoing = "";
            _foolName = "";
            _redemptionStatus = "";
            _witness = "";
            _evidence = "";
            _grudgeLevel = "";
        }
        // Конструктор с параметрами
        public GreatBookOfGrudgesRecord(uint grudgeNumber, string grudge, string dateOfWrongdoing, string foolName, string redemptionStatus, string witness, string evidence, string grudgeLevel)
        {
            _grudgeNumber = grudgeNumber;
            _grudge = grudge;
            _dateOfWrongdoing = dateOfWrongdoing;
            _foolName = foolName;
            _redemptionStatus = redemptionStatus;
            _witness = witness;
            _evidence = evidence;
            _grudgeLevel = grudgeLevel;
        }

        // Свойство для получения или установки номера обиды
        [Key]
        public uint GrudgeNumber
        {
            get { return _grudgeNumber; }
            set
            {
                _grudgeNumber = value;
                //OnPropertyChanged("GrudgeNumber");
            }
        }
        // Свойство для получения или установки описания обиды
        public string Grudge
        {
            get { return _grudge; }
            set
            {
                _grudge = value;
                //OnPropertyChanged("Grudge");
            }
        }

        // Свойство для получения или установки даты нанесения обиды
        public string DateOfWrongdoing
        {
            get { return _dateOfWrongdoing; }
            set
            {
                _dateOfWrongdoing = value;
                //OnPropertyChanged("DateOfWrongdoing");
            }
        }

        // Свойство для получения или установки имени обидчика
        public string FoolName
        {
            get { return _foolName; }
            set
            {
                _foolName = value;
                //OnPropertyChanged("FoolName");
            }
        }

        // Свойство для получения или установки статуса искупления обиды
        public string RedemptionStatus
        {
            get { return _redemptionStatus; }
            set
            {
                _redemptionStatus = value;
                //OnPropertyChanged("RedemptionStatus");
            }
        }

        // Свойство для получения или установки имени свидетеля обиды
        public string Witness
        {
            get { return _witness; }
            set
            {
                _witness = value;
                //OnPropertyChanged("Witness");
            }
        }

        // Свойство для получения или установки доказательств обиды
        public string Evidence
        {
            get { return _evidence; }
            set
            {
                _evidence = value;
                //OnPropertyChanged("Evidence");
            }
        }

        // Свойство для получения или установки уровня обиды
        public string GrudgeLevel
        {
            get { return _grudgeLevel; }
            set
            {
                _grudgeLevel = value;
                //OnPropertyChanged("GrudgeLevel");
            }
        }



    }
    //Класс, представляющий собой книгу обид. Он хранит в себе записи GreatBookOfGrudgesRecord в ObservableCollection 
    public class GreatBookOfGrudges
    {
        private string _databasePath;
        public string Path()
        {
            return _databasePath;
        }
        public void UpdatePath(string new_path)
        {
            _databasePath = new_path;
            helper = new SQLiteHelper(new_path);
        }
        private SQLiteHelper helper;
        //Информация об обидах
        public ObservableCollection<GreatBookOfGrudgesRecord> Records { get; set; }
        //Конструктор. Проверяет, существует ли файл. Если он существует, то загружает из файла, если нет, то не загружает
        public GreatBookOfGrudges(string databasePath)
        {
            _databasePath = databasePath;
            Records = new ObservableCollection<GreatBookOfGrudgesRecord>();
            helper = new SQLiteHelper(_databasePath);
            //bool fileExists = File.Exists(_databasePath);
        }

        public void UpdateRecord(GreatBookOfGrudgesRecord record)
        {
            // Находим индекс записи в коллекции
            int index = Records.IndexOf(record);

            // Если запись найдена, обновляем ее
            if (index >= 0)
            {
                Records[index] = record;
                //OnPropertyChanged("Records"); // Уведомляем об изменении коллекции Records
            }
        }

        //Функция сохранения данных в БД
        public void Save()
        {
            helper.SaveObservableCollection(Records);

        }
        //Функция загрузки данных в БД
        public void Load()
        {
            //try
            //{
                //Очистка данных
                this.Records.Clear();
                //Загрузка данных
                this.Records = helper.LoadObservableCollection();
            //}
            //catch(Exception e)
            //{
            //    MessageBox.Show(e.Message, "Ошибка!");
            //}

        }


    }
    //Класс контекста. Необходим для сохранения БД
    public class BookContext : DbContext
    {
        public DbSet<GreatBookOfGrudgesRecord> Records { get; set; } = null!;
        public string _path;
        public BookContext(string databasePath)
        {
            _path = databasePath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_path}");
        }
    }
    //Класс, для сохранения БД. Используется статически
    public class SQLiteHelper
    {
        //Куда сохранять БД (имя файла)
        static public string _databasePath { get; set; }

        public SQLiteHelper(string databasePath)
        {
            _databasePath = databasePath;
        }
        //Статический метод сохранения данных
        public async void SaveObservableCollection(ObservableCollection<GreatBookOfGrudgesRecord> records)
        {
            //Создание контекста
            var dbContext = new BookContext(_databasePath);
            //Заставляем точно создать базу данных, если ее не было
            dbContext.Database.EnsureCreated();
            //Сохранение значений
            using (var context = new BookContext(_databasePath))
            {
                //Очистка базы данных. Закомментировать, если не нужно. Иначе Номер Обиды будет увеличиваться с каждой новой записью
                //File.Delete(_databasePath);
                context.Records.RemoveRange(context.Records);
                await context.SaveChangesAsync();
                context.Records.AddRange(records);
                await context.SaveChangesAsync();
            }
        }
        //Статический метод загрузки данных.
        public ObservableCollection<GreatBookOfGrudgesRecord> LoadObservableCollection()
        {
            //Проверка существования БД
            bool fileExists = File.Exists(_databasePath);
            if (!fileExists)
                throw new Exception("Ошибка загрузки Файла. Возможно, его нет.");
            //Проверка на пустоту файла
            long fileSize = new FileInfo(_databasePath).Length;
            if (fileSize == 0)
                throw new Exception("Ошибка загрузки Файла. Возможно, он пуст.");
            using (var context = new BookContext(_databasePath))
            {
                var records = context.Records.ToList();
                
                return new ObservableCollection<GreatBookOfGrudgesRecord>(records);
            }
        }

    }


}
