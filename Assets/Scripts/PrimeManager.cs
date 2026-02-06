using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrimeManager : MonoBehaviour
{
	float[] primes = {2f, 3f, 5f, 7f, 11f, 13f, 17f};

	const float epsilon = 0.0001f;

    /// <summary>
    /// Checks if a number can fit in a given position on the board, accounting for areas.
    /// </summary>
    /// <param name="number">Number to check, from 1 to 7</param>
    /// <param name="fieldX">X coordinate of the position, 0 to 6</param>
    /// <param name="fieldY">Y coordinate of the position, 0 to 6</param>
    /// <param name="columns">Prime products of the columns</param>
    /// <param name="rows">Prime products of the rows</param>
    /// <param name="areaField">Two dimensional array containing the area ID for each position</param>
    /// <param name="areaProducts">Prime products for each area ID</param>
    public bool DoesNumberFitInField(int number, int fieldX, int fieldY, float[] columns, float[] rows, int[,] areaField, float[] areaProducts)
    {
        float primeNumber = GetPrimeForNumber(number);
        return !HasDecimals(rows[fieldX] / primeNumber) && !HasDecimals(columns[fieldY] / primeNumber) && !HasDecimals(areaProducts[areaField[fieldX, fieldY]] / primeNumber);
    }

    /// <summary>
    /// Inserts number in the field at a given position, accounting for areas..
    /// </summary>
    /// <param name="number">Number to insert, from 1 to 7</param>
    /// <param name="fieldX">X coordinate of the position, 0 to 6</param>
    /// <param name="fieldY">Y coordinate of the position, 0 to 6</param>
    /// <param name="columns">Prime products of the columns</param>
    /// <param name="rows">Prime products of the rows</param>
    public bool InsertNumberInField(int number, int fieldX, int fieldY, float[] columns, float[] rows, int[,] areaField, float[] areaProducts, int[,] numberField)
    {
        if (!DoesNumberFitInField(number, fieldX, fieldY, columns, rows, areaField, areaProducts))
        {
            return false;
        }

        float primeNumber = GetPrimeForNumber(number);
        numberField[fieldX, fieldY] = number;
        rows[fieldX] /= primeNumber;
        columns[fieldY] /= primeNumber;

        return true;
    }

    /// <summary>
    /// Checks if a number can fit in a given position on the board, not accounting for areas.
    /// </summary>
    /// <param name="number">Number to check, from 1 to 7</param>
    /// <param name="fieldX">X coordinate of the position, 0 to 6</param>
    /// <param name="fieldY">Y coordinate of the position, 0 to 6</param>
    /// <param name="columns">Prime products of the columns</param>
    /// <param name="rows">Prime products of the rows</param>
    public bool DoesNumberFitInField(int number, int fieldX, int fieldY, float[] columns, float[] rows)
    {
        float primeNumber = GetPrimeForNumber(number);
        return !HasDecimals(rows[fieldX] / primeNumber) && !HasDecimals(columns[fieldY] / primeNumber);
    }

    /// <summary>
    /// Inserts number in the field at a given position, not accounting for areas..
    /// </summary>
    /// <param name="number">Number to insert, from 1 to 7</param>
    /// <param name="fieldX">X coordinate of the position, 0 to 6</param>
    /// <param name="fieldY">Y coordinate of the position, 0 to 6</param>
    /// <param name="columns">Prime products of the columns</param>
    /// <param name="rows">Prime products of the rows</param>
    public bool InsertNumberInField(int number, int fieldX, int fieldY, float[] columns, float[] rows, int[,] numberField)
    {
        if (!DoesNumberFitInField(number, fieldX, fieldY, columns, rows))
        {
            return false;
        }

        float primeNumber = GetPrimeForNumber(number);
        numberField[fieldX, fieldY] = number;
        rows[fieldX] /= primeNumber;
        columns[fieldY] /= primeNumber;

        return true;
    }

    /// <summary>
    /// Checks if a number is already contained in a given row/column/area.
    /// </summary>
    /// <param name="number">Number to check, from 1 to 7</param>
    /// <param name="rowColumnAreaproduct">Product of the row/column/area</param>
    public bool IsNumberContainedInRowColumnArea(int number, float rowColumnAreaproduct)
    {
        if (HasDecimals(rowColumnAreaproduct / GetPrimeForNumber(number)))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the prime for a given ordinal number.
    /// </summary>
    /// <param name="number">Ordinal number, 0 to 6</param>
    public float GetPrimeForNumber(int number)
    {
        return primes[number - 1];
    }

    public bool HasDecimals(float value)
    {
        return !((value % 1) < epsilon);
    }
}
