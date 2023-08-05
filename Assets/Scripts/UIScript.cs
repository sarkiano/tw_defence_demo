using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScript : MonoBehaviour {
    [SerializeField] private TMP_Text goldLabel;
    [SerializeField] private TMP_Text healthLabel;
    [SerializeField] private TMP_Text enemiesKilledLabel;
    [SerializeField] private TMP_Text UpgradeCost;
    [SerializeField] private Image endWindow;
    [SerializeField] private Image upgradeWindow;

    private static GameObject lastTower;
    public static UIScript Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        }
        else {
            Instance = this;
        }
        endWindow.gameObject.SetActive(false);
        upgradeWindow.gameObject.SetActive(false);
    }

    private void OnGUI() {
        goldLabel.text = GM.money.ToString();
        healthLabel.text = Mathf.Round(GM.castleHealth).ToString();
    }

    public void UpgradeMenu(GameObject tower) {
        upgradeWindow.gameObject.SetActive(true);
        UpgradeCost.text = tower.GetComponent<Tower>().upgradeCost.ToString();
        lastTower = tower;
    }

    public void UpgradeTower() {
        lastTower.GetComponent<Tower>().UpgradeTower();
        CloseUpgradeMenu();
    }

    public void CloseUpgradeMenu() {
        upgradeWindow.gameObject.SetActive(false);
    }

    public void RestartMenu() {
        endWindow.gameObject.SetActive(true);
        enemiesKilledLabel.text = $"Game Over\r\n{GM.totalEnemiesKilled} greenskins killed";
        Time.timeScale = 0;
    }
}
