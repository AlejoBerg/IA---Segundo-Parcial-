using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBFS<T>
{
    private HashSet<T> visited = new HashSet<T>();
    private Queue<T> pendingVisit = new Queue<T>();
    private Dictionary<T, T> childParent = new Dictionary<T, T>();

    public delegate bool SatisfiesCondition(T current); //Forma generica de pedir un nodo destino. En la clase que lo implementa voy a tener que tener una funcion con la misma firma y pasarle esa 
    public delegate List<T> GetNeighbour(T current);

    public List<T> RunPathfinding(T initial, SatisfiesCondition satisfiesCondition, GetNeighbour getNeigbours, float watchdog = 150)
    {
        watchdog--;
        if (watchdog <= 0) return new List<T>();
        pendingVisit.Enqueue(initial);

        while (pendingVisit.Count != 0)
        {
            T current = pendingVisit.Dequeue();
            if(satisfiesCondition(current))
            {
                List<T> myPath = new List<T>();
                myPath.Add(current);
                while (childParent.ContainsKey(myPath[myPath.Count - 1])) //Me aseguro que el ultimo item de la lista de hijo padre se encuentre en el diccionario, para chequear que tenga padre 
                {
                    var lastNodeMyPath = myPath[myPath.Count - 1];
                    myPath.Add(lastNodeMyPath);
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
                    T currentItem = neighbours[i];
                    if (visited.Contains(currentItem) || pendingVisit.Contains(currentItem)) continue;
                    pendingVisit.Enqueue(currentItem);
                    childParent[currentItem] = current; //El padre del item es current
                }
            }
        }
        return new List<T>();
    } 
}
