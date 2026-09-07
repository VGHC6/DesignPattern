using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//流场寻路网格节点
public class FlowFieldGridNode
{
    public int x;
    public int y;
    public int cost;//移动成本
    public int fcost;//总成本
    public Vector3 direction;//方向
    public bool isWalkable;//是否可走

    public FlowFieldGridNode(int x, int y, bool isWalkable)
    {
        this.x = x;
        this.y = y;
        this.isWalkable = isWalkable;
        this.cost = isWalkable ? 10 : int.MaxValue;//如果可以走代价就不存在
        fcost = int.MaxValue;//默认总成本最大
    }
}
