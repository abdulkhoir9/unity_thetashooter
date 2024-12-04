using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFieldOfView : MonoBehaviour
{
    [SerializeField]
    private LayerMask _layerMask;
    private Mesh _mesh;
    private float _fieldOfView;
    private Vector3 _origin;
    private float _startingAngle;

    private void Start()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        _fieldOfView = 90;
        _origin = Vector3.zero;
    }

    private void LateUpdate()
    {
        int rayCount = 100;
        float angle = _startingAngle;
        float angleIncrease = _fieldOfView / rayCount;
        float viewDistance = 7.5f;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = _origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex = _origin + (GetVectorFromAngle(angle) * viewDistance);
            RaycastHit2D raycastHit2D = Physics2D.Raycast(_origin, GetVectorFromAngle(angle), viewDistance, _layerMask);

            if (raycastHit2D.collider == null)
            {
                vertex = _origin + (GetVectorFromAngle(angle) * viewDistance);
            }
            else
            {
                vertex = raycastHit2D.point;
            }

            vertices[vertexIndex] = vertex;

            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;

        _mesh.RecalculateBounds();
    }

    public void SetOrigin(Vector3 origin)
    {
        this._origin = origin;
    }

    public void SetViewDirection(Vector3 lookDirection)
    {
        _startingAngle = GetAngleFromVectorFloat(lookDirection) + (_fieldOfView / 2);
    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180);

        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    private float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        return angle;
    }
}
