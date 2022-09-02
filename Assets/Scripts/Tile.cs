using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    //save the piece that is on the field
    Piece piece = null;
    int affiliation = -1;

    #region Start&Update
    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void SetAffiliation(int team)
    {
        affiliation = team;
        switch (affiliation)
        {
            case 0:
                SetColor(Color.black);
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
        //damn
        gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().color = color;
    }
}