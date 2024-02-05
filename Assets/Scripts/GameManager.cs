using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public BuySelectUnit buySelectUnit;
    public  PlayerUnitController playerUnitController;
    public MixUnitManager mixUnitManager;
    public ShowRandomUnit showRandomUnit;
    public LevelManager levelManager;
    public ItemController itemController;
    public Vector2 mousePos;
    public GameObject[] itemPrefabs;
    public GameObject[] playerUnitPrefabs;
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    public Collider2D MouseRayCast(string tag, string layer)
    {
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, LayerMask.GetMask(layer));

        if (hit.collider != null && hit.collider.CompareTag(tag))
        {
            return hit.collider;
        }
        else
            return null;
    }

    public Collider2D MouseRayCast(string layer)
    {
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, 0f, LayerMask.GetMask(layer));

        if (hit.collider != null)
        {
            return hit.collider;
        }
        else
            return null;
    }
}
