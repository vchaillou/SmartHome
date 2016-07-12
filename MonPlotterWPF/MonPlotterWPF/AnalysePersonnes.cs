using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MonPlotterWPF
{
    internal class AnalysePersonnes : AnalyseGrandeur
    {
        public IEnumerable<string> NomGrandeurAnalysée()
        {
            yield return "Bruit";
            yield return "Co2";
        }

        public string AnalyseString()
        {
            return "Aucune statistique pour cette analyse";
        }

        public AnalyseGrandeur Analyser(PlotterViewModel Model, IEnumerable<Donnée> Données)
        {
            const int intervalleAnalyse = 5;
            if (!Données.Any()) return null;

            Model.SetAxeAnalyse("Nombre de personnes (0/1/2/2+)");

            // Pour chaque salle
            var desDonnées = Données.GroupBy(donnée => donnée.Capteur.Lieu, donnée => donnée);
            foreach (var groupes in desDonnées)
            {
                int seuilParIntervalleCO2;
                switch (groupes.Key)
                {
                    // Valeurs à confirmer, elles ne prennent pas en compte les différences de volume entre les salles
                    case "Salle":
                        seuilParIntervalleCO2 = 60;
                        break;
                    case "Chambre Alain":
                        seuilParIntervalleCO2 = 80;
                        break;
                    case "Chambre Beatrice":
                        seuilParIntervalleCO2 = 80;
                        break;
                    case "Cuisine":
                        seuilParIntervalleCO2 = 80;
                        break;
                    case "Petite Maison":
                        seuilParIntervalleCO2 = 80;
                        break;
                    default:
                        seuilParIntervalleCO2 = 80;
                        break;
                }

                LineSeries lineSeries = new LineSeries
                {
                    Title = "Personnes dans la salle \"" + groupes.Key + "\""
                };
                
                int nbPersonnes = 0;
                var autresDonnées = groupes.GroupBy(donnée => donnée.Temps, donnée => donnée).ToArray();
                for (int i = intervalleAnalyse; i + intervalleAnalyse < autresDonnées.Count(); i+=intervalleAnalyse)
                {
                    int nbPersonnesCo2 = -1;
                    int nbPersonnesBruit = -1;
                    DateTime dateCourante = autresDonnées[i].Key;
                    DateTime datePrecedente = autresDonnées[i - intervalleAnalyse].Key;
                    DateTime dateSuivante = autresDonnées[i + intervalleAnalyse].Key;
                    // Différence de Co2 sur un intervalle
                    if (groupes.Any(donnée => donnée.Capteur.Grandeur.NomGrandeur == "Co2" && donnée.Temps == dateCourante))
                    {
                        nbPersonnesCo2 = Convert.ToInt32((
                            groupes.First(donnée => donnée.Capteur.Grandeur.NomGrandeur == "Co2" &&
                                            donnée.Temps == dateCourante).Valeur -
                            groupes.First(donnée => donnée.Capteur.Grandeur.NomGrandeur == "Co2" &&
                                            donnée.Temps == datePrecedente).Valeur)
                            / (1.0 * seuilParIntervalleCO2));

                        if (nbPersonnesCo2 < 0) nbPersonnesCo2 = 0;
                    }

                    // Moyenne de bruit sur deux intervalles
                    if (groupes.Any(donnée => donnée.Capteur.Grandeur.NomGrandeur == "Bruit" && donnée.Temps == dateCourante))
                    {
                        nbPersonnesBruit = getNbPersonnesBruit(
                            groupes.Where(donnée => donnée.Capteur.Grandeur.NomGrandeur == "Bruit" &&
                                            donnée.Temps >= datePrecedente &&
                                            donnée.Temps < dateSuivante)
                                   .Sum(donnée => donnée.Valeur) / (2 * intervalleAnalyse));
                    }

                    // On détermine le réel nombre de personnes suite aux autres données
                    if (nbPersonnesBruit > -1 && nbPersonnesCo2 > -1)
                    {
                        if (nbPersonnesBruit == nbPersonnesCo2)
                        {
                            nbPersonnes = nbPersonnesCo2;
                        }
                        else
                        {
                            nbPersonnes = (nbPersonnesBruit + nbPersonnesCo2)/2;
                            double temp = (nbPersonnesBruit + nbPersonnesCo2)/2.0;
                            if (temp - nbPersonnes > 0.49) // la moyenne des deux nombres finit par 0.5
                            {
                                // On arrondit vers le nombre du Co2 qui est plus fiable
                                nbPersonnes += nbPersonnes < nbPersonnesCo2 ? 1 : 0;
                            }
                        }
                    }
                    else
                    {
                        nbPersonnes = nbPersonnesCo2 > -1 ? nbPersonnesCo2 : nbPersonnesBruit;
                    }

                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(dateCourante), nbPersonnes));
                }

                Model.SetCourbeAnalyse(lineSeries);
            }
            return this;
        }

        public override string ToString()
        {
            return "Analyse Personnes";
        }

        // Convertisseur de décibels en nombre de personne
        // Part du principe qu'une augmentation de 3 decibels équivaut à une multiplication par 2 du nombre de personnes
        private int getNbPersonnesBruit(double decibels)
        {
            const int nbDecibelsPourUnePersonne = 65;
            const int seuilDecibelsDoublePersonnes = 3;

            int nbPersonnes = 1;
            if (decibels < nbDecibelsPourUnePersonne)
            {
                nbPersonnes = 0;
            }

            while (decibels - nbDecibelsPourUnePersonne > seuilDecibelsDoublePersonnes)
            {
                decibels -= seuilDecibelsDoublePersonnes;
                nbPersonnes *= 2;
            }
            return nbPersonnes;
        }
    }
}
