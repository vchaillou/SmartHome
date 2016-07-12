using System;

namespace MonPlotterWPF
{
    public class Donnée
    {
        public Capteur Capteur { get; }
        public DateTime Temps { get; }
        public float Valeur { get; }

        public Donnée(Capteur unCapteur, DateTime unTemps, float uneValeur)
        {
            Capteur = unCapteur;
            Temps = unTemps;
            Valeur = uneValeur;
        }
    }
}