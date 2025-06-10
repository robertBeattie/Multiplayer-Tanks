using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SceneManager : NetworkBehaviour
{
    //first index is scene, children is spawn locations
    public List<Transform> spawnLocationsPlayers;
    public List<Transform> spawnLocationsEnemys;
    public List<NetworkObject> ActiveEnemys;

    public List<PlayerControl> ActivePlayers;

    public int level = 0;

    public NetworkObject brown;
    public NetworkObject grey;
    public NetworkObject teal;
    public NetworkObject red;
    public NetworkObject yellow;
    public NetworkObject green;
    public NetworkObject purple;
    public NetworkObject white;
    public NetworkObject black;

    public static SceneManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

        if (level != 0 && AllPlayerDead())
        {
            RestartCurrentLevel();
        }
        else if (level != 0 && ActiveEnemys.Count == 0)
        {
            level++;
            switch (level)
            {
                case 1:
                    Level1();
                    break;
                case 2:
                    Level2();
                    break;
                case 3:
                    Level3();
                    break;
                case 4:
                    Level4();
                    break;
                case 5:
                    Level5();
                    break;
                case 6:
                    Level6();
                    break;
                case 7:
                    Level7();
                    break;
                case 8:
                    Level8();
                    break;
                case 9:
                    Level9();
                    break;
                case 10:
                    Level10();
                    break;
                default:
                    RandomLevel();
                    break;
            }
        }

        

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            Level0();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Level1();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Level2();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Level3();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Level4();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Level5();
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Level6();
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Level7();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Level8();
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Level9();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Level10();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RandomLevel();
        }


    }

    public bool AllPlayerDead() {
        foreach (PlayerControl player in ActivePlayers)
        {
            if (!player.IsDead)
            {
                return false;
            }
        }

        return true;
    }

    public void RestartCurrentLevel() {
        //spawn enemys
        for (int i = 0; i < spawnLocationsEnemys[level].childCount && i < ActiveEnemys.Count; i++)
        {
            ActiveEnemys[i].transform.position = spawnLocationsEnemys[level].GetChild(i).position;
        }

        //move players
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int spawnIndex = 0;
        foreach (GameObject player in players)
        {
            player.transform.position = spawnLocationsPlayers[level].GetChild(spawnIndex).position;
            spawnIndex++;
            if (spawnIndex >= spawnLocationsPlayers[level].childCount)
            {
                spawnIndex = 0;
            }
        }
        foreach (var player in ActivePlayers)
        {
            player.IsDead = false;
        }
    }

    //Lobby
    void Level0()
    {
        //despawn old enemys
        foreach (NetworkObject enemy in ActiveEnemys)
        {
            if (enemy != null)
            {
                enemy.Despawn(true);
            }

        }
        ActiveEnemys.Clear();

        //moveCamera
        Vector3 newCameraPosition = new Vector3(0, Camera.main.transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.position = newCameraPosition;

        //move players
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int spawnIndex = 0;
        foreach (GameObject player in players)
        {
            player.transform.position = spawnLocationsPlayers[0].GetChild(spawnIndex).position;
            spawnIndex++;
            if (spawnIndex >= spawnLocationsPlayers[0].childCount)
            {
                spawnIndex = 0;
            }
        }

        level = 0;
    }
    void Level1()
    {
        List<NetworkObject> enemys = new List<NetworkObject>();
        enemys.Add(brown);

        Level(1, enemys);
    }

    void Level2()
    {
        List<NetworkObject> enemys = new List<NetworkObject>();
        enemys.Add(grey);

        Level(2, enemys);
    }
    void Level3()
    {
        List<NetworkObject> enemys = new List<NetworkObject>();
        enemys.Add(grey);
        enemys.Add(brown);
        enemys.Add(grey);

        Level(3, enemys);
    }

    void Level4()
    {
        List<NetworkObject> enemys = new List<NetworkObject>();
        enemys.Add(teal);
        enemys.Add(teal);


        Level(4, enemys);
    }
    void Level5()
    {
        List<NetworkObject> enemys = new List<NetworkObject>();
        enemys.Add(grey);
        enemys.Add(teal);
        enemys.Add(teal);
        enemys.Add(grey);

        Level(5, enemys);
    }

    void Level6()
    {
        List<NetworkObject> enemys = new List<NetworkObject>();
        enemys.Add(teal);
        enemys.Add(teal);
        enemys.Add(teal);
        enemys.Add(teal);

        Level(6, enemys);
    }
    void Level7()
    {
        List<NetworkObject> enemys = new List<NetworkObject>();
        enemys.Add(yellow);
        enemys.Add(yellow);
        enemys.Add(yellow);
        enemys.Add(teal);
        enemys.Add(teal);

        Level(7, enemys);
    }

    void Level8()
    {
        List<NetworkObject> enemys = new List<NetworkObject>();
        enemys.Add(red);
        enemys.Add(red);
        Level(8, enemys);
    }

    void Level9()
    {
        List<NetworkObject> enemys = new List<NetworkObject>();
        enemys.Add(purple);
        enemys.Add(purple);
        enemys.Add(purple);
        enemys.Add(green);
        enemys.Add(green);

        Level(9, enemys);
    }


    void Level10()
    {
        List<NetworkObject> enemys = new List<NetworkObject>();
        enemys.Add(teal);
        enemys.Add(red);
        enemys.Add(purple);
        enemys.Add(purple);
        enemys.Add(green);
        enemys.Add(teal);

        Level(10, enemys);
    }



    void RandomLevel()
    {
        List<NetworkObject> enemys = new List<NetworkObject>();
        enemys.Add(brown);
        enemys.Add(grey);
        enemys.Add(teal);

        enemys.Add(red);
        enemys.Add(yellow);
        enemys.Add(green);
        enemys.Add(purple);

        enemys.Add(black);
        enemys.Add(white);

        List<NetworkObject> randEnemys = new List<NetworkObject>();

        for (int i = 0; i < 15; i++)
        {
            randEnemys.Add(enemys[Random.Range(0, enemys.Count)]);
        }

        Level(Random.Range(0, spawnLocationsEnemys.Count), randEnemys);
    }
    void Level(int level, List<NetworkObject> enemys)
    {
        this.level = level;
        //despawn old enemys
        foreach (NetworkObject enemy in ActiveEnemys)
        {
            if (enemy != null)
            {
                enemy.Despawn(true);
            }

        }
        ActiveEnemys.Clear();

        //spawn enemys
        for (int i = 0; i < spawnLocationsEnemys[level].childCount; i++)
        {
            NetworkObject enemy = Instantiate(enemys[i], spawnLocationsEnemys[level].GetChild(i).position, Quaternion.identity);
            enemy.Spawn();
            ActiveEnemys.Add(enemy);
        }

        //moveCamera
        Vector3 newCameraPosition = new Vector3(level * 30, Camera.main.transform.position.y, Camera.main.transform.position.z);
        Camera.main.transform.position = newCameraPosition;

        //move players
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int spawnIndex = 0;
        foreach (GameObject player in players)
        {
            player.transform.position = spawnLocationsPlayers[level].GetChild(spawnIndex).position;
            spawnIndex++;
            if (spawnIndex >= spawnLocationsPlayers[level].childCount)
            {
                spawnIndex = 0;
            }
        }
        foreach (var player in ActivePlayers)
        {
            player.IsDead = false;
        }
    }

    public void RemoveEnemy(NetworkObject networkObject)
    {
        if (ActiveEnemys.Contains(networkObject))
        {
            ActiveEnemys.Remove(networkObject);
        }
    }
}
