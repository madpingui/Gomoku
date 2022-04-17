using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private const int MINY = -7, MAXY = 7, MINX = -7, MAXX = 7;

    [SerializeField] private GameObject finalObject;
    [SerializeField] private Transform pieceContainer;

    private TurnManager turnManager;
    private WinnerManager winnerManager;
    private RaycastHit2D piecesChecker;

    private List<Vector2> fourRowPlacesBlack = new List<Vector2>();
    private List<Vector2> threeRowPlacesBlack = new List<Vector2>();
    private List<Vector2> twoRowPlacesBlack = new List<Vector2>();

    private List<Vector2> fourRowPlacesWhite = new List<Vector2>();
    private List<Vector2> threeRowPlacesWhite = new List<Vector2>();
    private List<Vector2> twoRowPlacesWhite = new List<Vector2>();

    private List<Vector2> possiblePlaces = new List<Vector2>();

    private bool canPlace;
    private int piecesRowPos;
    private int lenghtRow;

    private void Awake()
    {
        turnManager = FindObjectOfType<TurnManager>();
        winnerManager = FindObjectOfType<WinnerManager>();
    }

    public void AnalizePieceContainer()
    {
        CleanAI();
        for (int x = pieceContainer.childCount; x > 0; x--)
        {
            Transform childTransform = pieceContainer.GetChild(x - 1);
            AnalizeBoard(childTransform, childTransform.gameObject.tag);
        }
        Invoke("PlacePiece", 1);
    }

    public void AnalizeBoard(Transform childTransform, string pieceColor)
    {
        //gather information of row lenghts

        string pieceToAnalize = "";
        string enemyPieceToAnalize = "";

        pieceToAnalize = pieceColor;

        if (pieceColor == "Black")
            enemyPieceToAnalize = "White";
        else if(pieceColor == "White")
            enemyPieceToAnalize = "Black";

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
                        if (piecesChecker.collider.tag == "Board" && pieceToAnalize == "Black")
                        {
                            possiblePlaces.Add(piecesChecker.point);
                            canPlace = true;
                        }
                        else if (piecesChecker.collider.tag == pieceToAnalize)
                        {
                            //Check for same pieces in a row

                            bool match = true;
                            piecesRowPos = 0;
                            lenghtRow = 1;

                            while (match == true)
                            {
                                piecesRowPos++;
                                piecesChecker = Physics2D.Raycast(new Vector2(childTransform.position.x, childTransform.position.y) + new Vector2(i * piecesRowPos, j * piecesRowPos), Vector2.zero, Mathf.Infinity);
                                if (piecesChecker.point.y >= MINY && piecesChecker.point.y <= MAXY && piecesChecker.point.x >= MINX && piecesChecker.point.x <= MAXX)
                                {
                                    if (piecesChecker.collider.tag == pieceToAnalize)
                                    {
                                        lenghtRow++;
                                    }
                                    else if (piecesChecker.collider.tag == enemyPieceToAnalize)
                                    {
                                        piecesChecker = Physics2D.Raycast(new Vector2(childTransform.position.x, childTransform.position.y) + new Vector2(-i, -j), Vector2.zero, Mathf.Infinity);

                                        if (piecesChecker.point.y >= MINY && piecesChecker.point.y <= MAXY && piecesChecker.point.x >= MINX && piecesChecker.point.x <= MAXX)
                                        {
                                            if (piecesChecker.collider != null)
                                            {
                                                if (piecesChecker.collider.tag == "Board")
                                                {
                                                    CategorizeRowLenghts(pieceToAnalize);
                                                }
                                                match = false;
                                            }
                                        }
                                    }
                                    else if (piecesChecker.collider.tag == "Board")
                                    {
                                        CategorizeRowLenghts(pieceToAnalize);
                                        match = false;
                                    }
                                }
                                else
                                {
                                    if (piecesChecker.collider.tag == "Board")
                                    {
                                        piecesChecker = Physics2D.Raycast(new Vector2(childTransform.position.x, childTransform.position.y) + new Vector2(-i, -j), Vector2.zero, Mathf.Infinity);

                                        if (piecesChecker.point.y >= MINY && piecesChecker.point.y <= MAXY && piecesChecker.point.x >= MINX && piecesChecker.point.x <= MAXX)
                                        {
                                            if (piecesChecker.collider != null)
                                            {
                                                if (piecesChecker.collider.tag == "Board")
                                                {
                                                    CategorizeRowLenghts(pieceToAnalize);
                                                }
                                            }
                                        }
                                    }
                                    match = false;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void CategorizeRowLenghts(string pieceAnalized)
    {
        //Categorize rows by lenght

        if(pieceAnalized == "Black")
        {
            switch (lenghtRow)
            {
                case 2:
                    twoRowPlacesBlack.Add(piecesChecker.point);
                    break;
                case 3:
                    threeRowPlacesBlack.Add(piecesChecker.point);
                    break;
                case 4:
                    fourRowPlacesBlack.Add(piecesChecker.point);
                    break;
            }
        }
        else if(pieceAnalized == "White")
        {
            switch (lenghtRow)
            {
                case 2:
                    twoRowPlacesWhite.Add(piecesChecker.point);
                    break;
                case 3:
                    threeRowPlacesWhite.Add(piecesChecker.point);
                    break;
                case 4:
                    fourRowPlacesWhite.Add(piecesChecker.point);
                    break;
            }
        }
    }

    public void PlacePiece()
    {
        //make a decition to attack or defend depending on number of lenght row of the player
        if (canPlace)
        {
            if (fourRowPlacesBlack.Count > 0)
            {
                if(fourRowPlacesWhite.Count > 0)
                {
                    Instantiate(finalObject, fourRowPlacesWhite[0], Quaternion.identity, pieceContainer);
                }
                else
                {
                    Instantiate(finalObject, fourRowPlacesBlack[0], Quaternion.identity, pieceContainer);
                }
                turnManager.ChangeTurn();
                return;

            }
            else if (threeRowPlacesBlack.Count > 0)
            {
                if(fourRowPlacesWhite.Count > 0)
                {
                    Instantiate(finalObject, fourRowPlacesWhite[0], Quaternion.identity, pieceContainer);
                }
                else if (threeRowPlacesWhite.Count > 0)
                {
                    Instantiate(finalObject, threeRowPlacesWhite[0], Quaternion.identity, pieceContainer);
                }
                else
                {
                    Instantiate(finalObject, threeRowPlacesBlack[0], Quaternion.identity, pieceContainer);
                }
                turnManager.ChangeTurn();
                return;
            }
            else if (twoRowPlacesBlack.Count > 0)
            {
                if (fourRowPlacesWhite.Count > 0)
                {
                    Instantiate(finalObject, fourRowPlacesWhite[0], Quaternion.identity, pieceContainer);
                }
                else if (threeRowPlacesWhite.Count > 0)
                {
                    Instantiate(finalObject, threeRowPlacesWhite[0], Quaternion.identity, pieceContainer);
                }
                else if (twoRowPlacesWhite.Count > 0)
                {
                    Instantiate(finalObject, twoRowPlacesWhite[0], Quaternion.identity, pieceContainer);
                }
                else
                {
                    Instantiate(finalObject, twoRowPlacesBlack[0], Quaternion.identity, pieceContainer);
                }
                turnManager.ChangeTurn();
                return;
            }
            else
            {
                Instantiate(finalObject, possiblePlaces[Random.Range(0, possiblePlaces.Count)], Quaternion.identity, pieceContainer);
                turnManager.ChangeTurn();
                return;
            }
        }
        else
        {
            winnerManager.Draw();
            return;
        }
    }

    public void CleanAI()
    {
        possiblePlaces.Clear();
        fourRowPlacesBlack.Clear();
        threeRowPlacesBlack.Clear();
        twoRowPlacesBlack.Clear();
        fourRowPlacesWhite.Clear();
        threeRowPlacesWhite.Clear();
        twoRowPlacesWhite.Clear();
        canPlace = false;
    }
}