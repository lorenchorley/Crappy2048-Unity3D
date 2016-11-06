using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Assertions;

public class BoardManager : MonoBehaviour {

    public UIManager UIManager;

    [Space]

    public bool CreateRandomOnStartup = false;
    public bool GamePaused = false;

    [Space]

    public int BoardDimension = 4;
    public Transform CenterBoardOn;
    public float BoardPositionWidth = 1;

    [Space]

    public GameObject BoardObjectPrefab;
    public GameObject BoardPositionPrefab;
    public Material[] ObjectMaterialsByLevel;

    private BoardPosition[,] Board;

    void Start() {
        Init();
        Reset();
    }

    // FINISHED
    public void Init() {

        Board = new BoardPosition[BoardDimension, BoardDimension];

        BoardPosition Position;

        // First pass to set all the board positions
        ForEachPosition((x, y) => {
            Position = GameObject.Instantiate<GameObject>(BoardPositionPrefab).GetComponent<BoardPosition>();
            Position.Init(x, y);
            Position.transform.position = CenterBoardOn.position + new Vector3(-(x - BoardDimension / 2), 0, y - BoardDimension / 2);
            Board[x, y] = Position;
        });

        BoardPosition[] Neighbours;

        // Second pass to set all the neighbours of the positions
        ForEachPosition((x, y) => {
            Neighbours = Board[x, y].Neighbours;
            if (y > 0)
                Neighbours[(int) Direction.Up] = Board[x, y - 1];
            if (y < BoardDimension - 1)
                Neighbours[(int) Direction.Down] = Board[x, y + 1];
            if (x > 0)
                Neighbours[(int) Direction.Left] = Board[x - 1, y];
            if (x < BoardDimension - 1)
                Neighbours[(int) Direction.Right] = Board[x + 1, y];
        });

    }

    // FINISHED
    public void Reset() {
        GamePaused = false;

        // Delete all objects
        ForEachPosition((x, y) => {
            BoardPosition position = Board[x, y];

            if (position.CurrentObject != null)
                DeleteExistingObject(position.CurrentObject);

        });

        if (CreateRandomOnStartup) {
            // Start with two objects
            NewRandomObject(1);
            NewRandomObject(2);
        }

    }

    // FINISHED
    // Returns whether the new object could be placed
    public bool NewRandomObject(int level) {

        // Get number of positions on board that don't have an object on them
        int unoccupied = GetNumberOfUnoccupiedPositions();

        // If there are none, then we're already done. The game's over
        if (unoccupied == 0)
            return false;

        // Otherwise generate a random number between 0 and one less than the number of unoccupied positions
        int countdown = UnityEngine.Random.Range(0, unoccupied);

        bool createdObject = false;
        ForEachPosition((x, y) => {
            BoardPosition Position = Board[x, y];

            // For each position on the board, either count down 1 or if the count down is finished, create the board object there
            if (Position.CurrentObject == null) {
                if (countdown == 0) {
                    CreateNewObjectIn(Position, level);

                    createdObject = true;
                    return false;
                } else {
                    countdown--;
                }
            }

            return true;
        });

        // Want to make sure that the object was created, otherwise there's a problem with the above logic
        if (!createdObject)
            throw new Exception("Oops, could not create object for bad bad reasons");

        return true;
    }

    // FINISHED
    public void CreateNewObjectAt(int x, int y, int level) {
        CreateNewObjectIn(Board[x, y], level);
    }

    // FINISHED
    public void CreateNewObjectIn(BoardPosition Position, int level) {
        if (GamePaused)
            return;

        Assert.IsTrue(level >= 1 && level <= ObjectMaterialsByLevel.Length, "Level is correct: " + level);

        // Create object and its connections
        BoardObject BoardObject = GameObject.Instantiate<GameObject>(BoardObjectPrefab).GetComponent<BoardObject>();
        BoardObject.BoardManager = this;
        BoardObject.CurrentPosition = Position;

        // Give it its material
        BoardObject.GetComponentInChildren<Renderer>().material = ObjectMaterialsByLevel[level - 1];

        // Give it its level value
        BoardObject.SetLevel(level);

        // Register the object with its position
        Position.SetObject(BoardObject);

        // Increment points by an appropriate amount
        UIManager.IncrementPointsBy(level);

    }

    // FINISHED
    public void DeleteExistingObject(BoardObject obj) {
        if (GamePaused)
            return;
        
        // Delete from position
        BoardPosition position = obj.CurrentPosition;
        position.CurrentObject = null;

        // Delete object from game
        Destroy(obj.gameObject);

    }

    // FINISHED
    public void IncreaseLevelOfExistingObject(BoardObject obj) {
        if (GamePaused)
            return;
        
        obj.SetLevel(obj.GetLevel() + 1);
        obj.GetComponentInChildren<Renderer>().material = ObjectMaterialsByLevel[obj.GetLevel() - 1];
    }

    // Testing
    public void MoveAllObjectsInDirection(int direction) {
        MoveAllObjectsInDirection((Direction) direction);
    }

    // FINISHED
    public void MoveAllObjectsInDirection(Direction direction) {
        if (GamePaused)
            return;

        // Move all the object
        bool movedSomething = false;
        ForEachPosition((x, y) => {
            if (Board[x, y].CurrentObject != null)
                movedSomething = Board[x, y].CurrentObject.MoveObjectAsFarAsPossibleInDirection(direction) || movedSomething;
        }, direction);

        if (movedSomething) { 
            // Make a new random object of a random level between 1 and 2
            bool CouldCreateNewObject = NewRandomObject(UnityEngine.Random.Range(1, 2));

            if (!CouldCreateNewObject) {
                UIManager.ShowLosingDialog();
            }

        } else {
            
            if (GetNumberOfUnoccupiedPositions() == 0) {
                UIManager.ShowLosingDialog();
            }

        }

    }

    // FINISHED
    public void ResolveMerger(BoardObject boardObject, Direction direction) {
        if (GamePaused)
            return;

        BoardPosition unmovingObjectsPosition = boardObject.CurrentPosition.Neighbours[(int) direction];
        Assert.IsNotNull(unmovingObjectsPosition, "There is a valid neighbouring position to merge with in the given direction");

        BoardObject unmovingObject = unmovingObjectsPosition.CurrentObject;
        Assert.IsNotNull(unmovingObject, "There is an object to merge with in the given direction");
        Assert.IsTrue(unmovingObject.CanMergeWith(boardObject), "Merger is possible");

        // Delete the moving object
        DeleteExistingObject(boardObject);

        // Increase the level of the unmoving object by one
        IncreaseLevelOfExistingObject(unmovingObject);

    }

    // FINISHED
    private int GetNumberOfUnoccupiedPositions() {
        int count = 0;
        ForEachPosition((i, j) => {
            if (Board[i, j].CurrentObject == null)
                count++;
        });
        return count;
    }



    // FINISHED
    private void ForEachPosition(Action<int, int> callback, Direction direction = Direction.Up) {
        // Define the x's to be from left to right and the y's to be from up to down (Checked!)

        switch (direction) {
        case Direction.Up:
        case Direction.Left:
            for (int x = 0; x < BoardDimension; x++) {
                for (int y = 0; y < BoardDimension; y++) {
                    callback(x, y); // Process from up to down and left to right
                }
            }
            break;
        case Direction.Down:
            for (int x = 0; x < BoardDimension; x++) {
                for (int y = 0; y < BoardDimension; y++) { 
                    callback(x, BoardDimension - y - 1); // Process from down to up
                }
            }
            break;
        case Direction.Right:
            for (int x = 0; x < BoardDimension; x++) {
                for (int y = 0; y < BoardDimension; y++) {
                    callback(BoardDimension - x - 1, y); // Process from right to left
                }
            }
            break;
        }

    }

    // FINISHED
    private void ForEachPosition(Func<int, int, bool> callback, Direction direction = Direction.Up) {
        // Define the x's to be from left to right and the y's to be from up to down (Checked!)

        switch (direction) {
        case Direction.Up:
        case Direction.Left:
            for (int x = 0; x < BoardDimension; x++) {
                for (int y = 0; y < BoardDimension; y++) {

                    // If the callback returns false, stop looping
                    if (callback(x, y) == false) // Process from up to down and left to right
                        return;

                }
            }
            break;
        case Direction.Down:
            for (int x = 0; x < BoardDimension; x++) {
                for (int y = 0; y < BoardDimension; y++) {

                    // If the callback returns false, stop looping
                    if (callback(x, BoardDimension - y - 1) == false) // Process from down to up
                        return;

                }
            }
            break;
        case Direction.Right:
            for (int x = 0; x < BoardDimension; x++) {
                for (int y = 0; y < BoardDimension; y++) {

                    // If the callback returns false, stop looping
                    if (callback(BoardDimension - x - 1, y) == false) // Process from right to left
                        return;

                }
            }
            break;
        }

    }

}
