using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class EnemyManager : NetworkBehaviour
{
    private static EnemyManager _instance;

	public static EnemyManager Instance { get { return _instance; } }

    [SerializeField] NetworkObject EnemyPrefab;

	private void Awake() {
		if (_instance != null && _instance != this) {
			Destroy(this.gameObject);
		} else {
			_instance = this;
		}
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(IsServer && Input.GetKeyDown(KeyCode.E)){
            NetworkObject enemy = Instantiate(EnemyPrefab, new Vector3(0,0,0), Quaternion.identity);
            enemy.Spawn(true);
        }
    }
}
