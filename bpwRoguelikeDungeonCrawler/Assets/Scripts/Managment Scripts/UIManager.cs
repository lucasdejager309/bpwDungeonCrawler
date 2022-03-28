using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public UIInventory inventory;
    public UIPanel inventoryCard;
    public UIPanel escMenu;
    public UIPanel deathMenu;
    public UIHotKeys hotKeys;
    public UISlider healthSlider;
    public UIInspectPanel inspectPanel;
    public Text bigAnnouncementText;
    public AimPointer aimpointer;

    

    void Awake() {
        Instance = this;
        EventManager.AddListener("UI_TOGGLE_WAIT", ToggleWait);
        EventManager.AddListener("UI_UPDATE_HEALTH", UpdateHealthBar);
        EventManager.AddListener("UIUPDATE_INVENTORY", UpdateInventory);
    }

    public void UpdateStats() {
        inventory.STR.text = "STR: " + GameManager.Instance.player.GetComponent<Player>().Strength.ToString();
        inventory.INT.text = "INT: " + GameManager.Instance.player.GetComponent<Player>().Intelligence.ToString();
    }

    public void DisplayAnnouncement(string announcement, int showDuration = 1) {
        bigAnnouncementText.gameObject.SetActive(true);
        bigAnnouncementText.text = announcement;

        Task t = new Task(FadeText(bigAnnouncementText, showDuration));
        t.Finished += delegate {
            bigAnnouncementText.gameObject.SetActive(false);
        };
    }

    public IEnumerator FadeText(Text text, float t)
    {
        if (text != null) {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
            while (text.color.a > 0.0f)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / t));
                yield return null;
            }
        }
    }

    void UpdateInventory() {
        inventory.UpdateInventory();
    }

    void UpdateHealthBar() {
        Player player = GameManager.Instance.player.GetComponent<Player>();
        healthSlider.SetMaxValue(player.MaxHealth);
        healthSlider.SetValue(player.Health);
    }

    //loading icon during opponent moves
    void ToggleWait() {
        Image wait = GameObject.FindGameObjectWithTag("UIWait").GetComponent<Image>();
        wait.enabled = !wait.enabled;
    }

    
}
