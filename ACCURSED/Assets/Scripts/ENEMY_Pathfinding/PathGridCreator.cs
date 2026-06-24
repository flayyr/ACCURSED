 using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PathGridCreator : MonoBehaviour
{
    [Header("Grid Properties")]
    [SerializeField] int gridSizeX = 10;
    [SerializeField] int gridSizeY = 10;
    [SerializeField] float cellSize = 1f;
    [SerializeField] LayerMask obstacleLayers;

    [Header("Movement")]
    [SerializeField] bool diagonalMovement = false;
    [SerializeField] bool squareAreaDetection = false;

    [Header("Debug")]
    [SerializeField] public bool showGridDebug = true;

    //Debug Stuff
    private TextMesh[,] debugTextArray;

    private Pathfinding pathfinding;

    #region Debug Text
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor anchor)
    {
        GameObject gameObject = new GameObject("WorldText", typeof(TextMesh));
        Transform trans = gameObject.transform;
        trans.SetParent(parent, false);
        trans.localPosition = localPosition;
        TextMesh mesh = gameObject.GetComponent<TextMesh>();
        mesh.anchor = anchor;
        mesh.text = text;
        mesh.color = color;
        mesh.fontSize = fontSize;
        return mesh;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + transform.position;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // THIS USES THE POSITION OF THIS OBJECT AS THE ORIGIN OF THE GRID!!!
        debugTextArray = new TextMesh[gridSizeX, gridSizeY];
        pathfinding = new Pathfinding(gridSizeX, gridSizeY, cellSize, transform.position, diagonalMovement);
        pathfinding.obstacleLayers = obstacleLayers;
        pathfinding.SQUARE_WALKABLITY_DETECTION = squareAreaDetection;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (showGridDebug)
        {
            //pathfinding.GetGrid().GizmoPrint(true,true,true);
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Gizmos.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1));
                    Gizmos.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y));
                }
            }
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * gridSizeX * cellSize);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * gridSizeY * cellSize);
            Gizmos.DrawLine(transform.position + Vector3.up * gridSizeY * cellSize, (transform.position + Vector3.up * gridSizeY * cellSize) + Vector3.right * gridSizeX * cellSize);
            Gizmos.DrawLine(transform.position + Vector3.right * gridSizeX * cellSize, (transform.position + Vector3.right * gridSizeX * cellSize) + Vector3.up * gridSizeY * cellSize);
        }
    }
}
