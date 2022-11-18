using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Linq;
using TMPro;
namespace Assets.Scripts
{
    public class EnterNameUI : MonoBehaviour
    {
        [SerializeField] private InputField _inputField;
        private RoomPlayer _roomPlayer;
        private LobbyManager _lobbyManager;
        // Use this for initialization
        void Start()
        {
            _inputField.onSubmit.AddListener(onValue);
            _lobbyManager = NetworkManager.singleton as LobbyManager;
        }


        private void onValue(string name)
        {
            int id = NetworkClient.connection.connectionId;

            if (_roomPlayer == null)
            {
                _roomPlayer = _lobbyManager.roomSlots.First(slot => slot.isOwned) as RoomPlayer;
            }
            _roomPlayer.CommandSetName(name);
        }

    }
}