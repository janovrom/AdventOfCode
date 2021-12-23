using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day12
{
    internal class SolutionPart2 : IntSolution
    {

        private int _pathFound = 0;
        private Dictionary<Node, List<Node>> _graph = new();
        private Stack<Node> _activeNodes = new();


        public int Solve()
        {
            IEnumerable<string> lines = File.ReadAllLines("./Content/Day12/1.txt");
            Dictionary<string, Node> string2Node = new();
            foreach (string line in lines)
            {
                string[] endpoints = line.Split("-");
                BuildNode(endpoints[0], string2Node);
                BuildNode(endpoints[1], string2Node);
            }

            foreach (string line in lines)
            {
                string[] endpoints = line.Split("-");
                AddConnection(endpoints[0], endpoints[1], string2Node);
            }

            FindAllRoutes();

            return _pathFound;
        }

        private void BuildNode(string name, Dictionary<string, Node> string2Node)
        {
            var node = new Node(name);
            if (!_graph.ContainsKey(node))
            {
                _graph.Add(node, new List<Node>());
                string2Node.Add(name, node);
            }
        }

        private void AddConnection(string start, string end, Dictionary<string, Node> string2Node)
        {
            var startNode = string2Node[start];
            var endNode = string2Node[end];

            // No connection from end
            // No connection to start
            if (start != "end" && end != "start")
                _graph[startNode].Add(endNode);

            if (start != "start" && end != "end")
                _graph[endNode].Add(startNode);
        }

        private void FindAllRoutes()
        {
            Node startNode = new Node("start");
            _activeNodes.Push(startNode);

            foreach (Node node in _graph[startNode])
            {
                Visit(node);
            }
        }

        private void Visit(Node node)
        {
            if (node.IsEndNode)
            {
                _pathFound += 1;
                //Console.WriteLine("Found path");
                //foreach (Node n in _activeNodes.Reverse())
                //    Console.Write(n.Name + ",");
                //Console.WriteLine();
                return;
            }

            node.Visit();
            _activeNodes.Push(node);

            foreach (Node child in _graph[node])
            {
                if (child.CanVisit)
                {
                    Visit(child);
                }
            }

            _activeNodes.Pop();
            node.Leave();
        }

        private class Node
        {
            private static Node _visitedTwice = null;

            public string Name { get; }
            public bool IsSmall { get; }
            public bool IsEndNode { get; }
            public int VisitCount { get; private set; }
            public bool CanVisit
            {
                get
                {
                    if (IsSmall)
                    {
                        if(_visitedTwice is null)
                        {
                            return VisitCount < 2;
                        }
                        else
                        {
                            return VisitCount == 0;
                        }
                    }

                    return true;
                }
            }

            internal Node(string name)
            {
                Name = name;
                IsSmall = !IsUpper(name);
                IsEndNode = name == "end";
            }

            internal void Visit()
            {
                VisitCount += 1;

                if (IsSmall && VisitCount == 2)
                    _visitedTwice = this;
            }

            internal void Leave()
            {
                VisitCount -= 1;

                if (_visitedTwice == this)
                    _visitedTwice = null;

                if (VisitCount < 0)
                    throw new InvalidOperationException("Leaving object more than actually visiting.");
            }

            public override bool Equals(object obj)
            {
                if (obj is Node other) return Name.Equals(other.Name);
                if (obj is string otherName) return Name.Equals(otherName);

                return false;
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }

            public override string ToString()
            {
                return $"Node(Name={Name}, Visited={VisitCount})";
            }

            private static bool IsUpper(string s)
            {
                foreach (char c in s)
                    if (char.IsLower(c))
                        return false;

                return true;
            }

        }

    }
}
