using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class BulletController : NetworkBehaviour
{
    [SerializeField] float speed = 5f;
    float lifetime = 2f;
	float deathTime;
	private void Start() {
		deathTime = Time.time + lifetime;
	}

	void Update()
    {
		if (IsServer) {
            transform.Translate(transform.forward * speed * Time.deltaTime);
			Debug.DrawRay(transform.position,transform.forward, Color.green, 2.01f);
			if(Time.time > deathTime) {
				GetComponent<NetworkObject>().Despawn(true);
			}
		}
    }

	private void OnCollisionEnter(Collision other) {
		if(!IsServer) return;
		string tag = other.collider.gameObject.tag;
		if(tag == "Player" ||
		tag == "Bullet" ||
		tag == "Mine"||
		tag == "Enemy"){

		}else{
			//wall  
            Vector3 v = Vector3.Reflect(this.transform.forward, other.contacts[0].normal);
            float rot = 90 - Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, rot, 0);
            
		}

	}
}
