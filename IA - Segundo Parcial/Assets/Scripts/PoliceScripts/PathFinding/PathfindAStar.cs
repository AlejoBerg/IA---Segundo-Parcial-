using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindAStar<T>
{
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
            Debug.Log("satisfiesCondition:" + satisfiesCondition(current));

            if (satisfiesCondition(current))
            {
                return ConstructPath(current, childParent);
            }
            else
            {
                visited.Add(current);
                List<T> neighbours = getNeigbours(current);
                //Debug.Log("neighbours for " + current + " = " + neighbours.Count);
                //Debug.Log("pendingVisit count = " + pendingVisit.Count());

                for (int i = 0; i < neighbours.Count; i++)
                {
                    T newItem = neighbours[i];
                    //Debug.Log("visited: " + visited.Contains(newItem));
                    if (visited.Contains(newItem)) continue; //Si lo visite lo omito, sino calculo el costo del nodo
                    float totalNodeCost = nodeCost[current] + getConnectionCost(current, newItem);
                    if (nodeCost.ContainsKey(newItem) && nodeCost[newItem] < totalNodeCost) continue; //Si el costo que tenia guardado antes es mayor, omito este nuevo
                    nodeCost[newItem] = totalNodeCost;
                    childParent[newItem] = current; //Piso el valor del padre por el nuevo en el caso de que hubiese uno antes 

                    pendingVisit.Enqueue(newItem, totalNodeCost + getHeuristic(newItem));
                    Debug.Log($"No satisfacio, agrego a {newItem}");
                }
            }
        }
        return new List<T>();
    }

    List<T> ConstructPath(T end, Dictionary<T,T> _childParents)
    {
        var path = new List<T>();
        path.Add(end);
        while (_childParents.ContainsKey(path[path.Count -1])) //Me aseguro que el ultimo item de la lista de hijo padre se encuentre en el diccionario, para chequear que tenga padre 
        {
            var lastNode = path[path.Count - 1];
            path.Add(_childParents[lastNode]);
        }
        path.Reverse();
        return path;
    }
}
