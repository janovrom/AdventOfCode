using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode.Solutions.Day23
{
    internal class SolutionPart2 : IntSolution
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

            var roomA = new Room('A', 2);
            var roomB = new Room('B', 4);
            var roomC = new Room('C', 6);
            var roomD = new Room('D', 8);
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


            var initialState = new State();
            initialState.RoomOccupants[0, 0] = 1;
            initialState.RoomOccupants[0, 1] = 3;
            initialState.RoomOccupants[0, 2] = 3;
            initialState.RoomOccupants[0, 3] = 3;

            initialState.RoomOccupants[1, 0] = 1;
            initialState.RoomOccupants[1, 1] = 2;
            initialState.RoomOccupants[1, 2] = 1;
            initialState.RoomOccupants[1, 3] = 2;

            initialState.RoomOccupants[2, 0] = 3;
            initialState.RoomOccupants[2, 1] = 1;
            initialState.RoomOccupants[2, 2] = 0;
            initialState.RoomOccupants[2, 3] = 0;

            initialState.RoomOccupants[3, 0] = 0;
            initialState.RoomOccupants[3, 1] = 0;
            initialState.RoomOccupants[3, 2] = 2;
            initialState.RoomOccupants[3, 3] = 2;

            return FindPath(initialState);
        }

        private int FindPath(State initialState)
        {
            Dictionary<State, int> stateCosts = new();
            Stack<(State, int)> openedStates = new();
            openedStates.Push((initialState, 0));

            while (openedStates.Count > 0)
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

                    if (_rooms[i].OccupantsAreCorrect(s))
                        continue; // I am in correct place, so don't move me

                    for (int j = 0; j < 4; ++j)
                    {
                        int occupant = s.RoomOccupants[i, j];
                        if (occupant < 0)
                            continue; // This is empty


                        Amphipod a = _amphipods[occupant];
                        foreach ((int node, int length) in _rooms[i].MoveStates[j].GetViableNodes(s))
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
            endState.RoomOccupants[0, 2] = 0;
            endState.RoomOccupants[0, 3] = 0;

            endState.RoomOccupants[1, 0] = 1;
            endState.RoomOccupants[1, 1] = 1;
            endState.RoomOccupants[1, 2] = 1;
            endState.RoomOccupants[1, 3] = 1;

            endState.RoomOccupants[2, 0] = 2;
            endState.RoomOccupants[2, 1] = 2;
            endState.RoomOccupants[2, 2] = 2;
            endState.RoomOccupants[2, 3] = 2;

            endState.RoomOccupants[3, 0] = 3;
            endState.RoomOccupants[3, 1] = 3;
            endState.RoomOccupants[3, 2] = 3;
            endState.RoomOccupants[3, 3] = 3;

            return stateCosts[endState];
        }

        private class State
        {

            public int[] HallwayOccupants = new int[HallwaySize];
            public int[,] RoomOccupants = new int[4, 4];

            public static State GetState()
            {
                return new State();
            }

            public State()
            {
                for (int i = 0; i < HallwaySize; ++i)
                    HallwayOccupants[i] = -1;
                for (int i = 0; i < 4; ++i)
                {
                    for (int j = 0; j < 4; ++j)
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
                    for (int j = 0; j < 4; ++j)
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
                    for (int j = 0; j < 4; ++j)
                    {
                        hash += RoomOccupants[i, j] * (i + 1) * (j + 1);
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

            public override bool Equals(object obj)
            {
                if (obj is not State other)
                    return false;

                for (int i = 0; i < HallwaySize; ++i)
                    if (HallwayOccupants[i] != other.HallwayOccupants[i])
                        return false;

                for (int i = 0; i < 4; ++i)
                {
                    for (int j = 0; j < 4; ++j)
                    {
                        if (RoomOccupants[i, j] != other.RoomOccupants[i, j])
                            return false;
                    }
                }

                return true;
            }

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
                    return Destination.GetLastEmpty(s);
                }

                return -1;
            }
        }

        private class MoveOutside
        {

            public int Block;
            public int Room;
            public int HallwayNodeIndex;

            public MoveOutside(int room, int block, int hallwayNodeIndex)
            {
                Block = block;
                HallwayNodeIndex = hallwayNodeIndex;
                Room = room;
            }


            internal IEnumerable<(int, int)> GetViableNodes(State s)
            {
                bool isBlocked = false;
                for (int i = 0; i < Block; ++i)
                    if (s.RoomOccupants[Room, i] > 0)
                        isBlocked = true;

                if (isBlocked) yield break;

                for (int i = HallwayNodeIndex; i >= 0; --i)
                {
                    if (!State.IsHallwayEmpty(s, i))
                        break;

                    if (_hallway[i].CanStop)
                        yield return (i, HallwayNodeIndex - i + Block + 1);
                }

                for (int i = HallwayNodeIndex; i < _hallway.Length; ++i)
                {
                    if (!State.IsHallwayEmpty(s, i))
                        break;

                    if (_hallway[i].CanStop)
                        yield return (i, i - HallwayNodeIndex + Block + 1);
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
            public MoveOutside[] MoveStates { get; } = new MoveOutside[4];
            public int HallwayEntry { get; }


            public Room(char acceptsType, int hallwayEntry)
            {
                AcceptedTyped = acceptsType;
                HallwayEntry = hallwayEntry;

                for (int i = 0; i < 4; ++i)
                {
                    MoveStates[i] = new MoveOutside(Amphipod.GetRoom(AcceptedTyped), i, HallwayEntry);
                }
            }

            public bool HasIntruder(State s)
            {
                int room = Amphipod.GetRoom(AcceptedTyped);
                bool hasIntruder = false;
                for (int i = 0; i < 4; ++i)
                {
                    if (!IsEmpty(s, i))
                        hasIntruder |= s.RoomOccupants[room, i] != room;

                }

                return hasIntruder;
            }

            public bool IsCorrectlyAssigned(State s)
            {
                int room = Amphipod.GetRoom(AcceptedTyped);
                for (int i = 0; i < 4; ++i)
                {
                    if (s.RoomOccupants[room, i] != room)
                        return false;
                }
                return true;
            }

            public bool IsEmpty(State s, int position)
            {
                int room = Amphipod.GetRoom(AcceptedTyped);
                return s.RoomOccupants[room, position] == -1;
            }

            internal bool OccupantsAreCorrect(State s)
            {
                int room = Amphipod.GetRoom(AcceptedTyped);
                for (int i = 3; i >= 0; --i)
                {
                    if (s.RoomOccupants[room, i] == -1)
                        return true; // Is empty, try if all are empty

                    if (s.RoomOccupants[room, i] != room)
                        return false;
                }

                return true;
            }

            internal int GetLastEmpty(State s)
            {
                int room = Amphipod.GetRoom(AcceptedTyped);
                for (int i = 3; i >= 0; --i)
                {
                    if (s.RoomOccupants[room, i] == -1)
                        return i; // Is empty, try if all are empty
                }

                return -1;
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
