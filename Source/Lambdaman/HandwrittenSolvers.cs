
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
        var cat = V("d");
        var c = V("c");
        var cc = V("cc");

        var outer = Lambda(cat, Apply(cat, Apply(cat, Apply(cat, S("RRRR")))));
        var func = Lambda(c, Concat(Concat(c,c), Concat(c,c)));
        var slv = Concat(S("solve lambdaman6 "), Apply(outer, func));

        return slv;
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
        // The problem is an open rectangle scan lines, alternating going right then left.
        var f = V("f");
        var r = V("r");
        var s = V("s");
        var c = V("c");

        var func = Lambda(f, Apply(f, Concat(
            Apply(f, S("RR")),
            Apply(f, S("LL")))));

        var r49 = Lambda(r, Lambda(s, Concat(Apply(r, Apply(r, Apply(r, s))), S("D"))));

        var cc = Lambda(c, Concat(c, Concat(c, c)));

        return Concat(S("solve lambdaman9 "), Apply(func, Apply(r49, cc)));

    }

    public static Expression? Lambdaman16(LambdaManGrid problem)
    {

        var f = V("f");
        var l = V("l");
        var d = V("d");
        var UP = I(0);
        var LEFT = I(1);
        var DOWN = I(2);
        var RIGHT = I(3);


        /* 
        U=>0
        L=>1
        D=>2
        R=>3
        */
        var func = RecursiveFunc(f, l, d)(
            If((l == I(1)),
                Take(I(6), Drop(d * I(6), S("DDRRUU RRDDLL UULLDD LLUURR".Replace(" ", "")))),
 Concat(
  Concat(
    Concat(
      RecursiveCall(f, l - I(1), StrToInt(Take(I(1),  Drop(d, S("badc"))))), // LURD
      Take(I(2), Drop(I(2) * d, S("DDRRUULL")))), // DDRRUULL
    Concat(
      RecursiveCall(f, l - I(1), d), //ULDR
      Take(I(2), Drop(I(2) * d, S("RRDDLLUU")))) // RRDDLLUU
  ),
  Concat(
    Concat(
      RecursiveCall(f, l - I(1), d), //ULDR
      Take(I(2), Drop(I(2) * d, S("UULLDDRR")))), // UULLDDRR
    RecursiveCall(f, l - I(1), StrToInt(Take(I(1), Drop(d, S("dcba"))))) //RDLU
  )
)
                
                ));
        /*
        If((d == UP),
        Concat(
          Concat(
            Concat(
              RecursiveCall(f, l - I(1), LEFT),
              S("DD")),
            Concat(
              RecursiveCall(f, l - I(1), UP),
              S("RR"))
          ),
          Concat(
            Concat(
              RecursiveCall(f, l - I(1), UP),
              S("UU")),
            RecursiveCall(f, l - I(1), RIGHT)
          )
        ),
        If((d == LEFT),
        Concat(
          Concat(
            Concat(
              RecursiveCall(f, l - I(1), UP),
              S("RR")),
            Concat(
              RecursiveCall(f, l - I(1), LEFT),
              S("DD"))
          ),
          Concat(
            Concat(
              RecursiveCall(f, l - I(1), LEFT),
              S("LL")),
            RecursiveCall(f, l - I(1), DOWN)
          )
        ),
        If((d == DOWN),
        Concat(
          Concat(
            Concat(
              RecursiveCall(f, l - I(1), RIGHT),
              S("UU")),
            Concat(
              RecursiveCall(f, l - I(1), DOWN),
              S("LL"))
            ),
          Concat(
            Concat(
              RecursiveCall(f, l - I(1), DOWN),
              S("DD")),
            RecursiveCall(f, l - I(1), LEFT)
          )
        ),

                        // RIGHT
        Concat(
          Concat(
            Concat(
              RecursiveCall(f, l - I(1), DOWN),
              S("LL")),
            Concat(
              RecursiveCall(f, l - I(1), RIGHT),
              S("UU"))
          ),
          Concat(
            Concat(
              RecursiveCall(f, l - I(1), RIGHT),
              S("RR")),
            RecursiveCall(f, l - I(1), UP)
          )
        )
        */







        return Concat(S("solve lambdaman16 "), Apply(Apply(func, I(6)), UP));
    }

}

