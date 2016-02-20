using UnityEngine;
using System.Collections;

public class SceneNew : MonoBehaviour {
    private Rect ForSceneDisplay = new Rect(5,200,400,200);
    public GUIStyle style;
    

	// Use this for initialization
	void Start () {
       
	
	}
    void OnGUI()
    {
        GUI.skin.button = style; 
        if (GUI.Button(new Rect(10, 200, 50, 50), "This is a String", style))
            Debug.Log("Clicked the button with an image");

    }

}
