using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Graph
{
    public LinkedList<int>[] adj;

    public Graph()
    {
        LinkedList<int>[] adj = new LinkedList<int>[3073];//64*48+1
        for (int i = 0; i < 3073; i++)
        {
            adj[i] = new LinkedList<int>();
        }
    }

    public Graph(LinkedList<int>[] adj)
    {
        this.adj = adj;
    }
}
