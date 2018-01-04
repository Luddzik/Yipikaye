using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerTile
{
    public Tile.Content content;

    public InnerTile()
    {
        content = Tile.Content.Floor;
    }

    public InnerTile(Tile.Content content)
    {
        this.content = content;
    }
}

public class Tile : MonoBehaviour{

    //public enum Type { Floor, OneSideWall, TwoSideWall, OneDoorOneWall, OneSideDoor, TwoSideDoor, CornerWall, DeadEnd}
    public enum Content { Floor, Start, Exit, HealthPickup, AbilityPickup, Guard, Block } //, BallPillarFire, SpikeTrap, FireTrap, GasTrap, CrushingWall, Catapult}
    public bool[] impassable; //fwd, back, right, left
    public bool isOuter;
    public bool hasStart;
    //public Type type = Type.Floor;
    public InnerTile[] contents;
    public GameObject[] walls;
    public MazeModel.Direction facing;
    public Vector3 localPosition = Vector3.zero;
    public static Vector3[] contentPositions = {
        new Vector3(-1.5f, 0, -1.5f), new Vector3(-0.5f, 0, -1.5f), new Vector3(0.5f, 0, -1.5f), new Vector3(1.5f, 0, -1.5f),
        new Vector3(-1.5f, 0, -0.5f), new Vector3(-0.5f, 0, -0.5f), new Vector3(0.5f, 0, -0.5f), new Vector3(1.5f, 0, -0.5f),
        new Vector3(-1.5f, 0, 0.5f),  new Vector3(-0.5f, 0, 0.5f),  new Vector3(0.5f, 0, 0.5f),  new Vector3(1.5f, 0, 0.5f),
        new Vector3(-1.5f, 0, 1.5f),  new Vector3(-0.5f, 0, 1.5f),  new Vector3(0.5f, 0, 1.5f),  new Vector3(1.5f, 0, 1.5f),
    };
    //public Transform localFwd;
    //public Transform localBk;
    //public Transform localRht;
    //public Transform localLft;

    public void Set(Vector3 pos)//bool isOuter, Vector3 pos) //bool cantPassFwd, bool cantPassBk, bool cantPassLft, bool cantPassRht)//, Content[] contents)
    {
        //impassable[0] = cantPassFwd;
        //impassable[3] = cantPassRht;
        //impassable[1] = cantPassBk;
        //impassable[2] = cantPassLft;
        //this.isOuter = isOuter;
        contents = new InnerTile[16];
        localPosition = pos;
    }

    public void CompleteRandomize(bool hasStart, bool hasExit, ref float healthPickupChance, ref float abilityPickupChance, ref float guardChance, ref float blockChance) //, float BallPillarFireChance, float SpikeTrapChance, 
        //float FireTrapChance, float GasTrapChance, float CrushingWallChance, float CatapultChance)
    {
        List<InnerTile> unmanagedContent = new List<InnerTile>();
        for (int i = 0; i < contents.Length; i++)
        {
            contents[i] = new InnerTile();
            unmanagedContent.Add(contents[i]);
        }
        Shuffle(ref unmanagedContent);

        if (hasStart)
        {
            unmanagedContent[unmanagedContent.Count - 1].content = Content.Start;
            unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        }
        else {
            if (hasExit)
            {
                unmanagedContent[unmanagedContent.Count - 1].content = Content.Exit;
                unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
            }

            if (unmanagedContent.Count > 0 && Random.value < healthPickupChance / 100)
            {
                unmanagedContent[unmanagedContent.Count - 1].content = Content.HealthPickup;
                unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
            }
            if (unmanagedContent.Count > 0 && Random.value < abilityPickupChance / 100)
            {
                unmanagedContent[unmanagedContent.Count - 1].content = Content.AbilityPickup;
                unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
            }

            if (unmanagedContent.Count > 0 && Random.value < guardChance / 100)
            {
                unmanagedContent[unmanagedContent.Count - 1].content = Content.Guard;
                unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
            }
            if (unmanagedContent.Count > 0 && Random.value < blockChance / 100)
            {
                unmanagedContent[unmanagedContent.Count - 1].content = Content.Block;
                unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
            }
        }
        
        //if (unmanagedContent.Count > 0 && Random.value > BallPillarFireChance / 100)
        //{
        //    unmanagedContent[unmanagedContent.Count - 1] = Content.BallPillarFire;
        //    unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        //}
        //if (unmanagedContent.Count > 0 && Random.value > SpikeTrapChance / 100)
        //{
        //    unmanagedContent[unmanagedContent.Count - 1] = Content.SpikeTrap;
        //    unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        //}
        //if (unmanagedContent.Count > 0 && Random.value > FireTrapChance / 100)
        //{
        //    unmanagedContent[unmanagedContent.Count - 1] = Content.FireTrap;
        //    unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        //}
        //if (unmanagedContent.Count > 0 && Random.value > GasTrapChance / 100)
        //{
        //    unmanagedContent[unmanagedContent.Count - 1] = Content.GasTrap;
        //    unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        //}
        //if (unmanagedContent.Count > 0 && Random.value > CrushingWallChance / 100)
        //{
        //    unmanagedContent[unmanagedContent.Count - 1] = Content.CrushingWall;
        //    unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        //}
        //if (unmanagedContent.Count > 0 && Random.value > CatapultChance / 100)
        //{
        //    unmanagedContent[unmanagedContent.Count - 1] = Content.Catapult;
        //    unmanagedContent.RemoveAt(unmanagedContent.Count - 1);
        //}
    }

    public void Shuffle(ref List<InnerTile> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            InnerTile value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
