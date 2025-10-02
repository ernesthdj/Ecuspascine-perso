# Architecture MVVM en WPF (.NET)

## Principe général

Le pattern **MVVM (Model -- View -- ViewModel)** est utilisé pour
séparer les responsabilités :

-   **Model** : données métier ou DTO (venant de
    `Ecauspacine.Contracts`).
-   **View** : interface graphique (XAML). Ne contient **aucune logique
    métier**.
-   **ViewModel** : logique de présentation. Expose des propriétés et
    des commandes, notifie la vue quand elles changent.

## Flux typique

1.  L'utilisateur interagit avec la **View** (ex: bouton cliqué).
2.  La **View** transmet l'action au **ViewModel** (via commande ou
    event binding).
3.  Le **ViewModel** appelle un **Service** (ex: ApiClient).
4.  Le **Service** communique avec l'API ou la DB et renvoie les
    données.
5.  Le **ViewModel** met à jour ses propriétés.
6.  Grâce à **INotifyPropertyChanged**, la **View** est automatiquement
    mise à jour.

## Exemple concret : Health Check

-   **View (HealthView.xaml)** : bouton "Check API" + TextBox liée à
    `StatusText`.
-   **ViewModel (HealthViewModel.cs)** :
    -   Propriété `StatusText`
    -   Méthode `CheckAsync()` qui appelle le service
-   **Service (HealthService.cs)** :
    -   Appelle `ApiClient.GetHealthAsync()`
-   **ApiClient.cs** :
    -   Fait un appel HTTP GET `/api/health`

### Schéma

    View (XAML)  <—bind—>  ViewModel  <—appelle—>  Service  <—appelle—>  ApiClient  <—HTTP—>  API

## Injection de dépendances (DI)

-   **Singleton** : une seule instance partagée dans toute l'application
    (ex: HttpClient, ApiClient).
-   **Transient** : une nouvelle instance à chaque demande (ex:
    ViewModels).

### Exemple (dans App.xaml.cs)

``` csharp
services.AddSingleton(new HttpClient { BaseAddress = new Uri("http://localhost:5001") });
services.AddSingleton<IApiClient, ApiClient>();
services.AddSingleton<IHealthService, HealthService>();
services.AddTransient<HealthViewModel>();
services.AddTransient<MainWindow>();
```

## Bonnes pratiques

-   Pas de logique métier dans les Views.
-   Les ViewModels ne connaissent pas les Views (découplage).
-   Les Services encapsulent l'accès à l'API ou la DB.
-   Les Models/DTO viennent de `Contracts` pour garder cohérence
    API/Client.
