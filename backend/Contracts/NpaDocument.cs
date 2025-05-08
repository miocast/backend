using Microsoft.Extensions.Primitives;

namespace backend.Contracts
{
    public class NpaDocument
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string FilePath { get; set; }
    }
}
