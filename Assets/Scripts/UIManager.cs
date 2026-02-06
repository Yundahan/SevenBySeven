using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public FieldGenerator fieldGenerator;
	public AreaGenerator areaGenerator;
	public Solver solver;
	public FieldRenderer fieldRenderer;
	public ClueGenerator cluegenerator;
	
	public Button generateButton;
	
    // Start is called before the first frame update
    void Start()
    {
        Button btn = generateButton.GetComponent<Button>();
        btn.onClick.AddListener(ClickGenerateButton);
    }
	
	void ClickGenerateButton()
    {
		int[,] numberField = fieldGenerator.GenerateField(0f);
		int[,] areaField = areaGenerator.GenerateAreas(numberField, 0f);
		int[] areaSums = areaGenerator.CalcSums(numberField, areaField);
		cluegenerator.InitialRemovals(numberField, areaField, areaSums.Length);
		cluegenerator.RemovalLoop(numberField, areaField, areaSums);
        fieldRenderer.ClearNumberField();
        fieldRenderer.ClearGrid();
        fieldRenderer.ClearAreaSums();
        fieldRenderer.RenderNumberField(numberField);
        fieldRenderer.RenderGrid(areaField);
        fieldRenderer.RenderAreaSums(areaField, areaSums);
		solver.Solve(numberField, areaField, areaSums, true);//for the history
    }
}
