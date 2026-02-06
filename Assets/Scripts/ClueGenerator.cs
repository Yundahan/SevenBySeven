using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueGenerator : MonoBehaviour
{
	public Solver solver;
	
	public void InitialRemovals(int[,] numberField, int[,] areaField, int areaCount)//remove numbers so that the puzzle definitely retains a unique solution
	{
		int rowIndex = Random.Range(0, 7);
		int columnIndex = Random.Range(0, 7);
		
		for(int i = 0; i < 7; i ++)//remove one row and one column
		{
			numberField[i, columnIndex] = -1;
			numberField[rowIndex, i] = -1;
		}
		
		List<List<Vector2Int>> areaList = new List<List<Vector2Int>>();//list at index i contains all grid positions for area i
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
			areaList.Add(new List<Vector2Int>());
		}
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				if(numberField[i, j] != -1)
				{
					areaList[areaField[i, j]].Add(new Vector2Int(i, j));
				}
			}
		}
		
		for(int i = 0; i < areaCount; i++)
		{
			if(areaList[i].Count > 0 && emptySpotsPerArea[i] == 0)//remove one number each from areas where no spot is free
			{
				int index = Random.Range(0, areaList[i].Count);
				numberField[areaList[i][index].x, areaList[i][index].y] = -1;
			}
		}
	}
	
	public int RemovalLoop(int[,] numberField, int[,] areaField, int[] areaSums)
	{
		List<Vector2Int> untested = new List<Vector2Int>();
		int clueCount = 0;
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				if(numberField[i, j] != -1)
				{
					untested.Add(new Vector2Int(i, j));
				}
			}
		}
		
		while(untested.Count > 0)
		{
			int index = Random.Range(0, untested.Count);
            Vector2Int pos = untested[index];
			int number = numberField[pos.x, pos.y];
			numberField[pos.x, pos.y] = -1;
			untested.RemoveAt(index);
			int[,] copy = CopyNumberField(numberField);
			int res = solver.SolveCount(copy, areaField, areaSums, false);
			
			if(res >= 2)
			{
				numberField[pos.x, pos.y] = number;
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
