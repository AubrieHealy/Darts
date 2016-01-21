using UnityEngine;
using System.Collections;
using System; 
namespace RSUnityToolkit

{


   public class ThrowArrow : MonoBehaviour
    {
        private PXCMSenseManager session = null; //create sensemanager
        private pxcmStatus sts; // sts for debug log
        public GameObject myObj; // current game object
        private PXCMPoint3DF32 lastFramesLocation; // to be filled with the first frames location
        private Vector3 speed; // Updates the speed of the dart 
        private float velocity;
        RaycastHit hit;
        public float speed2; 
        public Transform Effect;
        private float FFTime;
        private PXCMHandModule handAnalyzer;
        public Rigidbody Dart;
        public Rigidbody ToClone; 
      
        // Use this for initialization
        void Start()
        {

            Dart = GetComponent<Rigidbody>(); 
            // Creates location data to compare current location
            lastFramesLocation.x = 0;
            lastFramesLocation.y = 0;
            lastFramesLocation.z = 0;


            // to speed up dart movement 
            speed = new Vector3(5, 5, 5);

            // Creates an instance of the sense manager to be called later
            session = PXCMSenseManager.CreateInstance();


            //Output an error if there is no instance of the sense manager 
            if (session == null)
            {
                Debug.LogError("SenseManager Init Failed!");
            }


            // Enables hand tracking
            sts = session.EnableHand();
            handAnalyzer = session.QueryHand();


            
            if (sts != pxcmStatus.PXCM_STATUS_NO_ERROR)
                Debug.LogError("PXCSenseManager.EnableHand: " + sts);



            // Creates the session 
            sts = session.Init();
            if (sts != pxcmStatus.PXCM_STATUS_NO_ERROR)
                Debug.LogError("PXCSenseManager.Init: " + sts);



            // Creates a hand config for future data 
            PXCMHandConfiguration handconfig = handAnalyzer.CreateActiveConfiguration();

            //Smoothes hand tracking movement 
            handconfig.SetSmoothingValue(10);
            
            //Sets the tracking bounds of the screen
            handconfig.SetTrackingBounds(-4, -14, 9, 7);


            //If there is handconfig instance
            if (handconfig != null)
            {
                handconfig.EnableAllAlerts();
                handconfig.ApplyChanges();
                handconfig.Dispose();
            }
           
        }

        // Update is called once per frame
        void FixedUpdate()
        {


            // Checks is there is a sensemanager session 
            if (session == null)
                return;
            
             // For accessing hand data
             handAnalyzer = session.QueryHand();


            // If there is a sense manager create an output for the hand data. 
            if (handAnalyzer != null)
            {
                PXCMHandData handData = handAnalyzer.CreateOutput();
                if (handData != null)
                {
                    handData.Update();
                  
                    PXCMHandData.IHand IHAND; // Ihand instance for accessing future data
                    Int32 IhandData; // for QueryOpenness Value
                    PXCMPoint3DF32 location; // Stores hand tracking position 

                    //Fills IHAND with information to later be grabbed and used for tracking + openness 
                    handData.QueryHandData(PXCMHandData.AccessOrderType.ACCESS_ORDER_NEAR_TO_FAR, 0, out IHAND);

                   
                    // If there is data in Ihand
                    if (IHAND != null)
                    {
                      
                        // Debug.DrawLine(transform.position, hit.point, Color.red);


                        // Inits hand tracking from the center of the hand. 
                        location = IHAND.QueryMassCenterWorld();



                        //Locates the intitial frame and sets it's value to the current location. 
                        if (lastFramesLocation.x == 0 && lastFramesLocation.y == 0 && lastFramesLocation.z == 0)
                        {
                            FFTime = Time.deltaTime; 
                            //  Debug.Log("Previous Frames Time: " + FFTime); 
                            //first frame
                         //   Debug.Log("LastFrameLocation: " + lastFramesLocation.z + "Current frame location: " + location.z); 
                            lastFramesLocation = location;
                            return;
                        }


                     
                        Vector3 temp = new Vector3((lastFramesLocation.x - location.x) * speed.x, (lastFramesLocation.y - location.y) * -1 * speed.y, (lastFramesLocation.z - location.z) * speed.z);
                        float Distance = (location.z - lastFramesLocation.z);
                     //   Debug.Log("Distance Traveled : " + Distance);
                        velocity = Math.Abs(Distance / .02f) ;
                        
                       Debug.Log("Velocity: " + velocity);
           






                        //     IhandData = IHAND.QueryOpenness();
                        //if (IhandData > 80)
                        if (velocity > 1)
                        {
                            //  Dart.useGravity = true ;
                            //Debug.Log("Hand Open!!");
                            Instantiate(ToClone,location, Quaternion.identity);
                            ToClone.AddForce(transform.up * ((velocity* 15)));


                            /*     Vector3 fwd = transform.TransformDirection(Vector3.forward);
                                  Ray ray = new Ray(transform.position, Vector3.forward);
                                  if (Physics.Raycast(ray, out hit))
                                  {

                                     // Debug.DrawLine(transform.position, hit.point, Color.red);

                                      //transform.position = Vector3.MoveTowards(transform.position, hit.point , step );

                                      if(hit.collider.tag == "Environment")
                                      {

                                          float step = speed2 * Time.deltaTime;
                                          //Instantiate(Effect, transform.position, transform.rotation);
                                          Debug.DrawRay(transform.position, Vector3.forward);

                                          //GameObject Clone = Instantiate(myObj, transform.position, transform.rotation) as GameObject;

                                      }
                                      */
                        }
                        else
                        {
                            myObj.transform.position += temp;
                            lastFramesLocation = location;
                        }
                    }

                    }

                   
                    handAnalyzer.Dispose();
                    session.ReleaseFrame();

                }
           
              }
            void OnCollisionEnter(Collision collision)
             {
                 ToClone.velocity = Vector3.zero;
             }
   }
 }
    

