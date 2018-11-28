namespace FindDuplicates
{
    public class SourceFile
    {
        public string FullPath { get; set; }

        public SourceLine[] SourceLines { get; set; }

        public ComparableStatement[] Statements { get; set; }
    }
}