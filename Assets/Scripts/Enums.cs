using System;
public class Enums {
    public enum DiceForm {
        cross_norm,
        t_norm,
        hand_norm,
        hand_rev,
        z_norm,
        z_rev,
        stairs_norm,
        stairs_rev,
        line_norm,
        line_rev
    }

    public enum Type {
        spellcaster,
        warrior,
        undead,
        beast,
        dragon,
        magic
    }

    public enum Crest {
        summon,
        movement,
        magic,
        attack,
        defense,
        trap
    }
    //self explanatory
    public static DiceForm GetNextEnumValueOf(DiceForm value)
    {
        int size = Enum.GetNames(typeof (DiceForm)).Length;
        int newValue = (int)value + 1;
        if(newValue == size) {
            newValue = 0;
        }
        return (DiceForm) newValue;
    }
}