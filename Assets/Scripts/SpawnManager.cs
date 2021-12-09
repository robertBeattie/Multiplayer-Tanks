using UnityEngine;
using Unity.Netcode;

public class SpawnManager : NetworkBehaviour {

    private static SpawnManager _instance;

	public static SpawnManager Instance { get { return _instance; } }


	private void Awake() {
		if (_instance != null && _instance != this) {
			Destroy(this.gameObject);
		} else {
			_instance = this;
		}
	}

    public uint GetPrefabHashFromGenerator(string name){
        return 1124547038;
    }

}