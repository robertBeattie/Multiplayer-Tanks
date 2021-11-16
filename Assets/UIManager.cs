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
		client.onClick.AddListener(() => {
			if (NetworkManager.Singleton.StartClient()) {
				Debug.Log("Client started...");
			} else {
				Debug.Log("Client Could not be started...");
			}
		});

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

	private void Update() {
		playersInGame.text = $"Players in game: {PlayersManager.Instance.GetPlayersInGame()}";
	}
}
