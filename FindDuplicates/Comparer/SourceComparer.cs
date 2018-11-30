namespace FindDuplicates.Comparer
{
    using System.Collections.Generic;
    using System.Linq;

    public class SourceComparer
    {
        public (IEnumerable<Duplicate> duplicates, int uniqueDuplicates, int duplicateInstances) FindDuplicates(IEnumerable<SourceFile> sourceFiles)
        {
            var sourceFileArray = sourceFiles.ToArray();

            var uniqueDuplicateStatements = 0;
            var duplicateStatementInstances = 0;

            var duplicates = new Dictionary<string, Duplicate>();

            for (var i = 0; i < sourceFileArray.Length; i++)
            {
                var sourceFile = sourceFileArray[i];

                for (var j = i + 1; j < sourceFileArray.Length; j++)
                {
                    var compareFile = sourceFileArray[j];

                    foreach (var sourceStatement in sourceFile.Statements)
                    {
                        foreach (var compareStatement in compareFile.Statements)
                        {
                            if (string.Equals(sourceStatement.StatementText, compareStatement.StatementText))
                            {
                                if (!duplicates.TryGetValue(sourceStatement.StatementText, out var duplicate))
                                {
                                    duplicate = new Duplicate
                                    {
                                        Instances = new Dictionary<ComparableStatement, DuplicateInstance>
                                        {
                                            [sourceStatement] = new DuplicateInstance
                                            {
                                                SourceFile = sourceFile,
                                                Statements = new[] { sourceStatement }
                                            }
                                        }
                                    };

                                    uniqueDuplicateStatements++;

                                    duplicates[sourceStatement.StatementText] = duplicate;
                                }
                                else if (!duplicate.Instances.ContainsKey(sourceStatement))
                                {
                                    duplicate.Instances.Add(
                                        sourceStatement,
                                        new DuplicateInstance
                                        {
                                            SourceFile = sourceFile,
                                            Statements = new[] { sourceStatement }
                                        });

                                    duplicateStatementInstances++;
                                }

                                if (!duplicate.Instances.ContainsKey(compareStatement))
                                {
                                    duplicate.Instances.Add(
                                        compareStatement,
                                        new DuplicateInstance
                                            {
                                                SourceFile = sourceFile,
                                                Statements = new[] { compareStatement }
                                            });

                                    duplicateStatementInstances++;
                                }
                            }
                        }
                    }
                }
            }

            return (duplicates.Values, uniqueDuplicateStatements, duplicateStatementInstances);
        }
    }
}