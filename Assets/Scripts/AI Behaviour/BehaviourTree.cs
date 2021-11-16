using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree : Node
{
    public BehaviourTree()
    {
        name = "tree";
    }

    public BehaviourTree(string n)
    {
        name = n;
    }

    public override Status Process()
    {
        return children[currentChild].Process();
    }

    public struct NodeLevel
    {
        public int level;
        public Node node;
    }

    public void PrintTree()
    {
        Stack<NodeLevel> nodeStack= new Stack<NodeLevel>();
        string printOut = "";
        nodeStack.Push(new NodeLevel { level = 0, node = this});

        while (nodeStack.Count != 0)
        {
            NodeLevel nextNode = nodeStack.Pop();
            printOut += new string(' ', nextNode.level * 4) + nextNode.node.name + "\n";
            for (int c = nextNode.node.children.Count-1; c >= 0; c--)
            {
                nodeStack.Push(new NodeLevel { level = nextNode.level+1, node = nextNode.node.children[c]});
            }
        }
    }
    
}
