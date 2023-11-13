
using UnityEngine;

public class Lighsaber : MonoBehaviour
{
    private const int numVertices = 12;

    [SerializeField] private GameObject blade;
    [SerializeField] private GameObject tip;
    [SerializeField] private GameObject slashBase;
    [SerializeField] private GameObject meshParent;
    [SerializeField] private int trailFrameLength = 3;


    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private int frameCount;
    private Vector3 previousTipPosition;
    private Vector3 previousBasePosition;

    private void Awake()
    {
        trailFrameLength = Application.targetFrameRate / 12;
    }

    private void Start()
    {
        InitializeMesh();
        SetInitialPositions();
    }

    private void InitializeMesh()
    {
        meshParent.transform.position = Vector3.zero;
        mesh = new Mesh();
        meshParent.GetComponent<MeshFilter>().mesh = mesh;
        vertices = new Vector3[trailFrameLength * numVertices];
        triangles = new int[vertices.Length];
    }

    private void SetInitialPositions()
    {
        previousTipPosition = tip.transform.position;
        previousBasePosition = slashBase.transform.position;
    }

    private void LateUpdate()
    {
        UpdateVerticesAndTriangles();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        frameCount += numVertices;
    }

    private void UpdateVerticesAndTriangles()
    {
        Vector3 slashBasePos = slashBase.transform.position;
        Vector3 tipPos = tip.transform.position;

        if (frameCount == trailFrameLength * numVertices)
            frameCount = 0;

        Vector3[] vertexPattern =
        {
            slashBasePos, tipPos, previousTipPosition,
            slashBasePos, previousTipPosition, tipPos,
            previousTipPosition, slashBasePos, previousBasePosition,
            previousTipPosition, previousBasePosition, slashBasePos
        };

        for (int i = 0; i < vertexPattern.Length; i++)
        {
            vertices[frameCount + i] = vertexPattern[i];
            triangles[frameCount + i] = frameCount + i;
        }

        previousTipPosition = tipPos;
        previousBasePosition = slashBasePos;
    }
}
