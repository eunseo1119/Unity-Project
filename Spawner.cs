using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Spawner.cs : 추상 클래스로 스포너 구현시 상속한다. 
public abstract class Spawner<T> : MonoBehaviour
{
    [SerializeField] protected GameObject objPrefab;//생성할 객체 프리팹
    protected ObjectPooling objPool; //스폰할 객체를 저장할 오브젝트 풀
    [SerializeField] protected int createObjCount = 10; //생성할 객체 갯수
    [HideInInspector] protected List<T> fieldObjList;//생성 객체 관리 리스트


    protected virtual void Awake()
    {
        objPool = new ObjectPooling();
        fieldObjList = new List<T>();
    }

    protected virtual void Start()
    {
        CreateObject(); //게임 시작시 풀에 객체를 생성한다.
    }

    protected abstract void CreateObject();//풀 생성
    public abstract void SpawnObject();//객체 호출
    public abstract void ReturnObject(GameObject _returnObject); //객체 반납
}
