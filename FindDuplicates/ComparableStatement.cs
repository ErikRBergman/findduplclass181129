namespace FindDuplicates
{
    public class ComparableStatement
    {
        public SourceLine SourceLine { get; set; }

        public int Position { get; set; }

        public int ScopeLevel { get; set; }

        public string StatementText { get; set; }

        public ComparableStatement ParentStatement { get; set; }

        public override string ToString()
        {
            return this.StatementText;
        }
    }
}