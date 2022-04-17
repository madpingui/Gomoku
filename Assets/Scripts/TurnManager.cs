using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    private PlayerController playerController;
    private EnemyController enemyController;
    private WinnerManager winnerManager;

    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private Image turnPieceImg;

    public bool playerTurn {private set; get;}

    private void Awake()
    {
        playerTurn = true;
        playerController = FindObjectOfType<PlayerController>();
        enemyController = FindObjectOfType<EnemyController>();
        winnerManager = FindObjectOfType<WinnerManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void ChangeTurn()
    {
        winnerManager.CheckWinner();

        if (playerController.isWin == true)
            return;

        playerTurn = !playerTurn;

        if (playerTurn)
        {
            playerController.gameObject.SetActive(true);
            turnText.text = "Your Turn";
            turnPieceImg.color = Color.black;
        }
        else
        {
            playerController.gameObject.SetActive(false);
            turnText.text = "AI Turn";
            turnPieceImg.color = Color.white;

            enemyController.AnalizePieceContainer();
        }
    }
}