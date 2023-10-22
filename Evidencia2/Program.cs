using Utils = Evidencia2.Utils;

// Se inicializan los datos de Aníbal y María
var data = new[]
{
    new[]
    {
        // Valores de entrada de Aníbal
        new double[]
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
        },
        // Valores de salida de Aníbal
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
        // Valores de salida de María
        new double[]
        {
            49, 57, 59, 61, 63, 65, 67, 69, 70, 71, 72, 74
        }
    }
};

var anibal = new[]
{
    // Funciones para los datos de Aníbal
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

var coeffs = new[]
{
    // Coeficientes del modelo de Aníbal
    Utils.LinearFit(data[0][0], data[0][1], anibal[0]),
    // Coeficientes del modelo de María
    Utils.LinearFit(data[1][0], data[1][1], maria[0])
};

var funcs = new Func<double, double>[]
{
    // Función completa de Aníbal
    t => coeffs[0][0] * anibal[0][0](t) + coeffs[0][1] * anibal[0][1](t),
    // Función completa de María
    t => coeffs[1][0] * maria[0][0](t) + coeffs[1][1] * maria[0][1](t)
};

var derivs = new Func<double, double>[]
{
    // Derivada de la función de Aníbal
    t => coeffs[0][0] * anibal[1][0](t) + coeffs[0][1] * anibal[1][1](t),
    // Derivada de la función de María
    t => coeffs[1][0] * maria[1][0](t) + coeffs[1][1] * maria[1][1](t)
};

var equations = new Func<double, double, double>[]
{
    // Ecuación de Aníbal igualada a 0
    (t, y) => funcs[0](t) - y,
    // Ecuación de Maria igualada a 0
    (t, y) => funcs[1](t) - y
};

var jacobian = new Func<double, double, double>[,]
{
    {
        // Derivada de Aníbal respecto a t
        (t, _) => derivs[0](t),
        // Derivada de Aníbal respecto a y
        (_, _) => -1
    },
    {
        // Derivada de María respecto a t
        (t, _) => derivs[1](t),
        // Derivada de María respecto a y
        (_, _) => -1
    }
};

// Precisión de aproximaciones
var standard = 0.0001;

// Intersección entre función de Aníbal y María
var intersec = Utils.SolveNonlinearSystem(equations, jacobian, 1, 1, standard);

// Método de Newton-Raphson
var newton = new[]
{
    // Aproximación de t para Aníbal == 60 
    Utils.NewtonRaphson(t => funcs[0](t) - 60, derivs[0], 1, standard),
    // Aproximación de t para María == 60
    Utils.NewtonRaphson(t => funcs[1](t) - 60, derivs[1], 1, standard)
};

// Método de Bisección
var bisection = new[]
{
    // Aproximación de t para Aníbal == 60 
    Utils.Secant(t => funcs[0](t) - 60, 1, 1, standard),
    // Aproximación de t para María == 60
    Utils.Secant(t => funcs[1](t) - 60, 1, 1, standard)
};

// Impresión de resultados
Console.Write($"""
              Constantes de la ecuación de Aníbal
              x1 = {coeffs[0][0]:F4}
              x2 = {coeffs[0][1]:F4}
              ecuación: {coeffs[0][0]:F4}ln(t) + {coeffs[0][1]:F4}
              
              Constantes de la ecuación de María
              x1 = {coeffs[1][0]:F4}
              x2 = {coeffs[1][1]:F4}
              ecuación: {coeffs[1][0]:F4}cos(t/8) + {coeffs[1][1]:F4}e^(t/10)
              
              Los bebés tendrán la misma estatura de {intersec[1]:F4} a los {intersec[0]:F4} meses.
              
              (Newton-Raphson)
              La estatura de Aníbal será de 60 centímetros a los {newton[0]:F4} meses.
              La estatura de María será de 60 centímetros a los {newton[1]:F4} meses.
              
              (Bisección)
              La estatura de Aníbal será de 60 centímetros a los {bisection[0]:F4} meses.
              La estatura de María será de 60 centímetros a los {bisection[1]:F4} meses.
              """);