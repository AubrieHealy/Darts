using UnityEngine;
using System.Collections;
using System;
namespace RSUnityToolkit

{

    public class ThrowArrow : MonoBehaviour
    {
        enum ThrowState
        {
            OpenHandPrompt,
            GrabDartPrompt,
            ThrowDartPrompt,
            InFlight,
        }

        private PXCMSenseManager mSenseManager = null; //create sensemanager
        public GameObject DartPrefab; // current game object

        private bool mReadyForDart = true;
        public GUIStyle style;

        private PXCMHandModule mHandModule;
        private PXCMHandData mHandData;

        public Camera MainCamera;
        public float RSScale = 1.0f;
        private PXCMFaceModule faceAnalyzer;
        private PXCMFaceData faceData;

        private PXCMHandCursorModule mCursorModule;
        private PXCMCursorData mCursorData;

        private VelocityAverage mVelocityAverage = new VelocityAverage(15);

        private bool mLastHandPositionAvailable;
        private Vector3 mLastHandPosition;

        public GameObject NearDart;
        public GameObject FarDart;
        public GameObject IdleDart;
        public GameObject RightLimitDart;
        public GameObject TopLimitDart;
        public GameObject BottomLimitDart;

        public float RealSenseNearZ = 0.33f;
        public float RealSenseFarZ = 0.7f;
        public float RealSenseFarRightX = 0.7f;
        public float RealSenseTopY = 0.33f;
        public float RealSenseBottomY = 0.7f;

        public float ThrowMultiplier = 4.0f;

        private Dart mCurrentDart;

        private ThrowState mThrowState;

        private HistoryTracker<int> mOpenessHistory = new HistoryTracker<int>(-1, 64);
        private HistoryTracker<bool> mHandTracked = new HistoryTracker<bool>(false, 64);
        private HistoryTracker<Vector3> mHandPositionRaw = new HistoryTracker<Vector3>(Vector3.zero, 64);
        private HistoryTracker<Vector3> mDartPositionWorld = new HistoryTracker<Vector3>(Vector3.zero, 64);

        private string mPromptText = "";

        public float MinThrowSpeed = 6.0f;
        public float MinThrowAngle = 15.0f;


        void Start()
        {
            
            pxcmStatus sts;

            mThrowState = ThrowState.OpenHandPrompt;

            //PXCMSession session = PXCMSession.CreateInstance();

            //mSenseManager = session.CreateSenseManager();
            mSenseManager = PXCMSenseManager.CreateInstance();
            if (mSenseManager == null)
            {
                Debug.LogError("SenseManager Init Failed!");
                return;
            }

            /*sts = mSenseManager.EnableHandCursor();
            if (sts != pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                Debug.LogError("PXCSenseManager.EnableHandCursor: " + sts);
                return;
            }
            mCursorModule = mSenseManager.QueryHandCursor();
            mCursorData = mCursorModule.CreateOutput();*/

            // Enables hand tracking
            sts = mSenseManager.EnableHand();
            if (sts != pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                Debug.LogError("PXCSenseManager.EnableHand: " + sts);
                return;
            }
            mHandModule = mSenseManager.QueryHand();
            mHandData = mHandModule.CreateOutput();

            
            sts = mSenseManager.Init();
            if (sts != pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                Debug.LogError("PXCSenseManager.Init: " + sts);
                return;
            }
                       

            // Creates a hand config for future data ... and face
            PXCMHandConfiguration handconfig = mHandModule.CreateActiveConfiguration();

            //handconfig.SetTrackingMode(PXCMHandData.TrackingModeType.TRACKING_MODE_CURSOR);

            //Smoothes hand tracking movement 
            //handconfig.SetSmoothingValue(10);

            //Sets the tracking bounds of the screen
            //handconfig.SetTrackingBounds(-4, -14, 9, 7);


            //If there is handcnGnfig instance
            if (handconfig != null)
            {
                handconfig.EnableAllAlerts();
                handconfig.ApplyChanges();
                handconfig.Dispose();
            }
            /*
            if (faceconfig != null)
            {
                faceconfig.pose.isEnabled = true;
                faceconfig.ApplyChanges();
                faceconfig.Dispose();
                faceData = faceAnalyzer.CreateOutput();
            }
            */

        }
        

        private void MCurrentDart_CollideEvent(Dart dart)
        {
            mReadyForDart = true;
            mThrowState = ThrowState.OpenHandPrompt;
        }

        private void ResetDartIdle()
        {
            if (mCurrentDart != null)
            {
                mCurrentDart.transform.position = IdleDart.transform.position;
                mCurrentDart.transform.rotation = IdleDart.transform.rotation;
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Checks is there is a sensemanager session 
            if (mSenseManager == null || mHandModule == null)
                return;

            int openness = -1;

            Vector3 headPosition = Vector3.zero;
            Vector3 dartPosition = Vector3.zero;
            
            bool lastHandPositionAvailable = mLastHandPositionAvailable;

            mLastHandPositionAvailable = false;

            mHandData.Update();

            PXCMHandData.IHand hand;
            //PXCMCursorData.ICursor cursor;
            PXCMPoint3DF32 location;
            
            //mCursorData.QueryCursorData(PXCMCursorData.AccessOrderType.ACCESS_ORDER_NEAR_TO_FAR, 0, out cursor);
            mHandData.QueryHandData(PXCMHandData.AccessOrderType.ACCESS_ORDER_NEAR_TO_FAR, 0, out hand);
            if (hand != null)
            {
                location = hand.QueryMassCenterWorld();
                openness = hand.QueryOpenness();

                dartPosition = new Vector3(location.x, location.y, location.z);
                //Debug.LogFormat("dartPosition = {0}, {1}, {2}", dartPosition.x, dartPosition.y, dartPosition.z);

                
                
            }
            else
            {
                mVelocityAverage.Reset();
            }
            mSenseManager.ReleaseFrame();

            mHandPositionRaw.Insert(dartPosition);
            mHandTracked.Insert(hand != null);
            mOpenessHistory.Insert(openness);

            switch( mThrowState )
            {
                case ThrowState.OpenHandPrompt:
                {
                    mPromptText = "Open your hand";
                        ResetDartIdle();
                        bool open = mOpenessHistory.CheckCriteria(5, (value)=> value > 80);
                    bool handTracked = mHandTracked.CheckCriteria(5, (value) => value );
                    if( open && handTracked )
                    {
                        mThrowState = ThrowState.GrabDartPrompt;
                    }
                }
                break;
                case ThrowState.GrabDartPrompt:
                    {
                        mPromptText = "Close your hand to grab the dart";

                        if( mCurrentDart == null )
                        {
                            mCurrentDart = Instantiate(DartPrefab).GetComponent<Dart>();
                            mCurrentDart.transform.SetParent(transform);
                            mCurrentDart.CollideEvent += MCurrentDart_CollideEvent;
                        }
                        ResetDartIdle();

                        // if tracking has been missing for 10 frames
                        if (mHandTracked.CheckCriteria(10, (value) => value == false))
                        {
                            mThrowState = ThrowState.OpenHandPrompt;
                        }
                        else
                        {
                            bool closed = mOpenessHistory.CheckCriteria(5, (value) => value < 60);
                            if( closed )
                            {
                                mThrowState = ThrowState.ThrowDartPrompt;
                            }
                        }
                        
                    }
                    break;
                case ThrowState.ThrowDartPrompt:
                    {
                        DartThrowingLogic();
                    }
                    break;
                case ThrowState.InFlight:
                    {
                        mPromptText = "Dart In Flight";
                    }
                    break;
                default: break;
            }

            if( mCurrentDart )
                mDartPositionWorld.Insert(mCurrentDart.transform.position);

        }

        float LerpNoClamp( float a, float b, float ratio )
        {
            return a + (b - a) * ratio;
        }

        Vector3 RawHandPositionToWorld( Vector3 pos )
        {
            float scale = Mathf.Abs((FarDart.transform.position.z - NearDart.transform.position.z) / (RealSenseFarZ - RealSenseNearZ));
            float ratioX = Math.Min(1.0f, Math.Max(-1.0f, pos.x / RealSenseFarRightX));
            float ratioY = Math.Min(1.0f, Math.Max(0.0f, (pos.y - RealSenseTopY) / (RealSenseBottomY - RealSenseTopY)));
            float ratioZ = Math.Min(1.0f, Math.Max(0.0f, (pos.z - RealSenseNearZ) / (RealSenseFarZ - RealSenseNearZ)));

            float newZ = Mathf.Lerp(FarDart.transform.position.z, NearDart.transform.position.z, ratioZ);
            float newY = LerpNoClamp(TopLimitDart.transform.position.y, BottomLimitDart.transform.position.y, ratioY);
            float newX = LerpNoClamp(transform.position.x, RightLimitDart.transform.position.x, ratioX);

            Vector3 handPositionWorld = new Vector3(newX, newY, newZ);
            return handPositionWorld;
        }

        void DartThrowingLogic( )
        {
            mPromptText = "Throw the dart!";

            if( mHandTracked.CheckCriteria(32, (value) => value == false ) )
            {
                Debug.Log("hand tracking lost for 32 frames");
                mThrowState = ThrowState.OpenHandPrompt;
                return;
            }
            if (!mHandTracked.CheckCriteria(2, (value) => value == true))
            {
                Debug.Log("hand tracking unavailable");
                return;
            }

            Vector3 handPositionWorld = RawHandPositionToWorld(mHandPositionRaw[0]);

            Vector3 dartCurPosition = mCurrentDart.transform.position;
            Vector3 dartNewPosition = Vector3.Lerp(dartCurPosition, handPositionWorld, 0.1f);
            dartNewPosition.z = Mathf.Lerp(dartCurPosition.z, handPositionWorld.z, 0.4f);

            Vector3 camPos = MainCamera.transform.position;
            camPos.x = dartNewPosition.x;
            camPos.y = dartNewPosition.y + 0.05f;
            MainCamera.transform.position = camPos;

            //Vector3 throwPosition = 
            //dartPosition = new Vector3(dartPosition.x * scale, dartPosition.y * scale, newZ);

            /*if (lastHandPositionAvailable)
            {
                Vector3 delta = dartPositionWorld - mLastHandPosition;
                mVelocityAverage.AddValue(delta / Time.deltaTime);
            }*/
            mLastHandPosition = dartNewPosition;
            mLastHandPositionAvailable = true;

            mCurrentDart.transform.position = dartNewPosition;
            mCurrentDart.transform.rotation = FarDart.transform.rotation;

            if( mOpenessHistory.CheckCriteria( 2, (value)=> value > 80 ) && mDartPositionWorld.Count > 4)
            {
                Vector3 dir = Vector3.Normalize(FarDart.transform.position - NearDart.transform.position);
                Vector3 velocity = (mDartPositionWorld[0] - mDartPositionWorld[4]) / (Time.deltaTime * 4);
                velocity.x = 0;
                velocity *= ThrowMultiplier;

                float angle = Vector3.Angle(dir, velocity.normalized);
              //  Debug.LogFormat("Angle Between: {0}", angle);
             //   Debug.LogFormat("Velocity: {0}, {1}, {2}, Speed: {3}", velocity.x, velocity.y, velocity.z, velocity.magnitude);
                if (angle <MinThrowAngle && velocity.magnitude > MinThrowSpeed)
                {
                    mCurrentDart.SetThrow(velocity);
                    mCurrentDart = null;
                    mThrowState = ThrowState.InFlight;
                }
            }
            /*if ( mOpenness > 95)
            {
                Vector3 dir = (FarDart.transform.position - NearDart.transform.position);
                Vector3 avgVelocity = mVelocityAverage.GetHighestAvgVelocity(4, dir);
                //Vector3 worldVelcoity = RSCamera.transform.TransformVector(avgVelocity);
                mCurrentDart.SetThrow(avgVelocity * ThrowMultiplier);
                mCurrentDart = null;
            }*/
        }

        void OnGUI()
        {


            //GUI.Label(new Rect(10, 10, 100, 20), mVelocityAverage.GetAverage().ToString());
            GUI.Label(new Rect(10, 10, 100, 20), mPromptText, style);// mVelocityAverage.GetAverage().ToString());
            GUI.Label(new Rect(10, 30, 100, 20), mHandPositionRaw[0].ToString(), style);// string.Format("openness {0}", mOpenessHistory[0]));// mVelocityAverage.GetAverage().ToString());


        }
        void OnCollisionEnter(Collision col)
        {
            Debug.Log("Collided"); 
        }
        
    }

}