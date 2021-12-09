using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class BulletController : NetworkBehaviour
{
    [SerializeField] float speed = 10f;
	int ricochetCount = 0;
	private void Start() {
		
	}

	void Update()
    {
		if (IsServer) {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
			Debug.DrawRay(transform.position,transform.forward, Color.green, 2.01f);
		
		}
    }

	private void OnCollisionEnter(Collision other) {
		if(!IsServer) return;
		string tag = other.collider.gameObject.tag;
		if(tag == "Player" ||
		tag == "Bullet" ||
		tag == "Mine"||
		tag == "Enemy"){
			GetComponent<NetworkObject>().Despawn(true);
		}else{
			if(ricochetCount >= 1){
				GetComponent<NetworkObject>().Despawn(true);
			}
			//wall  
            Vector3 v = Vector3.Reflect(this.transform.forward, other.contacts[0].normal);
            float rot = 90 - Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, rot, 0);
			ricochetCount++;
		}

	}
}
