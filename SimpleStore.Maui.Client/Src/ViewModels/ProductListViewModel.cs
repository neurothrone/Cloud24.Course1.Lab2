using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using SimpleStore.Core.Enums;
using SimpleStore.Core.Interfaces;
using SimpleStore.Core.Models;
using SimpleStore.Core.Services;
using SimpleStore.Core.Utils;
using SimpleStore.Core.ViewModels;
using SimpleStore.Maui.Client.Messages;
using SimpleStore.Maui.Client.Navigation;
using SimpleStore.Maui.Client.Services;

namespace SimpleStore.Maui.Client.ViewModels;

public class ProductListViewModel :
    ViewModel,
    IRecipient<CheckoutCompletedMessage>,
    IRecipient<ClearCartMessage>,
    IRecipient<CurrencyChangedMessage>
{
    private readonly INavigator _navigator;
    private readonly CurrencyService _currencyService;
    private readonly ProductStoreService _productStoreService;
    private readonly AppState _appState;

    private ObservableCollection<CartProductViewModel> _cartProducts = [];

    public ObservableCollection<CartProductViewModel> CartProducts
    {
        get => _cartProducts;
        set
        {
            _cartProducts = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<StoreProductViewModel> _storeProducts = [];

    public ObservableCollection<StoreProductViewModel> StoreProducts
    {
        get => _storeProducts;
        set
        {
            _storeProducts = value;
            OnPropertyChanged();
        }
    }

    public string CurrencyFormat => _appState.Currency.Formatted();

    public ICommand AddProductCommand { get; }
    public ICommand RemoveProductCommand { get; }
    public ICommand CheckoutCommand { get; }
    public ICommand ClearCartCommand { get; }

    public ProductListViewModel(
        INavigator navigator,
        CurrencyService currencyService,
        ProductStoreService productStoreService,
        AppState appState)
    {
        _navigator = navigator;
        _currencyService = currencyService;
        _productStoreService = productStoreService;
        _appState = appState;

        AddProductCommand = new Command<ProductViewModel>(AddProduct);
        RemoveProductCommand = new Command<ProductViewModel>(RemoveProduct);

        CheckoutCommand = new Command(Checkout, HasProductsInCart);
        ClearCartCommand = new Command(() => ClearCart(returnCart: true), HasProductsInCart);

        CartProducts.CollectionChanged += CartProducts_CollectionChanged;
        SubscribeToMessages();
    }

    ~ProductListViewModel() => CartProducts.CollectionChanged -= CartProducts_CollectionChanged;

    private void CartProducts_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ((Command)CheckoutCommand).ChangeCanExecute();
        ((Command)ClearCartCommand).ChangeCanExecute();
    }

    private void SubscribeToMessages()
    {
        WeakReferenceMessenger.Default.Register<CheckoutCompletedMessage>(this);
        WeakReferenceMessenger.Default.Register<ClearCartMessage>(this);
        WeakReferenceMessenger.Default.Register<CurrencyChangedMessage>(this);
    }

    private bool HasProductsInCart() => CartProducts.Count > 0;

    #region Load Store Products

    protected override void Initialize()
    {
        LoadStoreProducts();
    }

    private void LoadStoreProducts()
    {
        Product[] products = _productStoreService.GetAvailableProducts();

        var viewModels = products
            .Select(p => new StoreProductViewModel(
                new Product
                {
                    Name = p.Name,
                    Price = _currencyService.Convert(p.Price, Currency.Sek, _appState.Currency),
                    Quantity = p.Quantity
                }))
            .ToArray();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var productViewModel in viewModels)
            {
                StoreProducts.Add(productViewModel);
            }
        });
    }

    #endregion

    #region Add Product to Cart

    private void AddProduct(ProductViewModel product)
    {
        if (product.Quantity < 1)
            return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            AddProductToCart(product);
            RemoveProductFromStore(product);
        });
    }

    private void AddProductToCart(ProductViewModel product)
    {
        var productInCart = CartProducts.FirstOrDefault(p => p.Name.Equals(product.Name));
        if (productInCart is not null)
        {
            productInCart.Quantity += 1;
            return;
        }

        var productToAdd = new CartProductViewModel(
            new Product { Name = product.Name, Price = product.Price, Quantity = 1 }
        );

        CartProducts.Add(productToAdd);
    }

    private void RemoveProductFromStore(ProductViewModel product)
    {
        var productToUpdate = StoreProducts.FirstOrDefault(p => p.Name.Equals(product.Name));
        if (productToUpdate is not null)
            productToUpdate.Quantity -= 1;
    }

    #endregion

    #region Remove Product from Cart

    private void RemoveProduct(ProductViewModel product)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (RemoveProductFromCart(product))
                ReturnProductToStore(product);
        });
    }

    private bool RemoveProductFromCart(ProductViewModel product)
    {
        var foundProduct = CartProducts.FirstOrDefault(p => p.Name.Equals(product.Name));
        if (foundProduct is null)
            return false;

        if (foundProduct.Quantity <= 1)
            CartProducts.Remove(foundProduct);
        else
            foundProduct.Quantity -= 1;

        return true;
    }

    #endregion

    #region Return Product to Store when clearing Cart

    private void ClearCart(bool returnCart = true)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (returnCart)
                ReturnCartProductsToStore();

            CartProducts.Clear();
        });
    }

    private void ReturnCartProductsToStore()
    {
        foreach (var product in CartProducts)
        {
            for (int i = 0; i < product.Quantity; i++)
            {
                ReturnProductToStore(product);
            }
        }
    }

    private void ReturnProductToStore(ProductViewModel product)
    {
        var productInStore = StoreProducts.FirstOrDefault(p => p.Name.Equals(product.Name));
        if (productInStore is not null)
            productInStore.Quantity += 1;
    }

    #endregion

    #region Navigate to Checkout Page

    private async void Checkout()
    {
        var parameters = new Dictionary<string, object>
        {
            { "products", CartProductsToJson() },
        };

        await _navigator.GoToAsync(nameof(AppRoute.Checkout), parameters);
    }

    private string CartProductsToJson()
    {
        var products = CartProducts
            .Select(viewModel => viewModel.Product)
            .ToArray();

        return JsonSerializer.Serialize(products);
    }

    #endregion

    #region Update Currency for all Products

    private void UpdateAllProductsCurrency(Currency currency)
    {
        _appState.Currency = currency;
        OnPropertyChanged(nameof(CurrencyFormat));

        var originalProducts = _productStoreService.GetAvailableProducts();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            UpdateProductPrices(originalProducts, ref _storeProducts);
            UpdateProductPrices(originalProducts, ref _cartProducts);
        });
    }

    private void UpdateProductPrices<T>(Product[] originalProducts, ref ObservableCollection<T> products)
        where T : ProductViewModel
    {
        foreach (var product in products)
        {
            var originalProduct = originalProducts.FirstOrDefault(p => p.Name.Equals(product.Name));
            product.Price = _currencyService.Convert(
                originalProduct?.Price ?? product.Price,
                Currency.Sek,
                _appState.Currency);
        }
    }

    #endregion

    #region IRecipient<CheckoutCompletedMessage>

    public void Receive(CheckoutCompletedMessage message)
    {
        ClearCart(returnCart: false);
    }

    #endregion

    #region IRecipient<ClearCartMessage>

    public void Receive(ClearCartMessage message)
    {
        ClearCart();
    }

    #endregion

    #region IRecipient<CurrencyChangedMessage>

    public void Receive(CurrencyChangedMessage message)
    {
        UpdateAllProductsCurrency(message.Currency);
    }

    #endregion
}