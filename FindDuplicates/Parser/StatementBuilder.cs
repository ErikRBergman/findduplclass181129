using System;
using System.Collections.Generic;
using System.Text;

namespace FindDuplicates.Parser
{
    using System.Runtime.CompilerServices;

    public class StatementBuilder
    {
        private readonly StringBuilder text;

        private readonly List<ComparableStatement> statements;

        private int scopeLevel;

        private class Scope
        {
            public Scope(int scopeLevel, ComparableStatement parentStatement)
            {
                this.ScopeLevel = scopeLevel;
                this.ParentStatement = parentStatement;
            }

            public int ScopeLevel { get; }

            public ComparableStatement ParentStatement { get; }
        }

        private Stack<Scope> scopes = new Stack<Scope>(50);

        public StatementBuilder(StringBuilder stringBuilder, List<ComparableStatement> statements)
        {
            this.text = stringBuilder;
            this.statements = statements;
        }

        public ComparableStatement LastCommittedStatement { get; private set; }

        private bool OnNewStatementAlwaysReturnsTrue(ComparableStatement _, int scopeLevel, List<ComparableStatement> _2)
        {
            return true;
        }

        public int Position { get; private set; } = -1;

        [IndexerName("Chars")]
        public char this[int index] => this.text[index];

        public void CommitStatement(string statement)
        {
            this.CommitStatement();

            if (statement.Length >= 0)
            {
                this.text.Append(statement);
                this.CommitStatement();
            }
        }

        public void RemoveLastCharacter()
        {
            if (this.text.Length > 0)
            {
                this.text.Length = this.text.Length - 1;
            }
        }

        public void CommitStatement()
        {
            if (!this.IsEmpty)
            {
                var statement = new ComparableStatement
                {
                    Position = this.Position,
                    SourceLine = this.SourceLine,
                    StatementText = this.text.ToString(),
                    ParentStatement = this.LastCommittedStatement
                };

                this.statements.Add(statement);

                this.Reset(false);
            }
        }

        public bool StartsWith(char ch)
        {
            if (this.text.Length == 0)
            {
                return false;
            }

            return this.text[0] == ch;
        }

        public bool StartsWith(string startText)
        {
            return string.Equals(this.text.ToString(0, startText.Length), startText);
        }

        public bool Is(char ch)
        {
            if (this.text.Length != 1)
            {
                return false;
            }

            return this.text[0] == ch;
        }

        public void AddCharacter(char ch, int position, SourceLine sourceLine)
        {
            if (this.text.Length == 0)
            {
                this.Position = position;
                this.SourceLine = sourceLine;
            }

            this.text.Append(ch);
        }

        public void EnterScope()
        {
            this.scopeLevel++;

            var currentStatement = this.CurrentParentStatement ?? this.LastCommittedStatement;

            this.scopes.Push(new Scope(this.scopeLevel, currentStatement));
        }

        public ComparableStatement CurrentParentStatement
        {
            get
            {
                if (this.scopes.Count == 0)
                {
                    return null;
                }

                return this.scopes.Peek().ParentStatement;
            }
        }

        public void ExitScope()
        {
            var scope = this.scopes.Pop();

            this.scopeLevel--;
        }

        public bool IsEmpty => this.text.Length == 0;

        public void Reset(bool full)
        {
            this.SourceLine = null;
            this.Position = -1;
            this.text.Clear();

            if (full)
            {
                this.scopeLevel = 0;
            }
        }

        public SourceLine SourceLine { get; set; }
    }
}