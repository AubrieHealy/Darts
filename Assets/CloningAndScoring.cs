using UnityEngine;
using System.Collections;

public class CloningAndScoring : MonoBehaviour {

    public int toThrow; 
	// Use this for initialization
	void Start () {
        toThrow = 0; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

   public void Cloning(float velocity)

    {
        //    new Vector3 ClonePos = Vector3(myObj.transform.position; )
        if (toThrow < 1)
        {

            GameObject Clone = Instantiate(gameObject, gameObject.transform.position, transform.rotation) as GameObject;
     //Instantiate(gameObject, gameObject.transform.position, transform.rotation) as GameObject;
            Rigidbody CloneDart = Clone.GetComponent<Rigidbody>();

            //CloneDart.GetComponent<Collider>();


            //Physics.IgnoreCollision(Dart.GetComponent<Collider>(), Clone.GetComponent<Collider>());
            //    toThrow = toThrow - 1;
            CloneDart.transform.position = gameObject.transform.position;
            CloneDart.AddForce(transform.up * ((velocity * 90)));
         //   Physics.IgnoreCollision(Clone.GetComponent<Collider>(), GetComponent<Collider>());


            toThrow = toThrow + 1;


        }

    }

    void OnCollisionEnter(Collision other)
    {
        
        // Dart.velocity = Vector3.zero;
        if(other.gameObject.tag == "Environment")
        { 
            Debug.Log("Tothrow: " + toThrow);

            toThrow = toThrow - 2;
            Destroy(this.gameObject);  
        }


    }

}
