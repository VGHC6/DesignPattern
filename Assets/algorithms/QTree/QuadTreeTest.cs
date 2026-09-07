using System.Collections.Generic;
using UnityEngine;


public class QuadTreeTest : MonoBehaviour
{
    private QuadTree _quadTree;
    public float _range = 5;
    public Rect _sceneBounds = new Rect(-50, -50, 100, 100);
    public int _capacity = 4;

    void Start()
    {
        RebuildQuadTree();
    }

    void Update()
    {
        FollowMouse();
        RebuildQuadTree();

        foreach (var sr in FindObjectsOfType<SpriteRenderer>())
        {
            sr.color = Color.white;
        }

        Rect playerRange = new Rect(transform.position.x - _range, transform.position.y - _range, _range * 2, _range * 2);
        List<GameObject> nearbyObjects = _quadTree.Query(playerRange);

        Debug.Log("查询到对象数量: " + nearbyObjects.Count);

        foreach (var obj in nearbyObjects)
        {
            if (obj != gameObject && IsColliding(obj))
            {
                Debug.Log("Collision detected with " + obj.name);
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = Color.red;
                }
            }
        }
        DrawRect(playerRange, Color.red);
    }

    void RebuildQuadTree()
    {
        _quadTree = new QuadTree(_sceneBounds, _capacity);

        foreach (var sr in FindObjectsOfType<SpriteRenderer>())
        {
            if (sr.gameObject != gameObject)
            {
                _quadTree.Insert(sr.gameObject);
            }
        }
    }

    bool IsColliding(GameObject other)
    {
        return Vector3.Distance(transform.position, other.transform.position) < _range;
    }

    void DrawRect(Rect rect, Color color)
    {
        Vector3 bottomLeft = new Vector3(rect.x, rect.y, 0);
        Vector3 bottomRight = new Vector3(rect.x + rect.width, rect.y, 0);
        Vector3 topLeft = new Vector3(rect.x, rect.y + rect.height, 0);
        Vector3 topRight = new Vector3(rect.x + rect.width, rect.y + rect.height, 0);

        Debug.DrawLine(bottomLeft, bottomRight, color);
        Debug.DrawLine(bottomRight, topRight, color);
        Debug.DrawLine(topRight, topLeft, color);
        Debug.DrawLine(topLeft, bottomLeft, color);
    }

    void FollowMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane + 10;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;
        transform.position = worldPos;
    }

    void OnDrawGizmos()
    {
        if (_quadTree != null)
        {
            _quadTree.DrawGizmos();
        }
    }
}