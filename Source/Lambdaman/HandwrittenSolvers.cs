﻿using Core;
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
        // Go right 
        var f = V("f");
        var c = V("c");

        var func = Lambda(f, Apply(f, Apply(f, Apply(f, S("RRRRRRRR")))));
        var cc = Lambda(c, Concat(c, Concat(c, c)));

        return Concat(problem.SolvePrefix(), Apply(func, cc)); ;
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
    // B. S3/,6%},!-"$!-!.^} B$ B$ L! B$ v! v! Lf Li ? B= vi I! SL B. B$ B$ vf vf B- vi I" ? B= IR B% vi IS S> BT I" BD B% B/ vi IS I# SLF I;W

    public static Expression? Lambdaman9(LambdaManGrid problem)
    {
        var f = V("f");
        var r = V("r");
        var s = V("s");
        var c = V("c");

        var func = Lambda(f, Apply(f, Concat(
            Concat(Apply(f, S("RR")), S("D")),
            Concat(Apply(f, S("LL")), S("D")))));
        var r49 = Lambda(r, Lambda(s, Apply(r, Apply(r, Apply(r, s)))));
        var cc = Lambda(c, Concat(c, Concat(c, c)));

        return Concat(problem.SolvePrefix(), Apply(func, Apply(r49, cc)));
    }
}
