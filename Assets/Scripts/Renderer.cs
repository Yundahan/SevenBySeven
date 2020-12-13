using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Renderer : MonoBehaviour
{
	public FieldGenerator fg;
	public AreaGenerator ag;
	public Solver s;
	
    // Start is called before the first frame update
    void Start()
    {
        int[,] numberField = fg.GenerateField(0f);
		int[,] areaField = ag.GenerateAreas(numberField, 0f);
		int[] areaSums = ag.CalcSums(numberField, areaField);
		
		fg.PrintNumberField(numberField);
		Debug.Log("");
		
		for(int i = 0; i < 7; i++)
		{
			numberField[i, 6] = -1;
			numberField[i, 5] = -1;
			numberField[i, 4] = -1;
			numberField[i, 3] = -1;
			numberField[i, 2] = -1;
		}
		
		fg.PrintNumberField(numberField);
		Debug.Log("");
		
		if(s.Solve(numberField, areaField, areaSums))
		{
			fg.PrintNumberField(numberField);
			Debug.Log("Successfully solved");
		}
		else
		{
			Debug.Log("Not solvable");
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
