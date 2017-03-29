using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class armo : MonoBehaviour {


    public SteamVR_TrackedObject trackObj;
    public GameObject rayObj;



    void Awake(){
       
    }

    void Start () {
		
	}
	
 
	void Update () {
        Ray ray = new Ray(rayObj.transform.position, rayObj.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50) && EventSystem.current.IsPointerOverGameObject())  {   //ugui 
            Debug.Log("ugui");       
        }
    }





    public void EnterTese()
    {
        /*      if (Physics.Raycast(ray, out hit, 100f, 5) && EventSystem.current.IsPointerOverGameObject())
              {
                  print("sss");

            }
         */
    }
    public void ExitTese()
    {
        print("exit");
    }
}
