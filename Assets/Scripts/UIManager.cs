using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Assertions;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIManager : MonoBehaviour {

    public BoardManager BoardManager;

    [Space]

    public GameObject LosingDialog;
    public Text PointCounter;

    [Space]

    public int Points = 0;

    void Start() {
        LosingDialog.SetActive(false);
    }

    // FINISHED
    public void ShowLosingDialog() {
        BoardManager.GamePaused = true;
        LosingDialog.SetActive(true);
    }

    // FINISHED
    public void IncrementPointsBy(int points) {
        Points += points;
        PointCounter.text = Points.ToString();
    }

    // FINISHED
    public void NewGame() {
        BoardManager.Reset();
        LosingDialog.SetActive(false);
    }

    // FINISHED
    public void Quit() {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
