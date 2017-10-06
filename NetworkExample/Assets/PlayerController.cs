using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
  public float coolDownTime = 0.125f;
  public GameObject projectile;
  
  private float lastFire = 0.0f;
    void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }
        
        if (Input.GetButtonDown("Fire1") && Time.time - lastFire > coolDownTime) {
          Instantiate(projectile, transform.position, transform.rotation);
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }
}