using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    BoardManager2D board;
    //save the piece that is on the field
    Piece piece = null;
    int affiliation = 0;
    [HideInInspector]
    public int highlight = 0;
    int showed = 0;
    int x_coord, y_coord;
    public void SetCoordinates(int x, int y) {
        x_coord = x;
        y_coord = y;
    }
    Color m_color = Color.white;

    #region Start&Update
    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        if(highlight != 0 && showed == 0) {
            showed = 1;
            switch(affiliation) {
                case 0: ShowColor(Color.green);
                        break;
                case 1: ShowColor(new Color(1.0f, 0.6f, 0.0f));
                        break;
                case 2: ShowColor(new Color(0.0f, 1.0f, 0.8f));
                        break;
                default:   ShowColor(Color.white);
                                break; 
            }
            ShowColor(Color.green);
        } else if(showed == 1 && highlight == 0) {
            showed = 0;
            ShowColor(m_color);
        }
    }
    #endregion

    //check if this tile already has a piece on it
    public bool HasPiece()
    {
        return (piece != null);
    }

    //place a new piece, destroy other one if already occupied
    public void PlacePiece(Piece newPiece)
    {
        if(piece != null)
        {
            Destroy(piece);
        }
        piece = newPiece;
        piece.gameObject.transform.SetParent(gameObject.transform);
        //glue the piece to the tile. for some reason it defaulted to 0, so i did it this way
        //probably better ways FIX
        piece.gameObject.transform.SetPositionAndRotation(new Vector3(gameObject.transform.position.x,
                                                          gameObject.transform.position.y,
                                                          gameObject.transform.position.z),
                                                          Quaternion.identity);
    }
    //Set the affiliation of the tile, so which team owns that tile
    public void SetAffiliation(int team)
    {
        affiliation = team;
        switch (affiliation)
        {
            case 0:
                SetColor(Color.white);
                break;
            case 1:
                SetColor(Color.blue);
                break;
            case 2:
                SetColor(Color.red);
                break;
            default:
                SetColor(new Color(0.32f, 0.32f, 0.41f));
                break;
        }
    }

    public int GetAffiliation()
    {
        return affiliation;
    }

    //set the color of the current tile
    void SetColor(Color color)
    {
        m_color = color;
        ShowColor(color);
    }

    void ShowColor(Color color) {
        gameObject.GetComponent<SpriteRenderer>().color = color;
    }

    public void SetBoard(BoardManager2D boardManager) {
        board = boardManager;
    }
    //Gets called when the mouse enters the hitbox
    void OnMouseEnter() {
        board.SetSelection(x_coord, y_coord);
        highlight = 1;
    }
    //Gets called when the mouse leaves the hitbox
    void OnMouseExit() {
        board.SetSelection(-1, -1);
        highlight = 0;
    }
}
