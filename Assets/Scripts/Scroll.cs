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
        if(this.transform.localPosition.Equals(new Vector3(-118, 118, 0))) {
            this.transform.localPosition = new Vector3(0, 0, 0);
        } else {
            this.transform.localPosition = this.transform.localPosition + new Vector3(-0.5f, 0.5f, 0); 
        }
        
    }
}
