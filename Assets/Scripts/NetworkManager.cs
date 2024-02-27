using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static NetworkManager Instance { get; private set; }


    private NetworkRunner _runner;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private NetworkPrefabRef _stagePrefab;
    // private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private Dictionary<PlayerRef, PlayerObjects> _spawnedCharacters = new Dictionary<PlayerRef, PlayerObjects>();
    int i = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
        
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Create a unique position for the player
            Vector3 playerSpawnPosition = new Vector3(0, 0, i*-50f);
            Vector3 stageSpawnPosition = new Vector3(-12, 0, i * -50f);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, playerSpawnPosition, Quaternion.Euler(0,-90,0), player);
            NetworkObject networkStageObject = runner.Spawn(_stagePrefab, stageSpawnPosition, Quaternion.identity, player);
            networkPlayerObject.GetComponent<PlayerManager>().GetPos(networkStageObject.GetComponent<StageManager>().leftPos, networkStageObject.GetComponent<StageManager>().centrePos, networkStageObject.GetComponent<StageManager>().rightPos);
            networkPlayerObject.GetComponent<PlayerNetworkMovement>().GetPos(networkStageObject.GetComponent<StageManager>().leftPos, networkStageObject.GetComponent<StageManager>().centrePos, networkStageObject.GetComponent<StageManager>().rightPos);
            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, gameObject.AddComponent<PlayerObjects>());
            i++;
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Check if the player exists in the dictionary
        if (_spawnedCharacters.TryGetValue(player, out PlayerObjects playerObjects))
        {
            // Despawn the player object
            if (playerObjects.PlayerObject != null)
            {
                runner.Despawn(playerObjects.PlayerObject);
            }

            // Despawn the stage object
            if (playerObjects.StageObject != null)
            {
                runner.Despawn(playerObjects.StageObject);
            }
        }
            // Remove the player from the dictionary
            _spawnedCharacters.Remove(player);

            /*if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                _spawnedCharacters.Remove(player);
            }*/
    }
    public static NetworkInputData bufferedInput = new NetworkInputData();
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        input.Set(bufferedInput);
        
    }







    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

}
