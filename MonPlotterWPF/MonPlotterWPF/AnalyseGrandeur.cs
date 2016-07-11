using System.Security.Cryptography.X509Certificates;

namespace MonPlotterWPF
{
    public interface AnalyseGrandeur<Grandeur>
    {
        string getAnalyseString();

        void Analyser(Analysable Model);
    }
}