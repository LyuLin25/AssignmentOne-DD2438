using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public TerrainManager manager;
    public Node[,] map;

    public TerrainInfo myInfo;
    public List<Node> path;
    public void CreateGrid()
    {
        if (manager == null)
        {
            manager = GameObject.Find("TerrainManager").GetComponent<TerrainManager>();
        }
        myInfo = manager.myInfo;
        map = new Node[myInfo.x_N, myInfo.z_N];
        bool walkable;
        Vector3 pos;

        for (int i = 0; i < myInfo.x_N; i++)
        {
            for (int j = 0; j < myInfo.z_N; j++)
            {
                if (myInfo.traversability[i, j] > 0.5)
                {
                    walkable = false;
                }
                else
                {
                    walkable = true;
                }
                pos = new Vector3(myInfo.get_x_pos(i), 0f, myInfo.get_z_pos(j));
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

                if (xPos >= 0 && xPos < myInfo.x_N && zPos >= 0 && zPos < myInfo.z_N)
                {
                    neighbours.Add(map[xPos, zPos]);
                }
            }
        }
        return neighbours;
    }

    public Node GetNodeInMap(Vector3 worldPosition)
    {

        float xPercentage = Mathf.Clamp01((worldPosition.x + (myInfo.x_high - myInfo.x_low) / 2) / myInfo.x_high - myInfo.x_low);
        float zPercentage = Mathf.Clamp01((worldPosition.z + (myInfo.z_high - myInfo.z_low) / 2) / myInfo.z_high - myInfo.z_low);

        int x = Mathf.RoundToInt((myInfo.x_N - 1) * xPercentage); // subtraction to accomidate for array index 0 to length - 1
        int z = Mathf.RoundToInt((myInfo.z_N - 1) * zPercentage);

        print(map[x, z]);
        return map[x, z];
    }

    void OnDrawGizmos()
    {
        if (map != null)
        {
            foreach (Node node in map)
            {
                if (node.traversable)
                    Gizmos.color = Color.white;
                else
                    Gizmos.color = Color.red;

                if (path != null)
                {
                    if (path.Contains(node))
                        Gizmos.color = Color.black;
                }
                Gizmos.DrawCube(node.position, new Vector3((myInfo.x_high - myInfo.x_low)/myInfo.x_N, 1, (myInfo.z_high - myInfo.z_low)/myInfo.z_N));
            }
        }
    }
}

