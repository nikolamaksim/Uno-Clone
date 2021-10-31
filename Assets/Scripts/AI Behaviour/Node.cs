using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum Status{ SUCCESS, RUNNING, FAILURE }
    public Status status;
    public List<Node> children = new List<Node>();
    public int currentChild = 0;
    public string name;

    public Node(){} // no idea why it's here

    public Node(string n) // constructor
    {
        name = n;
    }

    public virtual Status Process()
    {
        return children[currentChild].status;
    }

    public void addChild(Node node) // adds a node as a child
    {
        children.Add(node);
    }

}
