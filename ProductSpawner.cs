using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ProductSpawner.cs :  생산 풀을 생성하여 생산물을 보관하고, 스폰하고, 반납시킨다.
//사용 오브젝트 : 생산 스포너
public class ProductSpawner : Spawner<Product>
{
    //생산 관련 변수
    public Product.ProductType productType; //생산물 타입 설정
    public Weapon.WeaponType weaponType; //무기 타입 설정
    public string productSpawnerContext; //생산버튼 내용 설정

    //생산물 호출 관련 변수
    private int callProductrCount = 1;
    [SerializeField] private float xPosMin = -1f;
    [SerializeField] private float xPosMax = 1f;
    [SerializeField] private float yPos = 0.2f;
    [SerializeField] private float zPosMin = -1f;
    [SerializeField] private float zPosMax = 1f;


    protected override void CreateObject() //풀에 생산물 생성
    {
        List<GameObject> createItemList = objPool.CreateNewObject(objPrefab, transform, createObjCount, transform); 
        //생산물을 오브젝트 주변에 랜덤 위치로 랫덤 개수만큼 생성
        GameObject[] createItems = createItemList.ToArray();

        for (int i = 0; i < createItems.Length; i++)
        {
            Item createItem = createItems[i].GetComponent<Item>();
            createItem.CreateSpawnItem(); //생산 스포너를 통해 생산된 아이템일 경우 셋팅
        }
    }

    public override void SpawnObject() //생산물 호출
    {
        if (GameManager.instance.productAnim.isProduct == true) //생산중이면
        {
            if (objPool.objCount >= callProductrCount) //풀의 오브젝트 갯수가 호출 갯수보다 클 때
            {
                Product product = objPool.TakeOutObject().GetComponent<Product>(); //생산물을 필드로 반환한다.
                fieldObjList.Add(product);
                PlaceProduct(product.transform);
            }
            GameManager.instance.productAnim.isProduct = false; //생산 종료

            if(GetComponent<AnimalAI>()) //AI스포너일 경우
            {
                GetComponent<AnimalAI>().CheckProduct(GameManager.instance.productAnim.isProduct); //생산중이 아님으로 변경
            }
        }
    }

    public override void ReturnObject(GameObject _returnObject) //생산물 반납
    {
        objPool.ReturnObject(_returnObject, transform);
        fieldObjList.Remove(_returnObject.GetComponent<Product>());
    }

    private void PlaceProduct(Transform _productTr) //생산물 스폰 위치
    {
        float xPos = Random.Range(xPosMin, xPosMax);
        float zPos = Random.Range(zPosMin, zPosMax);
        Vector3 tmpPos = new Vector3(xPos, yPos, zPos);
        _productTr.position = transform.position + tmpPos;
    }
}
