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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace new_tech
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // БД подкл
        private Entities db = new Entities();

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Загрузка данных при старте окна.
        /// </summary>
        private void Window_loaded(object sender, RoutedEventArgs e)
        {
                var list = db.Partner_products_request_import_
                    .Join(db.Partners_import_,
                        r => r.id_partner,
                        p => p.id,
                        (r, p) => new { r, p })
                    .Join(db.Type_partner_import_,
                        rp => rp.p.id_type_partner,
                        t => t.id,
                        (rp, t) => new { rp.r, rp.p, t })
                    .Join(db.Products_import_,
                        rpt => rpt.r.id_product,
                        prod => prod.id,
                        (rpt, prod) => new
                        {
                            PartnerName = rpt.p.name,
                            TypeName = rpt.t.name,
                            ProductName = prod.name,
                            MinCost = prod.mincost_forpartner,
                            Count = rpt.r.value_product,
                            UrAdress = rpt.p.adress_partner,
                            Telephone = rpt.p.telephone,
                            Rating = rpt.p.rating,

                            // расчёт стоимости для одной заявки
                            TotalCost = (double)(prod.mincost_forpartner * rpt.r.value_product),
                            RequestId = rpt.r.id,
                            PartnerId = rpt.p.id

                        })
                    .ToList();

                partnersList.ItemsSource = list;

        }
        private void partnersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        /// <summary>
        /// Кнопка открытия окна добавления заявки
        /// </summary>

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var addRequest = new AddRequest();
            addRequest.Show();
            this.Close();
        }

        /// <summary>
        /// Кнопка "Редактировать заявку" — открываем форму редактирования для выбранного партнёра
        /// </summary>
        private void Redact_Click(object sender, RoutedEventArgs e)
        {
            if (partnersList.SelectedItem == null)
            {
                MessageBox.Show("Выберите заявку для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

                dynamic row = partnersList.SelectedItem;

                int requestId = row.RequestId;
                int partnerId = row.PartnerId;

                var request = db.Partner_products_request_import_.FirstOrDefault(r => r.id == requestId);
                var partner = db.Partners_import_.FirstOrDefault(p => p.id == partnerId);

                if (request != null && partner != null)
                {
                    Redact redact = new Redact(partner, request);
                    redact.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось найти данные для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }


        /// <summary>
        /// Кнопка "Выбрать продукцию" — открываем окно выбора продукции для выбранного партнёра
        /// </summary>
        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (partnersList.SelectedItem == null)
            {
                MessageBox.Show("Выберите заявку.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

                dynamic row = partnersList.SelectedItem;

                int requestId = row.RequestId;
                int partnerId = row.PartnerId;

                var request = db.Partner_products_request_import_.FirstOrDefault(r => r.id == requestId);
                var partner = db.Partners_import_.FirstOrDefault(p => p.id == partnerId);

                if (request != null && partner != null)
                {
                    ViborProduct window = new ViborProduct(partner, request);
                    window.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Не удалось найти данные заявки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
        }
    }
}
