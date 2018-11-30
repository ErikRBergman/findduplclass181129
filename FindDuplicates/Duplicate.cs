namespace FindDuplicates
{
    using System.Collections.Generic;

    public class Duplicate
    {
        public Dictionary<ComparableStatement, DuplicateInstance> Instances { get; set; }

    }
}