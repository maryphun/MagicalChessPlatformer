using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhotonRoomMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text roomCodeText;
    [SerializeField] private Image[] playerSlots;
    [SerializeField] private TMP_Text[] playerNamesText;

    public void RoomInitiate(string RoomCode)
    {
        roomCodeText.text = "Room Code: " + RoomCode;
    }

    public void SetPlayerSlotName(int index, string name)
    {
        playerNamesText[index-1].text = name;
    }

    public void SetPlayerSlotColor(int index, Color textColor, Color boxColor)
    {
        playerNamesText[index - 1].color = textColor;
        playerSlots[index - 1].color = boxColor;
    }
}
