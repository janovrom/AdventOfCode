namespace AdventOfCode.Solutions.Day21
{
    internal class Player
    {

        public int StartingPosition { get; }
        public int Position { get; private set; }
        public int Score { get; private set; }

        internal Player(int startingPosition)
        {
            StartingPosition = startingPosition;
            Position = startingPosition;
        }

        internal void PlayRound(Dice dice)
        {
            int movement = dice.Roll3();
            Position += movement;
            Position = ((Position - 1) % 10) + 1;
            Score += Position;
        }

        internal void Reset()
        {
            Position = StartingPosition;
            Score = 0;
        }

    }
}
