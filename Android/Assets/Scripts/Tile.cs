using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour{

    //public enum Type { Floor, OneSideWall, TwoSideWall, OneDoorOneWall, OneSideDoor, TwoSideDoor, CornerWall, DeadEnd}
    public enum Content { Start, Exit, Pickup, Enemy, Wall}
    public bool[] impassable; //fwd, back, left, right
    public bool isOuter;
    //public Type type = Type.Floor;
    public Content[] content;

    public MazeModel.Direction facing;
    public Vector3 localPosition = Vector3.zero;
    public Transform localFwd;
    public Transform localBk;
    public Transform localLft;
    public Transform localRht;

    //public Tile(bool canPassFwd, bool canPassBk, bool canPassLft, bool canPassRht, Vector3 pos, bool isOuter)
    //{
    //    localPosition = pos;
    //    //this.type = type;
    //    //this.isOuter = isOuter;
    //    facing = MazeModel.Direction.Forward;
    //    impassable = new bool[4];
    //    content = new Content[9];
    //    impassable[0] = canPassFwd;
    //    impassable[1] = canPassBk;
    //    impassable[2] = canPassLft;
    //    impassable[3] = canPassRht;
    //    //switch (type)
    //    //{
    //    //    case Type.Floor:
    //    //        impassable[0] = false;
    //    //        impassable[1] = false;
    //    //        impassable[2] = false;
    //    //        impassable[3] = false;
    //    //        break;
    //    //    case Type.OneSideWall:
    //    //        impassable[0] = true;
    //    //        impassable[1] = false;
    //    //        impassable[2] = false;
    //    //        impassable[3] = false;
    //    //        break;
    //    //    case Type.TwoSideWall:
    //    //        impassable[0] = true;
    //    //        impassable[1] = true;
    //    //        impassable[2] = false;
    //    //        impassable[3] = false;
    //    //        break;
    //    //    case Type.OneDoorOneWall:
    //    //        impassable[0] = false;
    //    //        impassable[1] = true;
    //    //        impassable[2] = false;
    //    //        impassable[3] = false;
    //    //        break;
    //    //    case Type.OneSideDoor:
    //    //        impassable[0] = false;
    //    //        impassable[1] = false;
    //    //        impassable[2] = false;
    //    //        impassable[3] = false;
    //    //        break;
    //    //    case Type.TwoSideDoor:
    //    //        impassable[0] = false;
    //    //        impassable[1] = false;
    //    //        impassable[2] = false;
    //    //        impassable[3] = false;
    //    //        break;
    //    //}
    //}



    public void Set(bool cantPassFwd, bool cantPassBk, bool cantPassLft, bool cantPassRht, bool isOuter)//, Content[] contents)
    {
        impassable[0] = cantPassFwd;
        impassable[1] = cantPassBk;
        impassable[2] = cantPassLft;
        impassable[3] = cantPassRht;
        this.isOuter = isOuter;
        //this.content = contents;
    }

    public void CompleteRandomize()
    {

    }

    public void ClearPathRandomize()
    {

    }
}
