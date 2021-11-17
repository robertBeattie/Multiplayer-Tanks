using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerControl : NetworkBehaviour
{
    [SerializeField]
    private float speed = 3.5f;
    [SerializeField] private Vector2 defaultPositionRange = new Vector2(-4, 4);
    [SerializeField] private NetworkVariable<float> verticalPosition = new NetworkVariable<float>();
    [SerializeField] private NetworkVariable<float> horizontalPosition = new NetworkVariable<float>();

    [SerializeField] private NetworkVariable<float> lookX = new NetworkVariable<float>();
    [SerializeField] private NetworkVariable<float> lookY = new NetworkVariable<float>();

    private float previousVerticalPosition;
    private float previousHorizontalPosition;

    [SerializeField] Transform barrelTransform;

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

        barrelTransform.LookAt(new Vector3(lookX.Value, barrelTransform.position.y, lookY.Value));

        //lay down track if moved away from last track by some distance

        
    }

    void UpdateClient(){
        PlayerMovement();
        PlayerMouseAim();
    }

    void PlayerMovement(){
        float horizontal = Input.GetAxisRaw("Horizontal") * speed;
        float vertical = Input.GetAxisRaw("Vertical") * speed;
    
        if (previousVerticalPosition != vertical ||
            previousHorizontalPosition != horizontal) {
            UpdateClientPositionServerRpc(vertical, horizontal);
            previousVerticalPosition = vertical;
            previousHorizontalPosition = horizontal;
        }
    }

    void PlayerMouseAim(){
        RaycastHit hit;
        Ray ray;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f))
        {
            UpdateClientLookingRotationServerRpc(hit.point.x,hit.point.z);
        }
        else
        {
            UpdateClientLookingRotationServerRpc(barrelTransform.position.x, barrelTransform.position.z);
        }
    }

    [ServerRpc]
    public void UpdateClientPositionServerRpc(float vertical, float horizontal) {
        verticalPosition.Value = vertical;
        horizontalPosition.Value = horizontal;
    }

    [ServerRpc]
    public void UpdateClientLookingRotationServerRpc(float x, float y) {
        lookX.Value = x;
        lookY.Value = y;
    }

    
}
