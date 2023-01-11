using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public BoardManager2D Board {get; set;}
    //save the piece that is on the field
    Piece piece = null;
    private int _affiliation = 0;
    public int Affiliation {get {return _affiliation;} 
                    set {
                        _affiliation = value;
                        switch (Affiliation)
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
                }
    [HideInInspector]
    public int highlight = 0;
    //int showed = 0; - deprecated
    public int[] Coordinates {get; set;}
    Color m_color = Color.white;

    #region Start&Update
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(highlight != 0) {
            switch(Affiliation) {
                case 0: ShowColor(Color.green);
                        break;
                case 1: ShowColor(Color.yellow);
                        break;
                case 2: ShowColor(Color.yellow);
                        break;
                default:   ShowColor(Color.white);
                                break; 
            }
        } else if(highlight == 0) {
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
                                                          gameObject.transform.position.z + 1),
                                                          Quaternion.identity);
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
    //Gets called when the mouse enters the hitbox
    void OnMouseEnter() {
        Board.SetSelection(Coordinates[0], Coordinates[1]);
        highlight = 1;
    }
    //Gets called when the mouse leaves the hitbox
    void OnMouseExit() {
        Board.SetSelection(-1, -1);
        highlight = 0;
    }
}
