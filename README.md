## **📃핵심 기술**

### ・곂치지앉게 갱신되는 유닛 목록

🤔**WHY?**

Auto게임의 특징 상, 구매할 수 있는 유닛 목록에 곂치는 유닛이 등장 시 밸런스가 한번에 무너져버릴 우려가 있음

🤔**HOW?**

 관련 코드

- ShowRandomUnit
    
    ```csharp
    using UnityEngine;
    
    public class ShowRandomUnit : MonoBehaviour
    {
        //랜덤으로 활성화시킬 9개의 이미지
        [Header("UnitImages")]
        [SerializeField] GameObject[] unitImages;
    
        //한번에 최대 활성화 유닛 이미지 수
        [HideInInspector]public int showMaxUnit = 5;
    
        /// <summary>
        /// 활성화되어있는 모든 유닛 이미지를 비활성화 후
        /// 랜덤한 5명의 유닛 이미지를 활성화
        /// 버튼으로 활성화
        /// </summary>
        public void RerollUnitImage()
        {
            //현재 터치중이거나, 전투 상태거나, 현재 골드가 리룰 비용보다 적으면 리턴
            if (GameManager.instance.playerUnitController.isTouch || GameManager.instance.levelManager.currentState == StateType.BATTLE)
                return;
    
            if(GameManager.instance.levelManager.currentGold < LevelManager.reRullCost)
            {
                GameManager.instance.levelManager.SetErrorMessage("보유 골드가 부족해 리롤을 할 수 없습니다!");
                return;
            }
    
            int[] random = new int[showMaxUnit];
            for (int i = 0; i < unitImages.Length; i++)
            {
                if (unitImages[i].activeSelf)
                unitImages[i].SetActive(false);
            }
            while(true)
            {
                random[0] = Random.Range(0, unitImages.Length);
                random[1] = Random.Range(0, unitImages.Length);
                random[2] = Random.Range(0, unitImages.Length);
                random[3] = Random.Range(0, unitImages.Length);
                random[4] = Random.Range(0, unitImages.Length);
    
                if (random[0] != random[1] && random[0] != random[2] && random[0] != random[3] && random[0] != random[4] &&
                    random[1] != random[2] && random[1] != random[3] && random[1] != random[4] &&
                    random[2] != random[3] && random[2] != random[4] &&
                    random[3] != random[4])
                    break;
            }
            for (int i = 0; i < random.Length; i++)
            {
                unitImages[random[i]].SetActive(true);
            }
            GameManager.instance.levelManager.SetGold(-LevelManager.reRullCost);
            GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.GOLD);
    
        }
    }
    ```
    

🤓**Result!**

새로고침 시, 항상 다른 종류의 유닛이 곂치지 않게 등장하게 되어 밸런스상 어색하지 않은 게임 플레이 경험 제공

### ・조건 충족 시 자동으로 유닛을 합성

🤔**WHY?**

반복적으로 필드의 유닛 수를 계산해 조건 충족시 합성을 진행해, 불필요한 로직이 프레임마다 반복됨.

🤔**HOW?**

 관련 코드

- MixUnitManager
    
    ```csharp
    using System.Collections.Generic;
    using UnityEngine;
    using DG.Tweening;
    using System.Collections;
    public class MixUnitManager : MonoBehaviour
    {
    	public void CheckUnitMix()
    	{
        //유닛 보관용 리스트
        var units = new List<PlayerUnitManager>();
        var typeEndNum = new List<PlayerUnitManager>();
        var num = new List<PlayerUnitManager>();
    
        //활성화된 모든 유닛을 호출
        var obj = GameObject.FindGameObjectsWithTag(LevelManager.playerTag);
        if (obj.Length > 2)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                //unit리스트에 모든 유닛을 대입
                units.Add(obj[i].GetComponent<PlayerUnitManager>());
            }
    
            for (int j = 1; j < LevelManager.maxLevel; j++)
            {
                //레벨이 같은 유닛을 리스트로 저장
                num = units.FindAll(x => x.level == j);
                if (num.Count > 2)
                    break;
            }
            if (num.Count > 2)
            {
                for (int i = 0; i <= (int)playerUnitType.MAGE2; i++)
                {
                    //레벨이 같은 유닛 중에서 타입이 같은 유닛을 리스트로 저장
                    typeEndNum = num.FindAll(x => x.playerUnitType == (playerUnitType)i);
                    if (typeEndNum.Count > 2)
                        break;
                }
    
                if (typeEndNum.Count > 2)
                {
                    //레벨,종류가 같은 유닛이 3체 이상 있을 시
                    if (typeEndNum.Count >= LevelManager.mixNum)
                    {
                        StartCoroutine(MixedUnitCorutine(typeEndNum, 0.2f));
                    }
                }
            }
        }
    	}
    	
    	IEnumerator MixedUnitCorutine(List<PlayerUnitManager> units, float time)
    	{
        units[1].transform.DOMove(units[0].transform.position, time);
        units[2].transform.DOMove(units[0].transform.position, time);
    
        units[1].transform.DOScale(Vector2.zero, time);
        units[2].transform.DOScale(Vector2.zero, time);
    
        yield return new WaitForSeconds(time);
        UnitLevelUp(units);
        DestroyUnit(units);
        yield return new WaitForSeconds(time * 1.1f);
        CheckUnitMix();
    	}
    }
    ```
    
- BuySelectUnit
    
    ```csharp
    using UnityEngine;
    
    public class BuySelectUnit : MonoBehaviour
    {
      public void BuySelectUnitClick(int index)
      {
          //유닛 터치상태가 아니고, 전투상태가 아니고, 현재 골드가 구입 비용보다 적으면 리턴
          if (GameManager.instance.playerUnitController.isTouch || GameManager.instance.levelManager.currentState == StateType.BATTLE)
              return;
    
          if (LevelManager.buyCost > GameManager.instance.levelManager.currentGold)
          {
              GameManager.instance.levelManager.SetErrorMessage("보유 골드가 부족해 유닛을 구매할 수 없습니다!");
              return;
          }
    
          int saveIndex = 0;
          for(int i = 0; i < summonIndex.Length; i++)
          {
              if (summonIndex[i] == false)
              {
                  saveIndex = i;
                  break;
              }
    
          }
          //현재 보관중인 유닛 수가 최대 보관 가능한 유닛 수보다 적을때만 소환
          if (currentActiveUnitNum < GameManager.instance.showRandomUnit.showMaxUnit)
          {
              var unit = Instantiate(unitPrefabs[index], summonPos[saveIndex], Quaternion.identity);
              unit.GetComponent<PlayerUnitManager>().savePos = summonPos[saveIndex];
              unit.GetComponent<PlayerUnitManager>().buyUnitIndex = saveIndex;
              unit.GetComponent<PlayerUnitManager>().currentUnitState = unitState.WAIT;
              summonIndex[saveIndex] = true;
              currentActiveUnitNum++;
              GameManager.instance.levelManager.SetGold(-LevelManager.buyCost);
              GameManager.instance.showRandomUnit.ShowRandomUnitImage();
              GameManager.instance.mixUnitManager.CheckUnitMix();
              GameManager.instance.audioManager.PlayerSfx(AudioManager.Sfx.GOLD);
    
          }
          else
          {
              //유닛이 꽉 찼으므로 UI등으로 경고 표시
              GameManager.instance.levelManager.SetErrorMessage("유닛을 더이상 보관할 수 없습니다!");
          }
    
      }
    }
    ```
    

🤓**Result!**

합성 조건을 반복적으로 계산하는 것이 아닌, 유닛을 구매했을 시에만 합성 조건을 체크해, 불필요하게 반복되는 로직을 제거

### ・레이캐스트를 이용한 마우스 오브젝트 선택

🤔**WHY?**

유닛, 아이템 등 각종 오브젝트들을 마우스로 선택할 수 있는 기능 구현

🤔**HOW?**

 관련 코드

- GameManager
    
    ```csharp
    public class GameManager : MonoBehaviour
    {
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
    ```
    

🤓**Result!**

특정 레이어를 매개변수로 해, 각종 오브젝트 선택 메소드를 호출하는 것 만으로 간단하게 오브젝트 선택 

### ・ScriptableObject를 이용한 데이터 관리

🤔**WHY?**

변동성이 없는 고정 데이터를 따로 저장하지 않고 그때그때 변수에 대입해, 체계적인 관리가 힘들어짐.

🤔**HOW?**

 관련 코드

- SynageData
    
    ```csharp
    using UnityEngine;
    [CreateAssetMenu(fileName = "SynageData", menuName = "ScriptableObject/Data/Synage")]
    public class SynageData : ScriptableObject
    {
        public float[] upAttackDamage;
        public float[] upReflection;
        public float[] upAttackSpeed;
        public float[] upAttackRange;
        public float[] downResistance;
    
        [TextArea]
        public string synageInfo;
    
    }
    ```
    

🤓**Result!**

ScriptableObject에 변동성 없는 데이터를 모두 모아 관리해, 유지보수성이 대폭 상승함
