using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.localPosition.x <= -118 && this.transform.localPosition.y >= 118) {
            this.transform.localPosition = new Vector3(0, 0, 0);
        } else {
            this.transform.localPosition = this.transform.localPosition + new Vector3(-0.2f, 0.2f, 0); 
        }
        
    }
}
