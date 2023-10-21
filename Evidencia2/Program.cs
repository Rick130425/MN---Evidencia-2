using Utils = Evidencia2.Utils;

// Se inicializan los datos de Anibal y María
var data = new[]
{
    new[]
    {
        // Valores de entrada de Anibal
        new double[]
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
        },
        // Valores de salida de Anibal
        new double[]
        {
            50, 55, 60, 61, 65, 67, 69, 70, 72, 73, 74, 76
        }
    },
    new[]
    {
        // Valores de entrada de María
        new double[]
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
        },
        // Valores de salida de Anibal
        new double[]
        {
            49, 57, 59, 61, 63, 65, 67, 69, 70, 71, 72, 74
        }
    }
};

var anibal = new[]
{
    // Funciones para los datos de Anibal
    new Func<double, double>[]
    {
        Math.Log,
        _ => 1
    },
    // Derivadas de las funciones
    new Func<double, double>[]
    {
        t => 1 / t,
        _ => 0
    }
};

var maria = new[]
{
    // Funciones para los datos de María
    new Func<double, double>[]
    {
        t => Math.Cos(t / 8),
        t => Math.Exp(t / 10)
    },
    // Derivadas de las funciones
    new Func<double, double>[]
    {
        t => -Math.Sin(t / 8) / 8,
        t => Math.Exp(t / 10) / 10
    }
};

