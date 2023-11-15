using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs;


public static class Prim 
{
    public class Edge : Graphs.Edge
    {
        public float Distance { get; private set; }

        public Edge(Vertex u, Vertex v) : base(u, v)
        {
            Distance = Vector3.Distance(u.Position, v.Position);
        }

        public static bool operator == (Edge left, Edge right)
        {
            return (left.U == right.U && left.V == right.V)
                || (left.U == right.V && left.V == right.U);
        }

        public static bool operator != (Edge left, Edge right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if(obj is Edge e)
            {
                return this == e;
            }

            return false;
        }

        public bool Equals(Edge e)
        {
            return this == e;
        }

        public override int GetHashCode()
        {
            return U.GetHashCode() ^ V.GetHashCode();
        }
    }

    public static List<Edge> MinimumSpanningTree(List<Edge> edges, Vertex start)
    {
        /*
         HashSet�̶�?
        HashSet�� C#���� �����ϴ� �÷��� Ÿ�� �� �ϳ���, �ߺ� ��Ҹ� ������� �ʰ�, 
        ������ �������� �ʴ� ����(Set)�� ��Ÿ���ϴ�. 
        ��, HashSet�� ������ ��ҵ��� �����ϸ�, ��ҵ��� ���� ���� ����˴ϴ�.

        HashSet�� Ư�� ����� ���� ���θ� ������ Ȯ���� �� �ְ�, �ߺ��� ��Ҹ� �ڵ����� �������ݴϴ�. 
        �̸� ���� �������� �ϰ����� �����ϰ� �ߺ� ��ҷ� ���� ������ ������ �� �ֽ��ϴ�.

        ex)
        HashSet<int> numbers = new HashSet<int>();
        numbers.Add(1);
        numbers.Add(2);
        numbers.Add(3);

        // �ߺ��� ��Ҵ� �ڵ����� ���ŵ�
        numbers.Add(1);

        // numbers���� �ߺ����� ���� 1, 2, 3�� ���Ե�

         */
        HashSet<Vertex> openSet = new HashSet<Vertex>();
        HashSet<Vertex> closedSet = new HashSet<Vertex>();

        foreach (var edge in edges)
        {
            openSet.Add(edge.U);
            openSet.Add(edge.V);
        }

        closedSet.Add(start);

        List<Edge> results = new List<Edge>();

        while (openSet.Count > 0)
        {
            bool chosen = false;
            Edge chosenEdge = null;
            float minWeight = float.PositiveInfinity;

            foreach (var edge in edges)
            {
                int closedVertices = 0;
                if (!closedSet.Contains(edge.U)) closedVertices++;
                if (!closedSet.Contains(edge.V)) closedVertices++;
                if (closedVertices != 1) continue;
                /*
                 �� �ڵ忡�� closedVertices ������ ���� ���� edge�� �� �� ���� �� MST�� �̹� �߰��� ������ ������ ��Ÿ���ϴ�. 
                closedSet�� ���Ե��� ���� ������ �� �� �̻��̰ų�(�� �� ���Ե��� ���� ���) �� ���� ���Ե��� ���� ���(closedVertices�� 0�� ���)���� continue ���� ����˴ϴ�.

                ��, MST�� �������� ���õ� �� �ִ� ������ �ݵ�� �� �� �� �������� MST�� ���ؾ� �մϴ�. 
                ���� �� �� ������ ��� MST�� ���ϰų� �ƹ� ������ ������ ������ �ش� ������ continue ������ ���õǰ�, ���� ������ �˻��ϴ� �ݺ������� �Ѿ�� �˴ϴ�.
                */

                if(edge.Distance < minWeight)
                {
                    chosenEdge = edge;
                    chosen = true;
                    minWeight = edge.Distance;
                }
            }

            if (!chosen) break;
            results.Add(chosenEdge);
            openSet.Remove(chosenEdge.U);
            openSet.Remove(chosenEdge.V);
            closedSet.Add(chosenEdge.U);
            closedSet.Add(chosenEdge.V);

        }

        return results;
    }
}
