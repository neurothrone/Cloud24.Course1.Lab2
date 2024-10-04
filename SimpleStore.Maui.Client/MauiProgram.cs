using Microsoft.Extensions.Logging;
using SimpleStore.Core.Interfaces;
using SimpleStore.Core.Services;
using SimpleStore.Maui.Client.Navigation;
using SimpleStore.Maui.Client.Repositories;
using SimpleStore.Maui.Client.Services;
using SimpleStore.Maui.Client.ViewModels;
using SimpleStore.Maui.Client.Views.Pages;

namespace SimpleStore.Maui.Client;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", "FaSolid");
            })
            .RegisterAppServices()
            .RegisterViewModels()
            .RegisterViews();

        // Register Routes
        Routing.RegisterRoute(nameof(AppRoute.Checkout), typeof(CheckoutPage));

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static MauiAppBuilder RegisterAppServices(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<IFileService, FileService>();
        builder.Services.AddSingleton<INavigator, Navigator>();
        builder.Services.AddSingleton<IUserPreferences, UserPreferences>();

        builder.Services.AddSingleton<AppState>();
        builder.Services.AddSingleton<CurrencyService>();
        builder.Services.AddSingleton<ProductStoreService>();

        return builder;
    }

    private static MauiAppBuilder RegisterViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<AuthViewModel>();
        builder.Services.AddSingleton<ProductListViewModel>();

        builder.Services.AddTransient<CheckoutViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        return builder;
    }

    // ReSharper disable once UnusedMethodReturnValue.Local
    private static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<ProductsPage>();
        builder.Services.AddTransient<CartPage>();
        builder.Services.AddTransient<CheckoutPage>();
        builder.Services.AddTransient<SettingsPage>();

        return builder;
    }
}