namespace KDV.CeusDL.Parser
{
    /*
     * Zum parsen von Attribut-Definitionen in interface und alter interface Statements
     */
    public abstract class AbstractParser<T> {
        public ParsableData Data {get; private set;}
        public AbstractParser(ParsableData data) {
            this.Data = data;
        }

        public abstract T Parse();
    }
}