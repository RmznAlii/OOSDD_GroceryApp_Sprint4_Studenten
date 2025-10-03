using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    /// <summary>
    /// ViewModel dat de aankopen per product toont. 
    /// Koppelt producten aan de klanten en boodschappenlijsten waarin ze voorkomen.
    /// </summary>
    public partial class BoughtProductsViewModel : BaseViewModel
    {
        private readonly IBoughtProductsService _boughtProductsService;
        
        [ObservableProperty]
        Product selectedProduct;
        public ObservableCollection<BoughtProducts> BoughtProductsList { get; set; } = [];
        public ObservableCollection<Product> Products { get; set; }

        /// <summary>
        /// Constructor, zet de services en vult meteen de lijst met beschikbare producten.
        /// </summary>
        public BoughtProductsViewModel(IBoughtProductsService boughtProductsService, IProductService productService)
        {
            _boughtProductsService = boughtProductsService;
            Products = new(productService.GetAll());
        }
        
        /// <summary>
        /// Callback die afgaat zodra de waarde van <see cref="SelectedProduct"/> verandert.
        /// Leegt eerst de huidige lijst en vult deze daarna met de aankopen van het gekozen product.
        /// </summary>
        /// <param name="oldValue">Product dat eerder geselecteerd was</param>
        /// <param name="newValue">Het product dat nu geselecteerd is</param>
        partial void OnSelectedProductChanged(Product? oldValue, Product newValue)
        {
            if (newValue != null)
            {
                System.Diagnostics.Debug.WriteLine($"Nieuw product geselecteerd: {newValue.Name} (Id: {newValue.Id})");
                
                BoughtProductsList.Clear();

                // Nieuwe aankopen ophalen via de service
                var boughtProducts = _boughtProductsService.Get(newValue.Id);
                System.Diagnostics.Debug.WriteLine($"Aantal resultaten opgehaald: {boughtProducts.Count}");
                
                // Toevoegen van elk resultaat aan de observable collectie
                foreach (var item in boughtProducts)
                {
                    System.Diagnostics.Debug.WriteLine($"Resultaat toegevoegd: {item.Client.Name} - {item.GroceryList.Name}");
                    BoughtProductsList.Add(item);
                }
                
                System.Diagnostics.Debug.WriteLine($"Huidige aantal items in BoughtProductsList: {BoughtProductsList.Count}");
            }
        }

        /// <summary>
        /// Command waarmee vanuit code-behind of de UI een product gekozen kan worden.
        /// Het instellen van <see cref="SelectedProduct"/> triggert automatisch de update van de lijst.
        /// </summary>
        /// <param name="product">Het product dat geselecteerd moet worden</param>
        [RelayCommand]
        public void NewSelectedProduct(Product product)
        {
            SelectedProduct = product;
        }
    }
}
