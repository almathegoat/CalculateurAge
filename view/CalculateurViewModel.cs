using System.Collections.ObjectModel;
using CalculateurAge.Views;

namespace CalculateurAge.ViewModels;

// Contient l'ETAT de l'ecran et les ACTIONS possibles.
// Aucun controle d'interface ici (pas de Label, Entry, DisplayAlert...).
public class CalculateurViewModel : BaseViewModel
{
    // Champs prives : la vraie donnee.
    private string _nom = "";
    private DateTime _dateNaissance = DateTime.Today.AddYears(-20);
    private string _resultat = "";
    private bool _resultatVisible;
    private string _message = "";
    private string _joursAnniversaire = "";
    private string _erreur = "";
    private int _age;

    // ---------- Proprietes publiques (ce que le XAML voit) ----------

    public string Nom
    {
        get => _nom;
        set
        {
            if (SetField(ref _nom, value))
                RafraichirCommandes();
        }
    }

    public DateTime DateNaissance
    {
        get => _dateNaissance;
        set
        {
            if (SetField(ref _dateNaissance, value))
            {
                // Fonctionnalite : refus d'une date future.
                Erreur = _dateNaissance.Date > DateTime.Today
                    ? "La date de naissance ne peut pas etre dans le futur."
                    : "";
                RafraichirCommandes();
            }
        }
    }

    public string Resultat
    {
        get => _resultat;
        set => SetField(ref _resultat, value);
    }

    public bool ResultatVisible
    {
        get => _resultatVisible;
        set => SetField(ref _resultatVisible, value);
    }

    // Fonctionnalite : message "Majeur" / "Mineur".
    public string Message
    {
        get => _message;
        set => SetField(ref _message, value);
    }

    // Fonctionnalite : jours restants avant le prochain anniversaire.
    public string JoursAnniversaire
    {
        get => _joursAnniversaire;
        set => SetField(ref _joursAnniversaire, value);
    }

    public string Erreur
    {
        get => _erreur;
        set
        {
            if (SetField(ref _erreur, value))
                OnPropertyChanged(nameof(ErreurVisible));
        }
    }

    public bool ErreurVisible => !string.IsNullOrEmpty(Erreur);

    // Fonctionnalite : historique des calculs.
    public ObservableCollection<string> Historique { get; } = new();

    // ---------- Commandes (liees aux Button.Command dans le XAML) ----------

    public RelayCommand CalculerCommand { get; }
    public RelayCommand EffacerCommand { get; }
    public RelayCommand ViderHistoriqueCommand { get; }
    public RelayCommand VoirDetailsCommand { get; }

    public CalculateurViewModel()
    {
        CalculerCommand = new RelayCommand(Calculer, PeutCalculer);
        EffacerCommand = new RelayCommand(Effacer);
        ViderHistoriqueCommand = new RelayCommand(() => Historique.Clear());
        VoirDetailsCommand = new RelayCommand(
            () => _ = VoirDetailsAsync(),
            () => ResultatVisible);
    }

    // ---------- Logique metier ----------

    private bool PeutCalculer()
        => !string.IsNullOrWhiteSpace(Nom)
           && DateNaissance.Date <= DateTime.Today;

    private void RafraichirCommandes()
    {
        CalculerCommand.Rafraichir();
        VoirDetailsCommand.Rafraichir();
    }

    private void Calculer()
    {
        DateTime aujourdhui = DateTime.Today;
        DateTime naissance = DateNaissance.Date;

        int age = aujourdhui.Year - naissance.Year;
        if (naissance > aujourdhui.AddYears(-age)) age--;
        _age = age;

        Resultat = $"{Nom}, vous avez {age} ans";
        Message = age >= 18 ? "Majeur" : "Mineur";
        JoursAnniversaire = TexteAnniversaire(naissance, aujourdhui);
        ResultatVisible = true;

        Historique.Insert(0,
            $"{Nom} : {age} ans (ne(e) le {naissance:dd/MM/yyyy})");

        RafraichirCommandes();
    }

    private static string TexteAnniversaire(DateTime naissance, DateTime aujourdhui)
    {
        DateTime prochain = AnniversaireEn(naissance, aujourdhui.Year);
        if (prochain < aujourdhui)
            prochain = AnniversaireEn(naissance, aujourdhui.Year + 1);

        int jours = (prochain - aujourdhui).Days;
        return jours == 0
            ? "Bon anniversaire, c'est aujourd'hui !"
            : $"Prochain anniversaire dans {jours} jour(s)";
    }

    // Gere le 29 fevrier les annees non bissextiles.
    private static DateTime AnniversaireEn(DateTime naissance, int annee)
    {
        int jour = naissance.Day;
        if (naissance.Month == 2 && jour == 29 && !DateTime.IsLeapYear(annee))
            jour = 28;
        return new DateTime(annee, naissance.Month, jour);
    }

    // Fonctionnalite : remise a zero de tous les champs.
    private void Effacer()
    {
        Nom = "";
        DateNaissance = DateTime.Today.AddYears(-20);
        Resultat = "";
        Message = "";
        JoursAnniversaire = "";
        Erreur = "";
        ResultatVisible = false;
        RafraichirCommandes();
    }

    // Navigation depuis le ViewModel vers ResultatPage.
    private async Task VoirDetailsAsync()
    {
        await Shell.Current.GoToAsync(
            $"{nameof(ResultatPage)}?nom={Uri.EscapeDataString(Nom)}&age={_age}");
    }
}