using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Grocery.Core.Services
{
    /// <summary>
    /// Service die alle aankopen (Client, GroceryList, Product) teruggeeft
    /// waarin een specifiek product voorkomt.
    /// </summary>
    public class BoughtProductsService : IBoughtProductsService
    {
        // Repositories die nodig zijn om data op te halen
        private readonly IGroceryListItemsRepository _listItemsRepo;
        private readonly IGroceryListRepository _listsRepo;
        private readonly IClientRepository _clientsRepo;
        private readonly IProductRepository _productsRepo;

        /// <summary>
        /// Constructor: injecteert de vereiste repositories.
        /// </summary>
        public BoughtProductsService(
            IGroceryListItemsRepository listItemsRepo,
            IGroceryListRepository listsRepo,
            IClientRepository clientsRepo,
            IProductRepository productsRepo)
        {
            _listItemsRepo = listItemsRepo;
            _listsRepo = listsRepo;
            _clientsRepo = clientsRepo;
            _productsRepo = productsRepo;
        }

        /// <summary>
        /// Haalt alle aankopen voor een bepaald productId op.
        /// Combineert data uit GroceryListItems, GroceryLists, Clients en Products.
        /// </summary>
        /// <param name="productId">Het ID van het gewenste product (null of <= 0 levert lege lijst op).</param>
        /// <returns>Lijst met BoughtProducts, elk bevat Client, GroceryList en Product.</returns>
        public List<BoughtProducts> Get(int? productId)
        {
            // Guard: als productId niet geldig is, meteen een lege lijst teruggeven
            if (productId is null || productId <= 0)
            {
                Debug.WriteLine($"[BoughtProductsService] Ongeldig productId: {(productId is null ? "null" : productId.Value.ToString())}");
                return new List<BoughtProducts>();
            }

            Debug.WriteLine($"[BoughtProductsService] Ophalen aankopen voor productId={productId}");

            //  Alle GroceryListItems ophalen en filteren op dit productId
            var itemsForProduct = _listItemsRepo
                .GetAll()
                .Where(i => i.ProductId == productId.Value)
                .ToList();

            Debug.WriteLine($"[BoughtProductsService] Items met productId={productId}: {itemsForProduct.Count}");

            // Resultaatlijst vullen door Client + List + Product
            var results = new List<BoughtProducts>();

            foreach (var item in itemsForProduct)
            {
                // Haal bijbehorende boodschappenlijst op
                var list = _listsRepo.Get(item.GroceryListId);
                if (list is null) continue;

                // Haal de client van deze boodschappenlijst op
                var client = _clientsRepo.Get(list.ClientId);
                if (client is null) continue;

                // Haal het product op
                var product = _productsRepo.Get(item.ProductId);
                if (product is null) continue;

                Debug.WriteLine($"[BoughtProductsService] Match gevonden: Client={client.Name}, List={list.Name}, Product={product.Name}, Amount={item.Amount}");

                // Voeg samen in BoughtProducts-model
                results.Add(new BoughtProducts(client, list, product));
            }

            // Sorteer de resultaten alfabetisch op Client-naam en List-naam
            results = results
                .OrderBy(r => r.Client.Name)
                .ThenBy(r => r.GroceryList.Name)
                .ToList();

            Debug.WriteLine($"[BoughtProductsService] Totaal matches: {results.Count}");
            return results;
        }
    }
}
