using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindAStar<T>
{
    HashSet<T> visited = new HashSet<T>();
    PriorityQueue<T> pendingVisit = new PriorityQueue<T>();
    Dictionary<T, T> childParent = new Dictionary<T, T>();
    Dictionary<T, float> nodeCost = new Dictionary<T, float>();

    public delegate bool SatisfiesCondition(T current); //Forma generica de pedir un nodo destino. En la clase que lo implementa voy a tener que tener una funcion con la misma firma y pasarle esa 
    public delegate List<T> GetNeighbour(T current);
    public delegate float GetConnectionCost(T parentNode, T childNode);
    public delegate float GetHeuristic(T current);

    public List<T> RunPathfinding(T initial, SatisfiesCondition satisfiesCondition, GetNeighbour getNeigbours, GetConnectionCost getConnectionCost, GetHeuristic getHeuristic ,float watchdog = 80)
    {
        HashSet<T> visited = new HashSet<T>();
        PriorityQueue<T> pendingVisit = new PriorityQueue<T>();
        Dictionary<T, T> childParent = new Dictionary<T, T>();
        Dictionary<T, float> nodeCost = new Dictionary<T, float>();

        pendingVisit.Enqueue(initial, 0);
        nodeCost.Add(initial, 0);

        while (!pendingVisit.IsEmpty)
        {
            watchdog--;
            if (watchdog <= 0) return new List<T>();

            T current = pendingVisit.Dequeue();
            if (satisfiesCondition(current))
            {
                List<T> myPath = new List<T>();
                myPath.Add(current);
                while (childParent.ContainsKey(myPath[myPath.Count - 1])) //Me aseguro que el ultimo item de la lista de hijo padre se encuentre en el diccionario, para chequear que tenga padre 
                {
                    var lastNodeMyPath = myPath[myPath.Count - 1];
                    myPath.Add(childParent[lastNodeMyPath]);
                }
                myPath.Reverse();
                return myPath;
            }
            else
            {
                visited.Add(current);
                List<T> neighbours = getNeigbours(current);
                for (int i = 0; i < neighbours.Count; i++)
                {
                    T newItem = neighbours[i];
                    if (visited.Contains(newItem)) continue; //Si lo visite lo omito, sino calculo el costo del nodo
                    float totalNodeCost = nodeCost[current] + getConnectionCost(current, newItem);
                    if (nodeCost.ContainsKey(newItem) && nodeCost[newItem] < totalNodeCost) continue; //Si el costo que tenia guardado antes es mayor, omito este nuevo
                    nodeCost[newItem] = totalNodeCost;
                    childParent[newItem] = current; //Piso el valor del padre por el nuevo en el caso de que hubiese uno antes 

                    pendingVisit.Enqueue(newItem, totalNodeCost + getHeuristic(newItem));
                }
            }
        }
        return new List<T>();
    }
}
