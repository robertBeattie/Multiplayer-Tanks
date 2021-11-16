using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class UIManager : MonoBehaviour {
    [SerializeField] Button client;
    [SerializeField] Button server;
    [SerializeField] Button host;
    [SerializeField] TextMeshProUGUI playersInGame;

	private void Awake() {
        Cursor.visible = true;
	}

	private void Start() {
		NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

		server.onClick.AddListener(() => {
			if (NetworkManager.Singleton.StartServer()) {
				Debug.Log("Server started...");
			} else {
				Debug.Log("Server Could not be started...");
			}
		});

		host.onClick.AddListener(() => {
			if (NetworkManager.Singleton.StartHost()) {
				Debug.Log("Host started...");
			} else {
				Debug.Log("Host Could not be started...");
			}
		});
	}

	public void Join() {
		NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("room password");
		if (NetworkManager.Singleton.StartClient()) {
			Debug.Log("Client started...");
		} else {
			Debug.Log("Client Could not be started...");
		}
	}

	private void Update() {
		playersInGame.text = $"Players in game: {PlayersManager.Instance.GetPlayersInGame()}";
	}


	private void Setup() {
		NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
		NetworkManager.Singleton.StartHost();
	}

	private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback) {
		//Your logic here
		bool approve = false;
		bool createPlayerObject = true;

		string password = System.Text.Encoding.ASCII.GetString(connectionData);
		if (password == "room password") {
			approve = true;
		}
		Debug.Log($"Approval: {approve}");

		// The prefab hash. Use null to use the default player prefab
		// If using this hash, replace "MyPrefabHashGenerator" with the name of a prefab added to the NetworkPrefabs field of your NetworkManager object in the scene
		//ulong? prefabHash =NetworkSpawnManager.GetPrefabHashFromGenerator("MyPrefabHashGenerator");
		uint? prefabHash = null;
		//If approve is true, the connection gets added. If it's false. The client gets disconnected
		Vector2 defaultPositionRange = new Vector2(-4, 4);
		Vector3 positionToSpawnAt = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 0, Random.Range(defaultPositionRange.x, defaultPositionRange.y));
		Quaternion rotationToSpawnWith = Quaternion.identity;
		callback(createPlayerObject, prefabHash, approve, positionToSpawnAt, rotationToSpawnWith);
	}
}
