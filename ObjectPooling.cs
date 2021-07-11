using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ObjectPooling.cs : 오브젝트 풀링, 객체를 풀에 생성하여, 필요시 객체를 호출하고, 사용 후 다시 반납 받는다.
public class ObjectPooling : ScriptableObject
{
    private Queue<GameObject> poolingObjQueue; //객체를 저장시킬 풀
    private int _objCount; //풀에 있는 객체 갯수
    public int objCount
    {
        get
        {
            return _objCount;
        }
    }


    //오브젝트 풀을 초기화 미리 빌려줄 오브젝트 생성
    public List<GameObject> CreateNewObject(
        GameObject _newObj, Transform _spownPointPos, int _count, Transform _spownPoint)
    {
        poolingObjQueue = new Queue<GameObject>();
        List<GameObject> createObjList = new List<GameObject>();

        for (int i = 0; i < _count; i++)
        {
            GameObject createObj = Instantiate(_newObj, _spownPointPos.position, Quaternion.identity, _spownPoint);//_count만큼 객체를 생성
            createObjList.Add(createObj); 
            poolingObjQueue.Enqueue(createObj); //생성한 객체 저장
            createObj.SetActive(false);
        }
        _objCount = poolingObjQueue.Count;
        return createObjList;
    }


    //풀에서 객체를 호출한다.
    public GameObject TakeOutObject()
    {
        if (poolingObjQueue.Count > 0) //풀에 객체가 있으면
        {
            GameObject obj = poolingObjQueue.Dequeue(); //빌려준다.
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);

            _objCount = poolingObjQueue.Count;
            return obj; //빌려준 객체 반환
        }
        else //객체가 없으면
        {
            _objCount = poolingObjQueue.Count;
            return null; //객체를 반환하지 않는다.
        }

    }

    //사용한 객체를 풀에 반납시킨다.
    public void ReturnObject(GameObject _returnObject, Transform _spownPoint)
    {
        poolingObjQueue.Enqueue(_returnObject);
        _returnObject.transform.position = _spownPoint.position;
        _returnObject.transform.SetParent(_spownPoint);
        _returnObject.SetActive(false);

        _objCount = poolingObjQueue.Count;
    }
}
