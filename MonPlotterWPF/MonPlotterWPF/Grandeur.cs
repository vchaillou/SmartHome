// Valentin CHAILLOU
// 4A-AL2 ESGI

namespace MonPlotterWPF
{
    public class Grandeur
    {
        public string AbreviationGrandeur { get; }
        private readonly string UniteGrandeur;
        public string NomGrandeur { get; }
        public AnalyseGrandeur<Grandeur> Analyse { get; set; }

        public Grandeur(string unNomGrandeur, string uneUniteGrandeur, string uneAbreviationGrandeur)
        {
            NomGrandeur = unNomGrandeur;
            UniteGrandeur = uneUniteGrandeur;
            AbreviationGrandeur = uneAbreviationGrandeur;
        }

        public override string ToString()
        {
            return NomGrandeur + " en " + UniteGrandeur + " (" + AbreviationGrandeur + ")";
        }
    }
}