using Fusion.Sockets;
using Fusion;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    [SerializeField] Joystick _joystick;
    [SerializeField] Button _fire;
    [SerializeField] bool _isFiring;
    [SerializeField] Button _fire2;
    [SerializeField] bool _isFiring2;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    private void Start()
    {
        _fire.onClick.AddListener(() =>
        {
            //When Fire Button Clicked...
            _isFiring = _isFiring | !_isFiring;
        });

        _fire2.onClick.AddListener(() =>
        {
            _isFiring2 = _isFiring2 | !_isFiring2;
        });
    }

    private void OnGUI()
    {
        Rect hostButtonRect = new Rect((Screen.width / 2f) - (200 / 2), (Screen.height / 2f) - (40 / 2), 200, 40);
        Rect joinButtonRect = new Rect((Screen.width / 2f) - (200 / 2), (Screen.height / 2f) + (40 / 2), 200, 40);
        if (_runner == null)
        {
            if (GUI.Button(new Rect(Screen.width / 2.5f, Screen.height / 2.5f, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
            }
            if (GUI.Button(new Rect(Screen.width / 2.5f, Screen.height / 2.5f + 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
            }
        }
    }

    async void StartGame(GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.DefaultPlayers) * 3, 60, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Find and remove the players avatar
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (_joystick.Vertical > 0.1f)
            data.direction += Vector3.forward;

        if (_joystick.Vertical < -0.1f)
            data.direction += Vector3.back;

        if (_joystick.Horizontal < -0.1f)
            data.direction += Vector3.left;

        if (_joystick.Horizontal > 0.1f)
            data.direction += Vector3.right;

        if (_isFiring)
            data.buttons |= NetworkInputData.BUTTON1;
        _isFiring = false;
        if (_isFiring2)
            data.buttons |= NetworkInputData.BUTTON2;
        _isFiring2 = false;

        input.Set(data);
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) 
    {
        Debug.Log("Connection failed...");
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}