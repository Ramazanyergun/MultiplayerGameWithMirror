using Mirror;
using TMPro;
using UnityEngine;
using Steamworks;

namespace Player
{
    public class PlayerScript : NetworkBehaviour
    {
        [SyncVar(hook = nameof(HandleSteamIDUpdated))]
        private ulong steamId;

        [SerializeField] private TextMeshProUGUI nameText;

        public void SetSteamID(ulong steamID)
        {
            this.steamId = steamID;
        }

        private void HandleSteamIDUpdated(ulong oldSteamId, ulong newSteamId)
        {
            var cSteamID = new CSteamID(newSteamId);
            nameText.text = SteamFriends.GetFriendPersonaName(cSteamID);
        }

        public override void OnStartClient()
        {
            DontDestroyOnLoad(gameObject);
            base.OnStartClient();
        }
    }
}