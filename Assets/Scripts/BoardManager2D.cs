using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BoardManager2D : MonoBehaviour
{
    public Camera camera;
    public GameObject boardPrefab;
    GameObject boardCanvas;
    public GameObject tilePrefab;
    GameObject[,] boardTiles;
    
    public int x_sel = -1, y_sel = -1;
    public int boardLength = 13;
    public int boardHeight = 19;

    //boolean to block the switch action (change form or rotation)
    bool blockSwitch = false;
    const float WAITTIME = 0.25f;
    float waitCounter = WAITTIME;

    Enums.DiceForm currentForm = Enums.DiceForm.cross_norm;
    int currentRotation = 0;
    int[][] currentCoords = {};
    // Start is called before the first frame update
    void Start()
    {
        this.transform.SetPositionAndRotation(new Vector3((boardLength/2) * 50, (boardHeight/2) * 50, 0), Quaternion.identity);
        SetupBoard();
        PlaceDice(3, 3, Enums.DiceForm.line_norm, 1, 0);
        PlaceDice(10, 3, Enums.DiceForm.line_norm, 2, 1);
        PlaceDice(3, 10, Enums.DiceForm.line_norm, 1, 2);
        PlaceDice(10, 10, Enums.DiceForm.line_norm, 2, 3);
    }

    // Update is called once per frame
    void Update()
    {
        UnShowDice();
        currentCoords = GetFormCoords(x_sel, y_sel, currentForm, currentRotation);
        try{
            ShowDice();
        } catch (IndexOutOfRangeException e) {
            if(x_sel != -1  || y_sel != -1) {
                boardTiles[x_sel, y_sel].GetComponent<Tile>().highlight = 1;
            }
        }
        //input to change form and rotation
        if(waitCounter > 0 && blockSwitch) {
            waitCounter -= Time.deltaTime;
        } else if (waitCounter <= 0){
            blockSwitch = false;
        }
        if (Input.GetKey(KeyCode.Tab) && !blockSwitch) {
            currentForm = Enums.GetNextEnumValueOf(currentForm);
            waitCounter = WAITTIME;
            blockSwitch = true;
        }
        if (Input.GetKey("q") && !blockSwitch) {
            currentRotation = (currentRotation + 1) % 4;
            waitCounter = WAITTIME;
            blockSwitch = true;
        }
        
        
    }
    //Setup a 13x19 big board
    async void SetupBoard()
    {
        if (boardCanvas != null)
        {
            Destroy(boardCanvas);
        }
        //Create new board with canvas to display the tiles
        boardCanvas = Instantiate(boardPrefab);
        boardCanvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        boardCanvas.GetComponent<Canvas>().worldCamera = camera;

        //this position just works, might need to learn why, expecting it to go the other
        boardCanvas.transform.SetPositionAndRotation(new Vector3(((int)boardLength/2) * 50, ((int)boardHeight/2) * 50, 0), Quaternion.identity);

        //Create the tiles via the prefab
        boardTiles = new GameObject[boardLength, boardHeight];
        //i row, j column
        for (int i = 0; i < boardLength; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
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
    void ShowDice() {
        foreach(int[] tileCoords in currentCoords) {
            if(tileCoords[0] < 0 || tileCoords[0] >= boardLength || tileCoords[1] < 0 || tileCoords[1] >= boardHeight) {
                throw new IndexOutOfRangeException("Indexes for DiceForm are not on the board");
            }
        }
        foreach(int[] tileCoords in currentCoords) {
            boardTiles[tileCoords[0], tileCoords[1]].GetComponent<Tile>().highlight = 1;
        }
    }
    //find a better name
    //sets highlight to 0 for the current coordinates
    void UnShowDice() {
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
    //Place a dice form on the board
    bool PlaceDice(int x, int y, Enums.DiceForm form, int team, int rotation = 0) {
        int[][] formCoords = GetFormCoords(x, y, form, rotation);
        SetAffiliation(formCoords, team);
        return true;
    }
    //Place a piece onto the board
    //Expects x and y values from 1 to 19/13
    bool PlacePiece(int x, int y, Piece piece)
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
    int GetAffiliation(int x, int y)
    {
        return boardTiles[x - 1, y - 1].GetComponent<Tile>().GetAffiliation();
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
