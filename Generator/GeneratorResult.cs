namespace KDV.CeusDL.Generator {
    public class GeneratorResult {
        public string FileName {get; private set;}
        public string Content {get; private set;}

        public GeneratorResult(string fileName, string content) {
            FileName = fileName;
            Content = content;
        }
    }
}