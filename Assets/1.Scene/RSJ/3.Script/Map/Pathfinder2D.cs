using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueRaja;

public class Pathfinder2D
{
    public class Node
    {
        /*
         �� ���� ��ġ, ���� ���, ��� ������ ���´�.
         */
        public Vector2Int Position { get; private set; } 
        public Node Previous { get; set; }
        public float Cost { get; set; }

        public Node(Vector2Int position)
        {
            Position = position;
        }
    }

    public struct PathCost
    {
        public bool traversable;
        public float cost;
    }

    //�ֺ� �̿� ����� ������� ��ġ�� ��Ÿ���� ��� �迭
    static readonly Vector2Int[] neighbors =
        {
        new Vector2Int(1,0), // ������
        new Vector2Int(-1, 0), // ����
        new Vector2Int(0, 1), // ��
        new Vector2Int(0, -1), // �Ʒ�
    };

    Grid2D<Node> grid; // 2D������ ������ ��ġ
    SimplePriorityQueue<Node, float> queue; //�켱���� ť, A*�˰��򿡼� ����.
    HashSet<Node> closed; // �̹� Ž���� ������ ������ ��Ÿ���� HashSet<Node>
    Stack<Vector2Int> stack; // ��θ� �籸���� �� ����� ����

    Grid2D<Node> gridEntrance;

    public Pathfinder2D(Vector2Int size)
    {
        grid = new Grid2D<Node>(size, Vector2Int.zero);
        queue = new SimplePriorityQueue<Node, float>();
        closed = new HashSet<Node>();
        stack = new Stack<Vector2Int>();
        /*
        �ν��Ͻ� �׻� ��������
        �ΰ� ������ �߻��ϴ� ��쿡�� stack�� ���� �ǰ� ������ �ν��Ͻ�ȭ���� �ʾ��� ���Դϴ�.
        ���� ������ �ذ��Ϸ��� stack ������ �ùٸ��� �ʱ�ȭ�Ǿ����� Ȯ���ؾ� �մϴ�. 
        C#���� ������ ����� ���� ������ ���� ������ �����ϰ� �ʱ�ȭ�ؾ� �մϴ�:
         */

        for (int x = 0; x < size.x; x++)
        {
            for(int y = 0; y < size.y; y++)
            {
                grid[x, y] = new Node(new Vector2Int(x, y)); // �׸��� ��ġ�� ��� ��ġ, ��� �����ڴ� ���� �Ű������� ���� ������ �״�� ���
            }
        }

        //����-���� ��ο� ����� �׸���
        gridEntrance = new Grid2D<Node>(size, Vector2Int.zero);

        for(int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                gridEntrance[x, y] = new Node(new Vector2Int(x, y)); 
            }
        }
    }

    // �׸��� ���� ��� ����� ���� ���� ����� �ʱ�ȭ
    /*
     ResetNodes �޼��� ������ node ������ grid�� �� ��Ҹ� �����ϰ� �ֽ��ϴ�. 
    grid�� 2D �迭�μ� Node Ŭ������ �ν��Ͻ��� ��� �ֽ��ϴ�. 
    ���� node ������ grid[x, y]�� ���۷����� ������ �ֽ��ϴ�.

    node.Previous�� node.Cost�� �ʱ�ȭ�ϰ� �ִ� ���� �� ���۷����� ���� ���� grid �迭 ���� Node ��ü�� �����Ͽ� ���� �����ϰ� �ֽ��ϴ�. 
    �̷��� ������ ResetNodes �޼��带 ȣ���ϸ� grid �迭 ���� ��� ��� ��ü�� Previous�� Cost �Ӽ��� �ʱ�ȭ�ǰ� �˴ϴ�.

    �޼��� ������ node ������ ���� ������ ����Ǿ�����, �� ������ grid�� ��ҿ� ���� ���۷����� �����ϰ� �����Ƿ� node�� ���� ������ ��� ��ü�� �Ӽ��� �����ϸ� ������ grid �迭 ���� ��� ��ü�� �ݿ��˴ϴ�. 
    �̷��� �����ν� ResetNodes �޼���� grid �迭 ���� ��� ��� ��ü�� �ʱ�ȭ�� �� �ֽ��ϴ�.
     */
    private void ResetNodes()
    {
        var size = grid.Size;

        for( int x= 0; x < size.x; x++)
        {
            for(int y = 0; y < size.y; y++)
            {
                var node = grid[x, y];
                node.Previous = null;
                node.Cost = float.PositiveInfinity;
            }
        }
    }

    //���������� ��ǥ ���������� ���� ��θ� ã�� �޼���. 
    //A*�˰����� ����Ͽ� ��θ� Ž���ϰ�, ��θ� ã���� ��θ� ��ȯ
    public List<Vector2Int> FindPath(Vector2Int start, Vector2 end, Func<Node, Node, PathCost> costFunction)
    // costFunction�� ���� ���� �̿� ��� ���� ����� ����ϴ� ��������Ʈ
    /*
     Func<Node, Node, PathCost>�� ��������Ʈ Ÿ������, �� ���� Node ��ü�� �Է����� �޾� PathCost ��ü�� ��ȯ�ϴ� �޼��峪 ���ٽ��� ��Ÿ���ϴ�. 
    �� ��������Ʈ�� FindPath �޼��忡�� ��� Ž�� �߿� �� ��� ���� ����� ����ϴ� ������ �մϴ�.

      delegate TResult System.Func<in T1, in T2, out TResult>(T1 args, T2 args)
     */
    {
        ResetNodes();
        queue.Clear();
        closed.Clear();

        queue = new SimplePriorityQueue<Node, float>(); // ���ο� queue��ü�� �����Ǿ� queue ���� �ʱ�ȭ�ؼ� �ٽ� ������. �Ű�������, Node�� �켱������ ����
        closed = new HashSet<Node>();

        grid[start].Cost = 0;
        queue.Enqueue(grid[start], 0); // �켱���� queue�� �߰��� �� a�� �����ȴ�. a:���۳��
                                       // �������� ���� �ִ´� �켱���� 0��, ���߿� Ž���Ҷ� ���� ���� Ž���� ��.

        //Debug.Log("ť�� �����Ҷ� ���? : " + queue.Count);
        bool isqueue = true;

        while (queue.Count > 0)
        {
            Node node = queue.Dequeue();
            closed.Add(node);

            //Debug.Log("ť�� ����̳� ����?" + queue.Count);

            if (node.Position == end)
            {
                //��尡 ���� �ٴٸ��� ����� �������� ��ȯ�Ѵ�. �̰� Generator2D�� �ִ� pathfinder�޼��� �ȿ� �ִ� var path��
                return ReconstructPath(node);
            }


            foreach (var offset in neighbors) // neighbors�� 4���� Vector2Int�� ���� �迭�̹Ƿ� foreach ��� ����
            {
                if (!grid.InBounds(node.Position + offset)) continue; //grid�ȿ� ������ �Ѿ��, �� ����� ��ġ�� �̿��� ��ġ�� ���ߴµ� �װ� grid ������ �Ѿ�� �ѱ��.
                                                                      //(��� ���Ѵ� ���) ex) ���� Ž���ߴµ� ���� ���̸� ������ ������ Ž��
                var neighbor = grid[node.Position + offset]; // grid ���� �ȿ� �ִٸ�, �ӽ÷� neighbor ������ ���� Ž���� ��ġ�� ���� �����Ѵ�.
                if (closed.Contains(neighbor)) continue; // �׸��� Ž���� neighbor�� ��ġ�� �̹� closed�� �ִٸ� ���� Ž������ ����.

                var pathCost = costFunction(node, neighbor); // ���� Ž���� ��ġ�̹Ƿ� pathCost�� ����غ���. 
                if (!pathCost.traversable) continue; // ����ġ�� ���������� �̵��� �� ������ �Ѿ��

                float newCost = node.Cost + pathCost.cost; // ������ ���(a)�� �ڽ�Ʈ�� ����� pathCost�� ���ϸ� ���ŵ� Cost�� ���´�.
                                                           // ������ node�� cost�� 0�̴�. grid[start].Cost = 0;

                    //Debug.Log("�������� �ڽ�Ʈ : " + node.Cost);
                    //Debug.Log("�̿��� �ڽ�Ʈ : " + neighbor.Cost);
                    //Debug.Log("pathCost�� �ڽ�Ʈ : " + pathCost.cost);
                    //Debug.Log("newCost�� �ڽ�Ʈ : " + newCost);



                if (newCost < neighbor.Cost) // ���θ��� �ڽ�Ʈ�� �̿��� �ڽ�Ʈ���� �۴ٸ�
                {
                    neighbor.Previous = node; // ���� ��忡 ���� ��带 �ְ�
                    neighbor.Cost = newCost; //�̿� �ڽ�Ʈ�� �� �ڽ�Ʈ�� �ִ´�.

                    if (queue.TryGetPriority(node, out float existingPriority)) // �켱���� ť���� ���� ����� �켱������ �����ɴϴ�.
                                                                                // ���� ť�� �̹� ���� ��尡 �����ϰ� �� �켱������ existingPriority�� ����Ǿ��ٸ� ������ �����մϴ�.
                    {
                        queue.UpdatePriority(node, newCost); //�켱���� ť���� ���� ����� �켱������ ���� ���� ���(newCost)���� ������Ʈ�մϴ�.
                    }
                    else // ���� ���� ��尡 �켱���� ť�� �������� �ʴ´ٸ� ������ �����մϴ�.
                    {
                        queue.Enqueue(neighbor, neighbor.Cost); //�켱���� ť�� �̿� ���(neighbor)�� �߰��ϰ�, �� ����� �켱������ ��������� �ּ� ���(neighbor.Cost)���� �����մϴ�.
                                                                //�̴� ���ο� ��θ� ã�� ��� �ش� ��带 �켱���� ť�� �߰����� �ǹ��մϴ�.
                    }
                }
            }
        }

        return null;
    }


    //���� ��θ� �籸���Ͽ� ��θ� ��ȯ
    //���� ��θ� �籸���Ͽ� stack�� �����ϰ�, ���Ŀ� �� ������ ����Ͽ� ���� ��θ� ��ȯ. 
    private List<Vector2Int> ReconstructPath(Node node)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        while (node != null)
        {
            stack.Push(node.Position);
            node = node.Previous;
        }

        while (stack.Count > 0)
        {
            result.Add(stack.Pop());
        }

        return result;
    }


    private void ResetNodesToGate()
    {
        var size = gridEntrance.Size;

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var node = gridEntrance[x, y];
                node.Previous = null;
                node.Cost = float.PositiveInfinity;
            }
        }
    }
    public List<Vector2Int> FindPathToGate(Vector2Int start, Vector2 end, Func<Node, Node, PathCost> costFunction)
    {
        ResetNodesToGate();
        queue.Clear();
        closed.Clear();

        queue = new SimplePriorityQueue<Node, float>();
        closed = new HashSet<Node>();

        gridEntrance[start].Cost = 0;
        queue.Enqueue(gridEntrance[start], 0); 

        bool isqueue = true;

        while (queue.Count > 0)
        {
            Node node = queue.Dequeue();
            closed.Add(node);

            if (node.Position == end)
            {
                return ReconstructPath(node);
            }

            foreach (var offset in neighbors) 
            {
                if (!gridEntrance.InBounds(node.Position + offset)) continue; 
                var neighbor = gridEntrance[node.Position + offset]; 
                if (closed.Contains(neighbor)) continue; 
                var pathCost = costFunction(node, neighbor); 
                if (!pathCost.traversable) continue; 

                float newCost = node.Cost + pathCost.cost; 

                if (newCost < neighbor.Cost) 
                {
                    neighbor.Previous = node; 
                    neighbor.Cost = newCost; 

                    if (queue.TryGetPriority(node, out float existingPriority)) 
                    {
                        queue.UpdatePriority(node, newCost);
                    }
                    else 
                    {
                        queue.Enqueue(neighbor, neighbor.Cost);
                    }
                }
            }
        }

        return null;
    }
}
