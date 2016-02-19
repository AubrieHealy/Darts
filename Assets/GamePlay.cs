﻿using UnityEngine;
using System.Collections;

public class GamePlay : MonoBehaviour
{
    private float DistanceFromCenter;
    private float tempx;
    private float tempy;
    private Vector3 pointOfContact = Vector3.zero;
    //public GameObject CenterPoint;
    public Transform center;
    private float pos;
    private Vector3 CenterPointPos;
    public float angle;
    //public GameObject DartPrefab;
   // private Dart mCurrentDart;
    public GUIStyle style = null; 
    public string text = "";
    public int score; 
    public Rect RectPos = new Rect(0, 0, 10, 10);
    public Rect RectPos2 = new Rect(-1, -1, 10, 10);
    public string text2 = "";

    // Use this for initialization
    void Start()
    {

        DistanceFromCenter = 0;
        tempx = 0;
        tempy = 0;
   // mCurrentDart =    Instantiate(DartPrefab).GetComponent<Dart>();
        // CenterPointPos = Vector3(-36, 0.041, 0);
        float angle = Vector3.Angle(pointOfContact, transform.position);
        // Debug.Log("COLLIDED + Angle" + angle);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        //    Debug.Log("COLLIDED + Angle" + angle);
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("MouseButtonDown");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            if (Physics.Raycast(ray))
            {

               
                float DistanceFromCenter = Mathf.Sqrt(Mathf.Pow(transform.position.x - center.position.x, 2f) + (Mathf.Pow(transform.position.y - center.position.y, 2f)));
                angle = Mathf.Abs(Mathf.Atan2(transform.position.y - center.position.y, transform.position.x - center.position.x));
                angle = Mathf.Rad2Deg * angle;
              //  pos = angle / 10;


                Debug.Log("Angle: " + angle + "DistanceFromCenter" + DistanceFromCenter );

            }

        }
        */
        
  
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "DartBoard")
        {


            float DistanceFromCenter = Mathf.Sqrt(Mathf.Pow(transform.position.x - center.position.x, 2f) + (Mathf.Pow(transform.position.y - center.position.y, 2f)));
            int[] points = new int[] { 6, 13, 4, 18, 1, 20, 5, 12, 9, 14, 11, 8, 16, 7, 19, 3, 17, 2, 15, 10 };
            Vector3 localPosition = center.InverseTransformPoint(col.contacts[0].point);
            // Debug.Log("Local Pos: " + localPosition);
            float angle = (Mathf.Atan2(localPosition.y, -localPosition.z) + Mathf.PI * 2.0f);
            float wedgeAngle = 360.0f / 20;
            angle = angle * Mathf.Rad2Deg;
            angle = (angle + 360f) % 360f;
            // Debug.Log("Angle: " + angle); 
            angle += wedgeAngle / 2.0f; // offset for initial tilt
                                        //Debug.Log("Angle: " + angle);
            int index = (int)(angle / wedgeAngle);
            score += points[index];
            // Debug.Log(points[index]);
            text2 = "Score:" + score;
            text = "";
























            //col.contacts[0].point;
            //  angle = Mathf.Abs(Mathf.Atan2 (transform.position.y - center.position.y, transform.position.x - center.position.x));
            // angle = (Mathf.Rad2Deg * angle) * 2;
            //   pos = angle / 20; 

            if (DistanceFromCenter > .44 && DistanceFromCenter < .49)
            {
                points[index] = points[index] * 3;
                text = "You hit a multiplier and scored: " + points[index];
                score += points[index];

            }
           
            if (DistanceFromCenter < .80 && DistanceFromCenter > .69)
            {
                points[index] = points[index] * 2;
                text = "You hit a multiplier and scored: " + points[index];
                score += points[index];
            }
            



            //   Debug.Log("Angle: " + angle + "DistanceFromCenter" + DistanceFromCenter + "Position: " + pos); 


                //  Debug.Log("ContactPoint" + pointOfContact.x + pointOfContact.y + pointOfContact.z);
                //    Debug.Log("DistanceFromCenter: " + DistanceFromCenter);


        }

    }
    private void OnGUI()
    {
        GUI.Label(RectPos, text, style);
        GUI.Label(RectPos2, text2, style);
    }
 

}
