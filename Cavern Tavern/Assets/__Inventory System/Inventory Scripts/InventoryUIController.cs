using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    public DynamicInventoryDisplay chestInventoryPanel;
    public GameObject chestInventoryUI;
    public GameObject backpackUI;
    public GameObject shopUI;

    private void Awake()
    {
        chestInventoryUI.SetActive(false);
        backpackUI.SetActive(false);
    }

    private void OnEnable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayChestInventory;
        InventoryHolder.OnDynamicInventoryDisplayDestroy += HideAllInventory;
    }

    private void OnDisable()
    {
        InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayChestInventory;
        InventoryHolder.OnDynamicInventoryDisplayDestroy -= HideAllInventory;
    }

    void Update()
    {
        //Backpack();
    }

    void Backpack()
    {
        if (backpackUI.activeInHierarchy == false && Input.GetKeyDown(KeyCode.G)) //Opens Inventory UI
        {
            backpackUI.SetActive(true);
            TimeManager.PauseGame();
        }
        else if (backpackUI.activeInHierarchy == true && Input.GetKeyDown(KeyCode.G)) //Closes Inventory UI
        {
            backpackUI.SetActive(false);
            TimeManager.UnpauseGame();
        }
    }

    /// <summary>
    /// Displays Inventory UI
    /// </summary>
    public void DisplayBackpack()
    {
        backpackUI.SetActive(true);
        TimeManager.PauseGame();
    }

    /// <summary>
    /// Hides Inventory UI
    /// </summary>
    public void HideBackpack()
    {
        backpackUI.SetActive(false);
        TimeManager.UnpauseGame();
    }

    public void DisplayChestInventory(InventorySystem invToDisplay)
    {
        chestInventoryUI.SetActive(true);
        backpackUI.SetActive(true);
        chestInventoryPanel.RefreshDynamicInventory(invToDisplay);
        TimeManager.PauseGame();
    }

    public void HideAllInventory(InventorySystem invToDisplay)
    {
        chestInventoryUI.SetActive(false);
        backpackUI.SetActive(false);
        TimeManager.UnpauseGame();
    }


}
