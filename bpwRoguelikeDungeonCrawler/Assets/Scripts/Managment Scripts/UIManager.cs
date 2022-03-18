using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public UIInventory inventory;
    public UIPanel inventoryCard;
    public UIPanel escMenu;
    public UISlider healthSlider;

    public AimPointer aimpointer;

    void Awake() {
        Instance = this;
        EventManager.AddListener("UI_TOGGLE_WAIT", ToggleWait);
        EventManager.AddListener("UI_UPDATE_STATS", UpdateHealthBar);
        EventManager.AddListener("PLAYER_SPAWNED", UpdateHealthBar);
    }

    void UpdateHealthBar() {
        Player player = GameManager.Instance.player.GetComponent<Player>();
        healthSlider.SetMaxValue(player.MaxHealth);
        healthSlider.SetValue(player.Health);
    }

    public void UpdateStats() {
        inventory.STR.text = "STR: " + GameManager.Instance.player.GetComponent<Player>().Strength.ToString();
        inventory.INT.text = "INT: " + GameManager.Instance.player.GetComponent<Player>().Inteligence.ToString();
    }

    //loading icon during opponent moves
    void ToggleWait() {
        Image wait = GameObject.FindGameObjectWithTag("UIWait").GetComponent<Image>();
        wait.enabled = !wait.enabled;
    }
}
