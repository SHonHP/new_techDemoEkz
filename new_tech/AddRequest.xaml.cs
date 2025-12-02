using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class AddRequest : Window
    {
        Entities db = new Entities();
        public AddRequest()
        {
            InitializeComponent();
            LoadTypes();
        }
        /// <summary>
        /// вывод типов партнеров
        /// </summary>
        private void LoadTypes()
        {
            var partnerTypes = db.Type_partner_import_
                .Select(p => new { p.id, p.name })
                .Distinct()
                .ToList();


            TypePartner.DisplayMemberPath = "name";
            TypePartner.SelectedValuePath = "id";
            TypePartner.ItemsSource = partnerTypes;
        }


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            string partn = TypePartner.Text?.Trim();
            string name = NamePartner.Text?.Trim();

            if (string.IsNullOrEmpty(partn) && string.IsNullOrEmpty(name))
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("У вас заполнены некоторые поля, вы уверены?",
                    "Подтверждение", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }

            }
        }
        /// <summary>
        /// обработка выбора типа партнера
        /// </summary>

        private void TypePartner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypePartner.SelectedItem == null)
                return;
                LoadPartnersByType();
                ClearPartnerFields();            
        }

        /// <summary>
        /// метод для вывода партнеров по выбранному типу
        /// </summary>
        private void LoadPartnersByType()
        {
            if (TypePartner.SelectedValue != null)
            {
                int selectedTypeId = (int)TypePartner.SelectedValue;

                var partners = db.Partners_import_
                    .Where(p => p.id_type_partner == selectedTypeId)
                    .Select(p => new { p.id, p.name })
                    .ToList();

                NamePartner.DisplayMemberPath = "name";
                NamePartner.SelectedValuePath = "id";
                NamePartner.ItemsSource = partners;

                NamePartner.SelectedIndex = -1;

            }
            else
            {
                NamePartner.ItemsSource = null;
                return;
            }
        }

        private void NamePartner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (NamePartner.SelectedValue != null)
            {
                var selectedPartner = db.Partners_import_
                    .FirstOrDefault(p => p.id == (int)NamePartner.SelectedValue);

                if (selectedPartner != null)
                {
                    Director.Text = selectedPartner.director;
                    Adres.Text = selectedPartner.adress_partner;
                    rating.Text = Convert.ToString(selectedPartner.rating);
                    Telephon.Text = selectedPartner.telephone;
                    email.Text = selectedPartner.email;
                }
            }
            else
            {
                ClearPartnerFields();
            }
        }

        private void ClearPartnerFields()
        {
            Director.Text = string.Empty;
            Adres.Text = string.Empty;
            rating.Text = string.Empty;
            Telephon.Text = string.Empty;
            email.Text = string.Empty;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
                return;
            int partnerId = GetPartnerId();
            var partnerExists = db.Partners_import_.Any(p => p.id == partnerId);
            if (!partnerExists)
            {
                MessageBox.Show("Выбранный партнёр не найден в базе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var newRequest = new Partner_products_request_import_
            {
                id_partner = partnerId,
                id_product = null,
                value_product = 0
            };

            db.Partner_products_request_import_.Add(newRequest);
            db.SaveChanges();

            MessageBox.Show("Заявка успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
        private bool ValidateForm()
        {
            if (NamePartner.SelectedValue == null)
            {
                MessageBox.Show("Выберите партнера");
                return false;
            }

            return true;
        }
        private int GetPartnerId()
        {
            int.TryParse(NamePartner.SelectedValue.ToString(), out int partnerId);
            return partnerId;
        }

    }
}
