namespace AdventOfCode.Solutions.Day21
{
    internal abstract class Dice
    {

        public int RollCount { get; private set; } = 0;

        internal int Roll()
        {
            RollCount += 1;
            return RollInternal();
        }

        protected abstract int RollInternal();

    }

    internal class DeterministicDice : Dice
    {

        private int _nextRoll = 1;

        protected override int RollInternal()
        {
            int roll = _nextRoll;
            _nextRoll += 1;
            if (_nextRoll > 100)
                _nextRoll -= 100;

            return roll;
        }
    }

    internal static class DiceExtensions
    {

        internal static int Roll3(this Dice dice)
        {
            return dice.Roll() + dice.Roll() + dice.Roll();
        }

    }


}
