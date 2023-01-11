using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BoardManager2D : MonoBehaviour
{
    public GameObject tilePrefab;
    GameObject[,] boardTiles;
    int x_sel = -1, y_sel = -1;
    public int boardLength = 13;
    public int boardHeight = 19;    

    Enums.DiceForm currentForm = Enums.DiceForm.cross_norm;
    int currentRotation = 0;
    int[][] currentCoords = {};
    // Start is called before the first frame update
    void Start()
    {
        this.transform.SetPositionAndRotation(new Vector3((boardLength/2) * 50, (boardHeight/2) * 50, 0), Quaternion.identity);
        SetupBoard();
    }

    // Update is called once per frame
    // TODO: This needs to be transported into the game manager
    void Update()
    {
        
    }
    //Setup a 13x19 big board
    void SetupBoard()
    {
        //Create the tiles via the prefab
        boardTiles = new GameObject[boardLength, boardHeight];
        //i column, j row
        for (int i = 0; i < boardLength; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                x_sel = -1;
                y_sel = -1;
                Vector3 position = new Vector3(i - boardLength/2, j - boardHeight/2, 0); 
                boardTiles[i, j] = Instantiate(tilePrefab, position, Quaternion.identity);
                boardTiles[i, j].name = "Tile " + (i) + " " + (j);
                boardTiles[i, j].GetComponent<Tile>().SetBoard(this);
                boardTiles[i, j].transform.SetParent(this.transform);
                boardTiles[i, j].GetComponent<Tile>().SetCoordinates(i, j);
            }
            Debug.Log("Finished row " + (i));
        }
        //place the commanders on their fields
        Debug.Log("Placing Commanders");
        PlacePiece(((int)boardLength/2), 0, GameManager.GetCommander(1));
        SetAffiliation(((int)boardLength/2), 0, 1);

        PlacePiece(((int)boardLength/2), boardHeight - 1, GameManager.GetCommander(2));
        SetAffiliation(((int)boardLength/2), boardHeight - 1, 2);
    }

    //Sets highlight to 1 for the current coordinates
    public void HighlightDice() {
        foreach(int[] tileCoords in currentCoords) {
            if(tileCoords[0] < 0 || tileCoords[0] >= boardLength || tileCoords[1] < 0 || tileCoords[1] >= boardHeight) {
                throw new IndexOutOfRangeException("Indexes for DiceForm are not on the board");
            }
        }
        foreach(int[] tileCoords in currentCoords) {
            boardTiles[tileCoords[0], tileCoords[1]].GetComponent<Tile>().highlight = 1;
        }
    }
    //Sets highlight to 1 for the current tile
    public void HighlightTile() {
        if(x_sel != -1  || y_sel != -1) {
                boardTiles[x_sel, y_sel].GetComponent<Tile>().highlight = 1;
            }
    }
    //find a better name
    //sets highlight to 0 for the current coordinates
    public void UnShowDice() {
        foreach(int[] tileCoords in currentCoords) {
            if(tileCoords[0] < 0 || tileCoords[0] >= boardLength || tileCoords[1] < 0 || tileCoords[1] >= boardHeight) {
                //throw new IndexOutOfRangeException("Indexes for DiceForm are not on the board");
                return;//Nothing to throw if there is nothing to unshow
            }
        }
        foreach(int[] tileCoords in currentCoords) {
            boardTiles[tileCoords[0], tileCoords[1]].GetComponent<Tile>().highlight = 0;
        }
    }

    //Place a dice form on the board with current variables
    public bool PlaceDice(int team) {
        return PlaceDice(x_sel, y_sel, currentForm, team, currentRotation);
    }
    //Place a dice form on the board
    //This only works if any of the adjacent tiles already has that affiliation
    bool PlaceDice(int x, int y, Enums.DiceForm form, int team, int rotation = 0) {
        int[][] formCoords = GetFormCoords(x, y, form, rotation);
        foreach(int[] coords in formCoords) {
            if(coords[0] < 0 || coords[0] >= boardLength) { // x out of range
                return false;
            }
            if(coords[1] < 0 || coords[1] >= boardHeight) { // y out of range
                return false;
            }
            if(GetAffiliation(coords[0], coords[1]) != 0) { // a block is already set
                return false;
            }
        }
        foreach(int[] coords in formCoords) { //check for adjacent affiliated tile
            if(GetAffiliation(coords[0] - 1, coords[1]) == team 
                        || GetAffiliation(coords[0], coords[1] - 1) == team 
                        || GetAffiliation(coords[0] + 1, coords[1]) == team 
                        || GetAffiliation(coords[0], coords[1] + 1) == team) {
                SetAffiliation(formCoords, team);
                return true;
            }
        }
        return false;
    }
    //Place a piece onto the board
    //Expects x and y values from 1 to 19/13
    public bool PlacePiece(int x, int y, Piece piece)
    {
        Tile tile = boardTiles[x, y].GetComponent<Tile>();
        if (tile.HasPiece())
        {
            return false;
        }
        Debug.Log("placing piece");
        tile.PlacePiece(piece);
        return true;
    }
    //Set which team a number of fields is assigned to
    bool SetAffiliation(int[][] points,int team, bool force = true) {
        //check if all points are available
        if(!force) {
            foreach(int[] point in points) {
                if (GetAffiliation(point[0], point[1]) != 0)
                {
                    return false;
                }
            }
        }
        //set affiliation on all points
        foreach(int[] point in points) {
            SetAffiliation(point[0], point[1], team, force);    
        }
        return true;
    }

    //Set the form to the next one
    public bool NextForm() {
        currentForm = Enums.GetNextEnumValueOf(currentForm);
        return true;
    }

    //Set the rotation to the next one
    public bool NextRotation(int direction = 1) {
        currentRotation = (currentRotation + direction) % 4;
        if(currentRotation < 0) {
            currentRotation += 4;
        }
        return true;
    }
    //Set which team a field is assigned to
    bool SetAffiliation(int x, int y, int team, bool force = true)
    {
        if (!force && GetAffiliation(x, y) != 0)
        {
            //Affiliation already set
            return false;
        }
        boardTiles[x, y].GetComponent<Tile>().SetAffiliation(team);
        return true;
    }
    //set the selection
    public bool SetSelection(int x, int y) {
        x_sel = x;
        y_sel = y;
        return true;
    } 
    //Set the coordinates of the current form-selection according to the variables saved in the boardmanager
    public bool SetCurrentCoords() {
        currentCoords = GetFormCoords(x_sel, y_sel, currentForm, currentRotation);
        return true;
    }
    //Set the coordinates of the current form-selection
    public bool SetCurrentCoords(int[][] coords) {
        this.currentCoords = coords;
        return true;
    }

    //Get the affiliation of the tile at x y
    //returns -1 if the index is out of range
    public int GetAffiliation(int x, int y)
    {
        try {
            return boardTiles[x, y].GetComponent<Tile>().GetAffiliation();
        } catch(IndexOutOfRangeException e) {
            return -1;
        }
    }

    //Get the coordinates of a form corresponding to the form and given coordinates
    int[][] GetFormCoords(int x, int y, Enums.DiceForm form, int rotation) {
        //each form has different coordinates to check
        //sine and cosine rotate the coordinates, just simple arrays, no hard functions needed
        int[] sin = {0, 1, 0, -1};
        int[] cos = {1, 0, -1, 0};
        try {
            switch(form) {
                case Enums.DiceForm.cross_norm: {
                        int[][] points = {new int[] {x, y},                                           // 0  0
                                        new int[] {x - sin[rotation], y + cos[rotation]},             // 0  1
                                        new int[] {x + sin[rotation], y - cos[rotation]},             // 0 -1
                                        new int[] {x + 2 * sin[rotation], y - 2 * cos[rotation]},     // 0 -2
                                        new int[] {x + cos[rotation], y + sin[rotation]},             // 1  0
                                        new int[] {x - cos[rotation], y - sin[rotation]} };           //-1  0
                        return points;
                        }
                case Enums.DiceForm.t_norm: {
                        int[][] points = {new int[] {x, y},                                                               // 0  0
                                        new int[] {x + sin[rotation], y + cos[rotation]},                                 // 0  1
                                        new int[] {x - sin[rotation], y - cos[rotation]},                                 // 0 -1
                                        new int[] {x - sin[rotation] - cos[rotation], y - sin[rotation] + cos[rotation]}, //-1  1
                                        new int[] {x - sin[rotation] + cos[rotation], y + sin[rotation] + cos[rotation]}, // 1  1
                                        new int[] {x + 2 * sin[rotation], y - 2 * cos[rotation]} };                       // 0 -2
                        return points;
                        }
                case Enums.DiceForm.hand_norm: {
                        int[][] points = {new int[] {x, y},                                                               // 0  0
                                        new int[] {x - cos[rotation], y - sin[rotation]},                                 //-1  0
                                        new int[] {x - cos[rotation] + sin[rotation], y - cos[rotation] - sin[rotation]}, //-1 -1
                                        new int[] {x - sin[rotation], y + cos[rotation]},                                 // 0  1
                                        new int[] {x - sin[rotation] + cos[rotation], y + sin[rotation] + cos[rotation]}, // 1  1
                                        new int[] {x - 2 * sin[rotation], y + 2 * cos[rotation]} };                       // 0  2
                        return points;
                        }
                case Enums.DiceForm.hand_rev: {
                        int[][] points = {new int[] {x, y},                                                               // 0  0
                                        new int[] {x + cos[rotation], y + sin[rotation]},                                 // 1  0
                                        new int[] {x + cos[rotation] + sin[rotation], y - cos[rotation] + sin[rotation]}, // 1 -1
                                        new int[] {x - sin[rotation], y + cos[rotation]},                                 // 0  1
                                        new int[] {x - cos[rotation] - sin[rotation], y + cos[rotation] - sin[rotation]}, //-1  1
                                        new int[] {x - 2 * sin[rotation], y + 2 * cos[rotation]} };                       // 0  2
                        return points;
                        }
                case Enums.DiceForm.z_norm: { 
                        int[][] points = {new int[] {x, y},                                                                           // 0  0
                                        new int[] {x + sin[rotation], y - cos[rotation]},                                             // 0 -1
                                        new int[] {x - cos[rotation] + sin[rotation], y - cos[rotation] - sin[rotation]},             //-1 -1
                                        new int[] {x - sin[rotation], y + cos[rotation]},                                             // 0  1
                                        new int[] {x - 2 * sin[rotation], y + 2 * cos[rotation]},                                     // 0  2
                                        new int[] {x + cos[rotation] - 2 * sin[rotation], y + 2 * cos[rotation] + sin[rotation] } };  // 1  2
                        return points;
                        }
                case Enums.DiceForm.z_rev: { 
                        int[][] points = {new int[] {x, y},                                                                           // 0  0
                                        new int[] {x + sin[rotation], y - cos[rotation]},                                             // 0 -1
                                        new int[] {x + cos[rotation] + sin[rotation], y - cos[rotation] + sin[rotation]},             // 1 -1
                                        new int[] {x - sin[rotation], y + cos[rotation]},                                             // 0  1
                                        new int[] {x - 2 * sin[rotation], y + 2 * cos[rotation]},                                     // 0  2
                                        new int[] {x - cos[rotation] - 2 * sin[rotation], y + 2 * cos[rotation] - sin[rotation] } };  //-1  2
                        return points;
                        }
                case Enums.DiceForm.stairs_norm: { 
                        int[][] points = {new int[] {x, y},                                                                           // 0  0
                                        new int[] {x - cos[rotation], y - sin[rotation]},                                             //-1  0
                                        new int[] {x - cos[rotation] + sin[rotation], y - cos[rotation] - sin[rotation]},             //-1 -1
                                        new int[] {x - sin[rotation], y + cos[rotation]},                                             // 0  1
                                        new int[] {x + cos[rotation] - sin[rotation], y + cos[rotation] + sin[rotation]},             // 1  1
                                        new int[] {x + cos[rotation] - 2 * sin[rotation], y + 2 * cos[rotation] + sin[rotation]} };   // 1  2
                        return points;
                        }
                case Enums.DiceForm.stairs_rev: {
                        int[][] points = {new int[] {x, y},                                                                           // 0  0
                                        new int[] {x + cos[rotation], y + sin[rotation]},                                             // 1  0
                                        new int[] {x + cos[rotation] + sin[rotation], y - cos[rotation] + sin[rotation]},             // 1 -1
                                        new int[] {x - sin[rotation], y + cos[rotation]},                                             // 0  1
                                        new int[] {x - cos[rotation] - sin[rotation], y + cos[rotation] - sin[rotation]},             //-1  1
                                        new int[] {x - cos[rotation] - 2 * sin[rotation], y + 2 * cos[rotation] - sin[rotation]} };   //-1  2
                        return points;
                        }
                case Enums.DiceForm.line_norm: {
                        int[][] points = {new int[] {x, y},                                                                           // 0  0
                                        new int[] {x + sin[rotation], y - cos[rotation]},                                             // 0 -1
                                        new int[] {x + 2 * sin[rotation], y - 2 * cos[rotation]},                                     // 0 -2
                                        new int[] {x - cos[rotation], y - sin[rotation]},                                             //-1  0
                                        new int[] {x - cos[rotation] - sin[rotation], y + cos[rotation] - sin[rotation]},             //-1  1
                                        new int[] {x - cos[rotation] - 2 * sin[rotation], y + 2 * cos[rotation] - sin[rotation]} };   //-1  2
                        return points;
                        }
                case Enums.DiceForm.line_rev: { 
                        int[][] points = {new int[] {x, y},                                                                           // 0  0
                                        new int[] {x + sin[rotation], y - cos[rotation]},                                             // 0 -1
                                        new int[] {x + 2 * sin[rotation], y - 2 * cos[rotation]},                                     // 0 -2
                                        new int[] {x + cos[rotation], y + sin[rotation]},                                             // 1  0
                                        new int[] {x + cos[rotation] - sin[rotation], y + cos[rotation] + sin[rotation]},             // 1  1
                                        new int[] {x + cos[rotation] - 2 * sin[rotation], y + 2 * cos[rotation] + sin[rotation]} };   // 1  2
                        return points;
                        }
                default: 
                    return null;
            }
        } catch(IndexOutOfRangeException e) {
            return null;
        }
    }
}
