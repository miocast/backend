namespace backend.Contracts
{
    public class CompleteAnalys
    {
        public List<Analys> Analyses { get; set; }
        public NpaFromWorker Npa { get; set; }
        public string DocumentId { get; set; }

    }

    public class Analys
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Explanation { get; set; }
        public string Regulation { get; set; }
    }

    public class DocumentAnalysBusiness
    {
        public string DocumentId { get; set; }
        public List<Analys> Analyses { get; set; }
        public List<NpaBusiness> Npas { get; set; }
        public string Text { get; set; }
    }

    public class NpaBusiness
    {
        public string Source { get; set; }
        public float DistancePercent { get; set; }
    }

    public class NpaFromWorker
    {
        public List<string> Npas { get; set; }
        public List<string> Sources { get; set; }
        public List<float> Distances { get; set; }
    }
}
