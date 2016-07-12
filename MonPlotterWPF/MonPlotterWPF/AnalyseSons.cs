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
    internal class AnalyseSons : AnalyseGrandeur
    {
        private string stats;

        public IEnumerable<string> NomGrandeurAnalysée()
        {
            yield return "Bruit";
        }

        public string AnalyseString()
        {
            // TODO
            return "";
        }

        public AnalyseGrandeur Analyser(PlotterViewModel Model, IEnumerable<Donnée> Données)
        {
            // TODO
            // stats
            const int sautAnalyse = 1;

            var desDonnées = Données.Where(donnée => donnée.Capteur.Lieu == "Salle") as Donnée[] ??
                             Données.Where(donnée => donnée.Capteur.Lieu == "Salle").ToArray();
            if (!desDonnées.Any()) return null;
            LineSeries lineSeries = new LineSeries
            {
                Title = "Bruit extrême"
            };

            //Valeur minimum pour être considéré comme un bruit "fort"
            int décibel_extrême = 60;
            //Difference en décibel
            int diff_bruit = 15;
            //Valeur temporaire de décibel (celle de la mesure d'avant)
            int temp_noise = 0;

            // On analyse cinq valeurs d'un coup pour éviter les discordances
            for (int i = 0; i + sautAnalyse < desDonnées.Count(); i++)
            {
                if (i == 0)
                {
                    temp_noise = Convert.ToInt32(desDonnées[i].Valeur);
                }
                else { 
                lineSeries.Points.Add(desDonnées.Count(donnée => donnée.Valeur > décibel_extrême && donnée.Valeur-diff_bruit >= temp_noise &&
                                      donnée.Temps >= desDonnées[i].Temps &&
                                      donnée.Temps < desDonnées[i + sautAnalyse].Temps) >= sautAnalyse
                    ? new DataPoint(DateTimeAxis.ToDouble(desDonnées[i].Temps), 1)
                    : new DataPoint(DateTimeAxis.ToDouble(desDonnées[i].Temps), 0));
                }

            }
            stats = "";
            Model.SetAxeAnalyse("Bruit extrême (Oui/Non)");
            Model.SetCourbeAnalyse(lineSeries);
            return this;
        }

        public override string ToString()
        {
            return "Analyse Sons";
        }
    }
}