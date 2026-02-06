using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solver : MonoBehaviour
{
    public PrimeManager primeManager;
    public FieldGenerator fieldGenerator;
	public FieldRenderer fieldRenderer;
	
	float[] areaProducts;
	float[] rowProducts;
	float[] columnProducts;
	int[] emptySpotsPerArea;
	bool changeOccurred = true;
	
	int stateCount = 0;
	
	float[] primes = {2f, 3f, 5f, 7f, 11f, 13f, 17f};
	
	public bool Solve(int[,] solvedField, int[,] areaField, int[] areaSums, bool logging)
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
		
		List<Vector2Int> guessHistory = new List<Vector2Int>();
		List<List<Vector2Int>> derivationHistory = new List<List<Vector2Int>>();
		List<Vector2Int> dPosList = new List<Vector2Int>();
		bool mistake = false;
		bool backtrack = true;
		
		if(logging)
		{
			LogCurrentState(solvedField, guessHistory, derivationHistory);
			stateCount++;
		}
		
		while(true)//REEEEEEEEEEEEEEEEEEEEEEEEEEE
		{
			changeOccurred = true;
			
			if(!mistake)
			{
				dPosList = new List<Vector2Int>();//list of the derivations for one single step/guess
				derivationHistory.Add(dPosList);
			}
			
			while(changeOccurred && !mistake)//derivations
			{
				changeOccurred = false;
				mistake = false;
				
				if(!InsertLastNumbersForAreas(solvedField, areaField, areaSums, dPosList))
				{
					mistake = true;
					break;
				}
				if(!InsertLastNumbersForRowsColumns(solvedField, areaField, areaSums, dPosList))
				{
					mistake = true;
					break;
				}
			}
			
			if(mistake)//backtrack
			{
				while(backtrack)
				{
					int ghc = guessHistory.Count;
					backtrack = false;
					
					if(ghc == 0)//there is nothing to backtrack, thus the field is not solvable
					{
						Debug.Log("This field has no solutions");
						return false;
					}
					
					foreach(Vector2Int dhpos in derivationHistory[derivationHistory.Count - 1])//undo the derivations
					{
						areaProducts[areaField[dhpos.x, dhpos.y]] *= primes[solvedField[dhpos.x, dhpos.y] - 1];
						rowProducts[dhpos.x] *= primes[solvedField[dhpos.x, dhpos.y] - 1];
						columnProducts[dhpos.y] *= primes[solvedField[dhpos.x, dhpos.y] - 1];
						solvedField[dhpos.x, dhpos.y] = -1;
						emptySpotsPerArea[areaField[dhpos.x, dhpos.y]]++;
					}
					
					derivationHistory.RemoveAt(derivationHistory.Count - 1);
					
					int x = guessHistory[ghc - 1].x;
					int y = guessHistory[ghc - 1].y;
					
					areaProducts[areaField[x, y]] *= primes[solvedField[x, y] - 1];
					rowProducts[x] *= primes[solvedField[x, y] - 1];
					columnProducts[y] *= primes[solvedField[x, y] - 1];
					
					if(solvedField[x, y] == 7)//this cant be incremented, backtrack further
					{
						emptySpotsPerArea[areaField[x, y]]++;
						solvedField[x, y] = -1;
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
							
							if(!fieldGenerator.HasDecimals(quotient1) && !fieldGenerator.HasDecimals(quotient2) && !fieldGenerator.HasDecimals(quotient3) && DoesNumberFitArea(solvedField, areaField, areaSums, x, y))//k can be inserted here
							{
								solvedField[x, y] = k + 1;
								areaProducts[areaField[x, y]] /= primes[k];
								rowProducts[x] /= primes[k];
								columnProducts[y] /= primes[k];
								numberFound = true;
								break;
							}
						}
						
						if(!numberFound)//increment failed, remove guess and continue backtrack
						{
							emptySpotsPerArea[areaField[x, y]]++;
							solvedField[x, y] = -1;
							guessHistory.RemoveAt(ghc - 1);
							backtrack = true;
							
							if(logging)
							{
								LogCurrentState(solvedField, guessHistory, derivationHistory);
								stateCount++;
							}
						}
					}
				}
				
				backtrack = true;
				mistake = false;
			}
			else//continue guessing
			{
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
								
								if(!fieldGenerator.HasDecimals(quotient1) && !fieldGenerator.HasDecimals(quotient2) && !fieldGenerator.HasDecimals(quotient3) && DoesNumberFitArea(solvedField, areaField, areaSums, i, j))//k can be inserted here
								{
									solvedField[i, j] = k + 1;
									areaProducts[areaField[i, j]] /= primes[k];
									rowProducts[i] /= primes[k];
									columnProducts[j] /= primes[k];
									guessHistory.Add(new Vector2Int(i, j));
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
				
				if(!spaceFound)//all fields are filled, solution found
				{
					if(logging)
					{
						LogCurrentState(solvedField, guessHistory, derivationHistory);
						stateCount++;
					}
					
					return true;
				}
			}
			
			if(logging)
			{
				LogCurrentState(solvedField, guessHistory, derivationHistory);
				stateCount++;
			}
		}
	}
	
	public int SolveCount(int[,] solvedField, int[,] areaField, int[] areaSums, bool logging)//find out if there are mutliple solutions
	{
		int areaCount = areaSums.Length;
		emptySpotsPerArea = new int[areaCount];
		areaProducts = new float[areaCount];
		columnProducts = new float[7] {510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f};
		rowProducts = new float[7] {510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f, 510510.0f};
		bool solved = false;
		
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
		
		List<Vector2Int> guessHistory = new List<Vector2Int>();
		List<List<Vector2Int>> derivationHistory = new List<List<Vector2Int>>();
		List<Vector2Int> dPosList = new List<Vector2Int>();
		bool mistake = false;
		bool backtrack = true;
		
		if(logging)
		{
			LogCurrentState(solvedField, guessHistory, derivationHistory);
			stateCount++;
		}
		
		while(true)//REEEEEEEEEEEEEEEEEEEEEEEEEEE
		{
			changeOccurred = true;
			
			if(!mistake)
			{
				dPosList = new List<Vector2Int>();//list of the derivations for one single step/guess
				derivationHistory.Add(dPosList);
			}
			
			while(changeOccurred && !mistake)//derivations
			{
				changeOccurred = false;
				mistake = false;
				
				if(!InsertLastNumbersForAreas(solvedField, areaField, areaSums, dPosList))
				{
					mistake = true;
					break;
				}
				if(!InsertLastNumbersForRowsColumns(solvedField, areaField, areaSums, dPosList))
				{
					mistake = true;
					break;
				}
			}
			
			if(mistake)//backtrack
			{
				while(backtrack)
				{
					int ghc = guessHistory.Count;
					backtrack = false;
					
					if(ghc == 0)//there is nothing to backtrack, computation ended
					{
						if(solved)//a solution was already found earlier
						{
							return 1;
						}
						
						return 0;//there is no solution
					}
					
					foreach(Vector2Int dhpos in derivationHistory[derivationHistory.Count - 1])//undo the derivations
					{
						areaProducts[areaField[dhpos.x, dhpos.y]] *= primes[solvedField[dhpos.x, dhpos.y] - 1];
						rowProducts[dhpos.x] *= primes[solvedField[dhpos.x, dhpos.y] - 1];
						columnProducts[dhpos.y] *= primes[solvedField[dhpos.x, dhpos.y] - 1];
						solvedField[dhpos.x, dhpos.y] = -1;
						emptySpotsPerArea[areaField[dhpos.x, dhpos.y]]++;
					}
					
					derivationHistory.RemoveAt(derivationHistory.Count - 1);
					
					int x = guessHistory[ghc - 1].x;
					int y = guessHistory[ghc - 1].y;
					
					areaProducts[areaField[x, y]] *= primes[solvedField[x, y] - 1];
					rowProducts[x] *= primes[solvedField[x, y] - 1];
					columnProducts[y] *= primes[solvedField[x, y] - 1];
					
					if(solvedField[x, y] == 7)//this cant be incremented, backtrack further
					{
						emptySpotsPerArea[areaField[x, y]]++;
						solvedField[x, y] = -1;
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
							
							if(!fieldGenerator.HasDecimals(quotient1) && !fieldGenerator.HasDecimals(quotient2) && !fieldGenerator.HasDecimals(quotient3) && DoesNumberFitArea(solvedField, areaField, areaSums, x, y))//k can be inserted here
							{
								solvedField[x, y] = k + 1;
								areaProducts[areaField[x, y]] /= primes[k];
								rowProducts[x] /= primes[k];
								columnProducts[y] /= primes[k];
								numberFound = true;
								break;
							}
						}
						
						if(!numberFound)//increment failed, remove guess and continue backtrack
						{
							emptySpotsPerArea[areaField[x, y]]++;
							solvedField[x, y] = -1;
							guessHistory.RemoveAt(ghc - 1);
							backtrack = true;
							
							if(logging)
							{
								LogCurrentState(solvedField, guessHistory, derivationHistory);
								stateCount++;
							}
						}
					}
				}
				
				backtrack = true;
				mistake = false;
			}
			else//continue guessing
			{
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
								
								if(!fieldGenerator.HasDecimals(quotient1) && !fieldGenerator.HasDecimals(quotient2) && !fieldGenerator.HasDecimals(quotient3) && DoesNumberFitArea(solvedField, areaField, areaSums, i, j))//k can be inserted here
								{
									solvedField[i, j] = k + 1;
									areaProducts[areaField[i, j]] /= primes[k];
									rowProducts[i] /= primes[k];
									columnProducts[j] /= primes[k];
									guessHistory.Add(new Vector2Int(i, j));
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
				
				if(!spaceFound)//all fields are filled, solution found
				{
					if(solved)
					{
						if(logging)
						{
							LogCurrentState(solvedField, guessHistory, derivationHistory);
							stateCount++;
						}
						
						return 2;
					}
					
					solved = true;
					mistake = true;//continue searching for second solution
				}
			}
			
			if(logging)
			{
				LogCurrentState(solvedField, guessHistory, derivationHistory);
				stateCount++;
			}
		}
	}
	
	public void LogCurrentState(int[,] numberField, List<Vector2Int> guessHistory, List<List<Vector2Int>> derivationHistory)
	{
		int[,] temp = new int[7, 7];
		Color[,] tempSupp = new Color[7, 7];
		CopyField(numberField, temp);
		fieldRenderer.history.Add(temp);
		
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				tempSupp[i, j] = Color.black;
			}
		}
		
		foreach(Vector2Int pos in guessHistory)
		{
			tempSupp[pos.x, pos.y] = Color.red;
		}
		
		foreach(List<Vector2Int> sublist in derivationHistory)
		{
			foreach(Vector2Int pos in sublist)
			{
				tempSupp[pos.x, pos.y] = Color.blue;
			}
		}
		
		fieldRenderer.historySupport.Add(tempSupp);
	}
	
	void CopyField(int[,] source, int[,] target)
	{
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				target[i, j] = source[i, j];
			}
		}
	}
	
	bool InsertLastNumbersForAreas(int[,] solvedField, int[,] areaField, int[] areaSums, List<Vector2Int> dPosList)
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
						if(fieldGenerator.HasDecimals(areaProducts[currentArea] / primes[k]))
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
					
					if(!fieldGenerator.HasDecimals(quotient1) && !fieldGenerator.HasDecimals(quotient2) && !fieldGenerator.HasDecimals(quotient3))
					{
						changes.Add(new int[2] {i, j});
						solvedField[i, j] = missingNumber;
						areaProducts[areaField[i, j]] /= primes[missingNumber - 1];
						rowProducts[i] /= primes[missingNumber - 1];
						columnProducts[j] /= primes[missingNumber - 1];
						changeOccurred = true;
						emptySpotsPerArea[areaField[i, j]]--;
						dPosList.Add(new Vector2Int(i, j));
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
	
	bool InsertLastNumbersForRowsColumns(int[,] solvedField, int[,] areaField, int[] areaSums, List<Vector2Int> dPosList)
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
						int currentArea = areaField[i, j];
						
						for(int k = 0; k < 7; k++)
						{
							if(!fieldGenerator.HasDecimals(rowProducts[i] / primes[k]))
							{
								missingNumber = k + 1;
								break;
							}
						}
						
						float quotient1 = areaProducts[currentArea] / primes[missingNumber - 1];
						float quotient2 = columnProducts[j] / primes[missingNumber - 1];
						
						if(!fieldGenerator.HasDecimals(quotient1) && !fieldGenerator.HasDecimals(quotient2) && DoesNumberFitArea(solvedField, areaField, areaSums, i, j))
						{
							changes.Add(new int[2] {i, j});
							solvedField[i, j] = missingNumber;
							emptySpotsPerColumn[j]--;
							areaProducts[currentArea] /= primes[missingNumber - 1];
							rowProducts[i] /= primes[missingNumber - 1];
							columnProducts[j] /= primes[missingNumber - 1];
							changeOccurred = true;
							emptySpotsPerArea[currentArea]--;
							dPosList.Add(new Vector2Int(i, j));
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
						int currentArea = areaField[i, j];
						
						for(int k = 0; k < 7; k++)
						{
							if(!fieldGenerator.HasDecimals(columnProducts[j] / primes[k]))
							{
								missingNumber = k + 1;
								break;
							}
						}
						
						float quotient1 = areaProducts[currentArea] / primes[missingNumber - 1];
						float quotient2 = rowProducts[i] / primes[missingNumber - 1];
						
						if(!fieldGenerator.HasDecimals(quotient1) && !fieldGenerator.HasDecimals(quotient2) && DoesNumberFitArea(solvedField, areaField, areaSums, i, j))
						{
							changes.Add(new int[2] {i, j});
							solvedField[i, j] = missingNumber;
							emptySpotsPerRow[i]--;
							areaProducts[currentArea] /= primes[missingNumber - 1];
							rowProducts[i] /= primes[missingNumber - 1];
							columnProducts[j] /= primes[missingNumber - 1];
							changeOccurred = true;
							dPosList.Add(new Vector2Int(i, j));
							emptySpotsPerArea[currentArea]--;
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
	
	bool DoesNumberFitArea(int[,] numberField, int[,] areaField, int[] areaSums, int x, int y)
	{
		int currentArea = areaField[x, y];
		int number = numberField[x, y];
		int sum = 0;
		//int offset = emptySpotsPerArea[currentArea] - 1;//account for free spots which still have to be filled as well
		
		for(int k = 1; k <= 7; k++)
		{
			if(this.primeManager.IsNumberContainedInRowColumnArea(k, areaProducts[currentArea]))
            {
                sum += k;
            }
		}
		
		return areaSums[currentArea] - sum > number;//+ offset!
	}
}
