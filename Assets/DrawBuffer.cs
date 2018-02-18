using UnityEngine;

class DrawBuffer : MonoBehaviour
{
    [SerializeField, HideInInspector] Shader _shader;
    [SerializeField, HideInInspector] ComputeShader _inputCompute;

    Mesh _mesh;
    Material _material;
    ComputeBuffer _triangleBuffer;

    ComputeBuffer _positionInputBuffer;
    ComputeBuffer _texCoordInputBuffer;
    ComputeBuffer _indexInputBuffer;

    const int kThreadGroupSize = 64;
    const int kTrianglesPerThreadGroup = 64;//256 + 64;
    const int kTriangleCount = kThreadGroupSize * kTrianglesPerThreadGroup;
    const int kVertexCount = 3 * kTriangleCount;
    const int kTriangleDataSize = 4 * (4 * 3 + 2 * 3 + 2 + 3 + 1);

    #region Public methods

    public void AddTriangles(Vector3[] vertices, Vector2[] uvs, int[] indices)
    {
        // Lazy initialization of input buffer.
        if (_positionInputBuffer == null)
            _positionInputBuffer = new ComputeBuffer(kVertexCount, 3 * 4);

        if (_texCoordInputBuffer == null)
            _texCoordInputBuffer = new ComputeBuffer(kVertexCount, 2 * 4);

        if (_indexInputBuffer == null)
            _indexInputBuffer = new ComputeBuffer(kVertexCount, 4);

        _positionInputBuffer.SetData(vertices);
        _texCoordInputBuffer.SetData(uvs);
        _indexInputBuffer.SetData(indices);

        _inputCompute.SetBuffer(0, "PositionInput", _positionInputBuffer);
        _inputCompute.SetBuffer(0, "TexCoordInput", _texCoordInputBuffer);
        _inputCompute.SetBuffer(0, "IndexInput", _indexInputBuffer);
        _inputCompute.SetInt("IndexCount", indices.Length);
        _inputCompute.SetFloat("Time", Time.time);

        _triangleBuffer.SetCounterValue(0);
        _inputCompute.SetBuffer(0, "Output", _triangleBuffer);

        _inputCompute.Dispatch(0, kTrianglesPerThreadGroup, 1, 1);
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        _mesh = CreateNullMesh();
        _material = new Material(_shader);

        _triangleBuffer = new ComputeBuffer(
            kTriangleCount, kTriangleDataSize,
            ComputeBufferType.Append
        );
    }

    void OnDestroy()
    {
        Destroy(_mesh);
        Destroy(_material);
        _triangleBuffer.Release();

        if (_positionInputBuffer != null) _positionInputBuffer.Release();
        if (_texCoordInputBuffer != null) _texCoordInputBuffer.Release();
        if (_indexInputBuffer != null) _indexInputBuffer.Release();
    }

    void Update()
    {
        _material.SetBuffer("_TriangleBuffer", _triangleBuffer);

        Graphics.DrawMesh(
            _mesh, transform.localToWorldMatrix,
            _material, gameObject.layer
        );
    }

    #endregion

    #region Private methods

    Mesh CreateNullMesh()
    {
        var indices = new int[kVertexCount];
        for (var i = 0; i < kVertexCount; i++) indices[i] = i;

        var mesh = new Mesh();
        mesh.vertices = new Vector3[kVertexCount];
        mesh.triangles = indices;
        mesh.UploadMeshData(true);
        return mesh;
    }

    #endregion
}
