using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System;
using UnityEngine.UI;

public class BoardObject : MonoBehaviour {

    public BoardManager BoardManager;
    public BoardPosition CurrentPosition;

    private int level;
    public Text Caption;

    // FINISHED
    // Just moves in a certain direction.
    public void MoveInDirection(Direction direction) {
        
        // Get the target position
        BoardPosition Position = CurrentPosition.Neighbours[(int) direction];

        // If it exists, go there
        if (Position != null)
            MoveToPosition(Position);

    }

    // FINISHED
    // Just moves to a certain position. Does a few checks, but ultimately does nothing about them.
    // It's the responsibility of the caller to make sure that this is used correctly
    public void MoveToPosition(BoardPosition NewPosition) {

        // Make sure that all the things are good and proper!
        Assert.IsTrue(CurrentPosition.CurrentObject == this, "Current position has this object");
        Assert.IsTrue(NewPosition.CurrentObject == null, "New position has no object");
        Assert.IsTrue(CurrentPosition != NewPosition, "Trying to move to a different possible");

        CurrentPosition.CurrentObject = null;
        NewPosition.CurrentObject = this;
        CurrentPosition = NewPosition;

        transform.SetParent(NewPosition.transform);
        transform.localPosition = Vector3.zero;

    }

    // FINISHED
    public void MoveObjectAsFarAsPossibleInDirection(int direction) {
        MoveObjectAsFarAsPossibleInDirection((Direction) direction);
    }

    // FINISHED
    // Returns whether the object was moved
    public bool MoveObjectAsFarAsPossibleInDirection(Direction direction) {
        MovementResult status;
        bool moved = false;

        // Move until we can move no more
        status = CanMoveInDirection(direction);
        while (status == MovementResult.AllowedNoCollision) {
            moved = true;
            MoveInDirection(direction);
            status = CanMoveInDirection(direction);
        }

        // In the case that the movement finished with a collision
        if (status == MovementResult.AllowedWithCollision) {
            moved = true;
            BoardManager.ResolveMerger(this, direction);
        }

        return moved;
    }

    // FINISHED
    public MovementResult CanMoveInDirection(Direction direction) {
        BoardPosition Position = CurrentPosition.Neighbours[(int) direction];

        if (Position == null) {

            // Edge of the board
            return MovementResult.NotAllowed;

        } else if (Position.CurrentObject == null) {

            // Position with no object
            return MovementResult.AllowedNoCollision;

        } else if (Position.CurrentObject.CanMergeWith(this)) {

            // Position with object, but merging possible
            return MovementResult.AllowedWithCollision;

        } else {

            // Position with object, and merging not possible
            return MovementResult.NotAllowed;

        }

    }

    // FINISHED
    // Can merge with another object if they are the same level
    public bool CanMergeWith(BoardObject other) {
        return level == other.level;
    }

    // FINISHED
    public void SetLevel(int level) {
        this.level = level;
        Caption.text = Mathf.Pow(2, level - 1).ToString();
    }

    // FINISHED
    public int GetLevel() {
        return level;
    }

}
