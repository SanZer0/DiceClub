using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{

    //Singleton setup
    public static GameManager instance = null;

    public Commander commander1;
    public Commander commander2;
    public static Commander[] commander = new Commander[2];

    //Control-variables for the user-input
    const float WAITTIME = 0.25f;
    float waitCounter = WAITTIME;
    bool blockSwitch = false;

    int activePlayer = 1;
    public BoardManager2D board2d;

    public GameObject playerText;

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
        UpdatePlayerText();
    }

    // Update is called once per frame
    void Update()
    {
        board2d.UnShowDice();
        board2d.SetCurrentCoords();
        try{
            board2d.HighlightDice();
        } catch (IndexOutOfRangeException) {
            board2d.HighlightTile();
        }
        //input to change form and rotation
        if(waitCounter > 0 && blockSwitch) {
            waitCounter -= Time.deltaTime;
        } else if (waitCounter <= 0){
            blockSwitch = false;
        }
        if (Input.GetKey(KeyCode.Tab) && !blockSwitch) {
            board2d.NextForm();
            waitCounter = WAITTIME;
            blockSwitch = true;
        }
        if (Input.GetKey("q") && !blockSwitch) {
            board2d.NextRotation(1);
            waitCounter = WAITTIME;
            blockSwitch = true;
        }
        if (Input.GetKey("e") && !blockSwitch) {
            board2d.NextRotation(-1);
            waitCounter = WAITTIME;
            blockSwitch = true;
        }
        
        if(Input.GetMouseButtonDown(0)) {
            if(board2d.PlaceDice(activePlayer)) {
                activePlayer = (activePlayer % 2) + 1; //set the next player
                UpdatePlayerText();
            }
        }
    }
    #endregion

    //expects value 1 or 2
    public static Commander GetCommander(int num)
    {
        return commander[num - 1];
    }

    //expects value 1 or 2 and a commander
    public static bool SetCommander(int num, Commander comm)
    {
        commander[num - 1] = comm;
        return true;
    }

    //Set the TMP to the current player
    void UpdatePlayerText() {
        TMP_Text textObject = playerText.GetComponent<TMP_Text>();
        switch (activePlayer) {
            case 1: textObject.SetText("<color=blue>PLAYER 1</color>");
                    break;
            case 2: textObject.SetText("<color=red>PLAYER 2</color>");
                    break;
            default: break;
        }
    }
}
