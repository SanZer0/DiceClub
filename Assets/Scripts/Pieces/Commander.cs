using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Commander : Piece
{
    public int life = 3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DamageCommander()
    {
        life -= 1;
    }
}
