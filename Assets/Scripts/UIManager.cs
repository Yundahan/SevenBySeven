using System.Collections;
using System.Collections.Generic;
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
		int[,] numberField = fg.GenerateField(0f);
		int[,] areaField = ag.GenerateAreas(numberField, 0f);
		int[] areaSums = ag.CalcSums(numberField, areaField);
		cg.InitialRemovals(numberField, areaField, areaSums.Length);
		cg.RemovalLoop(numberField, areaField, areaSums);
		r.ClearNumberField();
		r.ClearGrid();
		r.ClearAreaSums();
		r.RenderNumberField(numberField);
		r.RenderGrid(areaField);
		r.RenderAreaSums(areaField, areaSums);
		s.Solve(numberField, areaField, areaSums, true);
    }
}
