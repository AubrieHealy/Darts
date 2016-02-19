using UnityEngine;
using System.Collections;

public class Dart : MonoBehaviour {


    public bool mInFlight = false;
    public Rigidbody ThrowingForce;
    public int toThrow = 1; 
    // private RigidbodyConstraints previousConstraints;
    // private RigidbodyConstraints freeze;

    
    private Collider mCollider;
    private Rigidbody mRigidBody;

    public delegate void CollidedDelegate(Dart dart);
    public event CollidedDelegate CollideEvent;
    
    // Use this for initialization
    void Start ()
    {
       
        mCollider = GetComponent<Collider>();
        mRigidBody = GetComponent<Rigidbody>();
        mRigidBody.isKinematic = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
       // Debug.Log("Velocity of RigidBody: " + ThrowingForce.velocity.z); 
	
	}

    public void SetThrow( Vector3 velocity )
    {
        
        mRigidBody.isKinematic = false;
        mRigidBody.useGravity = true;
        mRigidBody.velocity = velocity;
        Debug.Log("Dart Velocity: " + velocity );
        mInFlight = true;
    }




        void OnCollisionEnter(Collision other)
    {
        if (!mInFlight)
            return;

        if (CollideEvent != null)
        {
            CollideEvent(this);
        }

        mRigidBody.isKinematic = true;
        mRigidBody.velocity = Vector3.zero;
        

    }
        /*if (other.gameObject.tag == "Environment")
        {
        
            ThrowingForce.velocity = Vector3.zero;
           
            toThrow = 0;
       

            gameObject.transform.position = new Vector3(0.678f, 8.5f, -11.89f);
       
        }

        if (other.gameObject.tag == "Center")
        {
            ThrowingForce.velocity = Vector3.zero;
            Debug.Log("YOU HIT THE CENTER!");
            toThrow = 0;
            gameObject.transform.position = new Vector3(0.678f, 8.5f, -11.89f);
        }

        if(other.gameObject.tag == "OuterCollider" )
        {
            ThrowingForce.velocity = Vector3.zero;
            Debug.Log("YOU HIT THE OUTTER!");
            toThrow = 0;
            //gameObject.transform.position = new Vector3(0.678f, 8.5f, -11.89f);
        }

        if (other.gameObject.tag == "OuterInnerCollider")
        {
            ThrowingForce.velocity = Vector3.zero;
            Debug.Log("YOU HIT THE INNER OUTTER!");
            toThrow = 0;
            //gameObject.transform.position = new Vector3(0.678f, 8.5f, -11.89f);
        }

        if (other.gameObject.tag == "InnerCollider")
        {
            ThrowingForce.velocity = Vector3.zero;
            Debug.Log("YOU HIT THE INNER !");
            toThrow = 0;
         //   gameObject.transform.position = new Vector3(0.678f, 8.5f, -11.89f);
        }*/
        

    }

 

