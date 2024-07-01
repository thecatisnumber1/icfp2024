using Core;
using LambdaMan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Shorthand;

namespace Lambdaman;

public class HandwrittenSolvers
{
    public static Expression? Lambdaman1(LambdaManGrid problem)
    {
        return S("LLLDURRRUDRRURR");
    }

    public static Expression? Lambdaman2(LambdaManGrid problem)
    {
        return S("RDURRDDRRUUDDLLLDLLDDRRRUR");
    }

    public static Expression? Lambdaman3(LambdaManGrid problem)
    {
        return S("DLRRDRLLLLLUURLURRLULUURRRRRDLLLRDRRDLRD");
    }

    public static Expression? Lambdaman6(LambdaManGrid problem)
    {
        var func = V("f");
        var n = V("n");

        var recFunc = RecursiveFunc(func, n)(
            If(
                (n == I(0)),
                problem.SolvePrefix(),
                Concat(
                    RecursiveCall(func, n - I(1)),
                    S("R")
                )
            )
        );

        return Apply(recFunc, I(200));
    }

    public static Expression? Lambdaman8(LambdaManGrid problem)
    {
        // The problem is a rectangular spiral. Just go 98 in a direction,
        // turn right, go 98, etc. until done.
        var func = V("f");
        var vi = V("i");

        var recFunc = RecursiveFunc(func, vi)(
            If((vi == I(0)),
                problem.SolvePrefix(),
                Concat(
                    RecursiveCall(func, vi - I(1)),
                    Take(I(1), Drop((vi / I(98)) % I(4), S("DLUR")))
                )
            )
        );

        return Apply(recFunc, I(9603));
    }
}
