using System.Collections.Generic;

namespace MonPlotterWPF
{
    internal class AnalysePersonnes : AnalyseGrandeur
    {
        public IEnumerable<string> NomGrandeurAnalysée()
        {
            yield return "Bruit";
            yield return "Co2";
        }

        public string AnalyseString(IEnumerable<Donnée> Données)
        {
            // TODO
            return "";
        }

        public AnalyseGrandeur Analyser(PlotterViewModel Model, IEnumerable<Donnée> Données)
        {
            // TODO
            // Nb personnes par salle
            return null;
        }

        public override string ToString()
        {
            return "Analyse Personnes";
        }
    }
}