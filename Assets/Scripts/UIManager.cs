using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

public class UIManager : NetworkBehaviour {
    [SerializeField] Button client;
    [SerializeField] Button server;
    [SerializeField] Button host;
	[SerializeField] Button disconnect;
	[SerializeField] TextMeshProUGUI playersInGame;
	[SerializeField] TMP_InputField inputIP;

	private void Awake() {
        Cursor.visible = true;
	}

	private void Start() {
		NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
	}

	public void Join() {
		if (inputIP.text != "") {
			NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = inputIP.text;
		} else {
			NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = "127.0.0.1";
		}
		
		NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("room password");
		if (NetworkManager.Singleton.StartClient()) {
			Debug.Log("Client started...");
			DisableConnectionUI();
		} else {
			Debug.Log("Client Could not be started...");
		}
	}

	public void Server(){
		if (NetworkManager.Singleton.StartServer()) {
			Debug.Log("Server started...");
			DisableConnectionUI();
		} else {
			Debug.Log("Server Could not be started...");
		}
	}

	public void Host(){
		if (NetworkManager.Singleton.StartHost()) {
			Debug.Log("Host started...");
			DisableConnectionUI();
		} else {
			Debug.Log("Host Could not be started...");
		}
	}

	public void Disconnect() {
		NetworkManager.Singleton.Shutdown();
		EnableConnectionUI();
		//UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
	}

	private void DisableConnectionUI() {
		client.transform.parent.gameObject.SetActive(false);
		disconnect.transform.parent.gameObject.SetActive(true);
	}
	private void EnableConnectionUI() {
		client.transform.parent.gameObject.SetActive(true);
		disconnect.transform.parent.gameObject.SetActive(false);
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

		/*
		The network Hash Generator doesn't update when we rename the prefab. 
		So it couldn't find the proper Hash. Secondly for the player approval callback a null value (as written in the documentation) 
		for finding Hash of Default Player Doesn't Work. You need to type the full name eg:	
		ulong? prefabHash = SpawnManager.GetPrefabHashFromGenerator("NetworkPlayer"); // The prefab hash. where "NetworkPlayer" is the name of prefab.
		*/
		
		uint? prefabHash = null;
		//If approve is true, the connection gets added. If it's false. The client gets disconnected
		Vector2 defaultPositionRange = new Vector2(-4, 4);
		Vector3 positionToSpawnAt = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 0, Random.Range(defaultPositionRange.x, defaultPositionRange.y));
		Quaternion rotationToSpawnWith = Quaternion.identity;
		callback(createPlayerObject, prefabHash, approve, positionToSpawnAt, rotationToSpawnWith);
	}
}
