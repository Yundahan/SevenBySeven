using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Renderer : MonoBehaviour
{
	public FieldGenerator fg;
	public AreaGenerator ag;
	public Solver s;
	public Canvas canvas;
	
	float smallWidth = 0.0f;
	float bigWidth = 0.1f;
	
    // Start is called before the first frame update
    void Start()
    {
        int[,] numberField = fg.GenerateField(0f);
		int[,] areaField = ag.GenerateAreas(numberField, 0f);
		int[] areaSums = ag.CalcSums(numberField, areaField);
		
		fg.PrintNumberField(areaField);
		Debug.Log("");
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
		
		RenderGrid(areaField);
		RenderAreaSums(areaField, areaSums);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void RenderGrid(int[,] areaField)
	{
		for(int i = 0; i > -7; i--)
		{
			for(int j = 1; j < 7; j++)
			{
				LineRenderer lry = (new GameObject("line")).AddComponent<LineRenderer>();
				lry.material.SetColor("_Color", Color.black);
				lry.SetPosition(0, new Vector3((float)(j), (float)(i), 0f));
				lry.SetPosition(1, new Vector3((float)(j), (float)(i) - 1f, 0f));
				lry.shadowCastingMode = 0;
				lry.receiveShadows = false;
				
				if(areaField[-i, j - 1] == areaField[-i, j])
				{
					lry.widthMultiplier = smallWidth;
				}
				else
				{
					lry.widthMultiplier = bigWidth;
				}
				
				LineRenderer lrx = (new GameObject("line")).AddComponent<LineRenderer>();
				lrx.material.SetColor("_Color", Color.black);
				lrx.SetPosition(0, new Vector3((float)(-i), (float)(-j), 0f));
				lrx.SetPosition(1, new Vector3((float)(-i) + 1f, (float)(-j), 0f));
				lrx.shadowCastingMode = 0;
				lrx.receiveShadows = false;
				
				if(areaField[j - 1, -i] == areaField[j, -i])
				{
					lrx.widthMultiplier = smallWidth;
				}
				else
				{
					lrx.widthMultiplier = bigWidth;
				}
			}
		}
	}
	
	void RenderAreaSums(int[,] areaField, int[] areaSums)
	{
		for(int i = 0; i < areaSums.Length; i++)
		{
			int[] pos = ag.FindPositionOfArea(areaField, i);
			GameObject textobj = new GameObject("textobj");
			textobj.transform.SetParent(canvas.transform);
			Text rentext = textobj.AddComponent<Text>();
			rentext.text = areaSums[i].ToString();
			rentext.fontSize = 30;
			rentext.color = Color.black;
			rentext.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
			rentext.transform.localScale = new Vector3(1f, 1f, 1f);
			textobj.transform.position = new Vector3((float)(pos[1]) + 0.6f, (float)(-pos[0]) - 0.6f, 0f);
		}
	}
}
