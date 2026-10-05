using CalculateurAge.Views;

namespace CalculateurAge;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        // Sans cette ligne : exception "route inconnue".
        Routing.RegisterRoute(nameof(ResultatPage), typeof(ResultatPage));
    }
}