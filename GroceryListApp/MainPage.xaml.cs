using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GroceryListApp
{
	public partial class MainPage 
	{

        public MainPage()
		{
			InitializeComponent();
        }

        internal async void OnLoginButtonClickedAsync(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GroceryListPage());
        }

        private void OnRegisterButtonClicked(object sender, EventArgs e)
        {

        }
    }
}
