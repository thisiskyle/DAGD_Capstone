﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ericson
{
    public class AStar : ESingletonMono<AStar>
    {

        public List<Tile> _path = new List<Tile>();
        public List<Tile> path
        {
            get { return _path; }
        }


        public AStar() { }


        public void FindPath(Vector2 start, Vector2 target)
        {
            Debug.Log("Start");
            Tile startTile = MapManager.ins.currentMap[start];
            Tile targetTile = MapManager.ins.currentMap[target];

            List<Tile> openSet = new List<Tile>();
            List<Tile> closedSet = new List<Tile>();

            openSet.Add(startTile);

            while (openSet.Count > 0)
            {
                Debug.Log("Finding Path");
                Tile currentTile = openSet[0];

                for (int i = 1; i < openSet.Count; i++)
                {
                    if ((openSet[i].f < currentTile.f) || (openSet[i].f == currentTile.f) && (openSet[i].h < currentTile.h))
                    {
                        currentTile = openSet[i];
                    }
                    openSet.Remove(currentTile);
                    closedSet.Add(currentTile);
                    currentTile.SetColor(Color.red);

                    if (currentTile == targetTile)
                    {
                        RetracePath(startTile, targetTile);
                        return;
                    }
                }

                foreach (Tile t in MapManager.ins.GetNeighbors(currentTile))
                {
                    if (GameScene.ins.currentSelected.data.size > t.data.maxSize || closedSet.Contains(t)) continue;

                    float newCost = currentTile.g + 1;
                    if ((newCost < t.g) || !openSet.Contains(t))
                    {
                        t.g = newCost;
                        t.h = Vector2.Distance(t.transform.position, targetTile.transform.position);
                        t.f = t.h + t.g;
                        t.parent = currentTile;

                        if (!openSet.Contains(t))
                        {
                            openSet.Add(t);
                        }
                    }
                }
            }
            foreach(Tile t in closedSet)
            {
                t.SetColor(Color.red);
            }
        }
        void RetracePath(Tile startTile, Tile targetTile)
        {
            List<Tile> path = new List<Tile>();
            Tile currentTile = targetTile;
            while(currentTile != startTile)
            {
                path.Add(currentTile);
                currentTile.Highlight();
                currentTile = currentTile.parent;
            }
            path.Reverse();
            _path.Clear();
            _path = path;
        }
        int GetDistance(Tile tile1, Tile tile2)
        {
            int dX = (int)Mathf.Abs(MapManager.ins.WorldToGrid(tile1.transform.position).x - MapManager.ins.WorldToGrid(tile2.transform.position).x);
            int dY = (int)Mathf.Abs(MapManager.ins.WorldToGrid(tile1.transform.position).y - MapManager.ins.WorldToGrid(tile2.transform.position).y);

            return dX + dY;
        }
    }
}