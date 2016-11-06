using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Assertions;

public class ControlsManager : MonoBehaviour {

    public BoardManager BoardManager;

    // FINISHED
    void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            BoardManager.MoveAllObjectsInDirection(Direction.Up);
        } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            BoardManager.MoveAllObjectsInDirection(Direction.Left);
        } else if(Input.GetKeyDown(KeyCode.RightArrow)) {
            BoardManager.MoveAllObjectsInDirection(Direction.Right);
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            BoardManager.MoveAllObjectsInDirection(Direction.Down);
        }
    }

}
