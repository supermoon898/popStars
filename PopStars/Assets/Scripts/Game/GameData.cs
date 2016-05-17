using UnityEngine;
using System.Collections;

public class GameData {

    public static int score;
    public static int high_score;

    public static void Reset()
    {
        score = 0;
        high_score = 0;
    }
}
