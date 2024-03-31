using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace DBWPFNETGUI
{
    /// <summary>
    /// Логика взаимодействия для WindowSearch.xaml
    /// </summary>
    public partial class WindowSearch : Window
    {
        //{Хранение книги
        private GreatBookOfGrudges greatBookOfGrudges = new GreatBookOfGrudges("");
        static string[] source = { "GrudgeNumber", "Grudge", "DateOfWrongdoing", "FoolName", "RedemptionStatus", "Witness", "Evidence", "GrudgeLevel" };
        public WindowSearch(GreatBookOfGrudges book)
        {

            InitializeComponent();
            this.greatBookOfGrudges = book;
            //Настройка
            dgRecords.CanUserAddRows = false;
            //Связывание источника и dataGrid
            dgRecords.ItemsSource = greatBookOfGrudges.Records;
            //Заполнение ListBox
            lstRecords.ItemsSource = source;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (lstRecords.SelectedIndex != -1)
            {
                string searchString = txtInput.Text;
                if (searchString != "")
                {

                
                ObservableCollection<GreatBookOfGrudgesRecord> foundGrudges = new ObservableCollection<GreatBookOfGrudgesRecord>();

                foreach (GreatBookOfGrudgesRecord grudge in greatBookOfGrudges.Records)
                {
                    switch (lstRecords.SelectedIndex)
                    {
                        case 0:
                            if (grudge.GrudgeNumber.ToString() == searchString)
                                foundGrudges.Add(grudge);
                            break;
                        case 1:
                            if (grudge.Grudge == searchString)
                                foundGrudges.Add(grudge);
                            break;
                        case 2:
                            if (grudge.DateOfWrongdoing == searchString)
                                foundGrudges.Add(grudge);
                            break;
                        case 3:
                            if (grudge.FoolName == searchString)
                                foundGrudges.Add(grudge);
                            break;
                        case 4:
                            if (grudge.RedemptionStatus == searchString)
                                foundGrudges.Add(grudge);
                            break;
                        case 5:
                            if (grudge.Witness == searchString)
                                foundGrudges.Add(grudge);
                            break;
                        case 6:
                            if (grudge.Evidence == searchString)
                                foundGrudges.Add(grudge);
                            break;
                        case 7:
                            if (grudge.GrudgeLevel == searchString)
                                foundGrudges.Add(grudge);
                            break;
                    }
                }
                dgRecords.ItemsSource = foundGrudges;
            }
                else
                {
                    dgRecords.ItemsSource = greatBookOfGrudges.Records;
                }
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            dgRecords.ItemsSource = greatBookOfGrudges.Records;
        }
    }
}
