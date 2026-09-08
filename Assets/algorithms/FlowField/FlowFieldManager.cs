using UnityEngine;

public class FlowFieldManager : MonoBehaviour
{
    public int gridWidth = 30;   // 列数
    public int gridHeight = 30;  // 行数
    public float cellSize = 0.1f;  // 每个格子在世界坐标中的大小
    public bool autoSetupDemo = true; //场景里缺目标/敌人时自动生成,保证直接可跑
    public static FlowFieldManager Instance;
    public FlowFieldGrid grid;
    public Transform target;

    public GameObject[] obstaclePrefab;//障碍物预制体(场景里的实际物体)
    public float obstacleClearance = 1f; // 障碍物四周额外堵死的格子圈数,越大敌人离障碍物越远

    void Awake() => Instance = this;

    void Start()
    {
        grid = CreateGrid();

        // 开箱即用:场景里没目标就自动造一个,没有敌人就自动生成一批
        if (autoSetupDemo)
        {
            if (target == null) target = CreateTargetVisual();
            if (FindObjectsOfType<Enemy>().Length == 0)
                SpawnDemoEnemies();
        }

        UpdateFlowField();
    }

    void Update() => UpdateFlowField();

    void UpdateFlowField()
    {
        if (target == null || grid == null) return;

        // 世界坐标(2D XY平面)转网格坐标,并限制在网格范围内
        int tx = Mathf.Clamp(Mathf.FloorToInt(target.position.x / cellSize), 0, gridWidth - 1);
        int ty = Mathf.Clamp(Mathf.FloorToInt(target.position.y / cellSize), 0, gridHeight - 1);
        FlowFieldGridNode targetNode = grid.GetNode(tx, ty);
        if (targetNode == null) return;

        grid.SetTarget(targetNode);
        grid.GenerateFlowField();
    }

    // 网格坐标(cx,cy) -> 格子中心的世界坐标(XY平面)
    public Vector3 CellToWorld(int cx, int cy)
    {
        return new Vector3((cx + 0.5f) * cellSize, (cy + 0.5f) * cellSize, 0);
    }

    FlowFieldGrid CreateGrid()
    {
        bool[,] map = new bool[gridWidth, gridHeight];
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                map[i, j] = true;
            }
        }

        for (int i = 0; i < obstaclePrefab.Length; i++)
            BlockObstacle(map, obstaclePrefab[i]);

        return new FlowFieldGrid(gridWidth, gridHeight, map);
    }

    // 按障碍物在世界中的实际大小,把它覆盖的所有格子(再外扩 obstacleClearance 圈)标为不可走
    void BlockObstacle(bool[,] map, GameObject obstacle)
    {
        if (obstacle == null) return;

        Vector3 pos = obstacle.transform.position;
        Vector2 size = ObstacleWorldSize(obstacle);
        float halfX = size.x * 0.5f + obstacleClearance * cellSize;
        float halfY = size.y * 0.5f + obstacleClearance * cellSize;

        int minX = Mathf.Max(0, Mathf.FloorToInt((pos.x - halfX) / cellSize));
        int maxX = Mathf.Min(gridWidth - 1, Mathf.FloorToInt((pos.x + halfX) / cellSize));
        int minY = Mathf.Max(0, Mathf.FloorToInt((pos.y - halfY) / cellSize));
        int maxY = Mathf.Min(gridHeight - 1, Mathf.FloorToInt((pos.y + halfY) / cellSize));

        for (int cx = minX; cx <= maxX; cx++)
            for (int cy = minY; cy <= maxY; cy++)
                map[cx, cy] = false;
    }

    // 取障碍物在世界坐标中的实际尺寸(支持Sprite/Cube等,自动含缩放)
    Vector2 ObstacleWorldSize(GameObject obstacle)
    {
        Renderer renderer = obstacle.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            Vector3 s = renderer.bounds.size;
            return new Vector2(s.x, s.y);
        }
        Collider2D collider = obstacle.GetComponentInChildren<Collider2D>();
        if (collider != null)
        {
            Vector3 s = collider.bounds.size;
            return new Vector2(s.x, s.y);
        }
        return Vector2.one * cellSize; // 兜底:只堵障碍物所在的一格
    }

    Transform CreateTargetVisual()
    {
        return MakeCube("Target", CellToWorld(gridWidth - 2, gridHeight - 2), 0.7f * cellSize, Color.red).transform;
    }

    void SpawnDemoEnemies()
    {
        int count = 8;
        for (int i = 0; i < count; i++)
        {
            int cx = 2 + (i * 3) % (gridWidth - 4);
            int cy = 2 + (i / 3) * 2;
            GameObject go = MakeCube("Enemy" + i, CellToWorld(cx, cy), 0.5f * cellSize, Color.blue);
            go.AddComponent<Enemy>();
        }
    }

    GameObject MakeCube(string name, Vector3 pos, float size, Color color)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * size;
        go.GetComponent<Renderer>().material.color = color;
        return go;
    }
}
