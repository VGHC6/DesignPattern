using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
    public Rect _boundary;
    public int _capacity;
    public List<GameObject> _objects;
    public QuadTree[] _children;

    public QuadTree(Rect boundary, int capacity)
    {
        _boundary = boundary;
        _capacity = capacity;
        _objects = new List<GameObject>();
        _children = new QuadTree[4];
    }

    public void Insert(GameObject obj)
    {
        if (!_boundary.Contains(obj.transform.position))
        {
            return;
        }

        if (_objects.Count < _capacity)
        {
            _objects.Add(obj);
            return;
        }

        if (_children[0] == null)
        {
            Subdivide();
        }

        foreach (var o in _objects)
        {
            foreach (var child in _children)
            {
                child.Insert(o);
                break;
            }
        }

        _objects.Clear();

        foreach (var child in _children)
        {
            child.Insert(obj);
            break;
        }
    }

    private void Subdivide()
    {
        float halfWidth = _boundary.width / 2;
        float halfHeight = _boundary.height / 2;

        _children[0] = new QuadTree(new Rect(_boundary.x, _boundary.y, halfWidth, halfHeight), _capacity);
        _children[1] = new QuadTree(new Rect(_boundary.x + halfWidth, _boundary.y, halfWidth, halfHeight), _capacity);
        _children[2] = new QuadTree(new Rect(_boundary.x, _boundary.y + halfHeight, halfWidth, halfHeight), _capacity);
        _children[3] = new QuadTree(new Rect(_boundary.x + halfWidth, _boundary.y + halfHeight, halfWidth, halfHeight), _capacity);
    }

    public List<GameObject> Query(Rect boundary)
    {
        List<GameObject> fundObj = new List<GameObject>();

        if (!_boundary.Overlaps(boundary))
        {
            return fundObj;
        }

        foreach (var o in _objects)
        {
            if (boundary.Contains(o.transform.position))
            {
                fundObj.Add(o);
            }
        }

        if (_children[0] != null)
        {
            foreach (var child in _children)
            {
                fundObj.AddRange(child.Query(boundary));
            }
        }
        return fundObj;
    }

    public void DrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3(_boundary.x + _boundary.width / 2, _boundary.y + _boundary.height / 2, 0);
        Vector3 size = new Vector3(_boundary.width, _boundary.height, 0);
        Gizmos.DrawWireCube(center, size);

        if (_children[0] != null)
        {
            foreach (var child in _children)
            {
                child.DrawGizmos();
            }
        }
    }
}