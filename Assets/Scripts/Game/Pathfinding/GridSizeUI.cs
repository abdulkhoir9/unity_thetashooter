using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridSizeUI : MonoBehaviour
{
    private TMP_Text _gridSizeText;

    private void Awake()
    {
        _gridSizeText = GetComponent<TMP_Text>();    
    }

    private void Start()
    {
        UpdateGridSizeText();
    }

    public void UpdateGridSizeText()
    {
        int gridWidth = GridMap.grid.GetLength(0);
        int gridHeight = GridMap.grid.GetLength(1);

        _gridSizeText.text = $"Grid size: {gridWidth} x {gridHeight} ";
    }
}
