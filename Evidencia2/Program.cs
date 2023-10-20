using Microsoft.VisualBasic.CompilerServices;
using Utils = Evidencia2.Utils;

var data = new[]
{
    new[]
    {
        new double[]{
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
        },
        new double[]{
            50, 55, 60, 61, 65, 67, 69, 70, 72, 73, 74, 76
        }
    },
    new[]
    {
        new double[]{
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
        },
        new double[]{
            49, 57, 59, 61, 63, 65, 67, 69, 70, 71, 72, 74
        }
    }
};

var anibal = new[]
{
    new Func<double, double>[]{
        Math.Log,
        _ => 1
    },
    new Func<double, double>[]{
        t => 1 / t,
        _ => 0
    }
};

var maria = new[]
{
    new Func<double, double>[]{
        t => Math.Cos(t / 8),
        t => Math.Exp(t / 10)
    },
    new Func<double, double>[]{
        t => -Math.Sin(t / 8) / 8,
        t => Math.Exp(t / 10) / 10
    }
};

var textAnibal = Utils.ToString(Utils.LinearFit(data[0][0], data[0][1], anibal[0]));
Console.WriteLine(textAnibal);
var textMaria = Utils.ToString(Utils.LinearFit(data[1][0], data[1][1], maria[0]));
Console.WriteLine(textMaria);