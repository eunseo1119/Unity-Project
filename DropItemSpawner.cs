using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//작성자 : 배수현
//DropItemSpawner.cs :  생산 풀을 생성하여 드랍 아이템을 보관하고, 스폰하고, 반납시킨다.
//사용 오브젝트 : 몬스터
public class DropItemSpawner : Spawner<Item>
{
    //생산물 호출 관련 변수
    [SerializeField] private int callDropItemMinCount;
    [SerializeField] private int callDropItemMaxCount;
    private int callDropItemRandomCount;
    [SerializeField] private float xPosMin;
    [SerializeField] private float xPosMax;
    [SerializeField] private float yPos = 0.2f;
    [SerializeField] private float zPosMin;
    [SerializeField] private float zPosMax;


    protected override void CreateObject() //몬스터 생성시 드랍 아이템 풀에 드랍 아이템들 생성
    {
        List<GameObject> createItemList = objPool.CreateNewObject(objPrefab, transform, createObjCount, transform);
        //드랍 아이템을 오브젝트 주변에 랜덤 위치로 랜덤 갯수만큼 생성
        GameObject[] createItems = createItemList.ToArray();

        for (int i = 0; i < createItems.Length; i++)
        {
            Item createItem = createItems[i].GetComponent<Item>();
            createItem.CreateSpawnItem();//생산 스포너를 통해 생산된 아이템일 경우 셋팅
        }
    }

    public override void SpawnObject() // 몬스터 처치시 드랍 아이템 호출
    {
        callDropItemRandomCount = Random.Range(callDropItemMinCount, callDropItemMaxCount + 1); 
        if (objPool.objCount >= callDropItemRandomCount) //풀에 있는 오브젝트의 개수가 랜덤 카운트보다는 클때 
        {
            for (int i = 0; i < callDropItemRandomCount; i++) //랜덤 갯수만큼 드랍 아이템 호출
            {
                Item item = objPool.TakeOutObject().GetComponent<Item>();
                fieldObjList.Add(item);
                PlaceDropItem(item.transform);
            }
        } 
    }

    public override void ReturnObject(GameObject _returnObject) //드랍아이템 반납
    {
        objPool.ReturnObject(_returnObject.gameObject, transform);
        fieldObjList.Remove(_returnObject.GetComponent<Item>());
    }

    private void PlaceDropItem(Transform _dropItemTr) //드랍 아이템 스폰 위치
    {
        float xPos = Random.Range(xPosMin, xPosMax);
        float zPos = Random.Range(zPosMin, zPosMax);
        Vector3 tmpPos = new Vector3(xPos, yPos, zPos);
        _dropItemTr.transform.position = transform.position + tmpPos;
    }
}
