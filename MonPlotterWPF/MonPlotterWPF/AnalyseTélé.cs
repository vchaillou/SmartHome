using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace MonPlotterWPF
{
    internal class AnalyseTélé : AnalyseGrandeur
    {
        private string stats;

        public IEnumerable<string> NomGrandeurAnalysée()
        {
            yield return "Bruit";
        }

        public string AnalyseString(IEnumerable<Donnée> Données)
        {
            // TODO
            return stats;
        }

        public AnalyseGrandeur Analyser(PlotterViewModel Model, IEnumerable<Donnée> Données)
        {
            // TODO
            // stats
            const int sautAnalyse = 5;
            var desDonnées = Données.Where(donnée => donnée.Capteur.Lieu == "Salle") as Donnée[] ?? 
                             Données.Where(donnée => donnée.Capteur.Lieu == "Salle").ToArray();
            if (!desDonnées.Any()) return null;
            LineSeries lineSeries = new LineSeries
            {
                Title = "Télévision allumée"
            };

            // On analyse cinq valeurs d'un coup pour éviter les discordances
            // On vérifie si sautAnalyse-1 valeurs conviennent (pour éviter les pics)
            for (int i = 0; i+sautAnalyse < desDonnées.Count(); i++)
            {
                lineSeries.Points.Add(desDonnées.Count(donnée => donnée.Valeur > 55 && donnée.Valeur < 70 &&
                                      donnée.Temps >= desDonnées[i].Temps &&
                                      donnée.Temps < desDonnées[i + sautAnalyse].Temps) >= sautAnalyse - 1
                    ? new DataPoint(DateTimeAxis.ToDouble(desDonnées[i].Temps), 1)
                    : new DataPoint(DateTimeAxis.ToDouble(desDonnées[i].Temps), 0));
            }
            stats = "";
            Model.SetAxeAnalyse("Télé allumée (Oui/Non)");
            Model.SetCourbeAnalyse(lineSeries);
            return this;
        }

        public override string ToString()
        {
            return "Analyse Télévision";
        }
    }
}