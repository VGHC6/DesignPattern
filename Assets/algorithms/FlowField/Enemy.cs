using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        FlowFieldManager manager = FlowFieldManager.Instance;
        if (manager == null || manager.grid == null || manager.target == null) return;

        int cellX = Mathf.FloorToInt(transform.position.x / manager.cellSize);
        int cellY = Mathf.FloorToInt(transform.position.y / manager.cellSize);

        FlowFieldGridNode node = manager.grid.GetNode(cellX, cellY);
        if (node == null) return;

        Vector3 dir = node.direction;
        if (dir.sqrMagnitude < 0.001f) return; // 已到达目标所在格子

        transform.position += dir.normalized * speed * Time.deltaTime;
    }
}
