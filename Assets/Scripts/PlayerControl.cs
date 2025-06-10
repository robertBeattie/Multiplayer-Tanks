using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerControl : NetworkBehaviour
{
    public bool IsDead = false;

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
    [SerializeField] Transform bulletSpawn;
    [SerializeField] NetworkObject bulletPrefab;
    [SerializeField] GameObject trackPrefab;
    Vector3 lastTrack;
    Camera mainCamera;

    //bullet limit / reload
    int bulletLimit = 5;

    int bulletUsed = 0;
    float bulletCoolDown = 2.5f;
    float bulletTimer = 0;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        SceneManager.Instance.ActivePlayers.Add(this);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        SceneManager.Instance.ActivePlayers.Remove(this);
    }

    [SerializeField] List<Material> colors = new List<Material>(10);
	private void Awake() {
        mainCamera = Camera.main;
    }

	// Start is called before the first frame update
	void Start()
    {
        //random offset for spawn
        transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 0, Random.Range(defaultPositionRange.x, defaultPositionRange.y));
        int id = (int)OwnerClientId;
        int light = (id % (colors.Count / 2)) * 2;
        int dark = light +1;
        SetColor(colors[light],colors[dark]);

        //SpawnTrack();
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

        Vector3 moveDirection = new Vector3(horizontalPosition.Value, 0, verticalPosition.Value);
        if (moveDirection != Vector3.zero)
        {
            Quaternion piviotBottomToRoate = Quaternion.LookRotation(moveDirection);
            transform.GetChild(1).rotation = Quaternion.Slerp(transform.GetChild(1).rotation, piviotBottomToRoate, speed * Time.deltaTime);
        }

        
        
    }

    void UpdateClient(){
        PlayerMovement();
        PlayerMouseAim();
        PlayerShoot();
        LayTack();
    }

    void LayTack()
    {
        //lay down track if moved away from last track by some distance
        //lay Tacks
        //Lay down tacks
        float distance = Vector3.Distance(lastTrack, transform.GetChild(1).position);
        if (distance >= .3)
        {
            SpawnTrack();
        }
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
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("Ground")))
        {
            UpdateClientLookingRotationServerRpc(hit.point.x,hit.point.z);
        }
        else
        {
            UpdateClientLookingRotationServerRpc(barrelTransform.position.x, barrelTransform.position.z);
        }
    }

    void PlayerShoot() {
		if (Input.GetButtonDown("Fire1")) {
            if (bulletUsed < bulletLimit)
            {
                SpawnBulletServerRpc();
                bulletTimer = 0;
                bulletUsed++;
            }            
        }
        if (bulletTimer >= bulletCoolDown)
        {
            bulletUsed = 0;
            bulletTimer = 0;
        }
        bulletTimer += Time.deltaTime;
	}

    [ServerRpc]
    public void UpdateClientPositionServerRpc(float vertical, float horizontal) {
        verticalPosition.Value = vertical;
        horizontalPosition.Value = horizontal;
    }

    [ServerRpc]
    public void ReSpawnServerRpc() {
       transform.position = new Vector3(Random.Range(defaultPositionRange.x, defaultPositionRange.y), 0, Random.Range(defaultPositionRange.x, defaultPositionRange.y));;
    }

    [ServerRpc]
    public void UpdateClientLookingRotationServerRpc(float x, float y) {
        lookX.Value = x;
        lookY.Value = y;
    }


    [ServerRpc]
    public void SpawnBulletServerRpc() {
        NetworkObject bulletInstnace = Instantiate(bulletPrefab, bulletSpawn.position, barrelTransform.rotation);
        bulletInstnace.Spawn();

    }

    
    public void SpawnTrack() {
        GameObject track = Instantiate(trackPrefab, transform.position, transform.GetChild(1).rotation);
        lastTrack = track.transform.position;
    }
    

    void SetColor(Material bright, Material dark){
        //holder, head, barrel, cap
        //body, accent
        transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>().material = bright;
        transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Renderer>().material = dark;
        transform.GetChild(0).GetChild(0).GetComponent<Renderer>().material = dark;

        transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Renderer>().material = bright;
        transform.GetChild(1).GetChild(0).GetComponent<Renderer>().material = dark;


    }
    
}
