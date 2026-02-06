using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AreaGenerator : MonoBehaviour
{
	public FieldGenerator fieldGenerator;
	public PrimeManager primeManager;
	
	const double mean = 4d;
	const double sigma = 0.7d;
	
	System.Random rand = new System.Random();
	
	public int[,] GenerateAreas(int[,] numberField, float t)
	{
		int[,] areaField = new int[7, 7];
		
		if(t > 1000f)
		{
			Debug.Log("Area generation failed over a thousand times, abort");
			return areaField;
		}
		
		List<int[]> degreesOfFreedom = new List<int[]>();
		List<int[]> free = new List<int[]>();
		int count = 49;
		int areaCount = 0;
	
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				areaField[i, j] = -1;
				free.Add(new int[2] {i, j});
			}
		}
		
		while (count > 0)
		{
			int index = UnityEngine.Random.Range(0, free.Count);//randomly choose next position for new area
			int x = free[index][0];
			int y = free[index][1];
			areaField[x, y] = areaCount;
			count--;
			free.RemoveAt(index);
			int areaSize = GenerateAreaSize();
			degreesOfFreedom.Clear();
			float product = 510510f / this.primeManager.GetPrimeForNumber(numberField[x, y]);
				
			while(count > 0 && areaSize > 0)
			{
				if(x < 6 && areaField[x + 1, y] == -1 && !this.primeManager.IsNumberContainedInRowColumnArea(numberField[x + 1, y], product))//check which adjacent positions are still free
				{
					int[] newPos = new int[2] {x + 1, y};
					
					if(FindArrayInList(degreesOfFreedom, newPos) == -1)
					{
						degreesOfFreedom.Add(new int[2] {x + 1, y});
					}
				}
				if(x > 0 && areaField[x - 1, y] == -1 && !this.primeManager.IsNumberContainedInRowColumnArea(numberField[x - 1, y], product))
				{
					int[] newPos = new int[2] {x - 1, y};
					
					if(FindArrayInList(degreesOfFreedom, newPos) == -1)
					{
						degreesOfFreedom.Add(new int[2] {x - 1, y});
					}
				}
				if(y < 6 && areaField[x, y + 1] == -1 && !this.primeManager.IsNumberContainedInRowColumnArea(numberField[x, y + 1], product))
				{
					int[] newPos = new int[2] {x, y + 1};
					
					if(FindArrayInList(degreesOfFreedom, newPos) == -1)
					{
						degreesOfFreedom.Add(new int[2] {x, y + 1});
					}
				}
				if(y > 0 && areaField[x, y - 1] == -1 && !this.primeManager.IsNumberContainedInRowColumnArea(numberField[x, y - 1], product))
				{
					int[] newPos = new int[2] {x, y - 1};
					
					if(FindArrayInList(degreesOfFreedom, newPos) == -1)
					{
						degreesOfFreedom.Add(new int[2] {x, y - 1});
					}
				}
				
				if(degreesOfFreedom.Count == 0)//no space left around the area
				{
					break;
				}
				
				int number = UnityEngine.Random.Range(0, degreesOfFreedom.Count);
				x = degreesOfFreedom[number][0];
				y = degreesOfFreedom[number][1];
				count--;
				areaSize--;
				product /= this.primeManager.GetPrimeForNumber(numberField[x, y]);
				int pos = FindArrayInList(free, degreesOfFreedom[number]);
				
				if(pos == -1)
				{
					Debug.Log("position contained in degreesOfFreedom was not found in free");
				}
				
				free.RemoveAt(pos);
				areaField[x, y] = areaCount;
				degreesOfFreedom.RemoveAt(number);
				pos = 0;
				
				//look for the newly added position in the degreesOfFreedom and remove it from there
				while(pos != -1)
				{
					pos = FindArrayInList(degreesOfFreedom, new int[] {x, y});
					
					if(pos != -1)
					{
						degreesOfFreedom.RemoveAt(pos);
					}
				}
				
				int value = numberField[x, y];//remove positions from degreesOfFreedom which have the same value that was just added
				
				for(int i = degreesOfFreedom.Count - 1; i > -1; i--)
				{
					int[] position = degreesOfFreedom[i];
					if(numberField[position[0], position[1]] == value)
					{
						degreesOfFreedom.RemoveAt(i);
					}
				}
			}
			
			areaCount++;
		}
		
		if(!TestAreaCorrectness(numberField, areaField, areaCount))
		{
			Debug.Log("Areas are incorrect, recalculate...");
			return GenerateAreas(numberField, t + 1f);
		}
		
		return areaField;
	}
	
	public int[] CalcSums(int[,] numberField, int[,] areaField)//calculate the sums for the areas
	{
		int areaCount = 0;
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				if(areaField[i, j] > areaCount)
				{
					areaCount = areaField[i, j];
				}
			}
		}
		
		int[] res = new int[areaCount + 1];
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				res[areaField[i, j]] += numberField[i, j];
			}
		}
		
		return res;
	}
	
	public int[] FindPositionOfArea(int[,] areaField, int areaID)//find the first position at which an area is found
	{
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				if(areaField[i, j] == areaID)
				{
					return new int[2] {i, j};
				}
			}
		}
		
		return new int[2] {-1, -1};
	}
	
	int FindArrayInList(List<int[]> list, int[] array)//find first position of a two element array in a list
	{
		int res = 0;
		
		foreach(int[] element in list)
		{
			if(element[0] == array[0] && element[1] == array[1])
			{
				return res;
			}
			
			res++;
		}
		
		return -1;
	}
	
	float GetAreaProduct(int[,] numberField, int[,] areaField, int areaID)
	{
		float product = 510510f;
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				if(areaField[i, j] == areaID)
				{
					product /= this.primeManager.GetPrimeForNumber(numberField[i, j]);
				}
			}
		}
		
		return product;
	}
	
	void ReturnNeighbors(int[] pos, List<int[]> res)
	{
		int x = pos[0];
		int y = pos[1];
		
		if(x > 0)
		{
			res.Add(new int[2] {x - 1, y});
		}
		if(x < 6)
		{
			res.Add(new int[2] {x + 1, y});
		}
		if(y > 0)
		{
			res.Add(new int[2] {x, y - 1});
		}
		if(y < 6)
		{
			res.Add(new int[2] {x, y + 1});
		}
	}
	
	int GenerateAreaSize()//normal distribution
	{
		double u1 = 1.0 - rand.NextDouble();
		double u2 = 1.0 - rand.NextDouble();
		double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
		double randNormal = mean + sigma * randStdNormal;
		int areaSize = (int)(randNormal + 0.5d) - 1;

        if (areaSize < 1)//clamp area size for extreme values
        {
            areaSize = 1;
        }
        else if (areaSize > 6)
        {
            areaSize = 6;
        }

        return areaSize;
	}
	
	bool TestAreaCorrectness(int[,] numberField, int[,] areaField, int areaCount)//ensure that no area contains a number more than once
	{
		float[] areas = new float[areaCount];
		
		for(int i = 0; i < areaCount; i++)
		{
			areas[i] = 510510f;
		}
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				areas[areaField[i, j]] /= this.primeManager.GetPrimeForNumber(numberField[i, j]);
				
				if(this.primeManager.HasDecimals(areas[areaField[i, j]]))
				{
					return false;
				}
			}
		}
		
		return true;
	}
}
