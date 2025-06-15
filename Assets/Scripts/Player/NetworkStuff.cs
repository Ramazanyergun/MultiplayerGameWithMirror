using System;
using Mirror;
using UnityEngine;

namespace Player
{
    public class NetworkStuff : NetworkBehaviour
    {
        [SerializeField] private GameObject fpsCamera = null, tpMesh = null, tpModelWeapon = null;

        private void Start()
        {
            if (isLocalPlayer)
            {
                fpsCamera.SetActive(true);
                tpMesh.SetActive(false);
                tpModelWeapon.SetActive(false);
            }
            else
            {
                fpsCamera.SetActive(false);
                tpMesh.SetActive(true);
                tpModelWeapon.SetActive(true);
            }
        }

        private void Update()
        {
            if (!isLocalPlayer)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
                LeaveGame();
        }

        public void LeaveGame()
        {
            if (isServer)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }
        }
    }
}