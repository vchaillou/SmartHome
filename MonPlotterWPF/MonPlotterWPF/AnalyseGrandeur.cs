using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace MonPlotterWPF
{
    public interface AnalyseGrandeur
    {
        IEnumerable<string> NomGrandeurAnalysée();

        string AnalyseString();

        AnalyseGrandeur Analyser(PlotterViewModel Model, IEnumerable<Donnée> Données);
    }
}