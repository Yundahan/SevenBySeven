using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
	int[,] numberField = new int[7, 7];
	const float epsilon = 0.0001f;
	float[] primes = {2f, 3f, 5f, 7f, 11f, 13f, 17f};
	
    // Start is called before the first frame update
    void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
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
		//int failcount = 0;
		
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
						
						/*if(failcount > 1000)
						{
							Debug.Log("1000 fails");
							return numberField;
						}
						
						if(j == 0)
						{
							Debug.Log("j == 0");
							printNumberField();
						}
						else
						{
							Debug.Log(numberField[i, j - 1]);
						}
						
						for(int k = 0; k < 7; k++)
						{
							if(numberField[i, k] == 0)
							{
								break;
							}
							
							columns[k] *= (float)numberField[i, k];
						}
						
						rows[i] = 510510.0f;
						failcount++;
						j = -1;
						break;*/
					}
					
					int n = UnityEngine.Random.Range(0, notTried.Count);
					n = notTried[n];
					float number = primes[n - 1];
					
					if(!HasDecimals(rows[i] / number) && !HasDecimals(columns[j] / number))
					{
						numberField[i, j] = n;
						rows[i] /= number;
						columns[j] /= number;
						notFound = false;
					}
					else
					{
						notTried.Remove(n);
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
				rowProduct *= primes[numberField[i, j] - 1];
				columnProduct *= primes[numberField[j, i] - 1];
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
	
	//Speedtests below
	/*void Test()
	{
		DateTime startTime = System.DateTime.Now;
		
		for(int i = 0; i < 1000000; i++)
		{
			float p = UnityEngine.Random.Range(0.0f, 1.0f);
			float product = 1;
			
			for(int j = 0; j < 7; j++)
			{
				if(UnityEngine.Random.Range(0.0f, 1.0f) < p)
				{
					product *= primes[j];
				}
			}
			
			for(int j = 0; j < 7; j++)
			{
				float quotient = product / primes[j];
				bool b = (quotient % 1) == 0;
			}
		}
		
		DateTime endTime = System.DateTime.Now;
		
		Debug.Log(endTime - startTime);
	}
	
	void Test2()
	{
		DateTime startTime = System.DateTime.Now;
		HashSet<int> numbers = new HashSet<int>();
		
		for(int i = 0; i < 1000000; i++)
		{
			float p = UnityEngine.Random.Range(0.0f, 1.0f);
			numbers.Clear();
			
			for(int j = 0; j < 7; j++)
			{
				if(UnityEngine.Random.Range(0.0f, 1.0f) < p)
				{
					numbers.Add(j);
				}
			}
			
			for(int j = 0; j < 7; j++)
			{
				bool b = numbers.Contains(j);
			}
		}
		
		DateTime endTime = System.DateTime.Now;
		
		Debug.Log(endTime - startTime);
	}*/
}
