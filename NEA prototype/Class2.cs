using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NEA_prototype
{
    internal class PolynomialRegression
    {


        public PolynomialRegression()
        {


        }

        public List<double> DoPolynomialRegression(List<(long, double)> points)
        {
            List<List<double>> ListOfCoeffcients = new List<List<double>>();
            for (int x = 2; x <= 8; x++)
            {
                List<double> coeffcients = new List<double>();
                BigFloat[,] matrixA = new BigFloat[x + 1, x + 1];
                BigFloat[] matrixB = new BigFloat[x + 1];
                for (int i = 0; i < matrixA.GetLength(0); i++)
                {
                    for (int j = 0; j < matrixA.GetLength(0); j++)
                    {
                        double sumOfx = 0;
                        foreach ((long, double) coordinate in points)
                        {
                            sumOfx += Math.Pow(coordinate.Item1, i + j);
                        }
                        matrixA[i, j] = sumOfx;
                    }
                    double sumOfxy = 0;
                    foreach ((long, double) coordinate in points)
                    {
                        sumOfxy += Math.Pow(coordinate.Item1, i) * coordinate.Item2;
                    }
                    matrixB[i] = sumOfxy;
                }
                BigFloat[,] inverseMatrixA = Inverse(matrixA);

                for (int i = 0; i < inverseMatrixA.GetLength(0); i++)
                {
                    BigFloat sum = 0;
                    for (int j = 0; j < inverseMatrixA.GetLength(0); j++)
                    {
                        sum += inverseMatrixA[i, j] * matrixB[j];
                    }
                    Console.WriteLine(sum);
                    coeffcients.Add((double)sum);
                }
                ListOfCoeffcients.Add(coeffcients);
                Console.WriteLine(x);

            }

            int bestLine = 0;
            BigFloat bestVariance = 99999999999;
            Console.WriteLine("please enter how many data points you would like to use to find the optimal order\nUsing more then 100 data points will take too long");
            int numOfPoints = int.Parse(Console.ReadLine());



            for (int i = 0; i < ListOfCoeffcients.Count; i++)
            {
                SumOfResiduals s = new SumOfResiduals(points, ListOfCoeffcients[i]); //try using decimals as that may have enough accuracy
                BigFloat variance = s.Residuals(numOfPoints);
                Console.WriteLine(variance);
                if (bestVariance > variance)
                {
                    bestVariance = variance;
                    bestLine = i;
                }

            }
            Console.WriteLine(bestLine);
            return ListOfCoeffcients[bestLine];
        }

        private BigFloat[,] Inverse(BigFloat[,] matrix)
        {
            BigFloat[,] inverse = new BigFloat[matrix.GetLength(0), matrix.GetLength(0)];
            if (matrix.GetLength(0) == 2)
            {
                BigFloat det = Determinant(matrix);
                inverse[0, 0] = matrix[1, 1] / det;
                inverse[1, 1] = matrix[0, 0] / det;
                inverse[0, 1] = -matrix[0, 1] / det;
                inverse[1, 0] = -matrix[1, 0] / det;
                return inverse;
            }
            else
            {

                BigFloat det = Determinant(matrix);
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(0); j++)
                    {
                        if (i % 2 == j % 2)
                        {

                            inverse[i, j] = Determinant(Cofactor(matrix, i, j)) / det;
                        }
                        else
                        {
                            inverse[i, j] = (-1) * Determinant(Cofactor(matrix, i, j)) / det;
                        }

                    }
                }

                inverse = Transpose(inverse);
            }



            return inverse;
        }
        private BigFloat Determinant(BigFloat[,] matrix)
        {
            BigFloat det = 0;
            if (matrix.GetLength(0) == 2)
            {
                det += matrix[0, 0] * matrix[1, 1] - matrix[1, 0] * matrix[0, 1];
            }
            else
            {
                for (int i = 0; i < matrix.GetLength(0); ++i)
                {
                    BigFloat[,] cofactorMatrix = Cofactor(matrix, 0, i);
                    if (i % 2 == 0)
                    {
                        det += matrix[0, i] * Determinant(cofactorMatrix);
                    }
                    else if (i % 2 == 1)
                    {
                        det -= matrix[0, i] * Determinant(cofactorMatrix);
                    }
                }
            }
            return det;
        }

        private BigFloat[,] Cofactor(BigFloat[,] matrix, BigFloat row, BigFloat coloum)
        {
            BigFloat[,] cofactorMatrix = new BigFloat[matrix.GetLength(0) - 1, matrix.GetLength(0) - 1];
            bool checkRow = false;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (i == row)
                {
                    checkRow = true;
                }
                else
                {
                    bool checkColoum = false;
                    for (int j = 0; j < matrix.GetLength(0); j++)
                    {
                        if (j == coloum)
                        {
                            checkColoum = true;
                        }
                        else if (checkColoum && checkRow)
                        {
                            cofactorMatrix[i - 1, j - 1] = matrix[i, j];
                        }
                        else if (checkRow)
                        {
                            cofactorMatrix[i - 1, j] = matrix[i, j];
                        }
                        else if (checkColoum)
                        {
                            cofactorMatrix[i, j - 1] = matrix[i, j];
                        }
                        else
                        {
                            cofactorMatrix[i, j] = matrix[i, j];
                        }
                    }
                }
            }
            return cofactorMatrix;
        }
        private BigFloat[,] Transpose(BigFloat[,] matrix)
        {
            BigFloat[,] transposed = new BigFloat[matrix.GetLength(0), matrix.GetLength(0)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    transposed[i, j] = matrix[j, i];
                }
            }
            return transposed;
        }
    }
}

