using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager2D : MonoBehaviour
{
    public GameObject boardPrefab;
    GameObject boardCanvas;
    public GameObject tilePrefab;
    GameObject[,] boardTiles;
    // Start is called before the first frame update
    void Start()
    {
        SetupBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Setup a 13x19 big board
    void SetupBoard()
    {
        if (boardCanvas != null)
        {
            Destroy(boardCanvas);
        }
        //Create new board with canvas to display the tiles
        boardCanvas = Instantiate(boardPrefab);
        boardCanvas.transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);

        //Create the tiles via the prefab
        boardTiles = new GameObject[19, 13];
        //i row, j column
        for (int i = 0; i < 19; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                Vector3 position = new Vector3((j-6) * 50, (i-9) * 50, 0); 
                boardTiles[i, j] = Instantiate(tilePrefab, position, Quaternion.identity);
                boardTiles[i, j].name = "Tile " + (i + 1) + " " + (j + 1);
                boardTiles[i, j].transform.SetParent(boardCanvas.transform);
            }
            Debug.Log("Finished row " + (i + 1));
        }

        //place the commanders on their fields
        Debug.Log("Placing Commanders");
        PlacePiece(1, 7, GameManager.GetCommander(1));
        PlacePiece(19, 7, GameManager.GetCommander(2));

        //testcode
        SetAffiliation(7, 8, 2);
        SetAffiliation(7, 9, 2);
        SetAffiliation(7, 10, 2);
        SetAffiliation(7, 11, 2);
        SetAffiliation(8, 10, 1);
        SetAffiliation(8, 11, 1);
        SetAffiliation(9, 11, 0);
    }

    //Place a piece onto the board
    //Expects x and y values from 1 to 19/13
    bool PlacePiece(int x, int y, Piece piece)
    {
        Tile tile = boardTiles[x - 1, y - 1].GetComponent<Tile>();
        if (tile.HasPiece())
        {
            return false;
        }
        Debug.Log("placing piece");
        tile.PlacePiece(piece);
        return true;

    }

    bool SetAffiliation(int x, int y, int team, bool force = true)
    {
        if (!force && GetAffiliation(x, y) != 0)
        {
            return false;
        }
        boardTiles[x - 1, y - 1].GetComponent<Tile>().SetAffiliation(team);
        return true;
    }

    int GetAffiliation(int x, int y)
    {
        return boardTiles[x - 1, y - 1].GetComponent<Tile>().GetAffiliation();
    }
}
