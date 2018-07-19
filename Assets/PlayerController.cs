using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {

    public Camera cam;
    public NavMeshAgent agent;

    private void Start()
    {
        //agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update () {
        
        //Mouse clicked
        if (Input.GetMouseButtonDown(0))
        {
            Ray  ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Debug.Log(Physics.Raycast(ray, out hit));

            if(Physics.Raycast(ray, out hit))
            {
                Debug.Log("ok");
                agent.SetDestination(hit.point);
            }
        }
		
	}
}
