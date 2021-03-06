﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathFinder : GreedyPathFinder
{
    public static int nodesOpened = 0;

    //ASSIGNMENT 2: EDIT BELOW THIS LINE, IMPLEMENT A*
    public override Vector3[] CalculatePath(GraphNode startNode, GraphNode goalNode)
    {
        nodesOpened = 0;

        AStarNode start = new AStarNode(null, startNode, Heuristic(startNode, goalNode));
        Dictionary<string, float> gScores = new Dictionary<string, float>();
        gScores[start.Location.ToString()] = 0;


        PriorityQueue<AStarNode> openSet = new PriorityQueue<AStarNode>();
        openSet.Enqueue(start);
        List<AStarNode> closedSet = new List<AStarNode>();
        int attempts = 0;
        while (openSet.Count() > 0 && attempts<10000)
        {
            attempts += 1;
            AStarNode currNode = openSet.Dequeue();

            //Did we find the goal?
            if (currNode.Location == goalNode.Location)
            {
                Debug.Log("CHECKED " + nodesOpened + " NODES");//Don't delete this line
                //Reconstruct the path?
                return ReconstructPath(start, currNode);
            }
            //Add current Node to closed set
            closedSet.Add(currNode);
            //closedSet.
            //Check each neighbor
            foreach (GraphNode neighbor in currNode.GraphNode.Neighbors)
            {
                //currently just adds every neighbor to the open set
                AStarNode aStarNeighbor = new AStarNode(currNode, neighbor, Heuristic(neighbor, goalNode));
                //openSet.Enqueue(aStarNeighbor);
                //If neighbour has already been checked skip
                if (closedSet.Contains(aStarNeighbor))
                {
                    continue;
                }
                //We don't need to update GScore because it is automatic
                //If neighbour not in openSet
                if (!gScores.ContainsKey(aStarNeighbor.Location.ToString()))
                {
                    openSet.Enqueue(aStarNeighbor);
                    gScores[aStarNeighbor.Location.ToString()] = aStarNeighbor.GScore;
                }
                else if (aStarNeighbor.GScore < gScores[aStarNeighbor.Location.ToString()])
                {
                    //openSet.Remove
                    List<AStarNode> temp = new List<AStarNode>();
                    while (openSet.Count() > 0)
                    {
                        AStarNode curr = openSet.Dequeue();
                        if (curr.Location.ToString() != aStarNeighbor.Location.ToString())
                        {
                            temp.Add(curr);
                        }
                    }
                    foreach (AStarNode curr in temp)
                    {
                        openSet.Enqueue(curr);
                    }
                    openSet.Enqueue(aStarNeighbor);
                }

            }
        }
        Debug.Log("CHECKED "+ nodesOpened + " NODES");//Don't delete this line
        return null;
    }
    //ASSIGNMENT 2: EDIT ABOVE THIS LINE, IMPLEMENT A*

    //EXTRA CREDIT ASSIGNMENT 2 EDIT BELOW THIS LINE
    public float Heuristic(GraphNode currNode, GraphNode goalNode)
    {
        Debug.Log(ObstacleHandler.Instance.AnyIntersect(currNode.Location, goalNode.Location));
        if (ObstacleHandler.Instance.AnyIntersect(currNode.Location, goalNode.Location))
        {
            return Mathf.Abs(currNode.Location.x - goalNode.Location.x) + Mathf.Abs(currNode.Location.y - goalNode.Location.y)+ 100f;
        }
        return Mathf.Abs(currNode.Location.x-goalNode.Location.x)+Mathf.Abs(currNode.Location.y - goalNode.Location.y);
    }
    //EXTRA CREDIT ASSIGNMENT 2 EDIT ABOVE THIS LINE

    private Vector3[] ReconstructPath(AStarNode startNode, AStarNode currNode)
    {
        List<Vector3> backwardsPath = new List<Vector3>();

        while (currNode != startNode)
        {
            backwardsPath.Add(currNode.Location);
            currNode = currNode.Parent;
        }
        backwardsPath.Reverse();

        return backwardsPath.ToArray();
    }
}


class AStarNode : IComparable<AStarNode>
{
    private AStarNode parent;
    public AStarNode Parent { get { return parent; } }
    private GraphNode graphNode;
    public GraphNode GraphNode { get { return graphNode; } }
    public Vector3 Location { get { return graphNode.Location; } }
    private float gScore;
    public float GScore { get { return gScore; } }
    private float hScore;
    public float HScore { get { return hScore; } }
    public float FScore {  get { return gScore + hScore; } }

    public AStarNode(AStarNode _parent, GraphNode _graphNode, float _hScore)
    {
        AStarPathFinder.nodesOpened += 1;
        this.parent = _parent;
        this.graphNode = _graphNode;
        this.gScore = 0f;
        if (parent != null) { 
            this.gScore = parent.gScore + 1f;
        }
        this.hScore = _hScore;
    }

    public float GetFScore()
    {
        return gScore + hScore;
    }

    public int CompareTo(AStarNode other)
    {
        if (other == null) return 1;

        AStarNode otherNode = other as AStarNode;
        if (otherNode != null)
            return this.FScore.CompareTo(otherNode.FScore);
        else
            throw new ArgumentException("Object is not an AStarNode");
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        AStarNode otherNode = obj as AStarNode;
        if (otherNode.Location == Location && otherNode.gScore.Equals(gScore) && otherNode.hScore.Equals(hScore))
            return true;
        else
            return false;
    }

    public override int GetHashCode()
    {
        return Location.GetHashCode() + GScore.GetHashCode() + HScore.GetHashCode();
    }
}
