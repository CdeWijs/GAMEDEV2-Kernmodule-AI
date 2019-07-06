﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public Node parent;
    public int gCost; // distance from starting node
    public int hCost; // distance from end node
    
    // Gcost + Hcost
    // Only be able to get fCost, because you don't want to be able to set it.
    public int fCost {
        get {
            return gCost + hCost;
        }
    }
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }
}
