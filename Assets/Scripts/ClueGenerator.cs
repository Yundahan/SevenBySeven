using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueGenerator : MonoBehaviour
{
	public Solver s;
	
	bool addClues = false;
	int minoffset = 0;
	int maxoffset = 3;
	
    // Start is called before the first frame update
    void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void InitialRemovals(int[,] numberField, int[,] areaField, int areaCount)//remove numbers so that the puzzle definitely retains a unique solution
	{
		int rowIndex = UnityEngine.Random.Range(0, 7);
		int columnIndex = UnityEngine.Random.Range(0, 7);
		
		for(int i = 0; i < 7; i ++)//remove one row and one column
		{
			numberField[i, columnIndex] = -1;
			numberField[rowIndex, i] = -1;
		}
		
		List<List<int[]>> areaList = new List<List<int[]>>();
		int[] emptySpotsPerArea = new int[areaCount];
		
		for(int i = 0; i < 7; i++)//compute emptySpotsPerArea
		{
			for(int j = 0; j < 7; j++)
			{
				if(numberField[i, j] == -1)
				{
					emptySpotsPerArea[areaField[i, j]]++;
				}
			}
		}
		
		for(int i = 0; i < areaCount; i++)
		{
			areaList.Add(new List<int[]>());
		}
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				if(numberField[i, j] != -1)
				{
					areaList[areaField[i, j]].Add(new int[] {i, j});
				}
			}
		}
		
		for(int i = 0; i < areaCount; i++)
		{
			if(areaList[i].Count > 0 && emptySpotsPerArea[i] == 0)//remove one number each from areas where no spot is free
			{
				int index = UnityEngine.Random.Range(0, areaList[i].Count);
				numberField[areaList[i][index][0], areaList[i][index][1]] = -1;
			}
		}
	}
	
	public int RemovalLoop(int[,] numberField, int[,] areaField, int[] areaSums)
	{
		List<int[]> untested = new List<int[]>();
		int clueCount = 0;
		int offset = 0;
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				if(numberField[i, j] != -1)
				{
					untested.Add(new int[] {i, j});
				}
			}
		}
		
		if(addClues)
		{
			offset = UnityEngine.Random.Range(minoffset, maxoffset);
		}
		
		while(untested.Count > offset)
		{
			int index = UnityEngine.Random.Range(0, untested.Count);
			int[] pos = untested[index];
			int number = numberField[pos[0], pos[1]];
			numberField[pos[0], pos[1]] = -1;
			untested.RemoveAt(index);
			int[,] copy = CopyNumberField(numberField);
			int res = s.SolveCount(copy, areaField, areaSums, false);
			
			if(res == 2)
			{
				numberField[pos[0], pos[1]] = number;
				clueCount++;
			}
			else if(res == 0)
			{
				Debug.Log("Critical Error, field is not solvable anymore in removal loop");
			}
		}
		
		return clueCount;
	}
	
	int[,] CopyNumberField(int[,] numberField)
	{
		int[,] copy = new int[7, 7];
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				copy[i, j] = numberField[i, j];
			}
		}
		
		return copy;
	}
}
