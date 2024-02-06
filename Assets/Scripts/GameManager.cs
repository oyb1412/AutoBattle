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

    //체력바 표시를 위한 정보 저장용 필드
    UnitManager player, enemy;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ShowHpSlider();
    }

    protected void ShowHpSlider()
    {
        if (instance.MouseRayCast("Player", "PlayerUnit"))
        {
            player = MouseRayCast("Player", "PlayerUnit").GetComponent<PlayerUnitManager>();
            if (player != null)
            {
                if (player.saveSlider != null)
                {
                    player.saveSlider.gameObject.SetActive(true);
                    player.saveSlider.value = player.currentHP / player.maxHP;
                    player.saveSlider.transform.position = Camera.main.WorldToScreenPoint(new Vector3(player.transform.position.x, player.transform.position.y + 1.4f, player.transform.position.z));
                }
            }
        }
        else if (player != null)
        {
            if (player.saveSlider != null)
            {
                player.saveSlider.gameObject.SetActive(false);
                player = null;
            }
        }
        if(player != null)
        {
            if (player.saveSlider != null)
                player.saveSlider.transform.position = Camera.main.WorldToScreenPoint(new Vector3(player.transform.position.x, player.transform.position.y + 1.4f, player.transform.position.z));

        }

        if (MouseRayCast("Enemy", "EnemyUnit") != null)
        {
            enemy = MouseRayCast("Enemy", "EnemyUnit").GetComponent<EnemyUnitManager>();
            enemy.saveSlider.gameObject.SetActive(true);
            enemy.saveSlider.value = enemy.currentHP / enemy.maxHP;
            enemy.saveSlider.transform.position = Camera.main.WorldToScreenPoint(new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1.4f, enemy.transform.position.z));
        }
        else if (enemy != null)
        {
            if (enemy.saveSlider != null)
            {
                enemy.saveSlider.gameObject.SetActive(false);
                enemy = null;
            }
        }
        if(enemy != null)
        {
            if (enemy.saveSlider != null)
                enemy.saveSlider.transform.position = Camera.main.WorldToScreenPoint(new Vector3(enemy.transform.position.x, enemy.transform.position.y + 1.4f, enemy.transform.position.z));
        }
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
