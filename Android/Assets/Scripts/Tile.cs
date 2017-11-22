using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour{

    //public enum Type { Floor, OneSideWall, TwoSideWall, OneDoorOneWall, OneSideDoor, TwoSideDoor, CornerWall, DeadEnd}
    public enum Content { Floor, Start, Exit, Pickup, Guard, Wall, BallPillarFire, SpikeTrap, FireTrap, GasTrap, CrushingWall, Catapult}
    public bool[] impassable; //fwd, back, right, left
    public bool isOuter;
    //public Type type = Type.Floor;
    public Content[] content;

    public MazeModel.Direction facing;
    public Vector3 localPosition = Vector3.zero;
    public Transform localFwd;
    public Transform localBk;
    public Transform localRht;
    public Transform localLft;

    public void Set(bool cantPassFwd, bool cantPassBk, bool cantPassLft, bool cantPassRht, bool isOuter)//, Content[] contents)
    {
        impassable[0] = cantPassFwd;
        impassable[3] = cantPassRht;
        impassable[1] = cantPassBk;
        impassable[2] = cantPassLft;
        this.isOuter = isOuter;
        content = new Content[14];
    }

    public void CompleteRandomize(bool hasStart, bool hasExit, float PickupChance, float GuardChance, float WallChance, float BallPillarFireChance, float SpikeTrapChance, 
        float FireTrapChance, float GasTrapChance, float CrushingWallChance, float CatapultChance)
    {
        List<Content> unmanagedContent = new List<Content>();
        for (int i = 0; i < content.Length; i++)
            unmanagedContent.Add(content[i]);
        Shuffle(ref unmanagedContent);

        if (hasStart)
        {
            unmanagedContent[unmanagedContent.Count - 1] = Content.Start;
            unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        }
        else if (hasExit)
        {
            unmanagedContent[unmanagedContent.Count - 1] = Content.Exit;
            unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        }
        if(unmanagedContent.Count>0 && Random.value> PickupChance / 100)
        {
            unmanagedContent[unmanagedContent.Count - 1] = Content.Pickup;
            unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        }

        if (unmanagedContent.Count > 0 && Random.value > GuardChance / 100)
        {
            unmanagedContent[unmanagedContent.Count - 1] = Content.Guard;
            unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        }
        if (unmanagedContent.Count > 0 && Random.value > WallChance / 100)
        {
            unmanagedContent[unmanagedContent.Count - 1] = Content.Wall;
            unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        }
        if (unmanagedContent.Count > 0 && Random.value > BallPillarFireChance / 100)
        {
            unmanagedContent[unmanagedContent.Count - 1] = Content.BallPillarFire;
            unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        }
        if (unmanagedContent.Count > 0 && Random.value > SpikeTrapChance / 100)
        {
            unmanagedContent[unmanagedContent.Count - 1] = Content.SpikeTrap;
            unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        }
        if (unmanagedContent.Count > 0 && Random.value > FireTrapChance / 100)
        {
            unmanagedContent[unmanagedContent.Count - 1] = Content.FireTrap;
            unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        }
        if (unmanagedContent.Count > 0 && Random.value > GasTrapChance / 100)
        {
            unmanagedContent[unmanagedContent.Count - 1] = Content.GasTrap;
            unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        }
        if (unmanagedContent.Count > 0 && Random.value > CrushingWallChance / 100)
        {
            unmanagedContent[unmanagedContent.Count - 1] = Content.CrushingWall;
            unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        }
        if (unmanagedContent.Count > 0 && Random.value > CatapultChance / 100)
        {
            unmanagedContent[unmanagedContent.Count - 1] = Content.Catapult;
            unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        }
    }

    public void Shuffle(ref List<Content> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            Content value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
