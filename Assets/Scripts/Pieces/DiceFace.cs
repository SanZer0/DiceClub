using System.Collections;
using System.Collections.Generic;

public class DiceFace
{
    public Enums.Crest Crest {get; private set;}
    public int Rank {get; private set;}
    public DiceFace(Enums.Crest crest, int rank) {
        Crest = crest;
        Rank = rank;
    }
}
