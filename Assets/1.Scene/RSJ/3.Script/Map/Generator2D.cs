using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;

public class Generator2D : MonoBehaviour
{
    enum CellType
    {
        None,
        Room,
        Hallway
    }

    enum CellDirection
    {
        right,
        left,
        up,
        down,
        none
    }

    enum HallwayCellPrefab
    {
        right,
        left,
        top
    }


    class Room
    {
        public RectInt bounds;

        public Room(Vector2Int location, Vector2Int size)
        {
            bounds = new RectInt(location, size);
        }

        public static bool Intersect(Room a, Room b)
        {
            return !((a.bounds.position.x -2 >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x -2)
                || (a.bounds.position.y -2 >= (b.bounds.position.y + b.bounds.size.y)) || (a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y -2);
            //a.bounds.position.x : a���� ���ʰ��, b.bounds.position.x + b.bounds.size.x : b ���� �����ʰ��.
            //a���� ���ʰ�谡 b���� ������ ��躸�� ũ�ų� ���ٸ� �� ���� ��ġ�� �ʴ´�.
        }
        /* RectInt�� position ���� -> minx�� miny�� ��´�.
          public Vector2Int position 
        {
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] get { return new Vector2Int(m_XMin, m_YMin); }
            [MethodImpl(MethodImplOptionsEx.AggressiveInlining)] set { m_XMin = value.x; m_YMin = value.y; }
        }
         */
    }

    [Header("���� ���� ����")]
    [SerializeField] Vector2Int size;
    [SerializeField] int roomCount;
    [SerializeField] Vector2Int roomMaxSize;

    [Header("������")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject parentPrefab; // �����ϱ� ���� ������
    [SerializeField] private GameObject cornerTopRightPrefab;
    [SerializeField] private GameObject cornerTopLeftPrefab;
    [SerializeField] private GameObject cornerBottomRightPrefab;
    [SerializeField] private GameObject cornerBottomLeftPrefab;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject[] wallTopPrefabs;
    [SerializeField] private GameObject[] wallBottomPrefabs;
    [SerializeField] private GameObject[] wallRightPrefabs;
    [SerializeField] private GameObject[] wallLeftPrefabs;
    [SerializeField] private GameObject[] hallwayXPrefabs;
    [SerializeField] private GameObject[] hallwayYPrefabs;
    [SerializeField] private GameObject hallwayCornerTRPrefab;
    [SerializeField] private GameObject hallwayCornerTLPrefab;
    [SerializeField] private GameObject hallwayCornerBRPrefab;
    [SerializeField] private GameObject hallwayCornerBLPrefab;
    [SerializeField] private GameObject hallwayCrossPrefab;
    [SerializeField] private GameObject roomCeilingPrefab;
    [SerializeField] private GameObject dungeonEntrancePrefab;
    [SerializeField] private GameObject bossRoomPrefab;
    [SerializeField] Material redMaterial;
    [SerializeField] Material blueMaterial;
    
    Random random;
    Grid2D<CellType> grid;
    List<Room> rooms;
    Delaunay2D delaunay;
    HashSet<Prim.Edge> selectedEdges;

    //�� ��ȣ
    private int count = 1;
    //��� ��ȣ
    private int countHall = 1;

    //�������� ���� �Ա����� ��θ� ����� ���� ����
    Grid2D<CellType> gridEntrance;
    Vector2Int sizeEntrance;
    Vector2Int entrancePosition;
    private bool canUseDungeonEntrance = false; // ����-�����Ա� �ѹ��� ���

    //������ ������
    Vector2 bossRoomPosition = Vector2.zero;
    float bossRoom_yMax = 0;
    float bossRoom_xMax = 0;


    private void Start()
    {
        Generate();
    }

    private void Generate()
    {
        random = new Random(); // Random�� �õ尪�� �ִ� ������ ���� ��� �ٲ�� ���� ��������
        grid = new Grid2D<CellType>(size, Vector2Int.zero);
        rooms = new List<Room>();

        sizeEntrance = new Vector2Int(size.x, 15);
        gridEntrance = new Grid2D<CellType>(sizeEntrance, Vector2Int.zero);

        //�����
        PlaceRooms();
        //��γ� �ﰢ����
        Triangulate();
        //��纹������
        CreateHallways();
        //����� ������ ����
        PathfindHallways();
        //�������� �����Ա���
        PathfindDungeonGate();
        //������ ����
        //CreateBossRoom();
        Invoke("CreateBossRoom", 0.1f);

    }

    private void PlaceRooms()
    {
        Vector2Int gatePosition = size; // ���� ū ���� ������

        for (int i =0; i<roomCount;i++)
        {
            Vector2Int location = new Vector2Int(random.Next(0, size.x), random.Next(2, size.y));
            Vector2Int roomSize = new Vector2Int(random.Next(6, roomMaxSize.x + 1), random.Next(6, roomMaxSize.y + 1));

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));

            //���� �� �����ǰ� ���� ���� x,z�� ���� ���� �������� ���� ���� ã�ƾ��Ѵ�.
            

            foreach (Room room in rooms)
            {
                if(Room.Intersect(room, buffer))
                {
                    Debug.Log("Intersect�� ���ͼ� add�� �ȵ�");
                    add = false;
                    break;
                }
            }

            if(newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y)
            {
                Debug.Log("�׸��� ���� ������ ���� ť�갡 �ִ���");
                add = false;
            }

            if(add)
            {
                if(newRoom.bounds.size.x > 3 && newRoom.bounds.size.y > 3)
                {
                    Debug.Log("���� �߰���");
                    rooms.Add(newRoom);
                    PlaceRoom(newRoom.bounds.position, newRoom.bounds.size);

                    //�� �Ѳ� ����� Ceiling
                    CreateRoomCeiling(newRoom);

                    foreach (var pos in newRoom.bounds.allPositionsWithin)
                    {
                        grid[pos] = CellType.Room;
                    }

                    //����-���� ��θ� ���� �� ����
                    if(gatePosition.y > newRoom.bounds.position.y)
                    {
                        gatePosition = newRoom.bounds.position;

                        entrancePosition.x = (int)((newRoom.bounds.xMin + newRoom.bounds.xMax) * 0.5f);
                        entrancePosition.y = newRoom.bounds.yMin;
                        //Debug.Log("newRoom�� ��ġ : " + newRoom.bounds.position);
                        //Debug.Log("���� �Ա��� ��ġ : " + entrancePosition);

                        //y �������� ���� ��쿡 x�� �� ������ ����ġ
                        if (gatePosition.x > newRoom.bounds.position.x)
                        {
                            gatePosition.x = newRoom.bounds.position.x;
                            entrancePosition.x = (int)((newRoom.bounds.xMin + newRoom.bounds.xMax) * 0.5f);
                        }
                    }

                    //������ ã�� �����
                    FindBossRoom(newRoom);
                }
            }
        }
    }

    private void Triangulate()
    {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms)
        {
            vertices.Add(new Vertex<Room>((Vector2)room.bounds.position + ((Vector2)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay2D.Triangulate(vertices);

        //Ȯ�ο뵵
        for (int i = 0; i < delaunay.Vertices.Count; i++)
        {
            Vector3 CheckPoint = new Vector3(delaunay.Vertices[i].Position.x, 0, delaunay.Vertices[i].Position.y);
            //Debug.DrawRay(CheckPoint, Vector3.up * 8f, Color.red, Mathf.Infinity);
            //Debug.Log(delaunay.Vertices[i].Position);
        }

        Debug.Log("������ ���°ǰ�?" + delaunay.Edges.Count);
        for (int i = 0; i < delaunay.Edges.Count; i++)
        {
            //Debug.Log("����� ����" + i);
            //Debug.Log("???? 11: " + delaunay.Edges[i].U.Position);
        }
    }

    private void CreateHallways()
    {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        //Debug.Log("???? 11: " + delaunay.Edges[0].U.Position);
        foreach (var edge in delaunay.Edges)
        {

            edges.Add(new Prim.Edge(edge.U, edge.V));
            //Debug.Log("???? 11: " + edge.U.Position);
            //Debug.Log("???? 22: " + edge.V.Position);
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U); //����Ʈ�� ������(0)

        selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        //���鳢�� ����� ���� �����ֱ�
        //StartCoroutine(ShowLine(edges));

        //Debug.DrawRay(remainingEdges[0].edg, Vector3.up * 3f, Color.blue, Mathf.Infinity);

        foreach (var edge in remainingEdges)
        {
            Vector3 checkEdgePosition_U = new Vector3(edge.U.Position.x, 0, edge.U.Position.y);
            Vector3 checkEdgePosition_V = new Vector3(edge.V.Position.x, 0, edge.V.Position.y);
            
            //���� ���� ��� ����Ǿ����� Edge(U,V)
            //Debug.DrawLine(checkEdgePosition_U, checkEdgePosition_V, Color.blue, Mathf.Infinity);

            if (random.NextDouble() < 0.125) // �����ٴ� �ǰ�? Ư�� �������� ���� �� �˰ڴ�.
                                             // ������ �� ���°���? ��� ��θ� ����°� ��ȿ�����̾�
                                             // �ٵ� ������ ������ ��θ� ���ϸ� � ���� ������ �ʳ�?
            {
                selectedEdges.Add(edge);

                //if���ȿ��� ����Ǵ°Ŵϱ� �� �����ȿ� �ִ� �༮�鸸 ���ͼ� ����ǰ���.
                //Debug.DrawLine(checkEdgePosition_U, checkEdgePosition_V, Color.green, Mathf.Infinity);
                //Debug.Log("SelectedEdges : " + edge.ToString());
                //Debug.DrawRay(checkEdgePosition_U, Vector3.up * 13f, Color.black, Mathf.Infinity);
                //Debug.DrawRay(checkEdgePosition_V, Vector3.up * 15f, Color.white, Mathf.Infinity);
                
            }
        }
    }

    private void PathfindHallways()
    {
        Pathfinder2D aStar = new Pathfinder2D(size);

        foreach (var edge in selectedEdges)
        {
            /*
             C#���� as Ű����� ���� ������ �ٸ� ���� �������� ��ȯ�� �� ���˴ϴ�. 
             as Ű����� ��������� ����ȯ�� �õ��ϰ�, �����ϸ� null�� ��ȯ�մϴ�.

             edge.U�� Vertex<Room> �������� ����ȯ�Ϸ� �õ��ϰ�, 
            �����ϸ� �ش� Vertex<Room> ��ü�� Item �Ӽ��� �����Ͽ� startRoom ������ �Ҵ��մϴ�. 
            ���� edge.U�� Vertex<Room> ������ �ƴ϶�� null�� startRoom�� �Ҵ�˴ϴ�.
             */
            var startRoom = (edge.U as Vertex<Room>).Item; // Vertex U�� edge�� ������
            var endRoom = (edge.V as Vertex<Room>).Item; // Vertex V�� edge�� ������

            var startPosf = startRoom.bounds.center; //�ӽ÷� �ε��Ҽ���(float)���� �߾Ӱ� �޾ƿ�
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);

            Vector3 startpositon = new Vector3(startPos.x, 1.6f, startPos.y);
            Vector3 endpositon = new Vector3(endPos.x, 1.6f, endPos.y);
            Debug.DrawLine(startpositon, endpositon, Color.red, Mathf.Infinity);
            Debug.DrawRay(startpositon, Vector3.up*3f, Color.green, Mathf.Infinity);
            Debug.DrawRay(endpositon, Vector3.up*2f, Color.blue, Mathf.Infinity);

            Debug.Log("selectedEdges�� ���� : " + selectedEdges.Count);

            //���� �θ������Ʈ (���̾��Ű ����)
            GameObject parent = Instantiate(parentPrefab, Vector3.zero, Quaternion.identity);
            parent.transform.name = "HallWay " + countHall + "��";

            //b�� ���� Ž������ �̿� ��带 ��Ÿ����. a�� ���س�����
            /*
             costFunction�� FindPath �Լ����� ȣ��� ��, ���� ����� a�� �̿� ����� b�� ������ �ڵ����� ���޵˴ϴ�. 
            �̶� a�� ���� Ž�� ���� ����� ��ġ�� ��Ÿ���ϴ�. 
            ���� a.Position�� ���� Ž�� ���� ����� ��ġ�� �˴ϴ�.

            �̷��� a�� ���� ����� ��ġ�� ��Ÿ���� ���� FindPath �Լ� ���ο��� ���� ����� ������ �ʱ�ȭ�ϰ�, �켱���� ť�� �߰��� �� �����˴ϴ�. 
            ���� ����� ������ �ʱ�ȭ�� �Ŀ� �켱���� ť�� ���� ��, a.Position�� �ش��ϴ� ��ġ ������ ����Ͽ� ���� ����� ��ġ�� �����մϴ�. 
            �׷��� ���� costFunction���� a�� ���� ��带 ��Ÿ���� �˴ϴ�.
             */
            var path = aStar.FindPath(startPos, endPos, (Pathfinder2D.Node a, Pathfinder2D.Node b) =>
            {
                var pathCost = new Pathfinder2D.PathCost();

                pathCost.cost = Vector2Int.Distance(b.Position, endPos); // heuristic // b�� ��ġ�� �������?

                if (grid[b.Position] == CellType.Room)
                {
                    pathCost.cost += 10;
                }
                else if (grid[b.Position] == CellType.None)
                {
                    pathCost.cost += 5;
                }
                else if (grid[b.Position] == CellType.Hallway)
                {
                    pathCost.cost += 1;
                }

                pathCost.traversable = true;

                return pathCost;
            }
            );


            if (path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    var current = path[i];
                    //Debug.DrawRay(new Vector3(current.x, 0, current.y), Vector3.up * 10f, Color.cyan, Mathf.Infinity);

                    if (grid[current] == CellType.None)
                    {
                        grid[current] = CellType.Hallway; // ����̸� ������ �ٲ۴�.

                        
                        if(grid[path[i-1]] == CellType.Room)
                        {
                            Debug.DrawRay(new Vector3(path[i-1].x, 0, path[i-1].y), Vector3.up * 4f, Color.black, Mathf.Infinity);
                            Debug.Log("i-1��°(ù ��ι��� ����)�� ���̴�");

                            Vector3 prevPos = new Vector3(path[i - 1].x, 2f, path[i - 1].y);

                            //��(Room)�� ���Ա� ����� �ٽ� �����.
                            RegenerateEntrance(prevPos, parent);
                        }

                        if(grid[path[i+1]] == CellType.Room)
                        {
                            Debug.DrawRay(new Vector3(path[i + 1].x, 0, path[i + 1].y), Vector3.up * 6f, Color.white, Mathf.Infinity);
                            Debug.Log("i+1��°(������ ��ι��� ����)�� ���̴�");

                            Vector3 nextPos = new Vector3(path[i + 1].x, 2f, path[i + 1].y);

                            RegenerateEntrance(nextPos, parent);
                        }

                    }

                    if (i > 0 && i < path.Count-1)
                    {
                        // ������ -> ����� -> ������, delta�� (1,0)�̸� ������, (-1,0)�̸� ����, (0,1)�̸� ��, (0,-1)�̸� �Ʒ�
                        var prev = path[i - 1];

                        var next = path[i + 1];

                        var delta1 = current - prev;

                        var delta2 = next - current;

                        CellDirection delta1Dir;
                        CellDirection delta2Dir;

                        delta1Dir = CheckCellDirection(delta1);
                        delta2Dir = CheckCellDirection(delta2);

                        if(grid[current] == CellType.Hallway)
                        {
                            PlaceHallway(delta1Dir, delta2Dir, current, parent);
                        }
                    }

                }

                foreach (var pos in path)
                {
                    if (grid[pos] == CellType.Hallway)
                    {
                        //PlaceHallway(pos);
                    }
                }
            }
            countHall++;
        }
    }

    private void PathfindDungeonGate()
    {
        // �ΰ��� ������ �������.

        Vertex vertexU = new Vertex(new Vector3(5, 0, 0));
        Vertex vertexV = new Vertex(new Vector3(entrancePosition.x, 0, entrancePosition.y));

        Edge edge = new Edge(vertexU, vertexV);

        Vector2Int startPos = new Vector2Int(10, 0);
        Vector2Int endPos = entrancePosition;

        gridEntrance[endPos] = CellType.Room;

        Debug.Log("��� ���� ������? Celltype�� �����ֳ�?" + gridEntrance[new Vector2Int(0, 0)]);
        Debug.Log("��� endpos��ġ�� Room�� �־����� Room�� ��������?" + gridEntrance[endPos]);

        Pathfinder2D aStar = new Pathfinder2D(sizeEntrance);

        GameObject parent = Instantiate(parentPrefab, Vector3.zero, Quaternion.identity);
        parent.transform.name = "����-���� ���";

        var path = aStar.FindPathToGate(startPos, endPos, (Pathfinder2D.Node a, Pathfinder2D.Node b) =>
        {
            var pathCost = new Pathfinder2D.PathCost();

            pathCost.cost = Vector2Int.Distance(b.Position, endPos); // heuristic // b�� ��ġ�� �������?

            if (gridEntrance[b.Position] == CellType.Room)
            {
                pathCost.cost += 10;
            }
            else if (gridEntrance[b.Position] == CellType.None)
            {
                pathCost.cost += 5;
            }
            else if (gridEntrance[b.Position] == CellType.Hallway)
            {
                pathCost.cost += 1;
            }

            pathCost.traversable = true;

            return pathCost;
        }
        );

        if (path != null)
        {
            Debug.Log("�� �׸��� path�� ���� ��⳪?");
            Debug.Log("�� �׸��� path �? " + path.Count);
            for (int i = 0; i < path.Count; i++)
            {
                var current = path[i];

                if (gridEntrance[current] == CellType.None)
                {
                    gridEntrance[current] = CellType.Hallway; // ����̸� ������ �ٲ۴�.


                    //if (gridEntrance[path[i - 1]] == CellType.Room)
                    //{
                    //    Vector3 prevPos = new Vector3(path[i - 1].x, 2f, path[i - 1].y);
                    //}

                    //if (gridEntrance[path[i + 1]] == CellType.Room)
                    //{
                    //    Vector3 nextPos = new Vector3(path[i + 1].x, 2f, path[i + 1].y);
                    //}

                }

                if (i > 0 && i < path.Count - 1)
                {
                    // ������ -> ����� -> ������, delta�� (1,0)�̸� ������, (-1,0)�̸� ����, (0,1)�̸� ��, (0,-1)�̸� �Ʒ�
                    var prev = path[i - 1];

                    var next = path[i + 1];

                    var delta1 = current - prev;

                    var delta2 = next - current;

                    CellDirection delta1Dir;
                    CellDirection delta2Dir;

                    delta1Dir = CheckCellDirection(delta1);
                    delta2Dir = CheckCellDirection(delta2);

                    if (gridEntrance[current] == CellType.Hallway)
                    {
                        PlaceHallway(delta1Dir, delta2Dir, current, parent);
                        Vector3 currentPos = new Vector3(current.x * 3, 2f, current.y * 3) - new Vector3(10f, 0.5f, 80f);
                        Debug.DrawRay(currentPos, Vector3.up * 10f, Color.red, Mathf.Infinity);
                        //�� �������� �����Ա��� �����ϰ� �Ѵ�.
                        if(!canUseDungeonEntrance)
                        {
                            canUseDungeonEntrance = true;
                            DungeonEntrance(currentPos, delta1Dir, parent);
                        }

                        
                    }
                }

                //ù�� ���Ա� �� ����
                if (i == path.Count - 2)
                {
                    Vector3 currentPos = new Vector3(current.x * 3, 2f, current.y * 3) - new Vector3(10f, 0, 80f);
                    DeleteFirstRoomWall(currentPos);
                }
            }
        }
    }

    //���� ���� �����
    private void CreateRoomCeiling(Room newRoom)
    {
        Vector2 centerPosf = newRoom.bounds.center;
        Vector2Int centerPos = new Vector2Int((int)centerPosf.x, (int)centerPosf.y);

        Vector3 offset = new Vector3(-10f, 0, -80f);

        //Width ¦�� -> x : -1.5 ����, Ȧ�� -> �״��
        if(newRoom.bounds.width % 2 == 0)
        {
            offset += new Vector3(-1.5f, 0, 0); 
        }
        
        //Height ¦�� -> z : -1.5 ����, Ȧ�� -> �״��
        if(newRoom.bounds.height % 2 == 0)
        {
            offset += new Vector3(0, 0, -1.5f);
        }

        Vector3 centerPosition = new Vector3(centerPos.x * 3, 0.6f, centerPos.y * 3) + offset;
        //Vector3 centerPositionf = new Vector3(centerPosf.x * 3, 1f, centerPosf.y * 3) - new Vector3(10f, 0, 80f);
        Debug.DrawRay(centerPosition, Vector3.up * 15f, Color.blue, Mathf.Infinity);
        //Debug.DrawRay(centerPositionf, Vector3.up * 15f, Color.red, Mathf.Infinity);
        GameObject roomCeiling = Instantiate(roomCeilingPrefab, centerPosition, Quaternion.identity);
        roomCeiling.transform.localScale = new Vector3(newRoom.bounds.width*3, 9f, newRoom.bounds.height*3);
    }

    //������ �����
    private void CreateBossRoom()
    {
        Vector3 offset = new Vector3(-10f, 0, -80f);
        RaycastHit hit;

        Vector3 bossRoomPos = new Vector3((int)bossRoomPosition.x * 3, 6f, (int)bossRoomPosition.y * 3) + offset;
        Debug.DrawRay(bossRoomPos - Vector3.up * 5, Vector3.up * 30f, Color.white, Mathf.Infinity);

        //������ ������ ��
        int halfSize_x = (int)bossRoom_xMax - (int)bossRoomPosition.x;
        int halfSize_z = (int)bossRoom_yMax - (int)bossRoomPosition.y;

        if(Physics.Raycast(bossRoomPos, Vector3.up, out hit, 100f))
        {
            Debug.Log("�����濡�� ���� ������ ���� �༮ �̸� : " + hit.transform.gameObject.name);
            Destroy(hit.transform.gameObject);
        }

        if (Physics.Raycast(bossRoomPos - Vector3.up*3, Vector3.back, out hit, 100f)) // z��
        {
            Debug.Log("������ ���� ���� �༮ �̸� : " + hit.transform.gameObject.name + "��ġ : " + hit.transform.position);
            if(!(hit.transform.gameObject.name == "Wall_01"))
            {
                GameObject bossRoom = Instantiate(bossRoomPrefab, bossRoomPos + new Vector3(0,0,halfSize_z*3), Quaternion.identity);
                bossRoom.GetComponent<Transform>().rotation = Quaternion.Euler(0, 180, 0);
            }
            else if (Physics.Raycast(bossRoomPos - Vector3.up * 3, Vector3.left, out hit, 100f)) // x��
            {
                Debug.Log("else if ������ ���� ���� �༮ �̸� : " + hit.transform.gameObject.name + "��ġ : " + hit.transform.position);
                if (!(hit.transform.gameObject.name == "Wall_01"))
                {
                    GameObject bossRoom = Instantiate(bossRoomPrefab, bossRoomPos + new Vector3(halfSize_x * 3, 0, 0), Quaternion.identity);
                    bossRoom.GetComponent<Transform>().rotation = Quaternion.Euler(0, 270, 0);
                }
                else if (Physics.Raycast(bossRoomPos - Vector3.up * 3, Vector3.right, out hit, 100f)) // x��
                {
                    Debug.Log("else if�� else if ������ ���� ���� �༮ �̸� : " + hit.transform.gameObject.name + "��ġ : " + hit.transform.position);
                    if (!(hit.transform.gameObject.name == "Wall_01"))
                    {
                        GameObject bossRoom = Instantiate(bossRoomPrefab, bossRoomPos - new Vector3(halfSize_x * 3, 0, 0), Quaternion.identity);
                        bossRoom.GetComponent<Transform>().rotation = Quaternion.Euler(0, 90, 0);
                    }
                }
            }
        }
        
    }

    //������ ã��
    private void FindBossRoom(Room newRoom)
    {
        if (bossRoom_yMax <= newRoom.bounds.yMax)
        {
            bossRoom_yMax = newRoom.bounds.yMax;
            bossRoomPosition.y = newRoom.bounds.center.y;

            //y �������� ���� ��쿡 x�� �� ū�� ������ġ
            if (bossRoomPosition.x < newRoom.bounds.position.x)
            {
                bossRoomPosition = newRoom.bounds.center;
            }

            bossRoomPosition.x = newRoom.bounds.center.x;
            bossRoom_xMax = newRoom.bounds.xMax;
        }
    }


    private void PlaceCube(Vector2Int location, Vector2Int size)
    {
        //if(size.x <= 2 || size.y <=2) �̷������� ����� �� ���ϸ� �������� �ʰ� �� �� �ִ�.
        //������ ���� �ĳ���.

        //���ǿ� ���� ���� ���� ���Ѵ�.
        //�ڳ��̸�, Corner -> Rotate y�� �ְ�, BL(BottomLeft), BR(BottomRight), UL(UpLeft), UR(UpRight)
        //���Ա���, Door
        //���� ��躮�̸�, Wall

        //�ڳ��� ���� -> BL : location.x, BR : location.x + size.x, UL : location.y, UR : location.y + size.y
        //���Ա��� ���� -> ��ΰ� Ȯ���Ǿ����
        //���� �� -> �ڳʿ� ���Ա�, ���θ� ������ ��� ��

        //���� �θ������Ʈ (���̾��Ű �����ϱ� ���ؼ�)
        GameObject parent = Instantiate(parentPrefab, Vector3.zero, Quaternion.identity);
        parent.gameObject.name = "Room " + count + "��";

        

        if (size.x > 3 && size.y > 3)
        {
            //GameObject go = Instantiate(cubePrefab, new Vector3(location.x, 0, location.y), Quaternion.identity);
            //go.GetComponent<Transform>().localScale = new Vector3(size.x, 3, size.y); // ���� local scale �� ũ�⸦ ���� �� �ִ�.
            //go.GetComponent<MeshRenderer>().material = material;

            //��
            for(int i = 0; i < size.y; i++)
            {
                //��
                for (int j = 0; j < size.x; j++)
                {
                    Vector3 createPos = new Vector3(location.x*3, 2f, location.y*3) + new Vector3(j*3, 0, i*3) - new Vector3(10f,0.5f,80f);

                    int index;
                    int rand = random.Next(0, 10);

                    if (rand >= 0 && rand < 6)
                    {
                        index = 0; // �Ϲ�
                    }
                    else
                    {
                        index = 1; // ȶ��
                    }

                    //�ڳ� i=0,j=0 , i=0,j=size.x-1 , i=size.y-1,j=0 , i=size.y-1 j=size.x
                    if (i == 0 && j == 0)
                    {

                        GameObject cornerBL = Instantiate(cornerBottomLeftPrefab, createPos, Quaternion.identity);
                        cornerBL.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                        cornerBL.gameObject.name = "Prefab_" + count + "cornerBL";
                        cornerBL.transform.parent = parent.transform;
                        continue;
                    }
                    else if(i==0 && j == size.x-1)
                    {
                        GameObject cornerBR = Instantiate(cornerBottomRightPrefab, createPos, Quaternion.identity);
                        cornerBR.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                        cornerBR.gameObject.name = "Prefab_" + count + "cornerBR";
                        cornerBR.transform.parent = parent.transform;
                        continue;
                    }
                    else if (i == size.y -1 && j == 0)
                    {
                        GameObject cornerTL = Instantiate(cornerTopLeftPrefab, createPos, Quaternion.identity);
                        cornerTL.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                        cornerTL.gameObject.name = "Prefab_" + count + "cornerUL";
                        cornerTL.transform.parent = parent.transform;
                        continue;
                    }
                    else if (i == size.y-1 && j == size.x - 1)
                    {
                        GameObject cornerTR = Instantiate(cornerTopRightPrefab, createPos, Quaternion.identity);
                        cornerTR.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                        cornerTR.gameObject.name = "Prefab_" + count + "cornerUR";
                        cornerTR.transform.parent = parent.transform;
                        continue;
                    }
                    //��
                    else if(i == 0 && (j != 0 || j != size.x-1))
                    {
                        GameObject wallBottom = Instantiate(wallBottomPrefabs[index], createPos, Quaternion.identity);
                        wallBottom.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                        wallBottom.gameObject.name = "Prefab_" + count + "wallBottom";
                        wallBottom.transform.parent = parent.transform;
                        continue;
                    }
                    else if(i == size.y -1 && (j != 0 || j != size.x - 1))
                    {
                        GameObject wallTop = Instantiate(wallTopPrefabs[index], createPos, Quaternion.identity);
                        wallTop.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                        wallTop.gameObject.name = "Prefab_" + count + "wallUp";
                        wallTop.transform.parent = parent.transform;
                        continue;
                    }
                    else if ((i != 0 || i != size.y - 1) && j == 0)
                    {
                        GameObject wallLeft = Instantiate(wallLeftPrefabs[index], createPos, Quaternion.identity);
                        wallLeft.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                        wallLeft.gameObject.name = "Prefab_" + count + "wallLeft";
                        wallLeft.transform.parent = parent.transform;
                        continue;
                    }
                    else if ((i != 0 || i != size.y - 1) && j == size.x - 1)
                    {
                        GameObject wallRight = Instantiate(wallRightPrefabs[index], createPos, Quaternion.identity);
                        wallRight.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                        wallRight.gameObject.name = "Prefab_" + count + "wallRight";
                        wallRight.transform.parent = parent.transform;
                        continue;
                    }


                    GameObject go = Instantiate(cubePrefab, createPos, Quaternion.identity);
                    go.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                    //go.gameObject.transform.localScale = new Vector3(1, 3, 1);
                    int index1 = size.x * i + j;
                    go.gameObject.name = "Prefab_" +count+" (" + index1 +")";
                    go.transform.parent = parent.transform;
                }
            }

            count++;
        }
        
        
        //���� ������ size.x�� ���϶� size.y�� ���� ��
        //�׿� �ش��ϴ� prefab�� �����
        //���� ����ϰ� �ִ� �޼��带 PlaceRoom���� �ΰ� ��δ� PlaceHallway�� �Ἥ �ٸ��� ������ �ȴ�.
        //���� �������� �ǵ��� ���� ���� ������ ��ġ location���ٰ� �ʺ�� ���̸� ���ؼ� 
        //Ÿ�� �ϳ��ϳ� �����ϰ� �ٿ������ ���� ������?
        //for���� �Ἥ ù��° index�� ��� ���� �����(���� ���)
        //�ϳ��� ť�꿡 

    }

    private void PlaceRoom(Vector2Int location, Vector2Int size)
    {
        PlaceCube(location, size);
    }

    //��� ����
    private void PlaceHallway(CellDirection delta1Dir, CellDirection delta2Dir, Vector2Int current, GameObject parent)
    {
        Vector3 createPos = new Vector3(current.x*3, 2f, current.y*3) - new Vector3(10f,0.5f,80f);

        //�����Ա��� enum����
        HallwayCellPrefab hallwaycell;

        //ȶ�� ������
        int index;
        int rand = random.Next(0, 10);

        if (rand >= 0 && rand < 6)
        {
            index = 0; // �Ϲ�
        }
        else
        {
            index = 1; // ȶ��
        }

        if (delta1Dir == CellDirection.right)
        {
            if (delta2Dir == CellDirection.right) //������-������ : �Ϲ������
            {
                //�����Ѵ�.
                GameObject Hallway_Straight = Instantiate(hallwayXPrefabs[index], createPos, Quaternion.identity);
                Hallway_Straight.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                Hallway_Straight.transform.parent = parent.transform;
            }
            else if (delta2Dir == CellDirection.up) // ������-�� : ���� ���
            {
                GameObject Hallway_Corner = Instantiate(hallwayCornerBRPrefab, createPos, Quaternion.identity);
                Hallway_Corner.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                Hallway_Corner.transform.parent = parent.transform;
            }
            else if (delta2Dir == CellDirection.down) // ������-�Ʒ� : ���� ���
            {
                GameObject Hallway_Corner = Instantiate(hallwayCornerTRPrefab, createPos, Quaternion.identity);
                Hallway_Corner.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                Hallway_Corner.transform.parent = parent.transform;
            }
        }
        else if (delta1Dir == CellDirection.left)
        {
            if (delta2Dir == CellDirection.left) // ����-����
            {
                GameObject Hallway_Straight = Instantiate(hallwayXPrefabs[index], createPos, Quaternion.identity);
                Hallway_Straight.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                Hallway_Straight.transform.parent = parent.transform;
            }
            else if (delta2Dir == CellDirection.up) // ����-��
            {
                GameObject Hallway_Corner = Instantiate(hallwayCornerBLPrefab, createPos, Quaternion.identity);
                Hallway_Corner.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                Hallway_Corner.transform.parent = parent.transform;
            }
            else if (delta2Dir == CellDirection.down) // ����-�Ʒ�
            {
                GameObject Hallway_Corner = Instantiate(hallwayCornerTLPrefab, createPos, Quaternion.identity);
                Hallway_Corner.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                Hallway_Corner.transform.parent = parent.transform;
            }
        }
        else if (delta1Dir == CellDirection.up)
        {
            if (delta2Dir == CellDirection.right) // ��-������
            {
                GameObject Hallway_Corner = Instantiate(hallwayCornerTLPrefab, createPos, Quaternion.identity);
                Hallway_Corner.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                Hallway_Corner.transform.parent = parent.transform;
            }
            else if (delta2Dir == CellDirection.left) // ��-����
            {
                GameObject Hallway_Corner = Instantiate(hallwayCornerTRPrefab, createPos, Quaternion.identity);
                Hallway_Corner.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                Hallway_Corner.transform.parent = parent.transform;
            }
            else if (delta2Dir == CellDirection.up) // ��-��
            {
                GameObject Hallway_Straight = Instantiate(hallwayYPrefabs[index], createPos, Quaternion.identity);
                Hallway_Straight.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                Hallway_Straight.transform.parent = parent.transform;
            }
        }
        else if (delta1Dir == CellDirection.down)
        {
            if (delta2Dir == CellDirection.right) // �Ʒ�-������
            {
                GameObject Hallway_Corner = Instantiate(hallwayCornerBLPrefab, createPos, Quaternion.identity);
                Hallway_Corner.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                Hallway_Corner.transform.parent = parent.transform;
            }
            else if (delta2Dir == CellDirection.left) //�Ʒ�-����
            {
                GameObject Hallway_Corner = Instantiate(hallwayCornerBRPrefab, createPos, Quaternion.identity);
                Hallway_Corner.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                Hallway_Corner.transform.parent = parent.transform;
            }
            else if (delta2Dir == CellDirection.down) // �Ʒ�-�Ʒ�
            {
                GameObject Hallway_Straight = Instantiate(hallwayYPrefabs[index], createPos, Quaternion.identity);
                Hallway_Straight.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                Hallway_Straight.transform.parent = parent.transform;
            }
        }

        //�ΰ��� ���� �������� �ִ��� Ȯ���ϰ� �ΰ���� ��� �������� �ϳ��� ����. �ΰ��ΰ��� ���ϰ� �Ʒ�, �Ǵ� �ϳ��� ���̴�.
        CheckOverlapHallway(createPos);

        //���� �ƹ��͵� ����� ��ġ�� �ڳ� ceiling ����
        //CreateCross(createPos);
    }

    private CellDirection CheckCellDirection(Vector2Int delta)
    {
        CellDirection result;

        if(delta == Vector2Int.right)
        {
            result = CellDirection.right;
        }
        else if(delta == Vector2Int.left)
        {
            result = CellDirection.left;
        }
        else if(delta == Vector2Int.up)
        {
            result = CellDirection.up;
        }
        else if(delta == Vector2Int.down)
        {
            result = CellDirection.down;
        }
        else
        {
            result = CellDirection.none;
        }

        return result;
    }

    //��(Room)�� ���Ա� ����� �ٽ� �����.
    private void RegenerateEntrance(Vector3 Pos, GameObject parent)
    {
        Pos = new Vector3(Pos.x *3, Pos.y, Pos.z *3) - new Vector3(10f,0.5f,80f);

        DeleteCube(Pos);

        GameObject RoomEntrance = Instantiate(cubePrefab, Pos, Quaternion.identity);
        RoomEntrance.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
        RoomEntrance.transform.name = "RoomEntrance";
        RoomEntrance.transform.parent = parent.transform;
    }

    //���� ��ġ�°� Ȯ���ϰ� ����
    private void CheckOverlapHallway(Vector3 checkPos)
    {
        int checkCount = 0;
        int checkCount_forward = 0;
        int checkCount_back = 0;
        int checkCount_right = 0;
        int checkCount_left = 0;

        //����ǥ��
        CellDirection checkCellDirection;

        //�Ʒ����� ��ġ�°� ����
        Collider[] colliders = Physics.OverlapCapsule(checkPos, checkPos + Vector3.down * 0.5f, 0.1f);
        foreach(Collider collider in colliders)
        {
            checkCount++;
            if(checkCount >= 2)
            {
                Debug.Log("2�� �̻� ��ü�� ��ģ���� �ִ�.");
                Debug.Log("colliders�� ���� : " + colliders.Length);
                DeleteCube(checkPos);
                Debug.Log(checkPos);
                

                //�չ���
                Collider[] colliders_forward = Physics.OverlapCapsule(checkPos, checkPos + Vector3.forward * 0.3f, 0.01f);
                foreach (Collider collider_forward in colliders_forward)
                {
                    checkCount_forward++;
                    if (checkCount_forward >= 2)
                    {
                        Debug.Log("������ ����, ��ġ : " + checkPos);
                        checkCellDirection = CellDirection.up;
                        CreateCrossCube(checkPos, checkCellDirection);
                    }
                }

                //�޹���
                Collider[] colliders_back = Physics.OverlapCapsule(checkPos, checkPos + Vector3.back * 0.3f, 0.01f);
                foreach (Collider collider_back in colliders_back)
                {
                    checkCount_back++;
                    if (checkCount_back >= 2)
                    {
                        Debug.Log("�� ����, ��ġ : " + checkPos);
                        checkCellDirection = CellDirection.down;
                        CreateCrossCube(checkPos, checkCellDirection);
                    }
                }

                //������
                Collider[] colliders_right = Physics.OverlapCapsule(checkPos, checkPos + Vector3.right * 0.3f, 0.01f);
                foreach (Collider collider_right in colliders_right)
                {
                    checkCount_right++;
                    if (checkCount_right >= 2)
                    {
                        Debug.Log("����Ʈ ����, ��ġ : " + checkPos);
                        checkCellDirection = CellDirection.right;
                        CreateCrossCube(checkPos, checkCellDirection);
                    }
                }

                //����
                Collider[] colliders_left = Physics.OverlapCapsule(checkPos, checkPos + Vector3.left * 0.3f, 0.01f);
                foreach (Collider collider_left in colliders_left)
                {
                    checkCount_left++;
                    if (checkCount_left >= 2)
                    {
                        Debug.Log("����Ʈ ����, ��ġ : " + checkPos);
                        checkCellDirection = CellDirection.right;
                        CreateCrossCube(checkPos, checkCellDirection);
                    }
                }
            }
        }

    }

    //6���� ����, up,down,right,left,forward,back
    private void DeleteCube(Vector3 Pos)
    {
        RaycastHit hit;
        RaycastHit[] hits;

        if (Physics.Raycast(Pos, Vector3.up, out hit, 10f))
        {
            Debug.Log("ù ���̿� �ε��� �༮ ��ġ : " + hit.transform.position + "���̿� �ε��� �༮ �̸� : " + hit.transform.name);
            Destroy(hit.transform.gameObject);
            Debug.Log("y�� ���� ���� ������ : " + Pos);
            
        }

        //hallway ��ġ�°� 2�� �� ����
        hits = Physics.RaycastAll(Pos, Vector3.up, 10f);
        foreach(var hit_up in hits)
        {
            Debug.Log("hit_up ��ġ : " + hit_up.transform.position + "hit_up �̸� : " + hit_up.transform.name);
            Destroy(hit_up.transform.gameObject);
        }

        if (Physics.Raycast(Pos, Vector3.down, out hit, 1f))
        {
            Destroy(hit.transform.gameObject);
        }

        if (Physics.Raycast(Pos, Vector3.forward, out hit, 0.5f))
        {
            DestroyTorch(hit);
            Destroy(hit.transform.gameObject);
        }

        if (Physics.Raycast(Pos, Vector3.back, out hit, 0.5f))
        {
            DestroyTorch(hit);
            Destroy(hit.transform.gameObject);
        }

        if (Physics.Raycast(Pos, Vector3.right, out hit, 0.5f))
        {
            DestroyTorch(hit);
            Destroy(hit.transform.gameObject);
        }

        if (Physics.Raycast(Pos, Vector3.left, out hit, 0.5f))
        {
            DestroyTorch(hit);
            Destroy(hit.transform.gameObject);
        }
    }

    //��ġ�� �ڳ� ����
    private void CreateCrossCube(Vector3 createPos, CellDirection cellDirection)
    {
        Debug.DrawRay(createPos + new Vector3(0, 10f, 0), Vector3.down * 10f, Color.red, Mathf.Infinity);

        switch (cellDirection)
        {
            case CellDirection.up:
                GameObject hallwayCross_up = Instantiate(hallwayCrossPrefab, createPos, Quaternion.identity);
                hallwayCross_up.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                break;
            case CellDirection.down:
                GameObject hallwayCross_down = Instantiate(hallwayCrossPrefab, createPos, Quaternion.identity);
                hallwayCross_down.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                hallwayCross_down.transform.localRotation = Quaternion.Euler(0, 180, 0);
                break;
            case CellDirection.right:
                GameObject hallwayCross_right = Instantiate(hallwayCrossPrefab, createPos, Quaternion.identity);
                hallwayCross_right.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                hallwayCross_right.transform.localRotation = Quaternion.Euler(0, 90, 0);
                break;
            case CellDirection.left:
                GameObject hallwayCross_left = Instantiate(hallwayCrossPrefab, createPos, Quaternion.identity);
                hallwayCross_left.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                hallwayCross_left.transform.localRotation = Quaternion.Euler(0, 270, 0);
                break;
        }
    }

    //����-�����Ա� ����, down �ؿ��� ����. �� �� �� 3����
    private void DungeonEntrance(Vector3 createPos, CellDirection cellDir, GameObject parent)
    {
        switch(cellDir)
        {
            case CellDirection.right:
                GameObject dungeonEntrance_right = Instantiate(dungeonEntrancePrefab, createPos, Quaternion.identity);
                dungeonEntrance_right.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                dungeonEntrance_right.transform.localRotation = Quaternion.Euler(0, 90, 0);
                dungeonEntrance_right.transform.parent = parent.transform;
                break;
            case CellDirection.left:
                GameObject dungeonEntrance_left = Instantiate(dungeonEntrancePrefab, createPos, Quaternion.identity);
                dungeonEntrance_left.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                dungeonEntrance_left.transform.localRotation = Quaternion.Euler(0, 270, 0);
                dungeonEntrance_left.transform.parent = parent.transform;
                break;
            case CellDirection.up:
                GameObject dungeonEntrance_top = Instantiate(dungeonEntrancePrefab, createPos, Quaternion.identity);
                dungeonEntrance_top.GetComponent<Transform>().localScale = new Vector3(3, 3f, 3);
                dungeonEntrance_top.transform.parent = parent.transform;
                break;
        }
    }

    //ù�� ���Ա� �� ����
    private void DeleteFirstRoomWall(Vector3 currentPos)
    {
        Debug.DrawRay(currentPos, Vector3.up * 30f, Color.white, Mathf.Infinity);
        RaycastHit hit;
        
        if(Physics.Raycast(currentPos, Vector3.forward, out hit, 5f))
        {
            Debug.Log("DeleteFirstRoomWall if�� ����");
            Destroy(hit.transform.gameObject);
        }
    }

    //ȶ������
    private void DestroyTorch(RaycastHit hit)
    {
        Debug.Log("���� �༮�� �ڽ� ���� : " + hit.transform.childCount);
        if(hit.transform.childCount > 0)
        {
            Debug.Log("ù��° �ڽ� �̸�? " + hit.transform.GetChild(0));
            if (hit.transform.GetChild(hit.transform.childCount - 1).transform.name == "Torch_01")
            {
                Debug.Log("������, ��ġ�� ? " + hit.transform.GetChild(hit.transform.childCount -1).transform.position);
                Destroy(hit.transform.GetChild(hit.transform.childCount - 1).transform.gameObject);
            }
        }
    }

//��� DrawLine �����ְ� �ڷ�ƾ
private IEnumerator ShowLine(List<Prim.Edge> edges)
    {
        for (int i = 1; i < edges.Count; i++)
        {

            Vector3 checkEdgePosition_U = new Vector3(edges[i - 1].U.Position.x, 0, edges[i - 1].U.Position.y);
            Vector3 checkEdgePosition_U2 = new Vector3(edges[i].U.Position.x, 0, edges[i].U.Position.y);
            Vector3 checkEdgePosition_V = new Vector3(edges[i - 1].V.Position.x, 0, edges[i - 1].V.Position.y);
            Vector3 checkEdgePosition_V2 = new Vector3(edges[i].V.Position.x, 0, edges[i].V.Position.y);

            Debug.Log("ù��° U�� ������ : " + checkEdgePosition_U);
            Debug.Log("�ι�° U�� ������ : " + checkEdgePosition_U2);

            Debug.DrawLine(checkEdgePosition_U, checkEdgePosition_U2, Color.green, Mathf.Infinity);
            Debug.DrawLine(checkEdgePosition_V, checkEdgePosition_V2, Color.blue, Mathf.Infinity);

            yield return new WaitForSeconds(0.3f);
        }
    }

}
