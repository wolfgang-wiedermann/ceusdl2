namespace KDV.CeusDL.Parser
{
    public class ParsableData {
        public string Content {get;set;}
        public int Position {get;set;}

        public ParsableData(string content) {
            this.Content = content;
            this.Position = 0;
        }

        public char Next() {
            lock(this) {
                return Content[Position++];
            }
        }

        public bool HasNext() {
            lock(this) {
                return Position < Content.Length-1;
            }
        }
    }
}