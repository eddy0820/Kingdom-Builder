using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Grid Object", menuName = "Building System/Grid Object")]
public class GridObjectSO : PlaceableObjectSO
{
    public int width;
    public int height;

    protected override void SetObjectType()
    {
        objectType = PlaceableObjectTypes.GridObject;
    }

    public int GetRotationAngle(Direction dir) 
    {
        switch(dir) 
        {
            default:
            case Direction.Down:  return 0;
            case Direction.Left:  return 90;
            case Direction.Up:    return 180;
            case Direction.Right: return 270;
        }
    }

    public Vector2Int GetRotationOffset(Direction dir) 
    {
        switch(dir) 
        {
            default:
            case Direction.Down:  return new Vector2Int(0, 0);
            case Direction.Left:  return new Vector2Int(0, width);
            case Direction.Up:    return new Vector2Int(width, height);
            case Direction.Right: return new Vector2Int(height, 0);
        }
    }

    public List<Vector2Int> GetGridPositionList(Vector2Int offset, Direction dir) 
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>();
        switch(dir) 
        {
            default:
            case Direction.Down:
            case Direction.Up:
                for(int x = 0; x < width; x++) 
                {
                    for(int y = 0; y < height; y++) 
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
            case Direction.Left:
            case Direction.Right:
                for(int x = 0; x < height; x++) 
                {
                    for(int y = 0; y < width; y++) 
                    {
                        gridPositionList.Add(offset + new Vector2Int(x, y));
                    }
                }
                break;
        }
        return gridPositionList;
    }

    public List<Vector2Int> GetGridAdjacentPositionList(List<Vector2Int> gridPositionList)
    {
        List<Vector2Int> gridAdjacentPositionList = new List<Vector2Int>();

        foreach(Vector2Int gridPosition in gridPositionList)
        {
            List<Vector2Int> singularAdjacentPositionList = new List<Vector2Int>();

            singularAdjacentPositionList.Add(gridPosition + new Vector2Int(0, -1)); //Down
            singularAdjacentPositionList.Add(gridPosition + new Vector2Int(0, 1)); //Up
            singularAdjacentPositionList.Add(gridPosition + new Vector2Int(-1, 0)); //Left
            singularAdjacentPositionList.Add(gridPosition + new Vector2Int(1, 0)); //Right

            foreach(Vector2Int singularAdjacentPositon in singularAdjacentPositionList)
            {
                if(!gridAdjacentPositionList.Contains(singularAdjacentPositon) && !gridPositionList.Contains(singularAdjacentPositon))
                {
                    gridAdjacentPositionList.Add(singularAdjacentPositon);
                }
            }
        }

        return gridAdjacentPositionList;
    }

    public static Direction GetNextDirection(Direction dir) 
    {
        switch(dir) 
        {
            default:
            case Direction.Down:      return Direction.Left;
            case Direction.Left:      return Direction.Up;
            case Direction.Up:        return Direction.Right;
            case Direction.Right:     return Direction.Down;
        }
    }

    public static Vector2Int GetDirectionForwardVector(Direction dir) 
    {
        switch(dir) 
        {
            default:
            case Direction.Down: return new Vector2Int(0, -1);
            case Direction.Left: return new Vector2Int(-1, 0);
            case Direction.Up: return new Vector2Int(0, +1);
            case Direction.Right: return new Vector2Int(+1, 0);
        }
    }

    public static Direction GetDirection(Vector2Int from, Vector2Int to) 
    {
        if(from.x < to.x) 
        {
            return Direction.Right;
        } 
        else 
        {
            if(from.x > to.x) 
            {
                return Direction.Left;
            } 
            else 
            {
                if(from.y < to.y) 
                {
                    return Direction.Up;
                } 
                else 
                {
                    return Direction.Down;
                }
            }
        }
    }  
}
