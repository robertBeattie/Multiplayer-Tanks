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
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

			if(Time.time > deathTime) {
				GetComponent<NetworkObject>().Despawn(true);
			}
		}
    }
}
