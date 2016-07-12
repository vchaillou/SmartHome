// Valentin CHAILLOU
// 4A-AL2 ESGI

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Xml.Linq;
using OxyPlot;
using OxyPlot.Axes;
using LineSeries = OxyPlot.Series.LineSeries;

namespace MonPlotterWPF
{
    // ViewModel associé au Plotter
    public class PlotterViewModel
    {
        public PlotModel Modèle { get; }
        private readonly ObservableCollection<Capteur> Capteurs;
        public ObservableCollection<Grandeur> Grandeurs { get; }
        private readonly ObservableCollection<Donnée> Données;
        private const string NomFichierXML = "..\\..\\Resources\\capteurs.xtim";
        private const string NomDossierSource = "..\\..\\Resources\\netatmo\\";

        // Filtres
        public DateTime DebutFiltre { get; set; }
        public DateTime FinFiltre { get; set; }

        // Analyses
        public ObservableCollection<AnalyseGrandeur> Analyses { get; }
        public AnalyseGrandeur AnalyseSélectionnée { get; set; }

        // Constructeur
        // Charge les fichiers XML et source
        public PlotterViewModel()
        {
            Modèle = new PlotModel
            {
                Subtitle = "SmartHome"
            };
            DateTimeAxis uneAbscisse = new DateTimeAxis
            {
                StringFormat = "dd/MM/yyyy HH:mm"
            };
            Modèle.Axes.Add(uneAbscisse);

            Capteurs = new ObservableCollection<Capteur>();
            Grandeurs = new ObservableCollection<Grandeur>();
            Données = new ObservableCollection<Donnée>();

            DebutFiltre = DateTime.Today;
            FinFiltre = DateTime.Today;

            Analyses = new ObservableCollection<AnalyseGrandeur>()
            {
                new AnalyseTélé(),
                new AnalysePersonnes(),
                new AnalyseSons()
            };
            AnalyseSélectionnée = null;

            MiseAJourXML();
            MiseAJourDataSource();
        }

        // Charge le fichier XML contenant les capteurs
        // Ils sont enregistrés dans le dictionnaire "Capteurs"
        // N'est appelé qu'une seule fois au début
        private void MiseAJourXML()
        {
            XElement desAttributsXML = XDocument.Load(NomFichierXML).Root;
            Capteurs.Clear();
            Grandeurs.Clear();
            if (desAttributsXML == null) return;
            IEnumerable<XElement> uneListeDeCapteursXML = desAttributsXML.Elements("capteur");
            foreach (XElement unCapteurXML in uneListeDeCapteursXML)
            {
                string unNom = unCapteurXML.Element("id").Value;
                string uneDescription = unCapteurXML.Element("description").Value;
                string unLieu = unCapteurXML.Element("lieu").Value;
                XElement uneGrandeurXML = unCapteurXML.Element("grandeur");
                string unNomGrandeur = uneGrandeurXML.Attribute("nom").Value;
                string uneUniteGrandeur = uneGrandeurXML.Attribute("unite").Value;
                string uneAbreviationGrandeur = uneGrandeurXML.Attribute("abreviation").Value;
                if (Grandeurs.Count(grandeur => grandeur.NomGrandeur == unNomGrandeur)==0)
                    Grandeurs.Add(new Grandeur(unNomGrandeur, uneUniteGrandeur, uneAbreviationGrandeur));

                Capteur unCapteur = new Capteur(unNom, uneDescription, unLieu, Grandeurs.First(grandeur => 
                    grandeur.NomGrandeur == unNomGrandeur));
                Capteurs.Add(unCapteur);
            }
        }

        // Charge les fichiers source contenant les données des capteurs
        // Les données sont enregistrées dans le dictionnaire "Données"
        // N'est appelé qu'une seule fois au début
        // Tout est stocké en mémoire pour qu'on puisse filtrer après sans devoir faire de nouvel import de données
        // Si la quantité de données devenait trop importante, il serait possible d'appliquer des filtres 
        // dans cette méthode
        private void MiseAJourDataSource()
        {
            Données.Clear();
            foreach (string unNomFichier in Directory.EnumerateFiles(NomDossierSource, "*.dt"))
            {
                string[] desLignes = File.ReadAllLines(unNomFichier);
                foreach (string uneLigne in desLignes)
                {
                    string uneDateStr = uneLigne.Substring(1, 19);
                    string unCapteurStr = uneLigne.Substring(22, uneLigne.Substring(22).IndexOf(' '));
                    string uneValeurStr = uneLigne.Substring(uneLigne.IndexOf(' ', 22) + 1);

                    DateTime uneDate = Convert.ToDateTime(uneDateStr);
                    float uneValeur = Convert.ToSingle(uneValeurStr);
                    Données.Add(new Donnée(Capteurs.First(capteur => capteur.NomCapteur == unCapteurStr), 
                        uneDate, uneValeur));
                }
            }
        }

        // Met à jour le "Modèle" du Plotter
        // Prend en compte les filtres (dates et capteur) pour n'afficher qu'une seule courbe sur un laps de temps donné
        // Est appelé à chaque mise à jour d'un filtre
        public void FiltreDonnées(IEnumerable<Capteur> CapteursFiltrés, bool Insertion)
        {
            // Suppression courbe analyse
            supprimerCourbeAnalyse();
            AnalyseSélectionnée = null;

            foreach (Capteur CapteurFiltré in CapteursFiltrés)
            {
                if(Insertion)       // Insertion données
                {
                    if (CapteurFiltré != null)
                    {
                        LinearAxis uneOrdonnée = new LinearAxis
                        {
                            Title = CapteurFiltré.Grandeur.ToString(),
                            Key = CapteurFiltré.Grandeur.NomGrandeur
                        };

                        LineSeries desDonnées = new LineSeries
                        {
                            Title = CapteurFiltré.Description,
                            ItemsSource = Données.Where(donnée => donnée.Capteur == CapteurFiltré &&
                                                                  donnée.Temps >= DebutFiltre &&
                                                                  donnée.Temps <= FinFiltre),
                            Mapping = donnée => new DataPoint(DateTimeAxis.ToDouble(((Donnée) donnée).Temps),
                                ((Donnée) donnée).Valeur)
                        };

                        // Insertion des axes et des données
                        if (Modèle.Axes.Count(axe => axe.Title == CapteurFiltré.Grandeur.ToString()) == 0)
                        {
                            if (Modèle.Axes.Count(axe => axe.Position == AxisPosition.Left) > 0)
                            {
                                uneOrdonnée.Position = AxisPosition.Right;
                            }
                            Modèle.Axes.Add(uneOrdonnée);
                            desDonnées.YAxisKey = uneOrdonnée.Key;
                        }
                        else
                        {
                            desDonnées.YAxisKey =
                                Modèle.Axes.First(axe => axe.Title == CapteurFiltré.Grandeur.ToString()).Key;
                        }
                        Modèle.Series.Add(desDonnées);
                    }
                }
                else        // Suppression données
                {
                    foreach (var serie in Modèle.Series.Where(serie => serie.Title == CapteurFiltré.Description))
                    {
                        Modèle.Series.Remove(serie);
                        break;
                    }

                    if (Modèle.Series.All(serie => ((LineSeries) serie).YAxisKey != CapteurFiltré.Grandeur.NomGrandeur))
                    {
                        foreach (var axe in Modèle.Axes.Where(axe => axe.Key == CapteurFiltré.Grandeur.NomGrandeur))
                        {
                            Modèle.Axes.Remove(axe);
                            break;
                        }
                    }
                }
                Modèle.InvalidatePlot(true);
            }
        }


        // Met à jour le filtre des capteurs en fonction des grandeurs sélectionnées
        // Une limite à deux grandeurs a été imposée car il n'y a que deux axes Y possibles (gauche et droite)
        // La limite est imposée dans MainWindow.xaml.cs
        // Les capteurs sélectionnés sont automatiquement désélectionnés si besoin
        // Dans ce cas, la méthode ci-dessus FiltreDonnées est appelée automatiquement
        public IEnumerable FiltreGrandeurs(IEnumerable<Grandeur> GrandeursFiltrées)
        {
            return Capteurs.Where(capteur => GrandeursFiltrées.Contains(capteur.Grandeur));
        }

        // Méthode simple qui met à jour les sources des données affichées pour prendre en compte le changement de dates
        // InvalidatePlot sert à rafraichir l'affichage du graphe
        // Tout le reste est automatique (Le mapping a été setté dans la méthode FiltreDonnées)
        public void FiltreDates()
        {
            // Suppression courbe analyse
            supprimerCourbeAnalyse();
            AnalyseSélectionnée = null;

            foreach (var serie in Modèle.Series)
            {
                ((LineSeries) serie).ItemsSource = Données.Where(donnée => donnée.Capteur.Description == serie.Title &&
                                                                           donnée.Temps >= DebutFiltre &&
                                                                           donnée.Temps <= FinFiltre);
            }
            Modèle.InvalidatePlot(true);
        }

        // Si une analyse est sélectionnée, lance l'analyse
        // Celle-ci peut ajouter une courbe au modèle en utilisant les méthodes "SetCourbeAnalyse" et "SetAxeAnalyse"
        // Passe en argument de l'analyse toutes les données affichées sur le graphe auxquelles la grandeur est
        // analysable par cette analyse
        public void FiltreAnalyses()
        {
            // Suppression courbe analyse
            supprimerCourbeAnalyse();

            if (AnalyseSélectionnée == null) return;
            IEnumerable<Donnée> desDonnées = new List<Donnée>();

            // Verion longue
            /*
            foreach (var serie in Modèle.Series)
            {
                desDonnées = desDonnées.Concat(Données.Where(donnée =>
                    AnalyseSélectionnée.NomGrandeurAnalysée().Contains(donnée.Capteur.Grandeur.NomGrandeur) &&
                    donnée.Temps >= DebutFiltre &&
                    donnée.Temps <= FinFiltre &&
                    donnée.Capteur.Description == serie.Title))
            }
            */

            // Version courte
            desDonnées = Modèle.Series.Aggregate(desDonnées, (current, serie) => current.Concat(Données.Where(donnée => 
                AnalyseSélectionnée.NomGrandeurAnalysée().Contains(donnée.Capteur.Grandeur.NomGrandeur) && 
                donnée.Temps >= DebutFiltre && donnée.Temps <= FinFiltre && donnée.Capteur.Description == serie.Title)));

            AnalyseSélectionnée = AnalyseSélectionnée.Analyser(this, desDonnées);
        }

        // Ajoute une courbe d'analyse
        // Elle se réfère automatiquement à l'axe Y d'analyse de clé "Analyse"
        // Plusieurs courbes peuvent êter ajoutées du moment qu'elles suivent la même échelle
        public void SetCourbeAnalyse(LineSeries lineSeries)
        {
           
            lineSeries.YAxisKey = "Analyse";
            Modèle.Series.Add(lineSeries);
            Modèle.InvalidatePlot(true);
        }

        // Crée l'axe auquel se réfère les courbes d'analyse (ne doit être appelé qu'une seule fois par analyse)
        // Utilise le mot clé "Analyse" pour cet axe
        // L'axe est créé en parallèle des autres axes et ne devrait pas apporter de problème particulier 
        // au filtrage des données
        public void SetAxeAnalyse(string descriptionOrdonnée)
        {
            LinearAxis uneOrdonnée = new LinearAxis()
            {
                Title = descriptionOrdonnée,
                Key = "Analyse",
                PositionTier = 1            // Evite le chevauchement des échelles en ordonnée
            };
            Modèle.Axes.Add(uneOrdonnée);
        }

        // Méthode interne qui facilite la suppression des courbes d'analyse
        // Elle se base sur le fait qu'elles utilisent toutes le même axe de clé "Analyse"
        // Cet axe sera également supprimé après que les courbes ont été eeffacées
        private void supprimerCourbeAnalyse()
        {
            if (Modèle.Axes.Any(axe => axe.Key == "Analyse"))
            {
                for (int i = Modèle.Series.Count() - 1; i >= 0; i--)
                {
                    if(((LineSeries)Modèle.Series[i]).YAxisKey == "Analyse")
                        Modèle.Series.RemoveAt(i);
                }
                Modèle.Axes.Remove(Modèle.Axes.First(axe => axe.Key == "Analyse"));
                Modèle.InvalidatePlot(true);
            }
        }

        // Retourne les statistiques calculées lors de la précédente analyse
        // Si il n'y a aucune analyse en cours, on le fait savoir
        public string AnalyseStatistiques()
        {
            return AnalyseSélectionnée == null ? "Aucune analyse" : AnalyseSélectionnée.AnalyseString();
        }
    }
}
