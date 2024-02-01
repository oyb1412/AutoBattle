using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public BuySelectUnit buySelectUnit;
    public  PlayerUnitController playerUnitController;
    public MixUnitManager mixUnitManager;
    public ShowRandomUnit showRandomUnit;
    public LevelManager levelManager;
    private void Awake()
    {
        instance = this;
    }
}
