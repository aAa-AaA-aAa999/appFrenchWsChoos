using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Xml.Serialization;


namespace appFrench
{
    public partial class ExperimentalForm : Form
    {
        //cписок кортежей для хранения имени и состояния "выбрано"
        internal static List<(string Category, bool IsChecked)> checkedCategoriesList = new List<(string, bool)>();
        
        //cтрока пути. Состоит из комбинации:
        //первая часть:
        //Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName
        //нужна для обращения к общему каталогу папки, в любом другом случае 
        //путь обращается к bin\debug
        // т.е. AppDomain.CurrentDomain.BaseDirectory получаем bin\debug
        // GetParent() принимает путь и возвращает
        // родительскую директорию этого пути
        // далее .Parent.Parent чтобы подняться от bin\debug
        // .FullName-свойство возвращает полный путь к папке в виде строки.
        // ну и вторая часть - обращение к нужным папкам
        private string filePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "Resources", "categoryInfo", "checkedCategoriesList.xml");


        public ExperimentalForm()
        {
            InitializeComponent();

            if (File.Exists(filePath) && new FileInfo(filePath).Length != 0)
            {
                MessageBox.Show("If");
                // десериализация списка кортежей с категорией и состоянием "выбрано"
                // при загрузке формы, если файл существует
                // и он не пустой
                checkedCategoriesList = ListExtensions.LoadCheckedListBoxState(filePath);
            }
            else
            {
                MessageBox.Show("Else");

                // Получение уникальных категорий из базы данных если объект
                // Eщё не был сериализован или по каким то
                // Причинам объект не был найден, или
                // Оказался пустым
                // Создание файла заново 
                checkedCategoriesList = ListExtensions.GetUniqueCategoriesFromDatabase();
                File.Create(filePath).Close();
            }
            
            // Заполнение элемента CheckedListBox с помощью списка
            foreach (var category in checkedCategoriesList)
            {
                checkedListBox1.Items.Add(category.Category);
                int index = checkedListBox1.Items.IndexOf(category.Category);
                if (category.IsChecked)
                {
                    checkedListBox1.SetItemChecked(index, true);
                }
            }
            // Обработчик события нажатия для CheckedListBox
            // Нужен для постоянного обновления списка
            // Чтобы в любой момент закрыть окно и сохранить состояния
            checkedListBox1.ItemCheck += CheckedListBox_ItemCheck;
        }
        public static List<(string Category, bool IsChecked)> getCategList()
        {
            return checkedCategoriesList;
        }
        private void ExperimentalForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //при закрытии формы сохраняем текущие значения в checkedCategoriesList
            ListExtensions.SaveCheckedListBoxState(filePath, checkedCategoriesList);
        }

        //метод внесения изменений в списке
        static void CheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            CheckedListBox checkedListBox = (CheckedListBox)sender;
            string category = checkedListBox.Items[e.Index].ToString();
            bool isChecked = e.NewValue == CheckState.Checked;

            // Ищем категорию в списке и обновляем её состояние выбранности
            for (int i = 0; i < checkedCategoriesList.Count; i++)
            {
                if (checkedCategoriesList[i].Category == category)
                {
                    checkedCategoriesList[i] = (category, isChecked);
                    ListExtensions.SaveCheckedListBoxState(ListExtensions.getFilePath(), checkedCategoriesList);
                    break;
                }
            }
        }


        //для перехода на др.страницу

        private void buttonListWord_Click(object sender, EventArgs e)
        {
            ListWordForm form = new ListWordForm();
            form.ShowDialog();
        }
    }
}
