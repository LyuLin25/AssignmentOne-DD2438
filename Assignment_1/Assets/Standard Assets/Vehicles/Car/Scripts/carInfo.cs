using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carInfo : MonoBehaviour
{
    public BoxCollider carCollider;
    // Start is called before the first frame update
    void Start()
    {
        carCollider = GameObject.Find("ColliderBottom").GetComponent<BoxCollider> ();   
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift)){
            print(carCollider.bounds.size.x);
            print(carCollider.bounds.size.z);
        }
    }
}
