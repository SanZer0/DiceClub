using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    //Singleton setup
    public static GameManager instance = null;

    public Commander commander1;
    public Commander commander2;
    public static Commander[] commander = new Commander[2];

    private void Awake()
    {
        //Singleton
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        //set the commanders
        SetCommander(1, commander1);
        SetCommander(2, commander2);

    }

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

    //expects value 1 or 2
    public static Commander GetCommander(int num)
    {
        return commander[num - 1];
    }

    //expects value 1 or 2
    public static bool SetCommander(int num, Commander comm)
    {
        commander[num - 1] = comm;
        return true;
    }
}
