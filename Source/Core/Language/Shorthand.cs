using System;

namespace Core;

public static class Shorthand
{
    public static Bool True = Bool.True;
    public static Bool False = Bool.False;
    public static Integer I(long val) => new(val);
    public static Str S(string val) => Str.Make(val);
    public static If If(Expression condition, Expression then, Expression @else) => new(condition, then, @else);
    public static Variable V(string key) => new(key);
    public static Lambda Lambda(Variable v, Expression body) => new(v.VarKey, body);

    public static Unary StrToInt(Expression e) => new('#', e);
    public static Unary IntToStr(Expression e) => new('$', e);
    public static Binary Concat(Expression e1, Expression e2) => new('.', e1, e2);
    public static Binary Take(Expression count, Expression str) => new('T', count, str);
    public static Binary Drop(Expression count, Expression str) => new('D', count, str);
    public static Binary Apply(Expression f, Expression x) => new('$', f, x);

    public static Func<Expression, Expression> RecursiveFunc(Variable funcName, params Variable[] args) => content =>
    {
        var func = content;
        for (int i = args.Length - 1; i >= 0; --i)
        {
            func = Lambda(args[i], func);
        }
        func = Lambda(funcName, func);
        var g = new Variable("!");
        return Apply(Lambda(g, Apply(g, g)), func);
    };

    public static Expression RecursiveCall(Variable funcName, params Expression[] args)
    {
        Expression result = Apply(funcName, funcName);
        for (int i = 0; i < args.Length; i++)
        {
            result = Apply(result, args[i]);
        }
        return result;
    }
}
