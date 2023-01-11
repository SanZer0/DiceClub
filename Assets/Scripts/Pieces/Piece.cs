using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public int DiceCost {get; protected set;}
    public DiceFace[] DiceFaces {get; protected set;}
    public int Type {get; protected set;}
    
    //probably not needed as pieces are saved on the specific tile
    //int xBoardPos;
    //int yBoardPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
