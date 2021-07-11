//작성자 : 신은서
//MonsterSpawner.cs : 스폰 위치에 몬스터 풀을 생성하여 몬스터를 보관하고, 스폰하고, 반납시킨다.
//몬스터의 순찰 포인트 관리
//사용 오브젝트 : Monster Spawner
//
//
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterSpawner : Spawner<Monster>
{
    //스포너 관련 변수
    private Transform monsterSpawnPos; //몬스터 스폰 포지션
    [SerializeField] private int callMonsterMinCount; //몬스터 호출 최소 갯수
    [SerializeField] private int callMonsterMaxCount; //몬스터 호출 최대 갯수
    private int callMonsterRandomCount; //랜덤 호출 갯수
    [SerializeField] private int callDistance; //플레이어와 몬스터스포너의 거리
    [SerializeField] private int respawnerCount = 10; //리스폰 횟수
    [SerializeField] private float respawnerDelayTime = 5f; //리스폰 시간

    //몬스터 관련 변수
    [HideInInspector] public Transform wayPointGroup; //스폰 포인트들의 부모 객체인 몬스터의 순찰 포인트


    protected override void Awake()
    {
        base.Awake();
        monsterSpawnPos = transform.FindChild("Monster Pool"); //몬스터 풀을 찾는다.
        wayPointGroup = transform.FindChild("Way Point Group"); //생성된 몬스터에 순찰 포인트 그룹을 넘겨준다.
    }

    private void Update()
    {
        SpawnObject(); //필드에 몬스터가 있는지 없는지 검사한다.
    }

    protected override void CreateObject() //풀에 몬스터 생성
    {
        objPool.CreateNewObject(objPrefab, monsterSpawnPos, createObjCount, monsterSpawnPos); //생성한 오브젝트풀링에서 필드 몬스터들을 스포너 자식으로 생성
    }

    public override void SpawnObject() //몬스터 스폰
    {
        float dist = Vector3.Distance(GameManager.instance.transform.position, transform.position); //플레이어 위치와 몬스터 스포너의 위치의 거리를 체크한다.
        
        if (fieldObjList.Count <= 0 //필드에 몬스터가 없고
            && respawnerCount > 0) //리스폰 횟수가 남아 있으면
        {
            callMonsterRandomCount = Random.Range(callMonsterMinCount, callMonsterMaxCount + 1);
            if (dist < callDistance) //플레이어 위치와 몬스터 스포너 위치 거리가 callDistance보다 가까우면
            {
                for (int j = 0; j < callMonsterRandomCount; j++) //랜던 갯수만큼 몬스터 호출
                {
                    Monster fieldMonster = objPool.TakeOutObject().GetComponent<Monster>();
                    fieldObjList.Add(fieldMonster); //스포너 필드 몬스터 리스트에 추가
                    MonsterManager.instance.fieldMonsterList.Add(fieldMonster); //전체 필드 몬스터 리스트에 추가
                }
                respawnerCount--;
            }
        }
        else if(fieldObjList.Count > 0 //필드에 몬스터가 있고
            && dist >= callDistance) //플레이어 위치와 몬스터 스폰 위치 거리가 callDistance보다 멀면
        {
            for (int i = 0; i < fieldObjList.Count; i++)
            {
                ReturnObject(fieldObjList[i].gameObject); //몬스터 반납
            }
        }
    }

    public override void ReturnObject(GameObject _returnObject) //사용 후 몬스터 반납
    {
        StartCoroutine(ReturnFieldMonsterCorutine(_returnObject));
    }

    private IEnumerator ReturnFieldMonsterCorutine(GameObject _returnObject)
    {
        _returnObject.GetComponent<MonsterAI>().InitializeMonster(); //몬스터 초기화
        objPool.ReturnObject(_returnObject, monsterSpawnPos); //몬스터 스폰 풀에 반환
        MonsterManager.instance.fieldMonsterList.Remove(_returnObject.GetComponent<Monster>()); 
        //전체 필드 몬스터 리스트에서 제거
        yield return new WaitForSeconds(respawnerDelayTime);

        fieldObjList.Remove(_returnObject.GetComponent<Monster>()); 
        //스포너 필드 몬스터 리스트에서 제거
    }
}
