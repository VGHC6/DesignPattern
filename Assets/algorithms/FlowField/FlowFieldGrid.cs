using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
//由FlowFieldGridNode组成的网格图
public class FlowFieldGrid
{
    private readonly int x;
    private readonly int y;
    //网格节点字典
    public readonly Dictionary<int, FlowFieldGridNode> grid = new Dictionary<int, FlowFieldGridNode>();

    public FlowFieldGrid(int x, int y, bool[,] map)
    {
        this.x = x;
        this.y = y;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                int index = i * this.y + j;//计算位置,与GetNode保持一致(x*this.y + y)
                //把节点加入字典中
                grid.Add(index, new FlowFieldGridNode(i, j, map[i, j]));
            }
        }
    }

    public FlowFieldGridNode GetNode(int x, int y)
    {
        if (x < 0 || x >= this.x || y < 0 || y >= this.y) return null;
        return grid[x * this.y + y];
    }

    //使用Texture2D时
    public FlowFieldGrid(int x, int y, Texture2D map)
    {
        this.x = x;
        this.y = y;
        byte[] bytes = map.GetRawTextureData();//获取纹理数据
        if (bytes.Length != x * y)
        {
            Debug.LogError("Texture2D大小与网格大小不匹配");
            return;
        }

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                int index = i * this.y + j;//计算位置,与GetNode保持一致(x*this.y + y)
                //把节点加入字典中,255表示可走,其他表示不可走
                grid.Add(index, new FlowFieldGridNode(i, j, bytes[index] == 255));
            }
        }
    }


    //设置目标节点,遍历相邻节点计算代价
    public void SetTarget(FlowFieldGridNode target)
    {
        if (target == null) return;
        foreach (var node in grid.Values)
        {
            node.cost = node.isWalkable ? 10 : int.MaxValue;
            node.fcost = int.MaxValue;
        }
        target.cost = 0;
        target.fcost = 0;
        target.direction = Vector3.zero;
        //队列,用于存储待处理节点
        Queue<FlowFieldGridNode> queue = new Queue<FlowFieldGridNode>();
        queue.Enqueue(target);
        while (queue.Count > 0)
        {
            //出列
            FlowFieldGridNode currentNode = queue.Dequeue();
            //获取当前节点的邻节点
            List<FlowFieldGridNode> neighbors = GetNeighbouringNodes(currentNode);
            //计算代价
            for (int i = 0; i < neighbors.Count; i++)
            {
                FlowFieldGridNode neighbor = neighbors[i];
                if (neighbor.cost == int.MaxValue) continue;

                neighbor.cost = CalculateCost(currentNode, neighbor);
                if (neighbor.cost + currentNode.fcost < neighbor.fcost)
                {
                    //更新代价
                    neighbor.fcost = neighbor.cost + currentNode.fcost;
                    queue.Enqueue(neighbor);
                }
            }
        }
    }


    //获取邻接点,八个方向
    private List<FlowFieldGridNode> GetNeighbouringNodes(FlowFieldGridNode node)
    {
        List<FlowFieldGridNode> neighbours = new List<FlowFieldGridNode>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                int x = node.x + i;
                int y = node.y + j;
                if (x >= 0 && x < this.x && y >= 0 && y < this.y)
                {
                    neighbours.Add(grid[x * this.y + y]);
                }
            }
        }
        return neighbours;
    }

    //计算代价
    private int CalculateCost(FlowFieldGridNode currentNode, FlowFieldGridNode neighbor)
    {
        int deltaX = currentNode.x - neighbor.x;
        if (deltaX < 0) deltaX = -deltaX;
        int deltaY = currentNode.y - neighbor.y;
        if (deltaY < 0) deltaY = -deltaY;
        int delta = deltaX - deltaY;
        if (delta < 0) delta = -delta;
        return 14 * (deltaX > deltaY ? deltaY : deltaX) + 10 * delta;
    }

    //生成流场
    public void GenerateFlowField()
    {
        foreach (var node in grid.Values)
        {
            List<FlowFieldGridNode> neighbours = GetNeighbouringNodes(node);
            int fcost = node.fcost;
            FlowFieldGridNode temp = null;
            for (int i = 0; i < neighbours.Count; i++)
            {
                FlowFieldGridNode neighbour = neighbours[i];
                if (neighbour.fcost < fcost)
                {
                    temp = neighbour;
                    fcost = neighbour.fcost;
                    node.direction = new Vector3(
                        neighbour.x - node.x,
                        neighbour.y - node.y,
                        0
                    );
                }
            }
        }
    }
}
