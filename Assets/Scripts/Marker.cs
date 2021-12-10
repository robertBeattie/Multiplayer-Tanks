using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Marker : NetworkBehaviour
{
    [SerializeField] List<Color> markerColors;
    [SerializeField] private NetworkVariable<int> id = new NetworkVariable<int>();
    int lastID = -1;

    private void Start() {
        SetIDServerRpc(-1);
    }

    
    private void Update() {
        if(id.Value != lastID){
            SetIDServerRpc(lastID);
            SetColor(id.Value);
        }
    }
    public void SetId(int id){
        lastID = id;
    }

    public void SetColor(int id){
        gameObject.GetComponentInChildren<SpriteRenderer>().color =  markerColor(id);
    }

    
	Color markerColor(int id){
		return markerColors[id % 8];
	}

    [ServerRpc]
    public void SetIDServerRpc(int _id){
        id.Value = _id;
    }
}
