using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solver : MonoBehaviour
{
	public FieldGenerator fg;
	public AreaGenerator ag;
	
	float[] areaProducts;
	float[] rowProducts;
	float[] columnProducts;
	int[] emptySpotsPerArea;
	bool changeOccurred = true;
	
	float[] primes = {2f, 3f, 5f, 7f, 11f, 13f, 17f};
	
    // Start is called before the first frame update
    void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public bool Solve(int[,] solvedField, int[,] areaField, int[] areaSums)
	{
		int areaCount = areaSums.Length;
		emptySpotsPerArea = new int[areaCount];
		areaProducts = new float[areaCount];
		columnProducts = new float[7] {510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f};
		rowProducts = new float[7] {510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f};
		
		for(int i = 0; i < areaCount; i++)
		{
			areaProducts[i] = 510510f;
		}
		
		for(int i = 0; i < 7; i++)//compute emptySpotsPerArea and products
		{
			for(int j = 0; j < 7; j++)
			{
				if(solvedField[i, j] != -1)
				{
					areaProducts[areaField[i, j]] /= primes[solvedField[i, j] - 1];
					rowProducts[i] /= primes[solvedField[i, j] - 1];
					columnProducts[j] /= primes[solvedField[i, j] - 1];
				}
				else
				{
					emptySpotsPerArea[areaField[i, j]]++;
				}
			}
		}
		
		List<int[]> guessHistory = new List<int[]>();
		List<List<int[]>> derivationHistory = new List<List<int[]>>();
		List<int[]> dPosList = new List<int[]>();
		bool mistake = false;
		bool backtrack = true;
		
		while(true)//REEEEEEEEEEEEEEEEEEEEEEEEEEE
		{
			changeOccurred = true;
			
			if(!mistake)
			{
				dPosList = new List<int[]>();//list of the derivations for one single step/guess
				derivationHistory.Add(dPosList);
			}
			
			while(changeOccurred && !mistake)//derivations
			{
				Debug.Log("derivation phase");
				changeOccurred = false;
				mistake = false;
				
				if(!InsertLastNumbersForAreas(solvedField, areaField, areaSums, dPosList))
				{
					mistake = true;
					//derivationHistory.RemoveAt(derivationHistory.Count - 1);
					break;
				}
				if(!InsertLastNumbersForRowsColumns(solvedField, areaField, areaSums, dPosList))
				{
					mistake = true;
					//derivationHistory.RemoveAt(derivationHistory.Count - 1);
					break;
				}
			}
			
			if(mistake)//backtrack
			{
				Debug.Log("backtrack phase");
				while(backtrack)
				{
					int ghc = guessHistory.Count;
					backtrack = false;
					
					if(ghc == 0)//there is nothing to backtrack, thus the field is not solvable
					{
						Debug.Log("This field has no solutions");
						return false;
					}
					
					foreach(int[] dhpos in derivationHistory[derivationHistory.Count - 1])//undo the derivations
					{
						areaProducts[areaField[dhpos[0], dhpos[1]]] *= primes[solvedField[dhpos[0], dhpos[1]] - 1];
						rowProducts[dhpos[0]] *= primes[solvedField[dhpos[0], dhpos[1]] - 1];
						columnProducts[dhpos[0]] *= primes[solvedField[dhpos[0], dhpos[1]] - 1];
						solvedField[dhpos[0], dhpos[1]] = -1;
						emptySpotsPerArea[areaField[dhpos[0], dhpos[1]]]++;
					}
					
					derivationHistory.RemoveAt(derivationHistory.Count - 1);
					
					int x = guessHistory[ghc - 1][0];
					int y = guessHistory[ghc - 1][1];
					
					areaProducts[areaField[x, y]] *= primes[solvedField[x, y] - 1];
					rowProducts[x] *= primes[solvedField[x, y] - 1];
					columnProducts[y] *= primes[solvedField[x, y] - 1];
					
					if(solvedField[x, y] == 7)//this cant be incremented, backtrack further
					{
						Debug.Log("remove guess 170");
						guessHistory.RemoveAt(ghc - 1);
						backtrack = true;
					}
					else//try to increment the guess
					{
						bool numberFound = false;
						
						for(int k = solvedField[x, y]; k < 7; k++)
						{
							float quotient1 = areaProducts[areaField[x, y]] / primes[k];
							float quotient2 = rowProducts[x] / primes[k];
							float quotient3 = columnProducts[y] / primes[k];
							
							if(!fg.HasDecimals(quotient1) && !fg.HasDecimals(quotient2) && !fg.HasDecimals(quotient3) && DoesNumberFitArea(solvedField, areaField, areaSums, x, y))//k can be inserted here
							{
								Debug.Log("increment guess 186");
								solvedField[x, y] = k + 1;
								areaProducts[areaField[x, y]] /= primes[k];
								rowProducts[x] /= primes[k];
								columnProducts[y] /= primes[k];
								numberFound = true;
								emptySpotsPerArea[areaField[x, y]]--;
								break;
							}
						}
						
						if(!numberFound)//increment failed, remove guess and continue backtrack
						{
							Debug.Log("remove guess 198");
							guessHistory.RemoveAt(ghc - 1);
							backtrack = true;
						}
					}
				}
				
				backtrack = true;
				mistake = false;
			}
			else//continue guessing
			{
				Debug.Log("guess phase");
				bool spaceFound = false;
				
				for(int i = 0; i < 7; i++)//search the next free space
				{
					for(int j = 0; j < 7; j++)
					{
						if(solvedField[i, j] == -1)
						{
							spaceFound = true;
							bool numberFound = false;
							
							for(int k = 0; k < 7; k++)//find a fitting number
							{
								float quotient1 = areaProducts[areaField[i, j]] / primes[k];
								float quotient2 = rowProducts[i] / primes[k];
								float quotient3 = columnProducts[j] / primes[k];
								
								if(!fg.HasDecimals(quotient1) && !fg.HasDecimals(quotient2) && !fg.HasDecimals(quotient3) && DoesNumberFitArea(solvedField, areaField, areaSums, i, j))//k can be inserted here
								{
									Debug.Log("add guess 230");
									solvedField[i, j] = k + 1;
									areaProducts[areaField[i, j]] /= primes[k];
									rowProducts[i] /= primes[k];
									columnProducts[j] /= primes[k];
									guessHistory.Add(new int[2] {i, j});
									numberFound = true;
									emptySpotsPerArea[areaField[i, j]]--;
									break;
								}
							}
							
							if(!numberFound)//no number could fit in the space
							{
								mistake = true;
							}
							
							break;
						}
					}
					
					if(spaceFound)
					{
						break;
					}
				}
				
				if(!spaceFound)//all fields are filled
				{
					return true;
				}
			}
			
			fg.PrintNumberField(solvedField);
			Debug.Log("");
		}
	}
	
	bool InsertLastNumbersForAreas(int[,] solvedField, int[,] areaField, int[] areaSums, List<int[]> dPosList)
	{
		bool noMistake = true;
		int areaCount = areaSums.Length;
		List<int[]> changes = new List<int[]>();//add position where change occurred
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				int currentArea = areaField[i, j];
				
				if(emptySpotsPerArea[currentArea] == 1 && solvedField[i, j] == -1)//missing number can be calculated here
				{
					int tempSum = 0;
					
					for(int k = 0; k < 7; k++)
					{
						if(fg.HasDecimals(areaProducts[currentArea] / primes[k]))
						{
							tempSum += k + 1;
						}
					}
					
					int missingNumber = areaSums[currentArea] - tempSum;
					
					if(missingNumber <= 0 || missingNumber >= 8)
					{
						noMistake = false;
						changeOccurred = false;
						break;
					}
					
					float quotient1 = areaProducts[currentArea] / primes[missingNumber - 1];
					float quotient2 = rowProducts[i] / primes[missingNumber - 1];
					float quotient3 = columnProducts[j] / primes[missingNumber - 1];
					
					if(!fg.HasDecimals(quotient1) && !fg.HasDecimals(quotient2) && !fg.HasDecimals(quotient3))
					{
						changes.Add(new int[2] {i, j});
						solvedField[i, j] = missingNumber;
						areaProducts[areaField[i, j]] /= primes[missingNumber - 1];
						rowProducts[i] /= primes[missingNumber - 1];
						columnProducts[j] /= primes[missingNumber - 1];
						changeOccurred = true;
						emptySpotsPerArea[areaField[i, j]]--;
						dPosList.Add(new int[2] {i, j});
					}
					else//the missing number is already contained in the area/row/column
					{
						noMistake = false;
						changeOccurred = false;
						break;
					}
				}
			}
			
			if(!noMistake)
			{
				break;
			}
		}
		
		/*if(!noMistake)//undo changes if a mistake was found
		{
			foreach(int[] element in changes)
			{
				int number = solvedField[element[0], element[1]];
				areaProducts[areaField[element[0], element[1]]] *= primes[number - 1];
				rowProducts[element[0]] *= primes[number - 1];
				columnProducts[element[1]] *= primes[number - 1];
				solvedField[element[0], element[1]] = -1;
				emptySpotsPerArea[areaField[element[0], element[1]]]++;
			}
		}*/
		
		return noMistake;
	}
	
	bool InsertLastNumbersForRowsColumns(int[,] solvedField, int[,] areaField, int[] areaSums, List<int[]> dPosList)
	{
		bool noMistake = true;
		int areaCount = areaSums.Length;
		int[] emptySpotsPerRow = new int[7];
		int[] emptySpotsPerColumn = new int[7];
		List<int[]> changes = new List<int[]>();//add array of format (i, j, number)
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				if(solvedField[i, j] == -1)
				{
					emptySpotsPerRow[i]++;
					emptySpotsPerColumn[j]++;
				}
			}
		}
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				if(solvedField[i, j] == -1)
				{
					if(emptySpotsPerRow[i] == 1)
					{
						int missingNumber = 0;
						
						for(int k = 0; k < 7; k++)
						{
							if(!fg.HasDecimals(rowProducts[i] / primes[k]))
							{
								missingNumber = k + 1;
								break;
							}
						}
						
						float quotient1 = areaProducts[areaField[i, j]] / primes[missingNumber - 1];
						float quotient2 = columnProducts[j] / primes[missingNumber - 1];
						
						if(!fg.HasDecimals(quotient1) && !fg.HasDecimals(quotient2) && DoesNumberFitArea(solvedField, areaField, areaSums, i, j))
						{
							changes.Add(new int[2] {i, j});
							solvedField[i, j] = missingNumber;
							emptySpotsPerColumn[j]--;
							areaProducts[areaField[i, j]] /= primes[missingNumber - 1];
							rowProducts[i] /= primes[missingNumber - 1];
							columnProducts[j] /= primes[missingNumber - 1];
							changeOccurred = true;
							emptySpotsPerArea[areaField[i, j]]--;
							dPosList.Add(new int[2] {i, j});
						}
						else//the missing number is already contained in the area/column
						{
							noMistake = false;
							changeOccurred = false;
							break;
						}
					}
					else if(emptySpotsPerColumn[j] == 1)
					{
						int missingNumber = 0;
						
						for(int k = 0; k < 7; k++)
						{
							if(!fg.HasDecimals(columnProducts[j] / primes[k]))
							{
								missingNumber = k + 1;
								break;
							}
						}
						
						float quotient1 = areaProducts[areaField[i, j]] / primes[missingNumber - 1];
						float quotient2 = rowProducts[i] / primes[missingNumber - 1];
						
						if(!fg.HasDecimals(quotient1) && !fg.HasDecimals(quotient2) && DoesNumberFitArea(solvedField, areaField, areaSums, i, j))
						{
							changes.Add(new int[2] {i, j});
							solvedField[i, j] = missingNumber;
							emptySpotsPerRow[i]--;
							areaProducts[areaField[i, j]] /= primes[missingNumber - 1];
							rowProducts[i] /= primes[missingNumber - 1];
							columnProducts[j] /= primes[missingNumber - 1];
							changeOccurred = true;
							dPosList.Add(new int[2] {i, j});
							emptySpotsPerArea[areaField[i, j]]--;
						}
						else//the missing number is already contained in the area/row
						{
							noMistake = false;
							changeOccurred = false;
							break;
						}
					}
				}
			}
			
			if(!noMistake)
			{
				break;
			}
		}
		
		/*if(!noMistake)//undo changes if a mistake was found
		{
			foreach(int[] element in changes)
			{
				int number = solvedField[element[0], element[1]];
				areaProducts[areaField[element[0], element[1]]] *= primes[number - 1];
				rowProducts[element[0]] *= primes[number - 1];
				columnProducts[element[1]] *= primes[number - 1];
				solvedField[element[0], element[1]] = -1;
				emptySpotsPerArea[areaField[element[0], element[1]]]++;
			}
		}*/
		
		return noMistake;
	}
	
	bool CanNumberBeInserted(int[,] areaField, int i, int j, int number)
	{
		float quotient1 = areaProducts[areaField[i, j]] / primes[number - 1];
		float quotient2 = rowProducts[i] / primes[number - 1];
		float quotient3 = columnProducts[j] / primes[number - 1];
		bool res = fg.HasDecimals(quotient1) && fg.HasDecimals(quotient2) && fg.HasDecimals(quotient3);
		return res;
	}
	
	bool DoesNumberFitArea(int[,] numberField, int[,] areaField, int[] areaSums, int x, int y)
	{
		int currentArea = areaField[x, y];
		int number = numberField[x, y];
		int sum = 0;
		
		for(int k = 0; k < 7; k++)
		{
			if(fg.HasDecimals(areaProducts[currentArea] / primes[k]))
			{
				sum += k + 1;
			}
		}
		
		return areaSums[currentArea] - sum > number;
	}
}
