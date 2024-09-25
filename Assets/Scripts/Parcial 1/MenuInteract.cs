using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuInteract : MonoBehaviour
{
    
    [SerializeField]private TMP_InputField MapX;
    [SerializeField]private TMP_InputField MapY;
    [SerializeField]private TMP_InputField TotalMines;
    [SerializeField]private TMP_InputField TotalMiners;
    [SerializeField]private TMP_InputField TotalCaravans;
   
    [SerializeField]private GameManager gameManager;
    
    [SerializeField]private GameObject Menu;
    [SerializeField]private GameObject UI;
    [SerializeField]private GameObject GrapfhView;

    public void StartGame()
    {
        int mapXValue = int.Parse(MapX.text);
        int mapYValue = int.Parse(MapY.text);
        int totalMinesValue = int.Parse(TotalMines.text);
        int totalMinersValue = int.Parse(TotalMiners.text);
        int totalCaravansValue = int.Parse(TotalCaravans.text);
        
        gameManager.StartGame(new Vector2Int(mapXValue, mapYValue), totalMinesValue, totalMinersValue, totalCaravansValue);
        Menu.SetActive(false);
        UI.SetActive(true);
        GrapfhView.SetActive(true); 
    }
}