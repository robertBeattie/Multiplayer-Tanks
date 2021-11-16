using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerControl : NetworkBehaviour
{
    [SerializeField]
    private float speed = 3.5f;

    [SerializeField]
    private Vector2 defaultPositionRange = new Vector2(-4, 4);

    [SerializeField]
    private NetworkVariable<float> verticalPosition = new NetworkVariable<float>();

    [SerializeField]
    private NetworkVariable<float> horizontalPosition = new NetworkVariable<float>();

    private float previousVerticalPosition;
    private float previousHorizontalPosition;

    // Start is called before the first frame update
    void Start()
    {
        //random offset for spawn
        transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 0, Random.Range(defaultPositionRange.x, defaultPositionRange.y));
    }

    // Update is called once per frame
    void Update()
    {
        if(IsServer){
            UpdateServer();
        }
        if(IsClient && IsOwner){
            UpdateClient();
        }
    }

    void UpdateServer(){
        transform.position = new Vector3(
            transform.position.x + horizontalPosition.Value * Time.deltaTime, 
            transform.position.y, 
            transform.position.z + verticalPosition.Value * Time.deltaTime);
    }

    void UpdateClient(){
        float horizontal = Input.GetAxisRaw("Horizontal") * speed;
        float vertical = Input.GetAxisRaw("Vertical") * speed;

        //Vector3 moveDelta = new Vector3(x, y, 0);
        // magic number is speed multiplyer
        //movement.Move(moveDelta * 4);


        if (previousVerticalPosition != vertical ||
            previousHorizontalPosition != horizontal) {
            UpdateClientPositionServerRpc(vertical, horizontal);
            previousVerticalPosition = vertical;
            previousHorizontalPosition = horizontal;
        }
    }

    [ServerRpc]
    public void UpdateClientPositionServerRpc(float vertical, float horizontal) {
        verticalPosition.Value = vertical;
        horizontalPosition.Value = horizontal;
    }

    
}
