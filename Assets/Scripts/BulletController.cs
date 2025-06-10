using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class BulletController : NetworkBehaviour
{
    [SerializeField] float speed = 10f;
	public int ricochetLimit = 1;
	private int ricochetCount = 0;

	[SerializeField] NetworkObject deathMarker;

	private void Start() {
		
	}

	void Update()
    {
		if (IsServer) {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
			//Debug.DrawRay(transform.position,transform.forward, Color.green, 2.01f);
		
		}
    }

	private void OnCollisionEnter(Collision other) {
		if(!IsServer) return;
		string tag = other.collider.gameObject.tag;
		if(tag == "Enemy"){
			NetworkObject marker = Instantiate(deathMarker, other.gameObject.transform.position,Quaternion.identity);
			marker.Spawn(true);
			marker.GetComponent<Marker>().SetId(8);
			NetworkObject networkObject = other.gameObject.GetComponent<NetworkObject>();
			SceneManager.Instance.RemoveEnemy(networkObject);
			networkObject.Despawn(true);
			GetComponent<NetworkObject>().Despawn(true);
		}else if (tag == "Player"){		
			NetworkObject marker = Instantiate(deathMarker, other.gameObject.transform.position,Quaternion.identity);
			int id = (int) other.collider.gameObject.GetComponent<NetworkObject>().OwnerClientId;
			marker.GetComponent<Marker>().SetId(id);
			marker.Spawn(true);
			//tp to jail
			Vector3 jail = new Vector3(Random.Range(-4, 4), 0, Random.Range(-4, 4) + 30);
			other.gameObject.transform.position = jail;
			other.gameObject.GetComponent<PlayerControl>().IsDead = true;
			GetComponent<NetworkObject>().Despawn(true);
		}else if(tag == "Bullet" || tag == "Mine"){
			GetComponent<NetworkObject>().Despawn(true);
		}else{
			if(ricochetCount >= ricochetLimit){
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
