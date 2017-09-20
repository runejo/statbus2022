namespace nscreg.Utilities.Configuration.StatUnitAnalysis
{
    /// <summary>
    /// Класс проверки правил анализа стат. едениц
    /// </summary>
    public class StatUnitAnalysisRules
    {
        public MandatoryFields MandatoryFields { get; set; }
        public Connections Connections { get; set; }
        public Orphan Orphan { get; set; }
        public Duplicates Duplicates { get; set; }
    }
}