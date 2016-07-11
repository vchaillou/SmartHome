// Valentin CHAILLOU
// 4A-AL2 ESGI

namespace MonPlotterWPF
{
    public class Capteur
    {
        public string NomCapteur { get; }
        public string Description { get; }
        public string Lieu { get; }
        public Grandeur Grandeur { get; }

        public Capteur(string unNomCapteur, string uneDescription, string unLieu, Grandeur uneGrandeur)
        {
            NomCapteur = unNomCapteur;
            Description = uneDescription;
            Lieu = unLieu;
            Grandeur = uneGrandeur;
        }

        public override string ToString()
        {
            return Description + " (" + Grandeur.AbreviationGrandeur + ")";
        }
    }
}