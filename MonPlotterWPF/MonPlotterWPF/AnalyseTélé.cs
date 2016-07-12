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

        public string AnalyseString()
        {
            return stats;
        }

        public AnalyseGrandeur Analyser(PlotterViewModel Model, IEnumerable<Donnée> Données)
        {
            const int intervalleAnalyse = 5;
            
            TimeSpan tempsTéléAllumée = TimeSpan.Zero;
            int nbTéléAllumée = 0;

            var desDonnées = Données as Donnée[] ?? Données.ToArray();
            if (!desDonnées.Any()) return null;
            LineSeries lineSeries = new LineSeries
            {
                Title = "Télévision allumée"
            };

            // On analyse dix valeurs d'un coup pour éviter les discordances (cinq à gauche, cinq à droite)
            // On vérifie si intervalleAnalyse-1 valeurs conviennent (pour éviter les pics)
            for (int i = intervalleAnalyse; i+intervalleAnalyse < desDonnées.Count(); i++)
            {
                if (desDonnées.Count(donnée => donnée.Valeur > 55 && donnée.Valeur < 70 &&
                                     donnée.Temps > desDonnées[i - intervalleAnalyse].Temps &&
                                     donnée.Temps < desDonnées[i + intervalleAnalyse].Temps) >= intervalleAnalyse - 1)
                {
                    if (Math.Abs(lineSeries.Points[lineSeries.Points.Count-1].Y) < 0.5)
                    {
                        nbTéléAllumée++;
                        // TODO => heure moyenne de l'allumage ????
                    }
                    else
                    {
                        tempsTéléAllumée += desDonnées[i].Temps - desDonnées[i - 1].Temps;
                    }
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(desDonnées[i].Temps), 1));
                }
                else
                {
                    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(desDonnées[i].Temps), 0));
                }
            }
            stats = "La télé a été allumée " + nbTéléAllumée + " fois et pendant " + tempsTéléAllumée;
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