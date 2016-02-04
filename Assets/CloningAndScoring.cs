using UnityEngine;
using System.Collections;

public class CloningAndScoring : MonoBehaviour {

    public Rigidbody ThrowingForce; 
    public int toThrow;
    private RigidbodyConstraints previousConstraints;
    private RigidbodyConstraints freeze;
    // Use this for initialization
    void Start () {

       // previousConstraints = RigidbodyConstraints.None;
      //  freeze = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
        toThrow = 0; 
	}
	
	// Update is called once per frame
	void Update () {
       // Debug.Log("Velocity of RigidBody: " + ThrowingForce.velocity.z); 
	
	}

   public void Cloning(float velocity)

    {
        //    new Vector3 ClonePos = Vector3(myObj.transform.position; )
    //    if (toThrow < 1)
    //    {

           ThrowingForce =  gameObject.GetComponent<Rigidbody>();

            //CloneDart.GetComponent<Collider>();
            //GameObject Clone = Instantiate(gameObject, gameObject.transform.position, transform.rotation) as GameObject;
            //Instantiate(gameObject, gameObject.transform.position, transform.rotation) as GameObject;
            //Physics.IgnoreCollision(Dart.GetComponent<Collider>(), Clone.GetComponent<Collider>());
            //    toThrow = toThrow - 1;dd
            //  gameObject.transform.position = gameObject.transform.position;
            //   Physics.IgnoreCollision(Clone.GetComponent<Collider>(), GetComponent<Collider>());
            ThrowingForce.AddForce(transform.up * ((velocity)));

         //   freeze = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;



            toThrow = 1;


     //   }

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Environment")
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
            gameObject.transform.position = new Vector3(0.678f, 8.5f, -11.89f);
        }

        if (other.gameObject.tag == "OuterInnerCollider")
        {
            ThrowingForce.velocity = Vector3.zero;
            Debug.Log("YOU HIT THE INNER OUTTER!");
            toThrow = 0;
            gameObject.transform.position = new Vector3(0.678f, 8.5f, -11.89f);
        }

        if (other.gameObject.tag == "InnerCollider")
        {
            ThrowingForce.velocity = Vector3.zero;
            Debug.Log("YOU HIT THE INNER !");
            toThrow = 0;
            gameObject.transform.position = new Vector3(0.678f, 8.5f, -11.89f);
        }


    }
 
}
