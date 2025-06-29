using System;
using System.Collections.Generic;

public class ParseTreeNode
{
    public string Rule; 
    // Название правила (Expr, Term, Factor, Operator, Number, ...)
    public string Token; 
    // Токен для терминальных узлов (число, оператор, скобка)
    public List<ParseTreeNode> Children = new List<ParseTreeNode>();

    public ParseTreeNode(string rule, string token = null)
    {
        Rule = rule;
        Token = token;
    }

    public void Print(string indent = "")
    {
        Console.WriteLine($"{indent}{Rule}{(Token != null ? ": " + Token : "")}");
        foreach (var child in Children)
            child.Print(indent + "  ");
    }
}

public class Parser
{
    private string _expr; // строка выражения без пробелов.
    private int _pos; // текущая позиция в строке выражения

    public Parser(string expr)
    {
        _expr = expr.Replace(" ", ""); // удаляем пробелы
        _pos = 0; 
    }

    public ParseTreeNode Parse()
    {
        var node = ParseExpr();
        if (_pos != _expr.Length)
            throw new Exception("Unexpected text after parsing");
        return node;
    }

    // Expr → Term { ('+'|'-') Term }
    private ParseTreeNode ParseExpr()
    {
        var node = new ParseTreeNode("Expr"); // узел для выражения
        node.Children.Add(ParseTerm()); 
        while (Match('+') || Match('-')) 
        {
            var op = new ParseTreeNode("Operator", _expr[_pos - 1].ToString());
            // создаем узел оператора
            node.Children.Add(op);
            node.Children.Add(ParseTerm());
        }
        return node; // возвращаем построенное поддерево для выражения
    }

    // Term → Factor { ('*'|'/') Factor }
    private ParseTreeNode ParseTerm()
    {
        var node = new ParseTreeNode("Term");
        node.Children.Add(ParseFactor());
        while (Match('*') || Match('/'))
        {
            var op = new ParseTreeNode("Operator", _expr[_pos - 1].ToString());
            node.Children.Add(op);
            node.Children.Add(ParseFactor());
        }
        return node;
    }

    // Factor → '(' Expr ')' | number
    private ParseTreeNode ParseFactor()
    {
        if (Match('('))
        {
            var node = new ParseTreeNode("Factor");
            node.Children.Add(new ParseTreeNode("LParen", "("));
            node.Children.Add(ParseExpr());
            if (!Match(')')) throw new Exception("Expected ')'");
            node.Children.Add(new ParseTreeNode("RParen", ")"));
            return node;
        }
        else
        {
            return ParseNumber();
        }
    }

    /// <summary>
    /// Парсинг числа.
    /// </summary>
    private ParseTreeNode ParseNumber()
    {
        int start = _pos; // запоминаем текущую позицию
        while (_pos < _expr.Length && char.IsDigit(_expr[_pos]))
            _pos++;
        if (start == _pos) // если не найдено ни одной цифры
            throw new Exception("Expected number");
        var value = _expr.Substring(start, _pos - start);
        // Получаем строку из исходного выражения,
        // которая содержит найденное число (от start до текущей позиции _pos)
        return new ParseTreeNode("Number", value);
    }

    // Проверить и сдвинуть позицию, если текущий символ совпадает
    private bool Match(char c)
    {
        if (_pos < _expr.Length && _expr[_pos] == c)
        // если текущий символ совпадает с c, то сдвигаем позицию
        {
            _pos++;
            return true;
        }
        return false;
    }
}

// Пример использования:
// var parser = new Parser("2*(3+4)");
// var tree = parser.Parse();
// tree.Print();