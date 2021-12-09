using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayersManager : NetworkBehaviour
{
	private static PlayersManager _instance;

	public static PlayersManager Instance { get { return _instance; } }

	[SerializeField] NetworkObject playerPrefab;
	private void Awake() {
		if (_instance != null && _instance != this) {
			Destroy(this.gameObject);
		} else {
			_instance = this;
		}
	}

	private NetworkVariable<int> playersInGame = new NetworkVariable<int>();
	public int GetPlayersInGame() {
		return playersInGame.Value;

	}

	private void Start() {
		NetworkManager.Singleton.OnClientConnectedCallback += (id) => {
			if (IsServer) {
				Debug.Log($"{id} just connected");
				playersInGame.Value++;
			}
		};
		NetworkManager.Singleton.OnClientDisconnectCallback += (id) => {
			if (IsServer) {
				Debug.Log($"{id} just disconnected");
				playersInGame.Value--;
			}
		};
	}

	private void Update() {
		if((IsClient) && Input.GetKeyDown(KeyCode.Space)){
			//ReSpawnPlayerServerRpc();		
		}
	}

	[ServerRpc]
	public void ReSpawnPlayerServerRpc(){
		NetworkObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
		player.Spawn(true);
	}

}
