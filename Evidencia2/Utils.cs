using System.Text;

namespace Evidencia2;

/// <summary>
/// Funciones útiles.
/// <remarks>
/// La clase incluye funciones para matrices, como impresión y producto. También incluye función para ajuste de curva.
/// </remarks>
/// </summary>
public static class Utils
{
    /// <summary>
    /// Regresa los coeficientes de las funciones para que se ajusten a los datos proporcionados.
    /// </summary>
    /// <param name="input">Datos de entrada.</param>
    /// <param name="output">Datos de salida esperados.</param>
    /// <param name="funcs">Funciones de la ecuación.</param>
    /// <returns>Coeficientes de las funcioens.</returns>
    public static double[] LinearFit(double[] input, double[] output, Func<double, double>[] funcs)
    {
        var jacobian = new double[input.Length, funcs.Length];
        var size = new int[] { jacobian.GetLength(0), jacobian.GetLength(1) };

        // Se llena la matriz jacobiana
        // Las filas indican el valor de entrada que se utiliza, mientras la columna indica la función que se utiliza.
        for (var i = 0; i < size[0]; i++)
        {
            for (var j = 0; j < size[1]; j++)
            {
                jacobian[i, j] = funcs[j](input[i]);
            }
        }

        var tJacobian = Transpose(jacobian);
        var augMatrix = new double[size[1], size[1] + 1];
        
        // Se copia los valores del producto punto de la jacobiana por su transpuesta en la matriz aumentada.
        augMatrix = CopyTo(DotProduct(tJacobian, jacobian), augMatrix);
        
        // Se copian los valores del producto punto de la matriz jacobiana transpuesta con la matriz transpuesta de 
        // los valores de salida a la matriz aumentada, colocándolos en la última columna de la matriz
        augMatrix = CopyTo(DotProduct(tJacobian, Transpose(ToMatrix(output))), augMatrix, startColumn: size[1]);
        
        augMatrix = GaussianElimination(augMatrix);
        // La cantidad de coeficientes es igual a la cantidad de funciones
        var coefficients = new double[size[1]];
        for (int i = 0; i < size[1]; i++)
        {
            coefficients[i] = augMatrix[i, size[1]];
        }

        return coefficients;
    }
    
    /// <summary>
    /// Resuelve un sistema de ecuaciones no lineales.
    /// </summary>
    /// <param name="equations">Ecuaciones.</param>
    /// <param name="jacobian">Matriz jacobiana de las ecuaciones.</param>
    /// <param name="x">Valor inicial de x.</param>
    /// <param name="y">Valor inicial de y.</param>
    /// <param name="standard">Criterio de aceptación (precisión).</param>
    /// <returns>Arreglo con los valores de la intersección.</returns>
    public static double[] SolveNonlinearSystem(
        Func<double, double, double>[] equations, 
        Func<double, double, double>[,] jacobian, 
        double x, double y, double standard)
    {
        var augMatrix = new double[2, 3];
        do
        {
            // Se calculan los valores de la jacobiana y se guardan
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 2; j++)
                {
                    augMatrix[i, j] = jacobian[i, j](x, y);
                }
            }
            
            // Se calculan los valores de las ecuaciones
            for (var i = 0; i < 2; i++)
            {
                augMatrix[i, 2] = equations[i](x, y);
            }
            
            // Se realiza una eliminación gaussiana
            augMatrix = GaussianElimination(augMatrix);
            
            // Se restan los valores correspondientes a x y y
            x -= augMatrix[0, 2];
            y -= augMatrix[1, 2];
            // Mientras no se cumpla el criterio, se repite el proceso anterior
        } while (Math.Abs(equations[0](x, y)) > standard || Math.Abs(equations[1](x, y)) > standard);

        return new[] { x, y };
    }
    
    /// <summary>
    /// Aproxima la raíz de una función mediante el método de Newton-Raphson
    /// </summary>
    /// <param name="func">Función.</param>
    /// <param name="deriv">Derivada de la función.</param>
    /// <param name="x">Valor inicial de x.</param>
    /// <param name="standard">Criterio de aceptación (precisión).</param>
    /// <returns></returns>
    public static double NewtonRaphson(Func<double, double> func, Func<double, double> deriv, double x, 
        double standard)
    {
        do
        {
            x -= func(x) / deriv(x); 
            // Mientras no se cumpla el criterio, se repite el proceso anterior
        } while (Math.Abs(func(x)) > standard);

        return x;
    }
    
    /// <summary>
    /// Aproxima la raíz de una función mediante el método de la Secante.
    /// </summary>
    /// <param name="func">Función.</param>
    /// <param name="x">Valor inicial de x.</param>
    /// <param name="stepSize">Tamaño de cambio.</param>
    /// <param name="standard">Criterio de aceptación (precisión).</param>
    /// <returns></returns>
    public static double Secant(Func<double, double> func, double x, double stepSize, double standard)
    {
        double prevX, prevY, midX, midY, y = func(x);
        // Se busca el rango donde está la raíz
        do
        {
            prevX = x;
            prevY = y;
            x += stepSize;
            y = func(x);
        } while (prevY * y > 0);

        do
        {
            // Se calcula el valor medio (punto donde la recta entre los dos puntos es 0)
            midX = x - (prevX - x) * y / (prevY - y);
            midY = func(midX);
            
            // Si es mayor
            if (midY > 0)
            {
                // Se mueve el límite superior
                x = midX;
                y = midY;
            }
            // Si no
            else
            {
                // Se mueve el límite inferior
                prevX = midX;
                prevY = midY;
            } 
            // Mientras no se cumpla el criterio, se repite el proceso anterior
        } while (midY > standard);

        return midX;
    }
    
    /// <summary>
    /// Realiza una eliminación gaussiana a la matriz introducida.
    /// </summary>
    /// <param name="augmentedMatrix">Matriz extendida.</param>
    /// <returns>Matriz resultante de la eliminación gaussiana.</returns>
    public static double[,] GaussianElimination(double[,] augmentedMatrix)
    {
        // Arreglo de dimensiones
        var size = new[] { augmentedMatrix.GetLength(0), augmentedMatrix.GetLength(1) };
        for (var pivotIndex = 0; pivotIndex < size[0]; pivotIndex++)
        {
            // Guardado del pivote
            var pivot = augmentedMatrix[pivotIndex, pivotIndex];
            for (var row = 0; row < size[0]; row++)
            {
                // Fila temporal (iniciando desde la fila del pivote)
                var tempRow = (pivotIndex + row) % size[0];
                // Si la fila es 0
                if (row == 0)
                {
                    for (var column = 0; column < size[1]; column++)
                    {
                        // División de la fila del pivote por el pivote
                        augmentedMatrix[tempRow, column] /= pivot;
                    }
                }
                // Si no
                else
                {
                    // Guardado del factor (valor de la columna del pivote en la fila)
                    var factor = augmentedMatrix[tempRow, pivotIndex];
                    for (var column = 0; column < size[1]; column++)
                    {
                        // Resta de los valores de la fila del pivote, multiplicados por el factor, a la fila actuál
                        augmentedMatrix[tempRow, column] -= augmentedMatrix[pivotIndex, column] * factor;
                    }
                }
            }
        }

        // Retorno de la matriz extendida resultante
        return augmentedMatrix;
    }
    
    /// <summary>
    /// Realiza el producto punto de dos matrices.
    /// </summary>
    /// <param name="matrixOne">Matriz uno.</param>
    /// <param name="matrixTwo">Matriz dos.</param>
    /// <returns>Resultado del producto punto.</returns>
    public static double[,] DotProduct(double[,] matrixOne, double[,] matrixTwo)
    {
        // Se guardan las dimensiones
        var measures = new[]
        {
            matrixOne.GetLength(0),
            matrixTwo.GetLength(1),
            matrixOne.GetLength(1)
        };
        var result = new double[measures[0], measures[1]];

        for (var i = 0; i < measures[0]; i++)
        {
            for (var j = 0; j < measures[1]; j++)
            {
                // Cada posición es igual a la suma del producto de la fila de la primera matriz, por la columna de la
                // segunda matriz (producto elemento por elemento)
                for (var k = 0; k < measures[2]; k++)
                {
                    result[i, j] += matrixOne[i, k] * matrixTwo[k, j];
                }
            }
        }

        return result;
    }
    
    /// <summary>
    /// Transpone una matriz.
    /// </summary>
    /// <param name="matrix">Matriz.</param>
    /// <returns>Matriz transpuesta.</returns>
    public static double[,] Transpose(double[,] matrix)
    {
        var size = new[] { matrix.GetLength(0), matrix.GetLength(1) };
        var result = new double[size[1], size[0]];

        for (var i = 0; i < size[1]; i++)
        {
            for (var j = 0; j < size[0]; j++)
            {
                // Se invierten las coordenadas
                result[i, j] = matrix[j, i];
            }
        }

        return result;
    }
    
    /// <summary>
    /// Copia los elementos de una matriz en otra.
    /// </summary>
    /// <param name="src">Matriz fuente.</param>
    /// <param name="destination">Matriz destino.</param>
    /// <param name="startRow">Fila donde inicia el pegado.</param>
    /// <param name="startColumn">Columna donde inicia el pegado.</param>
    /// <returns>Matriz con los valores pegados.</returns>
    public static double[,] CopyTo(double[,] src, double[,] destination, int startRow = 0, int startColumn = 0)
    {
        var size = new[] { src.GetLength(0), src.GetLength(1) };

        for (var i = 0; i < size[0]; i++)
        {
            for (var j = 0; j < size[1]; j++)
            {
                // Copia los elementos de la matriz fuente a la matriz resultante, iniciando el pegado
                // desde la fila y columna iniciales.
                destination[i + startRow, j + startColumn] = src[i, j];
            }
        }

        return destination;
    }
    
    /// <summary>
    /// Convierte un arreglo en una matriz.
    /// </summary>
    /// <remarks>
    /// La cantidad de filas será 1, y la cantidad de columnas será el tamaño del arreglo.
    /// </remarks>
    /// <param name="array">Arreglo.</param>
    /// <returns>Matriz del arreglo.</returns>
    public static double[,] ToMatrix(double[] array)
    {
        int size = array.Length;
        double[,] result = new double[1, size];

        for (int i = 0; i < size; i++)
        {
            result[0, i] = array[i];
        }

        return result;
    }

    /// <summary>
    /// Regresa la representación en texto de un arreglo.
    /// </summary>
    /// <param name="array">Arreglo.</param>
    /// <returns>Representación en texto del arreglo.</returns>
    public static string ToString(double[] array)
    {
        // StringBuilder para formar el texto
        StringBuilder text = new StringBuilder();
        // Se guarda el tamaño del arreglo
        var size = array.GetLength(0);
        // Se realiza un for para ir guardando los valores
        for (var i = 0; i < size; i++)
        {
            text.Append(array[i] + (i == size - 1 ? "" : ", "));
        }

        // Se regresa el texto
        return text.ToString();
    }

}