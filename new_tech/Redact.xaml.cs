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
    public partial class Redact : Window
    {
        private Partner_products_request_import_ _value;
        private Partners_import_ _partner;
        private Entities db = new Entities();

        public Redact(Partners_import_ partner, Partner_products_request_import_ value)
        {
            InitializeComponent();
            _partner = partner;
            _value = value;
            LoadPartnerData();
            LoadComboBoxData();
        }
        /// <summary>
        /// загрузка партнеров
        /// </summary>
        private void LoadPartnerData()
        {
            Director.Text = _partner.director;
            Adres.Text = _partner.adress_partner;
            rating.Text = _partner.rating?.ToString();
            Telephon.Text = _partner.telephone;
            email.Text = _partner.email;
        }

        private void LoadComboBoxData()
        {
                var typePartner = db.Type_partner_import_.ToList();
                TypePartner.DisplayMemberPath = "name";
                TypePartner.SelectedValuePath = "id";
                TypePartner.ItemsSource = typePartner;

                TypePartner.SelectedValue = _partner.id_type_partner;

                var namePartn = db.Partners_import_.ToList();
                NamePartner.DisplayMemberPath = "name";
                NamePartner.SelectedValuePath = "id";
                NamePartner.ItemsSource = namePartn;

                NamePartner.SelectedValue = _partner.id;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void RedactButton_Click(object sender, RoutedEventArgs e)
        {
                int pid = GetPartnerId();
                if (pid <= 0)
                {
                    MessageBox.Show("Выберите корректного партнёра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _value.id_partner = pid;
                db.SaveChanges();

                MessageBox.Show("Заявка успешно обновлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
        }

        /// <summary>
        /// получение id партнера
        /// </summary>
        private int GetPartnerId()
        {
            if (NamePartner.SelectedValue == null)
                return -1;

            int.TryParse(NamePartner.SelectedValue.ToString(), out int partnerId);
            return partnerId;
        }

        private void TypePartner_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypePartner.SelectedItem == null)
                return;
                LoadPartnersByType();
                ClearPartnerFields();
        }
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
    }
}
