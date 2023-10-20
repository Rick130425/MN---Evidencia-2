using System.Text;

namespace Evidencia2;

public static class Utils
{
    public static double[] LinearFit(double[] input, double[] output, Func<double, double>[] funcs)
    {
        var jacobian = new double[input.Length, funcs.Length];
        var size = new int[] { jacobian.GetLength(0), jacobian.GetLength(1) };

        for (var i = 0; i < size[0]; i++)
        {
            for (var j = 0; j < size[1]; j++)
            {
                jacobian[i, j] = funcs[j](input[i]);
            }
        }

        var tJacobian = Transpose(jacobian);
        var augMatrix = new double[size[1], size[1] + 1];
        augMatrix = copyTo(DotProduct(tJacobian, jacobian), augMatrix);
        augMatrix = copyTo(DotProduct(tJacobian, Transpose(ToMatrix(output))), augMatrix, startColumn: size[1]);
        augMatrix = GaussianElimination(augMatrix);
        var coefficients = new double[size[1]];
        for (int i = 0; i < size[1]; i++)
        {
            coefficients[i] = augMatrix[i, size[1]];
        }

        return coefficients;
    }

    public static double[,] DotProduct(double[,] matrixOne, double[,] matrixTwo)
    {
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
                for (var k = 0; k < measures[2]; k++)
                {
                    result[i, j] += matrixOne[i, k] * matrixTwo[k, j];
                }
            }
        }

        return result;
    }

    public static double[,] Transpose(double[,] matrix)
    {
        var size = new[] { matrix.GetLength(0), matrix.GetLength(1) };
        var result = new double[size[1], size[0]];

        for (var i = 0; i < size[1]; i++)
        {
            for (var j = 0; j < size[0]; j++)
            {
                result[i, j] = matrix[j, i];
            }
        }

        return result;
    }

    public static double[,] copyTo(double[,] src, double[,] destination, int startRow = 0, int startColumn = 0)
    {
        var size = new[] { src.GetLength(0), src.GetLength(1) };
        
        for (var i = 0; i < size[0]; i++)
        {
            for (var j = 0; j < size[1]; j++)
            {
                destination[i + startRow, j + startColumn] = src[i, j];
            }
        }

        return destination;
    }

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

    public static string ToString(double[] array)
    {
        StringBuilder text = new StringBuilder();
        var size = array.GetLength(0);
        for (var i = 0; i < size; i++)
        {
            text.Append(array[i] + (i == size - 1 ? "" : ", "));
        }

        return text.ToString();
    }
}