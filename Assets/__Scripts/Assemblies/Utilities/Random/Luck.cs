namespace Assemblies.Utilities.Random
{
    public static class Luck
    {
        public static bool CoinFlip =>
            UnityEngine.Random.Range(0, 2) == 0;

        public static int Roll(int sides)
        {
            if (sides < 1)
                throw new System.ArgumentException("Sides count must be greater than 0");
            
            return UnityEngine.Random.Range(1, sides + 1);
        }

        public static bool Hit(int goal) =>
            UnityEngine.Random.Range(0, goal) == goal - 1;
    }
}
