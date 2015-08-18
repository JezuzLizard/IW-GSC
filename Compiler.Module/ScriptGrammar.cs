using Irony.Parsing;
using static Compiler.Module.ScriptGrammarConst;

namespace Compiler.Module
{
    internal class ScriptGrammar : Grammar
    {
        public ScriptGrammar()
        {
            var blockComment = new CommentTerminal("block-comment", "/*", "*/");
            var lineComment = new CommentTerminal("line-comment", "//",
                "\r", "\n", "\u2085", "\u2028", "\u2029");
            NonGrammarTerminals.Add(blockComment);
            NonGrammarTerminals.Add(lineComment);

            var numberLiteral = new NumberLiteral(NumberId, NumberOptions.AllowSign);
            var stringLiteral = new StringLiteral(StringId, "\"");
            var identifier = new IdentifierTerminal(IdentifierId, @"_/\", "_");

            var comma = ToTerm(",");
            var leftPar = ToTerm("(");
            var rightPar = ToTerm(")");
            var leftBrace = ToTerm("{");
            var rightBrace = ToTerm("}");
            var semicolon = ToTerm(";");

            MarkPunctuation("(", ")", "{", "}", "[", "]", ",", ".", ";", "::", "[[", "]]");

            RegisterOperators(1, "+", "-");
            RegisterOperators(2, "*", "/", "%");
            RegisterOperators(3, "|", "&", "^");
            RegisterOperators(4, "&&", "||");
            RegisterBracePair("(", ")");

            var function = new NonTerminal(FunctionId);
            var parameters = new NonTerminal(ParametersId);
            var singleParameter = new NonTerminal(SingleParameterId);
            var parsParameters = new NonTerminal(ParsParametersId);
            var line = new NonTerminal(LineId);
            var lines = new NonTerminal(LinesId);
            var call = new NonTerminal(CallId);
            var simpleCall = new NonTerminal(SimpleCallId);
            var functionCall = new NonTerminal(FunctionCallId);
            var threadFunctionCall = new NonTerminal(FunctionThreadCallId);
            var methodCall = new NonTerminal(MethodCallId);
            var methodThreadCall = new NonTerminal(MethodThreadCallId);
            var block = new NonTerminal(BlockId);
            var rootStatement = new NonTerminal(RootStatementId);
            var rootStatements = new NonTerminal(RootStatementsId);
            var scriptStatement = new NonTerminal(ScriptStatementId);
            var whileStatement = new NonTerminal(WhileStatementId);

            singleParameter.Rule = identifier | stringLiteral | numberLiteral;
            parameters.Rule = MakeStarRule(parameters, comma, singleParameter) | singleParameter;
            functionCall.Rule = identifier + parameters | identifier + parsParameters;
            threadFunctionCall.Rule = Thread + functionCall;
            methodCall.Rule = singleParameter + functionCall;
            methodThreadCall.Rule = singleParameter + threadFunctionCall;
            call.Rule = functionCall | threadFunctionCall | methodCall | methodThreadCall;
            parsParameters.Rule = leftPar + rightPar | leftPar + parameters + rightPar;

            simpleCall.Rule = call + semicolon;


            scriptStatement.Rule = whileStatement;
            whileStatement.Rule = ToTerm("while") + parsParameters + block;

            line.Rule = simpleCall | scriptStatement;
            lines.Rule = MakePlusRule(lines, line);

            block.Rule = leftBrace + rightBrace | leftBrace + lines + rightBrace;
            function.Rule = identifier + parsParameters + block;

            rootStatement.Rule = function;
            rootStatements.Rule = MakePlusRule(rootStatements, rootStatement);

            Root = rootStatements;

            MarkTransient(rootStatement, singleParameter, line, parsParameters, block);
        }
    }
}