using System.Collections.Generic;

namespace MonPlotterWPF
{
    internal class AnalyseSons : AnalyseGrandeur
    {
        public IEnumerable<string> NomGrandeurAnalysée()
        {
            yield return "Bruit";
        }

        public string AnalyseString(IEnumerable<Donnée> Données)
        {
            // TODO
            return "";
        }

        public AnalyseGrandeur Analyser(PlotterViewModel Model, IEnumerable<Donnée> Données)
        {
            // TODO
            // Sons
            return null;
        }

        public override string ToString()
        {
            return "Analyse Sons";
        }
    }
}