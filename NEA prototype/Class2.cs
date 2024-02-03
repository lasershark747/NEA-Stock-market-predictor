using System;
using System.Collections.Generic;
using System.Numerics;

namespace NEA_prototype
{
    internal class PolynomialRegression
    {
        public PolynomialRegression() { }
        public List<double> DoPolynomialRegression(List<(long, double)> points)
        {
            List<List<double>> ListOfCoefficients = new List<List<double>>();
            for (int x = 1; x <= Math.Min(Math.Max(points.Count / 100, 3),7); x++)
            {
                List<double> coefficients = new List<double>();

                BigFloat[,] matrixA = new BigFloat[x + 1, x + 1];
                BigFloat[] matrixB = new BigFloat[x + 1];

                bool outOFRange = false;

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
                    double sum = 0;

                    for (int j = 0; j < inverseMatrixA.GetLength(0); j++)
                    {
                        if (inverseMatrixA[i, j] * matrixB[j] < double.MinValue)
                        {
                            outOFRange = true;
                        }
                        else
                        {
                            sum += (double)(inverseMatrixA[i, j] * matrixB[j]);
                        }

                    }
                    if (Double.IsNaN(sum))
                    {
                        outOFRange = true;
                    }
                    coefficients.Add(sum);
                }

                if (!outOFRange)
                {
                    Console.WriteLine("Degree " + x + " polynomial has been successfully generated");
                    ListOfCoefficients.Add(coefficients);
                }
                else
                {
                    Console.WriteLine("Degree " + x + " polynomial hasn't been successfully generated");
                }
            }

            int bestLine = 0;
            double bestVariance = double.MaxValue;

            for (int i = 0; i < ListOfCoefficients.Count; i++)
            {
                SumOfResiduals s = new SumOfResiduals(points, ListOfCoefficients[i]);
                double variance = s.Residuals();
                if (bestVariance > variance)
                {
                    bestVariance = variance;
                    bestLine = i;
                }
            }

            return ListOfCoefficients[bestLine];
        }
        private BigFloat[,] Inverse(BigFloat[,] matrix)
        {
            BigFloat[,] inverse = new BigFloat[matrix.GetLength(0), matrix.GetLength(0)];

            if (matrix.GetLength(0) == 2)
            {
                double det = (double)Determinant(matrix);
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
                            inverse[i, j] = (Determinant(Cofactor(matrix, i, j)) / det);
                        }
                        else
                        {
                            inverse[i, j] = -(Determinant(Cofactor(matrix, i, j)) / det);
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
        private BigFloat[,] Cofactor(BigFloat[,] matrix, double row, double column)
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
                    bool checkColumn = false;

                    for (int j = 0; j < matrix.GetLength(0); j++)
                    {
                        int x = j;
                        int y = i;

                        if (j == column)
                        {
                            checkColumn = true;
                        }
                        else
                        {
                            if (checkRow)
                            {
                                y--;
                            }
                            if (checkColumn)
                            {
                                x--;
                            }

                            cofactorMatrix[y, x] = matrix[i, j];
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