using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Steamworks;

namespace Player
{
    public class FpsNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject enterAdressPanel, landingPage, lobbyUI;
        [SerializeField] private TMP_InputField addressField;
        [SerializeField] private GameObject startGameButton;

        public List<PlayerScript> playersList = new List<PlayerScript>();
        [SerializeField] private GameObject playerGO;
        [SerializeField] private bool isUsedSteam = true;

        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> gameLobyJoinRequested;
        protected Callback<LobbyEnter_t> lobbyEntered;

        public static CSteamID lobbyID;

        private void Start()
        {
            if (!isUsedSteam)
                return;
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }


        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            CSteamID steamID = SteamMatchmaking.GetLobbyMemberByIndex(lobbyID, numPlayers - 1);
            var playerScript = conn.identity.GetComponent<PlayerScript>();
            playerScript.SetSteamID(steamID.m_SteamID);

            PlayerScript playerStartPrefab = conn.identity.GetComponent<PlayerScript>();
            playersList.Add(playerStartPrefab);
            if (playersList.Count == 2)
            {
                startGameButton.SetActive(true);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            PlayerScript playerStartPrefab = conn.identity.GetComponent<PlayerScript>();
            playersList.Remove(playerStartPrefab);
            startGameButton.SetActive(false);
        }

        public void HostLobby()
        {
            landingPage.SetActive(false);
            if (isUsedSteam)
            {
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 2);
                return;
            }

            singleton.StartHost();
        }

        public void JoinButton()
        {
            enterAdressPanel.SetActive(true);
            landingPage.SetActive(false);
        }

        public void JoinLobby()
        {
            singleton.networkAddress = addressField.text;
            singleton.StartClient();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            lobbyUI.SetActive(true);
            enterAdressPanel.SetActive(false);
            landingPage.SetActive(false);
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            SceneManager.LoadScene(0);
            landingPage.SetActive(true);
            lobbyUI.SetActive(false);
            enterAdressPanel.SetActive(false);
        }

        public override void OnStopHost()
        {
            base.OnStopHost();
            SceneManager.LoadScene(0);
        }

        public void LeaveLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                singleton.StopHost();
            }
            else
            {
                singleton.StopClient();
            }
        }

        public void StartGame()
        {
            ServerChangeScene("GameScene");
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);
            if (SceneManager.GetActiveScene().name.StartsWith("GameScene"))
            {
                foreach (PlayerScript player in playersList)
                {
                    var connectionTC = player.connectionToClient;
                    GameObject playerPrefab = Instantiate(playerGO, GetStartPosition().transform.position,
                        Quaternion.identity);
                    NetworkServer.ReplacePlayerForConnection(connectionTC, playerPrefab);
                    Destroy(player.gameObject);
                }
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                landingPage.SetActive(true);
                return;
            }

            singleton.StartHost();
            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostIP",
                SteamUser.GetSteamID().ToString());
        }

        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            lobbyID = new CSteamID(callback.m_ulSteamIDLobby);

            if (NetworkServer.active)
                return;
            string hostIP = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostIP");
            singleton.networkAddress = hostIP;
            singleton.StartClient();
            landingPage.SetActive(false);
        }

        public void CloseAdressPanel()
        {
            enterAdressPanel.SetActive(false);
            landingPage.SetActive(true);
        }
    }
}