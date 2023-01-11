using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blueEyesWhiteDragon : Unit
{
    // Start is called before the first frame update
    void Start()
    {
        DiceCost = 4;
        DiceFaces = new DiceFace[6];
        DiceFaces[0] = new DiceFace(Enums.Crest.summon, 4);
        DiceFaces[1] = new DiceFace(Enums.Crest.movement, 1);
        DiceFaces[2] = new DiceFace(Enums.Crest.movement, 2);
        DiceFaces[3] = new DiceFace(Enums.Crest.attack, 4);
        DiceFaces[4] = new DiceFace(Enums.Crest.defense, 1);
        DiceFaces[5] = new DiceFace(Enums.Crest.magic, 1);
        Health = 50;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
