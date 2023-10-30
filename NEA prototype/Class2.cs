using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA_prototype
{
    internal class PolynomialRegression
    {
        List<(double x, int y)> quadratic, cubic, quartic, quintic, sextic;



        public PolynomialRegression()
        {


            PopulateGraphs();
        }


        private void PopulateGraphs()
        {
            for(int i = 1; i <=15; i++)
            {
                quadratic.Add((3 * Math.Pow(i, 2) - 5 * i + 7, i));
                cubic.Add((2 * Math.Pow(i, 3) + Math.Pow(i, 2) - 5, i));
                quartic.Add((-6 * Math.Pow(i, 4) + Math.Pow(i, 3) + 2 * Math.Pow(i, 2) + 9 * i - 1, i));
                quintic.Add((3 * Math.Pow(i, 5) - 5 * Math.Pow(i, 4) + Math.Pow(i, 3) - 9 * Math.Pow(i, 2) - 7 * i + 8, i));
                sextic.Add((Math.Pow(i, 6), i));
            }
        }



        private double[,] Inverse(double[,] matrix)
        {
            double[,] inverse = new double[matrix.GetLength(0), matrix.GetLength(0)];
            if (matrix.GetLength(0) == 2)
            {
                double det = Determinant(matrix);
                inverse[0, 0] = matrix[1, 1] / det;
                inverse[1, 1] = matrix[0, 0] / det;
                inverse[0, 1] = -matrix[0, 1] / det;
                inverse[1, 0] = -matrix[1, 0] / det;
                return inverse;
            }
            else
            {

                double det = Determinant(matrix);
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

        //Subroutines below here work
        private double Determinant(double[,] matrix)
        {
            double det = 0;
            if (matrix.GetLength(0) == 2)
            {
                det += matrix[0, 0] * matrix[1, 1] - matrix[1, 0] * matrix[0, 1];
            }
            else
            {
                for (int i = 0; i < matrix.GetLength(0); ++i)
                {
                    double[,] cofactorMatrix = Cofactor(matrix, 0, i);
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

        private double[,] Cofactor(double[,] matrix, double row, double coloum)
        {
            double[,] cofactorMatrix = new double[matrix.GetLength(0) - 1, matrix.GetLength(0) - 1];
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
        private double[,] Transpose(double[,] matrix)
        {
            double[,] transposed = new double[matrix.GetLength(0), matrix.GetLength(0)];
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
}
