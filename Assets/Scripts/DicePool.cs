using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicePool : MonoBehaviour {
    public List<Piece> pieces = new List<Piece>();

    void BuildPool() {

    }

    bool AddPiece(Piece piece) {
        if(pieces.Count < 15) {
            pieces.Add(piece);
            return true;
        }
        return false;
    }


}