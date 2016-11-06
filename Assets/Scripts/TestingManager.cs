using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class TestingManager : MonoBehaviour {

    public BoardManager BoardManager;

    public Slider UpToDown;
    public Text UpToDownText;
    public Slider LeftToRight;
    public Text LeftToRightText;
    public Slider Level;
    public Text LevelText;

    void Start() {
        UpToDown.maxValue = BoardManager.BoardDimension - 1;
        LeftToRight.maxValue = BoardManager.BoardDimension - 1;
        Level.minValue = 1;
        Level.maxValue = BoardManager.ObjectMaterialsByLevel.Length;

        UpToDown.onValueChanged.AddListener((y) => UpToDownText.text = y.ToString());
        LeftToRight.onValueChanged.AddListener((x) => LeftToRightText.text = x.ToString());
        Level.onValueChanged.AddListener((l) => LevelText.text = l.ToString());

        UpToDownText.text = UpToDown.value.ToString();
        LeftToRightText.text = LeftToRight.value.ToString();
        LevelText.text = Level.value.ToString();
    }

    public void MakeNewButtonHandler() {
        BoardManager.CreateNewObjectAt((int) LeftToRight.value, (int) UpToDown.value, (int) Level.value);
    }
    
}
