using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const int MINY = -7, MAXY = 7, MINX = -7, MAXX = 7;

    [HideInInspector] public bool isWin;

    [SerializeField] private GameObject finalObject;
    [SerializeField] private Transform pieceContainer;

    private TurnManager turnManager;
    private RaycastHit2D piecesChecker;
    private Vector2 mousePos;

    private void Awake()
    {
        turnManager = FindObjectOfType<TurnManager>();
    }

    void Update()
    {
        if (isWin)
            return;

        if (!turnManager.playerTurn)
            return;

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        float xPos = Mathf.Clamp(mousePos.x, MINX, MAXX);
        float yPos = Mathf.Clamp(mousePos.y, MINY, MAXY);

        transform.position = new Vector2(Mathf.Round(xPos), Mathf.Round(yPos));

        if (Input.GetMouseButtonDown(0))
        {
            //Place black piece

            piecesChecker = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity);

            if (piecesChecker.collider.tag == "Board")
            {
                Instantiate(finalObject, transform.position, Quaternion.identity, pieceContainer);
                turnManager.ChangeTurn();
            }
            else
            {
                Debug.Log("Fuera del tablero / ya hay una pieza");
            }
        }
    }
}