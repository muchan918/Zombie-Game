using UnityEngine;

public class CoinItem : MonoBehaviour, IItem
{
    public int amount = 1000;

    public void Use(GameObject target)
    {
        var findGo = GameObject.FindWithTag("GameController");
        var gm = findGo.GetComponent<GameManager>();
        if (gm != null)
        {
            gm.AddScore(amount);
        }
    }
}
