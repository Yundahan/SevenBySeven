using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Renderer : MonoBehaviour
{
	public AreaGenerator ag;
	public Canvas canvas;
	
	public List<GameObject> fieldtexts = new List<GameObject>();
	public List<GameObject> gridlines = new List<GameObject>();
	public List<GameObject> areaObjs = new List<GameObject>();
	
	public List<int[,]> history = new List<int[,]>();
	public List<Color[,]> historySupport = new List<Color[,]>();
	public bool logger = true;
	int currentPos = 0;
	int savedPos = 0;
	
	public Material lineMaterial;
	
	float smallWidth = 0.03f;
	float bigWidth = 0.1f;
	
    // Start is called before the first frame update
    void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
		if(history.Count == 0 || !logger)//dont enable arrow keys if logger is disabled
		{
			return;
		}
		
        if(Input.GetKeyDown(KeyCode.E))
		{
			ClearNumberField();
			currentPos = 0;
			RenderNumberField(history[currentPos], historySupport[currentPos]);
		}
		else if(Input.GetKeyDown(KeyCode.T))
		{
			ClearNumberField();
			currentPos = history.Count - 1;
			RenderNumberField(history[currentPos], historySupport[currentPos]);
		}
        else if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			if(currentPos >= history.Count - 1)
			{
				return;
			}
			
			ClearNumberField();
			currentPos++;
			RenderNumberField(history[currentPos], historySupport[currentPos]);
		}
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
		{
			if(currentPos <= 0)
			{
				return;
			}
			
			ClearNumberField();
			currentPos--;
			RenderNumberField(history[currentPos], historySupport[currentPos]);
		}
		else if(Input.GetKeyDown(KeyCode.S))
		{
			savedPos = currentPos;
		}
		else if(Input.GetKeyDown(KeyCode.D))
		{
			ClearNumberField();
			currentPos = savedPos;
			RenderNumberField(history[currentPos], historySupport[currentPos]);
		}
		
		GameObject obj = new GameObject("peter");
		RenderText(obj, currentPos.ToString(), -1f, -3f, 30, Color.black);
		fieldtexts.Add(obj);
    }
	
	public void RenderNumberField(int[,] numberField)//render black text
	{
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				if(numberField[i, j] != -1)
				{
					GameObject textObj = new GameObject("fieldtext");
					RenderText(textObj, numberField[i, j].ToString(), (float)(j) + 0.5f, (float)(-i) - 0.5f, 50, Color.black);
					fieldtexts.Add(textObj);
				}
			}
		}
	}
	
	public void RenderNumberField(int[,] numberField, Color[,] colorField)//render colored text
	{
		for(int i = 0; i < 7; i++)
		{
			for(int j = 0; j < 7; j++)
			{
				if(numberField[i, j] != -1)
				{
					GameObject textObj = new GameObject("fieldtext");
					RenderText(textObj, numberField[i, j].ToString(), (float)(j) + 0.5f, (float)(-i) - 0.5f, 50, colorField[i, j]);
					fieldtexts.Add(textObj);
				}
			}
		}
	}
	
	public void RenderGrid(int[,] areaField)
	{
		for(int i = 0; i > -7; i--)
		{
			for(int j = 1; j < 7; j++)
			{
				GameObject lineObj1 = new GameObject("line");
				LineRenderer lry = lineObj1.AddComponent<LineRenderer>();
				
				lry.material = lineMaterial;
				lry.startColor = Color.black;
				lry.endColor = Color.black;
				
				lry.SetPosition(0, new Vector3((float)(j), (float)(i), 0f));
				lry.SetPosition(1, new Vector3((float)(j), (float)(i) - 1f, 0f));
				lry.shadowCastingMode = 0;
				lry.receiveShadows = false;
				gridlines.Add(lineObj1);
				
				if(areaField[-i, j - 1] == areaField[-i, j])
				{
					lry.widthMultiplier = smallWidth;
				}
				else
				{
					lry.widthMultiplier = bigWidth;
				}
				
				GameObject lineObj2 = new GameObject("line");
				LineRenderer lrx = lineObj2.AddComponent<LineRenderer>();
				
				lrx.material = lineMaterial;
				lrx.startColor = Color.black;
				lrx.endColor = Color.black;
				
				lrx.SetPosition(0, new Vector3((float)(-i), (float)(-j), 0f));
				lrx.SetPosition(1, new Vector3((float)(-i) + 1f, (float)(-j), 0f));
				lrx.shadowCastingMode = 0;
				lrx.receiveShadows = false;
				gridlines.Add(lineObj2);
				
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
	
	public void RenderAreaSums(int[,] areaField, int[] areaSums)
	{
		for(int i = 0; i < areaSums.Length; i++)
		{
			int[] pos = ag.FindPositionOfArea(areaField, i);
			GameObject textobj = new GameObject("textobj");
			RenderText(textobj, areaSums[i].ToString(), (float)(pos[1]) + 0.25f, (float)(-pos[0]) - 0.25f, 30, Color.black);
			areaObjs.Add(textobj);
		}
	}
	
	public void RenderText(GameObject obj, string text, float x, float y, int fontSize, Color color, float width = 100f, float height = 100f)
	{
		obj.transform.SetParent(canvas.transform);
		Text rentext = obj.AddComponent<Text>();
		rentext.text = text;
		rentext.alignment = TextAnchor.MiddleCenter;
		rentext.fontSize = fontSize;
		rentext.color = color;
		rentext.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		rentext.transform.localScale = new Vector3(1f, 1f, 1f);
		obj.transform.position = new Vector3(x, y, 0f);
		obj.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
	}
	
	public void ClearNumberField()
	{
		foreach(GameObject obj in fieldtexts)
		{
			Destroy(obj);
		}
		
		fieldtexts.Clear();
	}
	
	public void ClearGrid()
	{
		foreach(GameObject obj in gridlines)
		{
			Destroy(obj);
		}
		
		gridlines.Clear();
	}
	
	public void ClearAreaSums()
	{
		foreach(GameObject obj in areaObjs)
		{
			Destroy(obj);
		}
		
		areaObjs.Clear();
	}
	
	public bool GetLogger()
	{
		return logger;
	}
}
