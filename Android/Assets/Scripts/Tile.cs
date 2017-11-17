using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour{

    public enum Type { Floor, OneSideWall, TwoSideWall, OneDoorOneWall, OneSideDoor, TwoSideDoor}
    public enum Content { Start, Exit, Pickup, Enemy}
    public Vector3 localPosition = Vector3.zero;
    public bool[] impassable = new bool[4]; //fwd, back, left, right
    public Type type = Type.Floor;
    public MazeModel.Direction facing;

    public Tile(Type type, Vector3 pos)
    {
        localPosition = pos;
        this.type = type;
        facing = MazeModel.Direction.Forward;
        switch (type)
        {
            case Type.Floor:
                impassable[0] = false;
                impassable[1] = false;
                impassable[2] = false;
                impassable[3] = false;
                break;
            case Type.OneSideWall:
                impassable[0] = true;
                impassable[1] = false;
                impassable[2] = false;
                impassable[3] = false;
                break;
            case Type.TwoSideWall:
                impassable[0] = true;
                impassable[1] = true;
                impassable[2] = false;
                impassable[3] = false;
                break;
            case Type.OneDoorOneWall:
                impassable[0] = false;
                impassable[1] = true;
                impassable[2] = false;
                impassable[3] = false;
                break;
            case Type.OneSideDoor:
                impassable[0] = false;
                impassable[1] = false;
                impassable[2] = false;
                impassable[3] = false;
                break;
            case Type.TwoSideDoor:
                impassable[0] = false;
                impassable[1] = false;
                impassable[2] = false;
                impassable[3] = false;
                break;
        }
    }
}
