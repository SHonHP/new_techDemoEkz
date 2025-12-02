using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace new_tech
{
    /// <summary>
    /// Логика взаимодействия для ViborProduct.xaml
    /// </summary>
    public partial class ViborProduct : Window
    {
        private Entities db = new Entities();
        private Partners_import_ _partner;
        private Partner_products_request_import_ _value;

        public ViborProduct(Partners_import_ partner, Partner_products_request_import_ value)
        {
            InitializeComponent();

            _partner = partner;
            _value = value;

            LoadPartnerData();
            LoadProductsList();
        }

        /// <summary>
        /// вывод имени партнера
        /// </summary>
        private void LoadPartnerData()
        {
            PartnerP.Content = _partner?.name ?? "Неизвестный партнёр";
        }

        /// <summary>
        /// загрузка всей продукции
        /// </summary>
        private void LoadProductsList()
        {
            var products = db.Products_import_.ToList();

            NameProduct.DisplayMemberPath = "name";
            NameProduct.SelectedValuePath = "id";
            NameProduct.ItemsSource = products;
        }

        /// <summary>
        /// показ минимальной стоимости
        /// </summary>
        private void NameProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NameProduct.SelectedValue == null)
            {
                costlb.Content = "";
                return;
            }

            int id = (int)NameProduct.SelectedValue;
            var p = db.Products_import_.FirstOrDefault(x => x.id == id);

            if (p != null)
                costlb.Content = String.Format("{0:F2}", p.mincost_forpartner);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow m = new MainWindow();
            m.Show();
            this.Close();
        }

        /// <summary>
        /// сохранение продукта в заявку
        /// </summary>
        private void Vvesti_Click(object sender, RoutedEventArgs e)
        {
            if (NameProduct.SelectedValue == null)
            {
                MessageBox.Show("Выберите продукцию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(Kolvo.Text, out int kol) || kol < 0)
            {
                MessageBox.Show("Введите корректное неотрицательное количество.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

                _value.id_product = (int)NameProduct.SelectedValue;
                _value.value_product = kol;
                db.SaveChanges();
                MessageBox.Show("Количество сохранено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow m = new MainWindow();
                m.Show();
                this.Close();
        }
    }
}
