using UnityEngine;

public static class PlayerProgress
{
    public static bool[] biomeVisited = new bool[3]; // 3 biomes
    public static int relics = 0;

    public static void MarkBiomeVisited(int biomeIndex)
    {
        biomeVisited[biomeIndex] = true;
    }

    public static void AddRelics(int amount)
    {
        relics += amount;
    }

    public static bool SpendRelics(int amount)
    {
        if (relics >= amount)
        {
            relics -= amount;
            return true;
        }
        return false;
    }
}
