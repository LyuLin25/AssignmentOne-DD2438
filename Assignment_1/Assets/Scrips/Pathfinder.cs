using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public TerrainManager manager;
    public TerrainInfo info;
    public Node[,] map;
    public List<Node> path;

    public void FindPath()
    {
        if (info == null)
        {
            info = GameObject.Find("TerrainManager").GetComponent<TerrainManager>().myInfo;
        }
        Node start = GetNodeInMap(info.start_pos);
        Node goal = GetNodeInMap(info.goal_pos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>(); //Does not keep duplicates

        int count = 0;
        openSet.Add(start);
        while (openSet.Count > 0 || count > 1000)
        {
            count++;
            Node currentNode = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].cost < currentNode.cost || (openSet[i].cost == currentNode.cost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);
            if (currentNode.position == goal.position)
            {
                print("found path");
                Path(start, goal);
                return;
            }

            foreach (Node neighbour in GetNeighbours(currentNode))
            {

                if (!neighbour.traversable || closedSet.Contains(neighbour))
                {
                    continue;
                }
                int newCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCost;
                    neighbour.hCost = GetDistance(neighbour, goal);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }

    void Path(Node start, Node goal)
    {
        List<Node> newPath = new List<Node>();
        Node currentNode = goal;
        int count = 0;
        while (currentNode.position != start.position || currentNode == null || count > 100)
        {
            newPath.Add(currentNode);
            currentNode = currentNode.parent;
            count++;
        }
        newPath.Reverse();
        path = newPath;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {

        int x = Mathf.Abs(nodeA.xGrid - nodeB.xGrid);
        int z = Mathf.Abs(nodeA.zGrid - nodeB.zGrid);
        if (x > z)
        {

            return 14 * z + 10 * (x - z); //14 represents the distance value of going diagonally between nodes
        }                             //10 represents the distance value of going vertically or horizontally
        else
        {

            return 14 * x + 10 * (z - x);
        }
    }



    public void CreateMap()
    {
        map = new Node[info.x_N, info.z_N];
        bool walkable;
        Vector3 pos;

        for (int i = 0; i < info.x_N; i++)
        {
            for (int j = 0; j < info.z_N; j++)
            {
                if (info.traversability[i, j] > 0.5)
                {
                    walkable = false;
                }
                else
                {
                    walkable = true;
                }
                pos = new Vector3(info.get_x_pos(i), 0f, info.get_z_pos(j));
                map[i, j] = new Node(walkable, pos, i, j);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                int xPos = node.xGrid + i;
                int zPos = node.zGrid + j;

                if (xPos >= 0 && xPos < info.x_N && zPos >= 0 && zPos < info.z_N)
                {
                    neighbours.Add(map[xPos, zPos]);
                }
            }
        }
        return neighbours;
    }

    public Node GetNodeInMap(Vector3 worldPosition)
    {
        print(worldPosition);
        /* float centrumSquareX = ((info.x_high - info.x_low)/info.x_N)/2;
        float centrumSquareZ = ((info.z_high - info.z_low)/info.z_N)/2;
        float xPercentage = (worldPosition.x - info.x_low + centrumSquareX) / (info.x_high - info.x_low);
        float zPercentage = (worldPosition.z - info.z_low + centrumSquareZ) / (info.z_high - info.z_low);
        int x = Mathf.RoundToInt((info.x_N - 1) * xPercentage); // subtraction to accomidate for array index 0 to length - 1
        int z = Mathf.RoundToInt((info.z_N - 1) * zPercentage);*/
        int x = 0;
        int z = 0;
        float xInterval = (info.x_high - info.x_low) / info.x_N;
        float zInterval = (info.z_high - info.z_low) / info.z_N;
        float count = info.x_low + xInterval;
        while (count <= worldPosition.x)
        {
            x++;
            count += xInterval;
        }
        count = info.z_low + zInterval;
        while (count <= worldPosition.z)
        {
            z++;
            count += zInterval;
        }
        print(x + " " + z);
        print(map[x, z].position);
        return map[x, z];
    }

    void OnDrawGizmos()
    {
        if (map != null)
        {
            foreach (Node node in map)
            {
                if (node.traversable)
                    Gizmos.color = Color.blue;
                else
                    Gizmos.color = Color.red;

                if (path != null)
                {
                    if (path.Contains(node))
                        Gizmos.color = Color.green;
                }
                Gizmos.DrawCube(node.position, new Vector3((info.x_high - info.x_low) / info.x_N, 1, (info.z_high - info.z_low) / info.z_N));
            }
        }
    }
}

public class Node
{
    public bool traversable;
    public Vector3 position;
    public int gCost = 0;
    public int hCost = 0;
    TerrainInfo info;
    public int xGrid;
    public int zGrid;
    public Node parent;
    public Node(bool _traversable, Vector3 _position, int x, int z)
    {
        traversable = _traversable;
        position = _position;
        xGrid = x;
        zGrid = z;
    }

    //Return the total value of traversing the node
    public int cost
    {
        get { return gCost + hCost; }
    }
}