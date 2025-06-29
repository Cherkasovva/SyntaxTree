using System;
using System.Collections.Generic;

public class TreeNode
{
    public string Value;
    public TreeNode Left;
    public TreeNode Right;

    public TreeNode(string value)
    {
        Value = value;
        Left = null;
        Right = null;
    }
}

public class ExpressionTreeBuilder
{
    private static int GetPriority(string op)
    {
        if (op == "+" || op == "-") return 1;
        if (op == "*" || op == "/") return 2;
        return 0;
    }

    public static TreeNode BuildExpressionTree(string expression)
    {
        var opsStack = new Stack<string>();
        var operandsStack = new Stack<TreeNode>();
        //  стек операндов (узлов-чисел и промежуточных деревьев)
        int i = 0;
        while (i < expression.Length)
        {
            if (char.IsDigit(expression[i]))
            {
                // Для многозначных чисел:
                int start = i;
                while (i < expression.Length && char.IsDigit(expression[i])) i++;
                string num = expression.Substring(start, i - start);
                operandsStack.Push(new TreeNode(num));
                continue;
            }
            else if (expression[i] == '(')
            {
                opsStack.Push(expression[i].ToString());
            }
            else if ("+-*/".Contains(expression[i]))
            {
                string currentOp = expression[i].ToString();
                while (opsStack.Count > 0 && opsStack.Peek() != "(" &&
                       GetPriority(opsStack.Peek()) >= GetPriority(currentOp))
                {
                    string op = opsStack.Pop();
                    TreeNode right = operandsStack.Pop();
                    TreeNode left = operandsStack.Pop();
                    TreeNode node = new TreeNode(op);
                    node.Left = left;
                    node.Right = right;
                    operandsStack.Push(node);
                }
                opsStack.Push(currentOp);
            }
            else if (expression[i] == ')')
            {
                while (opsStack.Count > 0 && opsStack.Peek() != "(")
                {
                    string op = opsStack.Pop();
                    TreeNode right = operandsStack.Pop();
                    TreeNode left = operandsStack.Pop();
                    TreeNode node = new TreeNode(op);
                    node.Left = left;
                    node.Right = right;
                    operandsStack.Push(node);
                }
                if (opsStack.Count > 0 && opsStack.Peek() == "(")
                    opsStack.Pop(); // Удаляем '(' из стека операций
            }
            i++;
        }

        while (opsStack.Count > 0)
        {
            string op = opsStack.Pop();
            TreeNode right = operandsStack.Pop();
            TreeNode left = operandsStack.Pop();
            TreeNode node = new TreeNode(op);
            node.Left = left;
            node.Right = right;
            operandsStack.Push(node);
        }

        return operandsStack.Pop();
    }
}