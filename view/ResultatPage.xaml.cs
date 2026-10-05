namespace CalculateurAge.Views;

// Relie les parametres "nom" et "age" de l'URL aux proprietes Nom et Age.
[QueryProperty(nameof(Nom), "nom")]
[QueryProperty(nameof(Age), "age")]
public partial class ResultatPage : ContentPage
{
    // Ces proprietes sont remplies par la navigation, APRES le constructeur.
    public string Nom { get; set; }
    public string Age { get; set; }

    public ResultatPage() => InitializeComponent();

    // Appele a CHAQUE affichage de la page.
    protected override void OnAppearing()
    {
        base.OnAppearing();
        lblMessage.Text = $"{Nom}, vous avez {Age} ans";
    }

    // ".." = revenir a la page precedente.
    private async void OnRetourClicked(object s, EventArgs e)
        => await Shell.Current.GoToAsync("..");
}