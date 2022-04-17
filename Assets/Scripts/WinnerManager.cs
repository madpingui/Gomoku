using TMPro;
using UnityEngine;

public class WinnerManager : MonoBehaviour
{
    private const int MINY = -7, MAXY = 7, MINX = -7, MAXX = 7;

    private PlayerController playerController;

    [SerializeField] private Transform pieceContainer;
    [SerializeField] private TextMeshProUGUI winnerText;

    private RaycastHit2D piecesChecker;

    private int piecesRowPos;
    private int lenghtRow;

    private void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    public void CheckWinner()
    {
        //Check if there is a row of 5 or more

        for (int x = 0; x < pieceContainer.childCount; x++)
        {
            Transform childTransform = pieceContainer.GetChild(x);
            string pieceColor = childTransform.gameObject.tag;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        //Cant analize itself
                    }
                    else
                    {
                        piecesChecker = Physics2D.Raycast(new Vector2(childTransform.position.x, childTransform.position.y) + new Vector2(i, j), Vector2.zero, Mathf.Infinity);

                        if (piecesChecker.point.y >= MINY && piecesChecker.point.y <= MAXY && piecesChecker.point.x >= MINX && piecesChecker.point.x <= MAXX)
                        {
                            if (piecesChecker.collider.tag == pieceColor)
                            {
                                bool match = true;
                                piecesRowPos = 0;
                                lenghtRow = 1;

                                while (match == true)
                                {
                                    piecesRowPos++;
                                    piecesChecker = Physics2D.Raycast(new Vector2(childTransform.position.x, childTransform.position.y) + new Vector2(i * piecesRowPos, j * piecesRowPos), Vector2.zero, Mathf.Infinity);
                                    if (piecesChecker.point.y >= MINY && piecesChecker.point.y <= MAXY && piecesChecker.point.x >= MINX && piecesChecker.point.x <= MAXX)
                                    {
                                        if (piecesChecker.collider.tag == pieceColor)
                                        {
                                            lenghtRow++;

                                            if (lenghtRow >= 5)
                                            {
                                                Winner(pieceColor);
                                            }
                                        }
                                        else
                                        {
                                            match = false;
                                        }
                                    }
                                    else
                                    {
                                        match = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void Winner(string ganador)
    {
        playerController.isWin = true;
        winnerText.text = ganador + " Wins!!";
        winnerText.transform.parent.gameObject.SetActive(true);
    }

    public void Draw()
    {
        playerController.isWin = true;
        winnerText.text = "Draw";
        winnerText.transform.parent.gameObject.SetActive(true);
    }
}