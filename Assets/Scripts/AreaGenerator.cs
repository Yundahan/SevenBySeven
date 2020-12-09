using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AreaGenerator : MonoBehaviour
{
	public FieldGenerator fg;
	
	int[,] numberField;
	System.Random rand = new System.Random();
	int[] sizes = new int[maxAreaSize + 1];
	
	float[] primes = {2f, 3f, 5f, 7f, 11f, 13f, 17f};
	
	const double mean = 4d;//make these adjustable in the system maybe, provide option to test distributions on a large scale
	const double sigma = 0.7d;
	const float mergeProbability = 0.5f;
	const int maxAreaSize = 7;
	const bool sizeOutput = true;
	
    // Start is called before the first frame update
    void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public int[,] GenerateAreas(int[,] nf, float t)
	{
		int[,] areaField = new int[7, 7];
		
		if(t > 1000f)
		{
			Debug.Log("Area generation failed over a thousand times, abort");
			return areaField;
		}
		
		List<int[]> dofs = new List<int[]>();
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
			/*int x = 0;
			int y = 0;
			bool found = false;
			
			for(x = 0; x < 7; x++)
			{
				for(y = 0; y < 7; y++)
				{
					if(areaField[x, y] == -1)
					{
						areaField[x, y] = areaCount;
						count--;
						found = true;
						break;
					}
				}
				
				if(found)
				{
					break;
				}
			}*/
			
			int index = UnityEngine.Random.Range(0, free.Count);//randomly choose next position for new area
			int x = free[index][0];
			int y = free[index][1];
			areaField[x, y] = areaCount;
			count--;
			free.RemoveAt(index);//cutoff here to enable the old version above
			int areaSize = GenerateAreaSize() - 1;
			dofs.Clear();
			float product = 510510f / primes[nf[x, y] - 1];
			
			if(areaSize < 1)//clamp area size for extreme values
			{
				areaSize = 1;
			}
			else if(areaSize > 6)
			{
				areaSize = 6;
			}
				
			while(count > 0 && areaSize > 0)
			{
				if(x < 6 && areaField[x + 1, y] == -1 && !fg.HasDecimals(product / primes[nf[x + 1, y] - 1]))//check which adjacent positions are still free
				{
					int[] newPos = new int[2] {x + 1, y};
					if(FindArrayInList(dofs, newPos) == -1)
					{
						dofs.Add(new int[2] {x + 1, y});
					}
				}
				if(x > 0 && areaField[x - 1, y] == -1 && !fg.HasDecimals(product / primes[nf[x - 1, y] - 1]))
				{
					int[] newPos = new int[2] {x - 1, y};
					if(FindArrayInList(dofs, newPos) == -1)
					{
						dofs.Add(new int[2] {x - 1, y});
					}
				}
				if(y < 6 && areaField[x, y + 1] == -1 && !fg.HasDecimals(product / primes[nf[x, y + 1] - 1]))
				{
					int[] newPos = new int[2] {x, y + 1};
					if(FindArrayInList(dofs, newPos) == -1)
					{
						dofs.Add(new int[2] {x, y + 1});
					}
				}
				if(y > 0 && areaField[x, y - 1] == -1 && !fg.HasDecimals(product / primes[nf[x, y - 1] - 1]))
				{
					int[] newPos = new int[2] {x, y - 1};
					if(FindArrayInList(dofs, newPos) == -1)
					{
						dofs.Add(new int[2] {x, y - 1});
					}
				}
				
				if(dofs.Count == 0)//no space left around the area
				{
					break;
				}
				
				int number = UnityEngine.Random.Range(0, dofs.Count);
				x = dofs[number][0];
				y = dofs[number][1];
				count--;
				areaSize--;
				product /= primes[nf[x, y] - 1];
				int pos = FindArrayInList(free, dofs[number]);
				
				if(pos == -1)
				{
					Debug.Log("position contained in dofs was not found in free");
				}
				
				free.RemoveAt(pos);
				areaField[x, y] = areaCount;
				dofs.RemoveAt(number);
				pos = 0;
				
				while(pos != -1)
				{
					pos = FindArrayInList(dofs, new int[] {x, y});
					
					if(pos != -1)
					{
						dofs.RemoveAt(pos);
					}
				}
				
				int value = nf[x, y];//remove positions from dofs which have the same value that was just added
				
				for(int i = dofs.Count - 1; i > -1; i--)
				{
					int[] position = dofs[i];
					if(nf[position[0], position[1]] == value)
					{
						dofs.RemoveAt(i);
					}
				}
			}
			
			areaCount++;
		}
		
		areaField = MergeOnes(areaField, areaCount);
		CalcAreaSizes(areaField, areaCount, false);
		
		if(!TestAreaCorrectness(nf, areaField, areaCount))
		{
			Debug.Log("Areas are incorrect, recalculate...");
			return GenerateAreas(nf, t + 1f);
		}
		
		return areaField;
	}
	
	public int[] CalcSums(int[,] nf, int[,] af)//calculate the sums for the areas
	{
		int areaCount = 0;
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				if(af[i, j] > areaCount)
				{
					areaCount = af[i, j];
				}
			}
		}
		
		int[] res = new int[areaCount + 1];
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				res[af[i, j]] += nf[i, j];
			}
		}
		
		return res;
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
	
	int[,] MergeOnes(int[,] af, int areaCount)//merge areas of size one with neighboring fields
	{
		int[] areaArray = new int[areaCount];
		List<int[]> candidateNeighbors = new List<int[]>();
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				areaArray[af[i, j]]++;
			}
		}
		
		for(int i = 0; i < areaCount; i++)
		{
			if(areaArray[i] == 1 && mergeProbability > UnityEngine.Random.Range(0f, 1f))
			{
				candidateNeighbors.Clear();
				int[] position = FindPositionOfArea(af, i);
				int x = position[0];
				int y = position[1];
				
				if(x > 0)
				{
					candidateNeighbors.Add(new int[2] {x - 1, y});
				}
				if(x < 6)
				{
					candidateNeighbors.Add(new int[2] {x + 1, y});
				}
				if(y > 0)
				{
					candidateNeighbors.Add(new int[2] {x, y - 1});
				}
				if(y < 6)
				{
					candidateNeighbors.Add(new int[2] {x, y + 1});
				}
				
				int mergePos = UnityEngine.Random.Range(0, candidateNeighbors.Count);
				areaArray[af[candidateNeighbors[mergePos][0], candidateNeighbors[mergePos][1]]]--;
				areaArray[i]++;
				af[candidateNeighbors[mergePos][0], candidateNeighbors[mergePos][1]] = i;
			}
		}
		
		return af;
	}
	
	int[] FindPositionOfArea(int[,] af, int areaID)//find the first position at which an area is found
	{
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				if(af[i, j] == areaID)
				{
					return new int[2] {i, j};
				}
			}
		}
		
		return new int[2] {-1, -1};
	}
	
	int GenerateAreaSize()//normal distribution
	{
		double u1 = 1.0 - rand.NextDouble();
		double u2 = 1.0 - rand.NextDouble();
		double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
		double randNormal = mean + sigma * randStdNormal;
		int areaSize = (int)(randNormal + 0.5d);
		return areaSize;
	}
	
	void CalcAreaSizes(int[,] areaField, int areaCount, bool print)//calculate the size of each area and store it in the sizes array
	{
		int[] array = new int[areaCount];
		
		for(int i = 0; i < 7; i ++)
		{
			for(int j = 0; j < 7; j++)
			{
				array[areaField[i, j]]++;
			}
		}
		
		for(int i = 0; i < areaCount; i++)
		{
			if(print)
			{
				Debug.Log(i + ": " + array[i]);
			}
			
			sizes[array[i]]++;
		}
	}
	
	bool TestAreaCorrectness(int[,] nf, int[,] af, int areaCount)//ensure that no area contains a number more than once
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
				areas[af[i, j]] /= primes[nf[i, j] - 1];
				
				if(fg.HasDecimals(areas[af[i, j]]))
				{
					return false;
				}
			}
		}
		
		return true;
	}
	
	void OutputSizes()//print the currently stored sizes
	{
		int sum = 0;
		
		for(int i = 1; i < sizes.Length; i++)
		{
			Debug.Log(i + ": " + sizes[i]);
			sum += sizes[i] * i;
		}
		
		Debug.Log("Checksum: " + sum);
	}
}
