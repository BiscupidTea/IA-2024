using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private Grapf<Node<CoordinateType>, CoordinateType> grapf;
    private VoronoidController voronoid = new VoronoidController();

    [SerializeField] private GrapfView grapfView;
    private Pathfinder<Node<CoordinateType>, CoordinateType> Pathfinder;

    [SerializeField] private int cellGap;
    [SerializeField] private int totalMountains;

    [SerializeField] private AudioClip alarmOn;
    [SerializeField] private AudioClip alarmOff;
    [SerializeField] private AudioSource audioSource;

    private int goldMineCuantity;
    private int minersCuantity;
    private int caravansCuantity;

    [SerializeField] private GameObject minerPrefab;
    [SerializeField] private GameObject caravanPrefab;
    private Node<CoordinateType> townCenter;
    private Node<CoordinateType> mine;

    private List<Node<CoordinateType>> mines = new List<Node<CoordinateType>>();
    private List<MinerAgent> miners = new List<MinerAgent>();
    private List<CaravanAgent> caravans = new List<CaravanAgent>();

    private bool Alarm;
    private Vector2Int grid;
    private void Update()
    {
        for (int i = mines.Count - 1; i >= 0; i--)
        {
            Node<CoordinateType> node = mines[i];
            TileClass newTileClass = node.GetTileClass();
    
            if (newTileClass is MineInventory mineInventory)
            {
                if (mineInventory.totalGold <= 0)
                {
                    mines.RemoveAt(i);
                    voronoid.StartVornonoid(mines, grid);
                    SetNewMine();
                }
            }
        }
    }

    public void StartGame(Vector2Int grid, int goldMineCuantity, int minersCuantity, int caravansCuantity)
    {
        this.goldMineCuantity = goldMineCuantity;
        this.minersCuantity = minersCuantity;
        this.caravansCuantity = caravansCuantity;

        this.grid = grid;

        Pathfinder = new AStarPathfinder<Node<CoordinateType>, CoordinateType>();

        grapf = new Grapf<Node<CoordinateType>, CoordinateType>(grid.x, grid.y, cellGap, Algorithm.AStarPathfinder);

        SetNodesData(grapf);

        voronoid.StartVornonoid(mines, grid);
        grapfView.SetGrapfView(grapf);

        SetMainers(grapf);
        SetCaravan(grapf);
    }

    private void SetNewMine()
    {
        foreach (MinerAgent miner in miners)
        {
            miner.SetNewMine(SerchNearMine(miner.transform.position));
        }

        foreach (CaravanAgent caravan in caravans)
        {
            caravan.SetNewMine(SerchNearMine(caravan.transform.position));
        }
    }

    private Node<CoordinateType> SerchNearMine(Vector3 position)
    {
        Node<CoordinateType> newPosition = new Node<CoordinateType>();
        CoordinateType coord = new CoordinateType();
        coord.Init((int)voronoid.GetVoronoiCenter(position).Value.x,
            (int)voronoid.GetVoronoiCenter(position).Value.y);
        newPosition.SetCoordinate(coord);
        Debug.Log("Near mine at: " + newPosition.GetCoordinate().GetXY()[0] + " , " +
                  newPosition.GetCoordinate().GetXY()[1]);

        return grapf.SerchNearNode(newPosition.GetCoordinate().GetXY()[0], newPosition.GetCoordinate().GetXY()[1]);
    }

    private void SetNodesData(Grapf<Node<CoordinateType>, CoordinateType> grapfh)
    {
        foreach (Node<CoordinateType> node in grapf.nodes.Values)
        {
            node.SetNodeType(NodeTypeCost.None);
        }


        Node<CoordinateType> currentNode;

        //Set Town Center
        currentNode = grapf.nodes[Random.Range(0, grapf.nodes.Count)];

        if (currentNode.GetNodeType() == NodeTypeCost.None)
        {
            currentNode.SetNodeType(NodeTypeCost.TownCenter);
            townCenter = currentNode;
        }

        //Set Gold Mine
        for (int i = 0; i < goldMineCuantity; i++)
        {
            currentNode = grapf.nodes[Random.Range(0, grapf.nodes.Count - 1)];

            if (currentNode.GetNodeType() == NodeTypeCost.None)
            {
                currentNode.SetNodeType(NodeTypeCost.GoldMine);
                mine = currentNode;
                mines.Add(currentNode);
            }
        }

        //Set Mountains
        for (int i = 0; i < totalMountains; i++)
        {
            currentNode = grapf.nodes[Random.Range(0, grapf.nodes.Count - 1)];

            if (currentNode.GetNodeType() == NodeTypeCost.None)
            {
                currentNode.SetNodeType(NodeTypeCost.Mountain);

                foreach (int nodeId in currentNode.GetNeighboursID())
                {
                    if (grapf.nodes[nodeId].GetNodeType() == NodeTypeCost.None)
                    {
                        grapf.nodes[nodeId].SetNodeType(NodeTypeCost.Plateau);
                    }
                }
            }
        }

        //Set Default Nodes Type
        foreach (Node<CoordinateType> node in grapfh)
        {
            if (node.GetNodeType() == NodeTypeCost.None)
            {
                node.SetNodeType(NodeTypeCost.Plain);
            }
        }
    }

    private void SetMainers(Grapf<Node<CoordinateType>, CoordinateType> grapfh)
    {
        GameObject newMiner;

        for (int i = 0; i < minersCuantity; i++)
        {
            newMiner = Instantiate(minerPrefab,
                new Vector3(townCenter.GetCoordinate().GetXY()[0], townCenter.GetCoordinate().GetXY()[1], 0),
                Quaternion.identity, transform);

            miners.Add(newMiner.GetComponent<MinerAgent>());
        }

        mine = SerchNearMine(new Vector3(townCenter.GetCoordinate().GetXY()[0], townCenter.GetCoordinate().GetXY()[1],
            0));

        foreach (MinerAgent currentMiner in miners)
        {
            currentMiner.StartAgent(grapf, townCenter, mine);
        }
    }

    private void SetCaravan(Grapf<Node<CoordinateType>, CoordinateType> grapfh)
    {
        GameObject newCaravan;

        for (int i = 0; i < caravansCuantity; i++)
        {
            newCaravan = Instantiate(caravanPrefab,
                new Vector3(townCenter.GetCoordinate().GetXY()[0], townCenter.GetCoordinate().GetXY()[1], 0),
                Quaternion.identity, transform);

            caravans.Add(newCaravan.GetComponent<CaravanAgent>());
        }

        foreach (CaravanAgent currentCaravan in caravans)
        {
            currentCaravan.StartAgent(grapf, townCenter, mine);
        }
    }

    public void CallAlarm()
    {
        Alarm = !Alarm;
        
        foreach (MinerAgent miner in miners)
        {
            miner.AlarmSound();
        }

        foreach (CaravanAgent caravan in caravans)
        {
            caravan.AlarmSound();
        }

        if (Alarm)
        {
            audioSource.clip = alarmOn;
        }
        else
        {
            audioSource.clip = alarmOff;
        }
        
        audioSource.Play();
    }

    private void OnDrawGizmos()
    {
        voronoid.OnDrawGizmos();
    }
}