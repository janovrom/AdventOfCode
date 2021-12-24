using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                _hallway[i] = new Node(!(i == 2 || i == 4 || i == 6 | i == 8), i);
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

            var moveOutsideA0 = new MoveOutside(0, false, 2);
            var moveOutsideA1 = new MoveOutside(0, true, 2);
            var moveOutsideB0 = new MoveOutside(1, false, 4);
            var moveOutsideB1 = new MoveOutside(1, true, 4);
            var moveOutsideC0 = new MoveOutside(2, false, 6);
            var moveOutsideC1 = new MoveOutside(2, true, 6);
            var moveOutsideD0 = new MoveOutside(3, false, 8);
            var moveOutsideD1 = new MoveOutside(3, true, 8);

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
            initialState.RoomOccupants[0, 1] = 3;
            initialState.RoomOccupants[1, 0] = 1;
            initialState.RoomOccupants[1, 1] = 2;
            initialState.RoomOccupants[2, 0] = 3;
            initialState.RoomOccupants[2, 1] = 0;
            initialState.RoomOccupants[3, 0] = 0;
            initialState.RoomOccupants[3, 1] = 2;

            return FindPath(initialState);
        }

        private int FindPath(State initialState) 
        {
            Dictionary<State, int> stateCosts = new();
            Stack<(State,int)> openedStates = new();
            List<(State,int)> solutions = new();
            openedStates.Push((initialState, 0));

            while(openedStates.Count > 0) 
            {
                (State s, int cost) = openedStates.Pop();

                bool foundSolution = true;
                for (int i = 0; i < 4; ++i)
                {
                    foundSolution = foundSolution && _rooms[i].IsCorrectlyAssigned(s);
                }

                if (stateCosts.TryGetValue(s, out int previousCost)) 
                {
                    // If we have been in this state already once,
                    // and the cost was lower or equal, don't open this
                    // state again.
                    if (previousCost <= cost)
                    {
                        continue;
                    }

                    // If this cost is newer, replace it
                    stateCosts[s] = cost;
                }
                else 
                {
                    stateCosts.Add(s, cost);
                }

                if (foundSolution) continue;

                // Open all viable paths
                for (int i = 0; i < 4; ++i)
                {
                    if (_rooms[i].IsCorrectlyAssigned(s))
                        continue;

                    for (int j = 0; j < 2; ++j) 
                    {
                        int occupant = s.RoomOccupants[i, j];
                        if (occupant < 0)
                            continue; // This is empty

                        if (_rooms[i].HasCorrectOccupant(s))
                            continue; // I am in correct place, so don't move me

                        Amphipod a = _amphipods[occupant];
                        foreach((int node, int length) in _rooms[i].MoveStates[j].GetViableNodes(s))
                        {
                            if (length <= 0)
                                continue;

                            State newState = State.GetState().CopyState(s);
                            newState.RoomOccupants[i, j] = -1;
                            newState.HallwayOccupants[node] = occupant;
                            openedStates.Push((newState, cost + length * a.MoveCost));
                        }
                    }
                }

                for (int i = 0; i < HallwaySize; ++i) 
                {
                    int occupant = s.HallwayOccupants[i];
                    if (occupant < 0)
                        continue; // This is empty

                    Amphipod a = _amphipods[occupant];
                    int entry = _rooms[Amphipod.GetRoom(a.Type)].HallwayEntry;
                    bool pathBlocked = false;
                    
                    if (entry > i)
                    {
                        for (int p = i + 1; p <= entry; ++p)
                        {
                            pathBlocked |= !State.IsHallwayEmpty(s, p);

                        }
                    }
                    else
                    {
                        for (int p = i - 1; p >= entry; --p)
                        {
                            pathBlocked |= !State.IsHallwayEmpty(s, p);
                        }
                    }

                    if (pathBlocked)
                        continue;

                    int lengthToEntry = Math.Abs(entry - i);
                    int node = _amphipodToRoomStates[a.Type].GetNode(s);
                    if (node >= 0)
                    {
                        State newState = State.GetState().CopyState(s);
                        newState.RoomOccupants[Amphipod.GetRoom(a.Type), node] = occupant;
                        newState.HallwayOccupants[i] = -1;
                        openedStates.Push((newState, cost + (node + 1 + lengthToEntry) * a.MoveCost));
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

            private static List<State> _statePool = new List<State>();

            public static State GetState()
            {
                return new State();
            }

            public static void DeleteState(State s)
            {
                _statePool.Add(s);
            }

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

            public State CopyState(State s) 
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

                return this;
            }

            public static bool IsHallwayEmpty(State s, int position)
            {
                return s.HallwayOccupants[position] == -1;
            }

            public static bool IsRoomEmpty(State s, int room, int pos)
            {
                return s.RoomOccupants[room, pos] == -1;
            }

            public override int GetHashCode()
            {
                int hash = 0;
                for (int i = 0; i < HallwaySize; ++i)
                {
                    hash += HallwayOccupants[i] * (i + 1);
                }

                for (int i = 0; i < 4; ++i)
                {
                    for (int j = 0; j < 2; ++j)
                    {
                        hash += RoomOccupants[i, j] * (i +1) * (j + 1);
                    }
                }

                return hash;

                //int h = HashCode.Combine(HallwayOccupants[0]);
                //for (int i = 1; i < HallwaySize; ++i)
                //    h = HashCode.Combine(h, HallwayOccupants[i]);

                //for (int i = 0; i < 4; ++i)
                //{
                //    for (int j = 0; j < 2; ++j) 
                //    {
                //        h = HashCode.Combine(h, RoomOccupants[i, j]);
                //    }
                //}

                //return h;
            }
            /*
                int hash = 0;
                for (int i = 0; i < HallwaySize; ++i)
                {
                    hash += HallwayOccupants[i] > 0 ? 1 : 0;
                    hash <<= 1;
                }

                for (int i = 0; i < 4; ++i)
                {
                    for (int j = 0; j < 2; ++j) 
                    {
                        hash += RoomOccupants[i, j] > 0 ? 1 : 0;
                        hash <<= 1;
                    }
                }

                return hash;
             */

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

            internal abstract int[] GetViableNodes(State s);

        }

        private class MoveToRoom
        {

            public Room Destination { get; }

            public MoveToRoom(Room destination)
            {
                Destination = destination;
            }

            internal int GetNode(State s)
            {
                if (!Destination.HasIntruder(s))
                {
                    if (Destination.IsEmpty(s, 0) && Destination.IsEmpty(s, 1))
                        return 1;

                    if (Destination.IsEmpty(s, 0))
                        return 0;
                }

                return -1;
            }
        }

        private class MoveOutside
        {

            public bool Block;
            public int Room;
            public int HallwayNodeIndex;

            public MoveOutside(int room, bool block, int hallwayNodeIndex)
            {
                Block = block;
                HallwayNodeIndex = hallwayNodeIndex;
                Room = room;
            }


            internal IEnumerable<(int,int)> GetViableNodes(State s) 
            {
                if (Block && s.RoomOccupants[Room, 0] > 0)
                    yield break;

                int offset = Block ? 2 : 1;
                for (int i = HallwayNodeIndex; i >= 0; --i) 
                {
                    if (!State.IsHallwayEmpty(s, i))
                        break;

                    if (_hallway[i].CanStop)
                        yield return (i, HallwayNodeIndex - i + offset);
                }

                for (int i = HallwayNodeIndex; i < _hallway.Length; ++i) 
                {
                    if (!State.IsHallwayEmpty(s, i))
                        break;
                    
                    if (_hallway[i].CanStop)
                        yield return (i, i - HallwayNodeIndex + offset);
                }
            }

        }

        [DebuggerDisplay("Index = {Index}, CanStop = {CanStop}")]
        private class Node
        {
            public bool CanStop { get; private set; }
            public int Index { get; }

            public Node(bool canStop, int index) 
            {
                CanStop = canStop;
                Index = index;
            }

        }

        private class Room 
        {
            public char AcceptedTyped { get; }
            public Node Node0 { get; }
            public Node Node1 { get; }
            public Node[] Nodes { get; }
            public MoveOutside[] MoveStates { get; } = new MoveOutside[2];
            public int HallwayEntry { get; }


            public Room(char acceptsType, Node enterNode, Node lastNode, int hallwayEntry)
            {
                AcceptedTyped = acceptsType;
                Node0 = enterNode;
                Node1 = lastNode;
                Nodes = new Node[] { Node0, Node1 };
                HallwayEntry = hallwayEntry;
            }

            public bool HasIntruder(State s)
            {
                int room = Amphipod.GetRoom(AcceptedTyped);
                bool hasIntruder = false;
                if (!IsEmpty(s, 0))
                    hasIntruder = s.RoomOccupants[room, 0] != room;

                if (!IsEmpty(s, 1))
                    hasIntruder |= s.RoomOccupants[room, 1] != room;

                return hasIntruder;
            }

            public bool IsCorrectlyAssigned(State s)
            {
                int room = Amphipod.GetRoom(AcceptedTyped);
                return s.RoomOccupants[room, 0] == room && s.RoomOccupants[room, 1] == room;
            }

            public bool IsEmpty(State s, int position)
            {
                int room = Amphipod.GetRoom(AcceptedTyped);
                return s.RoomOccupants[room, position] == -1;
            }

            internal bool HasCorrectOccupant(State s)
            {
                int room = Amphipod.GetRoom(AcceptedTyped);
                return IsEmpty(s, 0) && s.RoomOccupants[room, 1] == room;
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

            public static int GetRoom(char c)
            {
                return c - 'A';
            }
            //{
            //    'A' => 0,
            //    'B' => 1,
            //    'C' => 2,
            //    'D' => 3,
            //    _ => throw new NotSupportedException($"Bug type not supported {c}")
            //};
        }

    }

}
