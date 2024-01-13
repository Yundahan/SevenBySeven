using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public FieldGenerator fg;
	public AreaGenerator ag;
	public Solver s;
	public Renderer r;
	public ClueGenerator cg;
	
	public Button generateButton;
	
    // Start is called before the first frame update
    void Start()
    {
        Button btn = generateButton.GetComponent<Button>();
        btn.onClick.AddListener(ClickGenerateButton);
    }

    // Update is called once per frame
    void Update()
    {
		
    }
	
	void ClickGenerateButton()
    {
		int[,] numberField = new int[7, 7] {
            { 7, 5, 3, 6, 4, 1, 2 },
			{ 3, 7, 1, 5, 2, 6, 4 },
			{ 2, 1, 4, 7, 3, 5, 6 },
			{ 6, 4, 7, 2, 1, 3, 5 },
			{ 4, 6, 2, 1, 5, 7, 3 },
			{ 5, 3, 6, 4, 7, 2, 1 },
			{ 1, 2, 5, 3, 6, 4, 7 }
        };
        int[,] areaField = new int[7, 7] {
            { 0, 1, 2, 2, 3, 3, 3 },
            { 1, 1, 1, 2, 2, 3, 4 },
            { 5, 5, 6, 2, 4, 4, 4 },
            { 5, 7, 6, 6, 4, 8, 9 },
            { 5, 7, 10, 6, 11, 9, 9 },
            { 12, 13, 10, 10, 11, 11, 9 },
            { 13, 13, 13, 14, 11, 11, 15 }
        };
        int[,] numberUnsolvedField = new int[7, 7] {
            { -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1 },
            { -1, 1, -1, -1, -1, -1, -1 },
            { 6, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1 },
            { -1, -1, -1, -1, -1, -1, -1 }
        };

        int[] areaSums = ag.CalcSums(numberField, areaField);

        int b = s.SolveCount(numberUnsolvedField, areaField, areaSums, true);
        Debug.Log(b);



        for (int i = 0; i < 7; i++)
		{
			String str = "";
			for (int j = 0; j < 7; j++)
			{
				str = str + numberUnsolvedField[i,j].ToString() + " ";
			}

			Debug.Log(str);
		}

		cg.InitialRemovals(numberField, areaField, areaSums.Length);
		cg.RemovalLoop(numberField, areaField, areaSums);
		r.ClearNumberField();
		r.ClearGrid();
		r.ClearAreaSums();
		r.RenderNumberField(numberUnsolvedField);
		r.RenderGrid(areaField);
		r.RenderAreaSums(areaField, areaSums);
		s.Solve(numberField, areaField, areaSums, true);
    }
}
