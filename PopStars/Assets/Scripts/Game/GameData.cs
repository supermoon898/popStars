using UnityEngine;
using System.Collections;

public class GameData {

    public int score;
    public int high_score;

    public void Reset()
    {
        score = 0;
        high_score = 0;
    }
}
