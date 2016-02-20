using UnityEngine;
using System.Collections;

public class EasyMode : MonoBehaviour {
    private Ray ray;
    private RaycastHit hitPoint;
    private LightType ToHighlight; 
        
	// Use this for initialization
	void Start () {
        ToHighlight = LightType.Spot; 
	}
	
	// Update is called once per frame
	void Update () {
        if (Physics.Raycast(ray, out hitPoint))
        {
            
        }
	
	}
}
