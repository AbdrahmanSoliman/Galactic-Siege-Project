using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitBasePrefab = null;
    [SerializeField] private GameOverHandler gameoverHandlerPrefab = null;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    private bool isGameInProgress;

    public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

    #region Server

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (!isGameInProgress) return;
        // game is in progress already, then kick who tries to join during the game
        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }
    

    public override void OnStopServer()
    {
        Players.Clear();

        isGameInProgress = false;
    }

    public void StartGame()
    {
        if(Players.Count < 2) return;

        isGameInProgress = true;

        ServerChangeScene("Scene_Map_01");
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        // Setting random team color for each player
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        Players.Add(player);

        player.SetTeamColor(new Color(UnityEngine.Random.Range(0f,1f), UnityEngine.Random.Range(0f,1f), UnityEngine.Random.Range(0f,1f)));

        //player.SetDisplayName($"Player {Players.Count}");
        player.SetDisplayName(RandomPlayerName());

        // Set Party Owner if there is only one player in the lobby, which will be the host, happens at the beginning when a player start a lobby
        player.SetPartyOwner(Players.Count == 1);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if(SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameoverHandlerPrefab);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            foreach(RTSPlayer player in Players)
            {
                GameObject baseInstance = Instantiate(unitBasePrefab, GetStartPosition().position, Quaternion.identity);

                NetworkServer.Spawn(baseInstance, player.connectionToClient);
            }
        }
    }

    #endregion

    #region Client

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion

    private string RandomPlayerName()
    {
        string[] playerNames = {"Abdelrahman","Ramy","Snake","Abyss","Aether","Alpha","Apex","Archon","Ascendant","Bane","Chaos","Chimera","Chronos",
        "Crimson","Dawn","Eclipse","Erebus","Ember","Enigma","Equinox","Flux","Genesis","Hades","Indigo","Ion","Iris","Kairos","Kether","Leviathan",
        "Lux","Malice","Nebula","Nexus","Onyx","Paradox","Phoenix","Prime","Prism","Revenant","Rift","Sage","Sol","Tempest","Umbra","Valor","Vortex",
        "Void","Xerxes","Zenith"};

        int randomIndex = UnityEngine.Random.Range(0, playerNames.Length);

        return playerNames[randomIndex];
    }

}
