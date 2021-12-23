using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day23
{
    internal class SolutionPart1 : IntSolution
    {

        public const int HallwaySize = 11;

        private static Node[] _hallway = new Node[HallwaySize];
        private static Amphipod[] _amphipods = new Amphipod[4];
        private static Room[] _rooms;
        private static Dictionary<char, MoveToRoom> _amphipodToRoomStates = new();

        public int Solve()
        {
            // skip the loading, too lazy for that
            for (int i = 0; i < HallwaySize; ++i)
            {
                _hallway[i] = new Node(i == 2 || i == 4 || i == 6 | i == 8, i);
            }

            _amphipods[0] = new Amphipod('A');
            _amphipods[1] = new Amphipod('B');
            _amphipods[2] = new Amphipod('C');
            _amphipods[3] = new Amphipod('D');

            var roomA = new Room('A', new Node(true, 0), new Node(true, 1), 2);
            var roomB = new Room('B', new Node(true, 0), new Node(true, 1), 4);
            var roomC = new Room('C', new Node(true, 0), new Node(true, 1), 6);
            var roomD = new Room('D', new Node(true, 0), new Node(true, 1), 8);
            _rooms = new Room[4];
            _rooms[0] = roomA;
            _rooms[1] = roomB;
            _rooms[2] = roomC;
            _rooms[3] = roomD;

            var moveToRoomA = new MoveToRoom(roomA);
            var moveToRoomB = new MoveToRoom(roomB);
            var moveToRoomC = new MoveToRoom(roomC);
            var moveToRoomD = new MoveToRoom(roomD);

            _amphipodToRoomStates.Add('A', moveToRoomA);
            _amphipodToRoomStates.Add('B', moveToRoomB);
            _amphipodToRoomStates.Add('C', moveToRoomC);
            _amphipodToRoomStates.Add('D', moveToRoomD);

            var moveOutsideA0 = new MoveOutside(null, 2);
            var moveOutsideA1 = new MoveOutside(roomA.Node0, 2);
            var moveOutsideB0 = new MoveOutside(null, 4);
            var moveOutsideB1 = new MoveOutside(roomB.Node0, 4);
            var moveOutsideC0 = new MoveOutside(null, 6);
            var moveOutsideC1 = new MoveOutside(roomC.Node0, 6);
            var moveOutsideD0 = new MoveOutside(null, 8);
            var moveOutsideD1 = new MoveOutside(roomD.Node0, 8);

            roomA.MoveStates[0] = moveOutsideA0;
            roomA.MoveStates[1] = moveOutsideA1;

            roomB.MoveStates[0] = moveOutsideB0;
            roomB.MoveStates[1] = moveOutsideB1;

            roomC.MoveStates[0] = moveOutsideC0;
            roomC.MoveStates[1] = moveOutsideC1;

            roomD.MoveStates[0] = moveOutsideD0;
            roomD.MoveStates[1] = moveOutsideD1;


            var initialState = new State();
            initialState.RoomOccupants[0, 0] = 1;
            initialState.RoomOccupants[0, 1] = 0;
            initialState.RoomOccupants[1, 0] = 2;
            initialState.RoomOccupants[1, 1] = 3;
            initialState.RoomOccupants[2, 0] = 1;
            initialState.RoomOccupants[2, 1] = 2;
            initialState.RoomOccupants[3, 0] = 3;
            initialState.RoomOccupants[3, 1] = 0;

            return FindPath(initialState);
        }

        private int FindPath(State initialState) 
        {
            Dictionary<State, int> stateCosts = new();
            Queue<(State,int)> openedStates = new();
            openedStates.Enqueue((initialState, 0));

            while(openedStates.Count > 0) 
            {
                (State s, int cost) = openedStates.Dequeue();
                if (stateCosts.TryGetValue(s, out int previousCost)) 
                {
                    // If we have been in this state already once,
                    // and the cost was lower or equal, don't open this
                    // state again.
                    if (previousCost <= cost)
                        continue;

                    // If this cost is newer, replace it
                    stateCosts[s] = cost;
                }
                else 
                {
                    stateCosts.Add(s, cost);
                }
                
                // Open all viable paths
                for (int i = 0; i < 4; ++i)
                {
                    for (int j = 0; j < 2; ++j) 
                    {
                        int occupant = s.RoomOccupants[i, j];
                        if (occupant < 0)
                            continue; // This is empty

                        Amphipod a = _amphipods[occupant];
                        List<(Node,int)> pathsAndLengths = _rooms[i].MoveStates[j].GetViableNodes();
                        int lengthToEntry = Math.Abs(_rooms[i].HallwayEntry - i);
                        foreach ((Node n, int length) in pathsAndLengths) 
                        {
                            State newState = new State(s);
                            newState.RoomOccupants[i, j] = -1;
                            newState.HallwayOccupants[n.Index] = occupant;
                            openedStates.Enqueue((newState, cost + (length + lengthToEntry) * a.MoveCost));
                        }
                    }
                }

                for (int i = 0; i < HallwaySize; ++i) 
                {
                    int occupant = s.HallwayOccupants[i];
                    if (occupant < 0)
                        continue; // This is empty

                    Amphipod a = _amphipods[occupant];
                    List<(Node,int)> pathsAndLengths = _amphipodToRoomStates[a.Type].GetViableNodes();
                    foreach ((Node n, int length) in pathsAndLengths) 
                    {
                        State newState = new State(s);
                        newState.RoomOccupants[Amphipod.GetRoom(a.Type), n.Index] = occupant;
                        newState.HallwayOccupants[i] = -1;
                        openedStates.Enqueue((newState, cost + (length ) * a.MoveCost));
                    }
                }
            }

            var endState = new State();
            endState.RoomOccupants[0, 0] = 0;
            endState.RoomOccupants[0, 1] = 0;
            endState.RoomOccupants[1, 0] = 1;
            endState.RoomOccupants[1, 1] = 1;
            endState.RoomOccupants[2, 0] = 2;
            endState.RoomOccupants[2, 1] = 2;
            endState.RoomOccupants[3, 0] = 3;
            endState.RoomOccupants[3, 1] = 3;

            return stateCosts[endState];
        }

        private class State
        {
            
            public int[] HallwayOccupants = new int[HallwaySize];
            public int[,] RoomOccupants = new int[4,2];

            public State() 
            {
                for (int i = 0; i < HallwaySize; ++i)
                    HallwayOccupants[i] = -1;
                for (int i = 0; i < 4; ++i)
                {
                    for (int j = 0; j < 2; ++j) 
                    {
                        RoomOccupants[i, j] = -1;
                    }
                }
            }

            public State(State s) 
            {
                for (int i = 0; i < HallwaySize; ++i)
                    HallwayOccupants[i] = s.HallwayOccupants[i];

                for (int i = 0; i < 4; ++i)
                {
                    for (int j = 0; j < 2; ++j) 
                    {
                        RoomOccupants[i, j] = s.RoomOccupants[i, j];
                    }
                }
            }

            public override int GetHashCode()
            {
                int h = HashCode.Combine(HallwayOccupants[0]);
                for (int i = 1; i < HallwaySize; ++i)
                    h = HashCode.Combine(h, HallwayOccupants[i]);

                for (int i = 0; i < 4; ++i)
                {
                    for (int j = 0; j < 2; ++j) 
                    {
                        h = HashCode.Combine(h, RoomOccupants[i, j]);
                    }
                }

                return h;
            }

            public override bool Equals(object obj)
            {
                if (obj is not State other)
                    return false;

                for (int i = 0; i < HallwaySize; ++i)
                    if (HallwayOccupants[i] != other.HallwayOccupants[i])
                        return false;

                for (int i = 0; i < 4; ++i)
                {
                    for (int j = 0; j < 2; ++j) 
                    {
                        if (RoomOccupants[i, j] != other.RoomOccupants[i, j])
                            return false;
                    }
                }
                
                return true;
            }

        }

        private abstract class MoveState 
        {

            internal abstract bool CanMove();
            internal abstract List<(Node, int)> GetViableNodes();

        }

        private class MoveToRoom : MoveState
        {

            public Room Destination { get; }

            public MoveToRoom(Room destination)
            {
                Destination = destination;
            }

            internal override bool CanMove()
            {
                // Contains someother type
                if (Destination.HasIntruder)
                    return false;

                return Destination.Node0.IsEmpty || Destination.Node1.IsEmpty;
            }

            internal override List<(Node,int)> GetViableNodes()
            {
                if (!CanMove())
                    return new();

                if (Destination.Node0.IsEmpty && Destination.Node1.IsEmpty)
                    return new List<(Node,int)>() { (Destination.Node1, 2) };

                if (Destination.Node0.IsEmpty)
                    return new List<(Node,int)>() { (Destination.Node0, 1) };

                return new();
            }
        }

        private class MoveOutside : MoveState
        {

            public Node Block;
            public int HallwayNodeIndex;

            public MoveOutside(Node block, int hallwayNodeIndex)
            {
                Block = block;
                HallwayNodeIndex = hallwayNodeIndex;
            }

            internal override bool CanMove()
            {
                return (Block is null || Block.IsEmpty) && _hallway[HallwayNodeIndex].IsEmpty;
            }

            internal override List<(Node,int)> GetViableNodes() 
            {
                if (!CanMove())
                    return new();

                List<(Node,int)> viableNodes = new List<(Node,int)>();
                int offset = Block is null ? 0 : 1;
                for (int i = HallwayNodeIndex; i >= 0; --i) 
                {
                    if (!_hallway[i].IsEmpty)
                        break;
                    
                    if (_hallway[i].CanStop)
                        viableNodes.Add((_hallway[i], HallwayNodeIndex - i + offset));
                }

                for (int i = HallwayNodeIndex; i < _hallway.Length; ++i) 
                {
                    if (!_hallway[i].IsEmpty)
                        break;
                    
                    if (_hallway[i].CanStop)
                        viableNodes.Add((_hallway[i], i - HallwayNodeIndex + offset));
                }

                return viableNodes;
            }

        }

        private class Node
        {
            private Amphipod _occupant;
            public Amphipod Occupant => _occupant;
            public bool IsEmpty => _occupant is null;
            public bool CanStop { get; private set; }
            public int Index { get; }

            public Node(bool canStop, int index) 
            {
                CanStop = canStop;
                Index = index;
            }

            public bool Enter(Amphipod a) 
            {
                if (IsEmpty)
                {
                   _occupant = a;
                    return true;
                }

                return false;
            }

            public Amphipod Leave() 
            {
                Amphipod a = _occupant;
                _occupant = null;
                return a;
            }

        }

        private class Room 
        {
            public char AcceptsType { get; }
            public Node Node0 { get; }
            public Node Node1 { get; }
            public Node[] Nodes { get; }
            public MoveState[] MoveStates { get; } = new MoveState[2];
            public bool HasIntruder => Node0.Occupant?.Type == AcceptsType && Node1.Occupant?.Type == AcceptsType;
            public int HallwayEntry { get; }



            public Room(char acceptsType, Node enterNode, Node lastNode, int hallwayEntry)
            {
                AcceptsType = acceptsType;
                Node0 = enterNode;
                Node1 = lastNode;
                Nodes = new Node[] { Node0, Node1 };
                HallwayEntry = hallwayEntry;
            }
        }

        private class Amphipod
        {
            public int MoveCost { get; private set; }
            public char Type { get; private set; }

            public Amphipod(char type)
            {
                MoveCost = GetCost(type);
                Type = type;
            }

            public static int GetCost(char c) => c switch
            {
                'A' => 1,
                'B' => 10,
                'C' => 100,
                'D' => 1000,
                _ => throw new NotSupportedException($"Bug type not supported {c}")
            };

            public static int GetRoom(char c) => c switch
            {
                'A' => 0,
                'B' => 1,
                'C' => 2,
                'D' => 3,
                _ => throw new NotSupportedException($"Bug type not supported {c}")
            };
        }

    }

}
