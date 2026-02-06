using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
	public PrimeManager primeManager;

	int[,] numberField = new int[7, 7];
	const float epsilon = 0.0001f;
	
	public void PrintNumberField(int[,] nf)
	{
		string line;
		
		for(int i = 0; i < 7; i++)
		{
			line = "";
			
			for(int j = 0; j < 7; j++)
			{
				line = line + nf[i, j].ToString() + " ";
			}
			
			Debug.Log(line);
		}
	}
	
	public bool HasDecimals(float value)
	{
		return !((value % 1) < epsilon);
	}
	
	public int[,] GenerateField(float x)
	{
		if(x > 10000f)
		{
			Debug.Log("Number field generation failed ten thousand times, abort");
			return numberField;
		}
		
		List<int> notTried;
		float[] columns = {510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f};
		float[] rows = {510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f};
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				notTried = new List<int> {1, 2, 3, 4, 5, 6, 7};
				bool notFound = true;
				
				while(notFound)
				{
					if(notTried.Count == 0)//redo row
					{
						return GenerateField(x + 1f);
					}
					
					int number = UnityEngine.Random.Range(0, notTried.Count);
                    number = notTried[number];
					
					if(this.primeManager.InsertNumberInField(number, i, j, columns, rows, numberField))
					{
						notFound = false;
					}
					else
					{
						notTried.Remove(number);
					}
				}
			}
		}
		
		TestFieldCorrectness(numberField);
		
		return numberField;
	}
	
	bool TestFieldCorrectness(int[,] nf)
	{
		for(int i = 0; i < 7; i++)
		{
			float rowProduct = 1f;
			float columnProduct = 1f;
			
			for(int j = 0; j < 7; j++)
			{
				rowProduct *= this.primeManager.GetPrimeForNumber(numberField[i, j]);
				columnProduct *= this.primeManager.GetPrimeForNumber(numberField[j, i]);
			}
			
			float rowDiff = 510510f - rowProduct;
			float columnDiff = 510510f - columnProduct;
			
			if(rowDiff > epsilon || columnDiff > epsilon)
			{
				Debug.Log("Number field is incorrect");
				return false;
			}
		}
		
		return true;
	}
}
