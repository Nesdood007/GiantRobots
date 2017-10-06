using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class ProjectileScript : NetworkBehaviour {

  public Vector3 acceleration;
  //Lifetime in Seconds
  public float lifetime = 5.0f;
  private float spawnTime;
  
	// Use this for initialization
	void Start () {
    //if (!isServer) return;
		spawnTime = Time.time;
    //gameObject.GetComponent<Rigidbody>().AddForce(acceleration);
	}
	
	// Update is called once per frame
	void Update () {
    //if (!isServer) return;
		if (Time.time - spawnTime > lifetime) {
      Destroy(gameObject);
    }
	}
  
  //Do Physics Here!
  void FixedUpdate() {
    //if (!isServer) return;
    transform.Translate(acceleration * Time.deltaTime);
  }
}
