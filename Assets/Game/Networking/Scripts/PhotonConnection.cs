using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Custom.Indicators;

public class PhotonConnection : PunBehaviour
{
    #region SINGLETON
    private static PhotonConnection _instance;
    public static PhotonConnection GetInstance()
    {
        return _instance;
    }

    private void Awake()
    {
        
        _instance = this;
    }
    #endregion

    public string gameVersion = "1.0";
    public string myServerName = "NATOSPHERE";

    public GameObject ownPlayer;

    public List<Player> playerList;

    public OffscreenIndicator indicators;

    //WEAPON ID
    private int weaponId;
    private int consumableId;
    public int WeaponID
    {
        get { return weaponId; }
        set { weaponId = value; }
    }

    public int ConsumableID
    {
        get { return consumableId; }
        set { consumableId = value; }
    }

    //ENEMIES
    public List<GameObject> enemiesList;
    int maxEnemies = 3;
    WaitForSeconds timeBeforeSpawing = new WaitForSeconds(3.0f);
    WaitForEndOfFrame waitForFrame = new WaitForEndOfFrame();
    public List<GameObject> patterPointsList;
    //SPAWNERS
    public List<WeaponPickup> weaponList;
    public List<Consumable> consumables;
    public TerrainGenerator terreno;
    

    public string prefabACrear;
    
    ///
    public int randomSeed;
    int playerWithLessEnemies = 0;
    void Start()
    {
        prefabACrear = Savenick.nombrePJ;
        weaponId = 0;
        playerList = new List<Player>();
        Connect();
        
            
        
    }

    public void Connect()
    {
        PhotonNetwork.gameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings(gameVersion);
    }

    public override void OnConnectedToPhoton()
    {
        Debug.Log("Conectado al Master");
        StartCoroutine(ConnectLobby());
    }

    IEnumerator ConnectLobby()
    {
        yield return new WaitForSeconds(1f);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Conectado al lobby");
        PhotonNetwork.playerName = myServerName;
        StartCoroutine(ConnectRoom());
    }

    IEnumerator ConnectRoom()
    {
        yield return new WaitForSeconds(2f);
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].IsOpen)
            {
                PhotonNetwork.JoinRoom(rooms[i].Name);
                yield break;//return de la corrutina
            }
        }
        CreateRoom();
    }

    public IEnumerator WaitFrame()
    {
        yield return waitForFrame;
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
       
        CreateRoom();
    }

    void CreateRoom()
    {
        randomSeed = Random.Range(0, 9999999);
        Debug.Log("NO HAY CUARTOS, CREANDO UNO");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "Dificultad", 0 }, { "SeedMapa", randomSeed } };
        PhotonNetwork.CreateRoom(myServerName, roomOptions, TypedLobby.Default);
         ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "Dificultad", 0 }, { "SeedMapa", randomSeed } };
        //CrearMapa(randomSeed);
    }


    public override void OnJoinedRoom()
    {

        
        Debug.Log("Conectado al cuarto:" + PhotonNetwork.room.Name);
        if (PhotonNetwork.isMasterClient)
        {
            // randomSeed = Random.Range(0, 9999999);
            //METODO CREAR MAPA
              ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "Dificultad", 0 }, { "SeedMapa", randomSeed } };
            //  Debug.Log(customRoomProperties["SeedMapa"].ToString());
            //ExitGames.Client.Photon.Hashtable customRoomProperties = PhotonNetwork.room.CustomProperties;
            randomSeed = (int)customRoomProperties["SeedMapa"];

            Debug.Log(customRoomProperties["SeedMapa"].ToString());

            //CrearMapa((int)customRoomProperties["SeedMapa"]);
        }
        else
        {
            
            ExitGames.Client.Photon.Hashtable customRoomProperties = PhotonNetwork.room.CustomProperties;
            Debug.Log(customRoomProperties.Count);
            Debug.Log(customRoomProperties["SeedMapa"].ToString());



            randomSeed = (int)customRoomProperties["SeedMapa"];



            //CrearMapa((int)customRoomProperties["SeedMapa"]);
            //Debug.Log((int)customRoomProperties["Dificultad"]);

        }

        //Debug.Log(terreno.PlayerSpawners.Count);
        //int randSpawn = Random.Range(0, terreno.PlayerSpawners.Count);

        if (prefabACrear != null)
            ownPlayer = PhotonNetwork.Instantiate(prefabACrear, new Vector3 (5,0.8f,5), Quaternion.identity, 0) as GameObject;
        else
            ownPlayer = PhotonNetwork.Instantiate("PlayerNetIvan", new Vector3(5, 0.8f, 5), Quaternion.identity, 0) as GameObject;

        //ownPlayer = PhotonNetwork.Instantiate("PlayerNet",terreno.PlayerSpawners[randSpawn].transform.position, Quaternion.identity, 0) as GameObject;

        /* int i = 0;
         foreach (PhotonPlayer p in PhotonNetwork.playerList)
         {



             indicators.AddTarget(ownPlayer);


         }*/


        patterPointsList = new List<GameObject>();
        enemiesList = new List<GameObject>();

        StartCoroutine(SpawnEnemy());
    }

    /*[PunRPC]
    public void CrearFlecha()
    {
        Debug.Log("Crear flecha", ownPlayer);
        indicators.AddTarget(ownPlayer);
    }*/

    IEnumerator SpawnEnemy()
    {
        
        bool shouldICreateENemy = false;
        shouldICreateENemy = (enemiesList.Count < maxEnemies);
        yield return new WaitForSeconds(3.0f);
        if (shouldICreateENemy)
        {
            int randSpawn = Random.Range(0, terreno.EnemySpawners.Count);
            enemiesList.Add(PhotonNetwork.Instantiate("Enemy", terreno.EnemySpawners[randSpawn].transform.position, Quaternion.identity, 0) as GameObject);
            patterPointsList.Add(PhotonNetwork.Instantiate("EnemyPatrollingPoints_1", terreno.EnemySpawners[randSpawn].transform.position, Quaternion.identity, 0) as GameObject);
     
            for (int i = 0; i < enemiesList.Count; i++)
            {
                enemiesList[i].GetComponent<EnemyIA>().patternPoint = patterPointsList[i].gameObject.transform;
            }
        }
        else
        {

            //Reciclo un enemigo ya creadoanteriormente


            for (int i = 0; i < enemiesList.Count; i++)
            {

                if (!enemiesList[i].activeSelf)
                {

                    int randSpawn = Random.Range(0, terreno.EnemySpawners.Count);

                    object[] parameters2 = new object[3];

                    parameters2[0] = i;
                    parameters2[1] = randSpawn;
                    parameters2[2] = terreno.EnemySpawners[randSpawn].transform.position;
                    randSpawn = Random.Range(0, terreno.EnemySpawners.Count);
                    patterPointsList[i].transform.position = terreno.EnemySpawners[randSpawn].transform.position;
                    PrepareRPCForDRevive(parameters2);

                    break;
                }
            }
        }
        StartCoroutine(SpawnEnemy());
        //PhotonView photonView = PhotonView.Get(this);
        //photonView.RPC("CheckPlayersEnemies", PhotonTargets.All,enemiesList,patterPointsList);
    }

    public void PrepareRPCForDRevive(object[] parameters)
    {
  
        StartCoroutine(RPCForEnemyRevive(parameters));
    }

    public IEnumerator RPCForEnemyRevive(object[] parameters)
    {
     
        yield return new WaitForEndOfFrame();
        PhotonNetwork.RPC(enemiesList[(int)parameters[0]].GetComponent<PhotonView>(), "ReviveEnemy", PhotonTargets.AllBuffered, false, parameters);
        //COLOCAR EL RPC EN ENEMY IA, ENTONCTRAR LA MANERA DE PASAR LOS PARAMETROS DE ENEMILIST Y DEMÁS AL SCRIPOT DE ENEMYAI
    }
    
    /* [PunRPC]
     public void ReviveEnemy(object[] parameters)
     {
         int i = (int)parameters[0];
         int randSpawn = (int)parameters[1];
         enemiesList[i].SetActive(true);
         enemiesList[i].transform.position = terreno.EnemySpawners[randSpawn].transform.position;
         randSpawn = Random.Range(0, terreno.EnemySpawners.Count);
         patterPointsList[i].transform.position = terreno.EnemySpawners[randSpawn].transform.position;
     }
     */
    [PunRPC]
    public void RevivePlayer()
    {
        ownPlayer.SetActive(true);
    }

    public void CrearMapa()
    {
        
        Random.InitState(randomSeed);
    }

    public Player GetPlayerById(int id)
    {
       
        for (int i = 0; i < playerList.Count; i++)
        {
            Debug.Log("Id In List: " + playerList[i].photonView.viewID);

            if (id == playerList[i].photonView.viewID)
            {
                Debug.Log("Found culprit!");
                return playerList[i];
            }
        }
        Debug.Log("Found Nothing!");
        return null;
    }
    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        
        Debug.Log("Nuevo Jugador:" + newPlayer.NickName);

        


            


        

    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        
        Debug.Log("Jugador se desconectó:" + otherPlayer.NickName);
    }



    public override void OnLeftRoom()
    {
        
        enemiesList.Clear();
        patterPointsList.Clear();
        terreno.EnemySpawners.Clear();
        terreno.PlayerSpawners.Clear();
        StopAllCoroutines();
        base.OnLeftRoom();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
    }
}