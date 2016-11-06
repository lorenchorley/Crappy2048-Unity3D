using UnityEngine;
using System.Collections;

public class BoardPosition : MonoBehaviour {

    public BoardPosition[] Neighbours;

    public BoardObject CurrentObject;
    public int x;
    public int y;

    // FINISHED
    public void Init(int x, int y) {
        this.x = x;
        this.y = y;

        Neighbours = new BoardPosition[4];
    }

    // FINISHED
    public void SetObject(BoardObject CurrentObject) {
        this.CurrentObject = CurrentObject;

        // Put inside this position
        CurrentObject.transform.SetParent(transform);
        CurrentObject.transform.localPosition = Vector3.zero;
    }

}
