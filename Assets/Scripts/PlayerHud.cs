using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayerHud : NetworkBehaviour
{
    private NetworkVariable<NetworkString> playersName = new NetworkVariable<NetworkString>();

    private bool overlaySet = false;

    public override void OnNetworkSpawn() {
		if (IsServer) {
			playersName.Value = $"Player {OwnerClientId}";
		}
	}

	public void SetOverlay() {
		TextMeshProUGUI localPlayerOverlay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
		localPlayerOverlay.text = playersName.Value;
	}

	private void Update() {
		if(!overlaySet && !string.IsNullOrEmpty(playersName.Value)) {
			SetOverlay();
			overlaySet = true;
		}
	}
}


