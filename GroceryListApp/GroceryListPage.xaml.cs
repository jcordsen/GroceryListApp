using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;
using GroceryListApp.Model;

namespace GroceryListApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GroceryListPage : ContentPage
    {
        public GroceryListPage()
        {
            InitializeComponent();
        
            Command cmd = new Command(() =>
            {
                string id = System.Guid.NewGuid().ToString();
                Model.GroceryItem item = new Model.GroceryItem()
                {
                    _id = id,
                    what = groceryItem.Text,
                    ownerId = "jac"
                };

                Task.Run(async () => await Model.ModelExtensions.AddItemAndGetListItemsFromCollection<Model.GroceryItem>(groceryItemsListView, "shopping", item, true));
            });
            AddItem.Command = cmd;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(async () => await Model.ModelExtensions.GetListItemsFromCollection<Model.GroceryItem>(groceryItemsListView, "shopping"));
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            Model.GroceryItem item = (sender as Switch).Parent.BindingContext as Model.GroceryItem;
            if(item != null) 
            {
                Task.Run(async () => await Model.ModelExtensions.PostItemToCollection<Model.GroceryItem>(groceryItemsListView, "shopping", item));
            }
        }
    }

    public class StringToBoolConverter : Xamarin.Forms.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return false;

            return (bool)(value as string == "yes" ? true : false);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "";
            return (bool)value ? "yes" : "no";
        }
    }

}