using UnityEngine;

class FaceRenderer : MonoBehaviour
{
    [SerializeField] Mesh _mesh;

    Vector3[] _vertices;
    Vector2[] _uvs;
    int[] _indices;

    void Start()
    {
        _vertices = _mesh.vertices;
        _uvs = _mesh.uv;
        _indices = _mesh.triangles;
    }

    void Update()
    {
        GetComponent<DrawBuffer>().AddTriangles(_vertices, _uvs, _indices);
    }
}
