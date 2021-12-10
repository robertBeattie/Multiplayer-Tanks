using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
public class Enemy : NetworkBehaviour
{
    public enum Movement { Stationary, Slow, Normal, Fast };
    public Movement movement;
    private float movementSpeed = 0f;

    //Behavuour:
    //Passive looks around randomly 
    //Defensive move away from player
    //Offensive move towards player
    //Active scans for player from angles
    public enum Behaviour { Passive, Defensive, Offensive, Active };
    public Behaviour behaviour;

    public enum AimBehaviour { Random, Scan, Direct };
    public AimBehaviour aimBehaviour;

    //Bullet Slow or Rocket Fast
    public enum BulletType { Bullet, Rocket };
    public BulletType bullet;
    public NetworkObject bulletPreFab;
    public NetworkObject RocketPreFab;
    private NetworkObject bulletObject;

    //How often they fire
    public enum FireRate { Slow, Fast };
    public FireRate fireRate;
    private float fireRateSpeed = 0f;

    //number of walls bullets can bounce off before destroying
    [SerializeField] private int ricochetLimit = 0;
    [SerializeField] private int aimLimit = 0;

    //number of bullets that can be shot before reloading
    [SerializeField] private int bulletLimit = 0;
    private int bulletfired = 0;

    //Tracks
    /*
    public GameObject trackPreFab;
    private Vector3 lastTrack;
    */

    //Navmesh
    NavMeshAgent agent;
    //Aim
    [SerializeField] float scanSpeed = 60f;

    //
    private GameObject[] players;
    private GameObject playerTarget;
    private GameObject piviotTop;
    private GameObject piviotBottom;
    public LayerMask layerMask;
    public Transform bulletSpawnLocation;
    private float fireRateCooldown = 1;
    GameObject centerRot;
    bool aimDir = false;

    public float wanderRadius;
    public float wanderTimer;
    private float timer;
    private float randomflipTimer = 0.1f;
    private float mineLayTimer = 3.5f;
    Vector3 finalPosition;
    private bool scanPause = false;

    //Calls before Start
    private void Awake() {
 
        agent = GetComponent<NavMeshAgent>();
        SetBulletType(bullet);
        SetFireRate(fireRate);
        SetMovmentSpeed(movement);
        piviotTop = transform.GetChild(0).gameObject;
        piviotBottom = transform.GetChild(1).gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
   
        centerRot = Instantiate(new GameObject("centerRot"), this.transform);
        //InitialTrack();
        timer = wanderTimer;
        aimDir = (Random.value > 0.5f);
        randomflipTimer = Random.Range(0.01f, 1f);
        finalPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsServer){
            FindPlayers();
            fireRateCooldown = fireRateCooldown - Time.deltaTime;
            GetBehavior(behaviour);
            //LayTracks();
        }
        
    }

    void FindPlayers(){
        if(playerTarget == null || Vector3.Distance(playerTarget.transform.position, transform.position) > 25){
            players = GameObject.FindGameObjectsWithTag("Player");
            playerTarget = players[Random.Range(0,players.Length)];
        }
        
        
    }

    void SetMovmentSpeed(Movement mv)
    {
        if(mv == Movement.Stationary)
        {
            movementSpeed = 0;
        }
        else if (mv == Movement.Slow)
        {
            movementSpeed = 1.5f;
        }
        else if (mv == Movement.Normal)
        {
            movementSpeed = 2.5f;
        }
        else if (mv == Movement.Fast)
        {
            movementSpeed = 5;
        }
        agent.speed = movementSpeed;
    }

    void GetBehavior(Behaviour bh)
    {
        if (bh == Behaviour.Passive)
        {
            //No Movment
            //PassiveAiming
            AimRandom(scanSpeed);
        }
        else if (bh == Behaviour.Defensive)
        {
            //Move away from player
            MovmentDefensive(wanderRadius);
            timer = 0;
            //Aiming center at player and scan back and forth
        }
        else if (bh == Behaviour.Offensive)
        {
            //Move towards player
            MovmentOffensive();
            //Look at player direct
           
        }
        else if (bh == Behaviour.Active)
        {
            //No Movment
            //multi reflex aiming
            
        }
        GetBehavior(aimBehaviour);
        AimReflexRay(bulletSpawnLocation.position, transform.GetChild(0).TransformDirection(Vector3.forward), aimLimit);
    }

    void GetBehavior(AimBehaviour ab)
    {
        if (ab == AimBehaviour.Random)
        {
            AimRandom(scanSpeed);
        }
        else if (ab == AimBehaviour.Scan)
        {
            AimAtPlayerScan(60, 100);
        }
        else if (ab == AimBehaviour.Direct)
        {
            AimAtPlayer();
        }

    }

    void SetBulletType(BulletType b)
    {
        if (b == BulletType.Bullet)
        {
            bulletObject = bulletPreFab;
        }
        else if (b == BulletType.Rocket)
        {
            bulletObject = RocketPreFab;
        }
        bulletObject.GetComponent<BulletController>().ricochetLimit = ricochetLimit;
    }

    void SetFireRate(FireRate fr)
    {
        if (fr == FireRate.Slow)
        {
            fireRateSpeed = 3;
        }
        else if (fr == FireRate.Fast)
        {
            fireRateSpeed = 1;
        }
       
    }

    //first Ray
    void AimReflexRay(Vector3 hitIn, Vector3 angle, int numReflections)
    {
        //reflection
        RaycastHit hitOut;
        //numReflections--;
        if (Physics.SphereCast(piviotTop.transform.position, 0.6f, piviotTop.transform.TransformDirection(Vector3.forward), out hitOut, Mathf.Infinity, layerMask))
        {
            if (hitOut.transform.gameObject.tag != "Enemy")
            {
                if (hitOut.transform.gameObject.tag == "Player")
                {
                    Debug.DrawRay(piviotTop.transform.position, piviotTop.transform.TransformDirection(Vector3.forward) * hitOut.distance, Color.white);
                    fireBullet();
                    scanPause = true;
                }
                else
                {
                    Debug.DrawRay(piviotTop.transform.position, piviotTop.transform.TransformDirection(Vector3.forward) * hitOut.distance, Color.red);
                    if (numReflections > 0)
                    {
                        AimReflexRay(hitOut, angle, numReflections);
                    }
                    scanPause = false;
                }
            }
            else
            {
                Debug.DrawRay(piviotTop.transform.position, piviotTop.transform.TransformDirection(Vector3.forward) * hitOut.distance, Color.green);
            }
        }
        
    }

    //Recursive Rays
    void AimReflexRay(RaycastHit hitIn, Vector3 angle, int numReflections)
    {
        //reflection
        numReflections--;
        RaycastHit hitOut;
        Vector3 angleOut = Vector3.Reflect(angle, hitIn.normal);
        if (Physics.SphereCast(hitIn.point, 0.6f, angleOut, out hitOut, Mathf.Infinity, layerMask))
        {
            if (hitOut.transform.gameObject.tag != "Enemy")
            {
                if (hitOut.transform.gameObject.tag == "Player")
                {
                    Debug.DrawRay(hitIn.point, angleOut * hitOut.distance, Color.white);
                    fireBullet();
                    scanPause = true;
                }
                else
                {
                    Debug.DrawRay(hitIn.point, angleOut * hitOut.distance, Color.blue);
                    if (numReflections > 0)
                    {
                        AimReflexRay(hitOut, angleOut, numReflections);
                    }
                    scanPause = false;
                }
            }
            else
            {
                Debug.DrawRay(hitIn.point, angleOut * hitOut.distance, Color.green);
            }
        }
    }

    void fireBullet()
    {
        
        if (fireRateCooldown <= 0)
        {
            
            SpawnBulletServerRpc();
            bulletfired++;
            if (bulletfired < bulletLimit)
            {
                fireRateCooldown = 0.35f; 
            }
            else
            {
                fireRateCooldown = fireRateSpeed;
                bulletfired = 0;
            }
                     
        }
    }

    
    [ServerRpc]
    public void SpawnBulletServerRpc() {
        NetworkObject bulletInstnace = Instantiate(bulletObject, bulletSpawnLocation.position, piviotTop.transform.rotation);
        bulletInstnace.Spawn();

    }

    void AimAtPlayerScan(float scanRange, float scanSpeed)
    {
        if(players.Length > 0 && playerTarget != null){
            if(scanPause) { return; }
            //center of the rot on lookatplayer
            Vector3 targetPostion = new Vector3(playerTarget.transform.position.x, piviotTop.transform.position.y, playerTarget.transform.position.z);
            centerRot.transform.LookAt(targetPostion);

            //piviotTop.transform.LookAt(targetPostion);
            //current look is greater than left or right limit rotate the other way
            Vector3 aimArms = new Vector3(0, Vector3.Angle(piviotTop.transform.forward, centerRot.transform.forward), 0);
            if (Vector3.SignedAngle(piviotTop.transform.forward, centerRot.transform.forward, Vector3.up) >= scanRange)
            {
                aimDir = false;
            }
            if (Vector3.SignedAngle(piviotTop.transform.forward, centerRot.transform.forward, Vector3.up) <= -scanRange)
            {
                aimDir = true;
            }
            if (aimDir)
            {
                piviotTop.transform.Rotate(new Vector3(0, scanSpeed * -Time.deltaTime, 0));
            }
            else
            {
                piviotTop.transform.Rotate(new Vector3(0, scanSpeed * Time.deltaTime, 0));
            }
        }else{
            AimRandom(scanSpeed);
        }
        
    }

    void AimAtPlayer()
    {
        if(players.Length > 0 && playerTarget != null){
        //center of the rot on lookatplayer
            Vector3 targetPostion = new Vector3(playerTarget.transform.position.x, piviotTop.transform.position.y, playerTarget.transform.position.z);
            piviotTop.transform.LookAt(targetPostion);
        }else{
            AimRandom(scanSpeed);
        }
    }

    void AimRandom(float scanspeed)
    {
        timer += Time.deltaTime;
        if (timer > randomflipTimer)
        {
            aimDir = !aimDir;
            randomflipTimer = Random.Range(0.1f, 1.5f);
            timer = 0;
        } 

        if (aimDir)
        {
            piviotTop.transform.Rotate(new Vector3(0, 60 * -Time.deltaTime, 0));
        }
        else
        {
            piviotTop.transform.Rotate(new Vector3(0, 60 * Time.deltaTime, 0));
        }
    }

     void MovmentOffensive()
    {
        if(players.Length > 0 && playerTarget != null){
            agent.SetDestination(playerTarget.transform.position);
        }else{
            MovmentDefensive(wanderRadius);
        }
        
    }

    void MovmentDefensive(float radius) 
    {
        agent.stoppingDistance = 0;
        if (Vector3.Distance(finalPosition, piviotBottom.transform.position) <= 2)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += transform.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
            {
                finalPosition = hit.position;
            }
            agent.SetDestination(finalPosition);
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        return navHit.position;
    }


    /*
    void InitialTrack()
    {
        lastTrack = Instantiate(trackPreFab, transform.position, transform.rotation).transform.position;
    }

    void LayTracks()
    {
        if (Vector3.Distance(lastTrack, piviotBottom.transform.position) >= .3)
        {
            GameObject track = Instantiate(trackPreFab, piviotBottom.transform.position, piviotBottom.transform.rotation);
            lastTrack = track.transform.position;
        }
    }
    */

    
}
