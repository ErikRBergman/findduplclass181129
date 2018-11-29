namespace FindDuplicates.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using System.Linq;
    using System.Text;

    public class CParser
    {
        public SourceFile Parse(RawSourceFile rawSourceFile)
        {
            var lines = GetSourceLines(rawSourceFile);
            var statements = GetStatementsFromSourceLines(lines);

            return new SourceFile
            {
                FullPath = rawSourceFile.FullPath,
                SourceLines = lines.ToArray(),
                Statements = statements.ToArray()
            };
        }

        private static IEnumerable<ComparableStatement> GetStatementsFromSourceLines(IEnumerable<SourceLine> lines)
        {
            var statements = new List<ComparableStatement>();

            bool inBlockComment = false;
            bool inString = false;

            var builder = new StatementBuilder(new StringBuilder(500), statements);

            var nextLastChar = (char)0;

            foreach (var line in lines)
            {
                try
                {
                    var text = line.Text.Trim();

                    if (text.Length == 0)
                    {
                        continue;
                    }

                    for (var index = 0; index < text.Length; index++)
                    {
                        var ch = text[index];
                        var lastChar = nextLastChar;
                        nextLastChar = ch;

                        // Ignore block comment contents
                        if (inBlockComment)
                        {
                            if (ch == '/' && lastChar == '*')
                            {
                                inBlockComment = false;
                            }

                            continue;
                        }

                        // Strings may not be altered
                        if (inString)
                        {
                            builder.AddCharacter(ch, index, line);

                            if (ch == '\"' && lastChar != '\\')
                            {
                                inString = false;
                            }

                            continue;
                        }

                        // Check if this starts a string
                        if (ch == '\"')
                        {
                            inString = true;
                            builder.AddCharacter(ch, index, line);
                            continue;
                        }

                        if (ch == '*' && lastChar == '/')
                        {
                            inBlockComment = true;

                            // remove the slash (/)
                            builder.RemoveLastCharacter();
                            continue;
                        }

                        if (ch == '\t')
                        {
                            ch = ' ';
                            nextLastChar = ch;
                        }

                        // ignore multiple tabs / spaces
                        if (ch == ' ' && lastChar == ' ')
                        {
                            continue;
                        }

                        // Begin scope always produce a new statement
                        if (ch == '{')
                        {
                            builder.CommitStatement("{");
                            // builder.EnterScope();
                            continue;
                        }

                        // End scope always produce a new statement
                        if (ch == '}')
                        {
                            builder.CommitStatement("}");
                            // builder.ExitScope();
                            continue;
                        }

                        // semi colon terminates a statement
                        if (ch == ';')
                        {
                            builder.AddCharacter(ch, index, line);
                            builder.CommitStatement();
                            continue;
                        }

                        builder.AddCharacter(ch, index, line);
                    }

                    //// This is the end of the line, pal
                    if (builder.IsEmpty == false)
                    {
                        var lineStart = builder[0];

                        // compiler directive line, end of line is end of statement
                        if (lineStart == '#')
                        {
                            builder.CommitStatement();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error on line {line.LineNumber}: " + e);
                    throw;
                }

                // End of file
                builder.CommitStatement();
                builder.Reset(true);
            }

            return statements;
        }

        private static IEnumerable<SourceLine> GetSourceLines(RawSourceFile rawSourceFile)
        {
            var lines = rawSourceFile.Content.Split("\r\n")
                .Select(
                    (text, lineNumber) => new SourceLine
                    {
                        LineNumber = lineNumber,
                        Text = text
                    });
            return lines;
        }
    }
}