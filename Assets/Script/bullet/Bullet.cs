using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    const float bulletMaxDistance = 200.0f;

    const float bulletMaxLifeTime = 3.0f; //불릿이 존재할 수 있는 최대 시간 

    public float bulletLifeTime = 0.0f;   //불릿이 존재할 수 있는 최대 시간 측정

    public GameObject attackTarget;    //총알의 최종 목적지  

    [SerializeField]
    float bulletSpeed;    // 총알의 이동 속도

    [SerializeField]
    string filePath; 

    [SerializeField]
    int bulletType; //0: 직선형 총알, 1: 곡선형 총알 

    [SerializeField]
    float reduceHeight;         // 곡선형 공격의 포물선 높이 조절 변수

    float distance = 0.0f;

    [SerializeField]
    float force;

    public GameObject attackOwner;

    // Update is called once per frame
    void Update()
    {
        UpdateBullet(); 
    }

    /// <summary>
    /// 총알 발사 업데이트 : 하은비
    /// </summary>
    void UpdateBullet()
    {
        //예외처리
        if (!attackTarget)
        {
            // 총알 파괴 모션
            SystemManager.Instance.PrefabCacheSystem.DisablePrefabCache(filePath, gameObject);
            return;
        }

        Vector3 bulletPos = transform.position;   // 총알의 위치
        Vector3 targetPos = attackTarget.transform.position;  // 타겟이 총알을 맞는 위치

        // 총알 방향 업데이트
        Vector3 bulletAttackDir = (attackTarget.transform.position - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(bulletAttackDir);
        transform.rotation = rotation;

        // 가속도 붙이기
        bulletSpeed += bulletSpeed * force;
        //Debug.Log("bulletSpeed= " + bulletSpeed);

        if (bulletType == 0) // 직선형
        {
            //transform.position = Vector3.Lerp(bulletPos, targetPos, moveDist*Time.deltaTime*0.2f);            
            Vector3 translation = (targetPos - bulletPos).normalized * Time.deltaTime * bulletSpeed * 1.5f;
            transform.position += translation;

        }
        else if (bulletType == 1) //곡선형
        {
            Vector3 center = (bulletPos + targetPos) / 2;
            center -= new Vector3(0, reduceHeight * 1.0f, 0);
            Vector3 startPos = bulletPos - center;
            Vector3 endPos = targetPos - center;

            transform.position = Vector3.Slerp(startPos, endPos, Time.deltaTime * bulletSpeed * 0.05f);
            transform.position += center;
        }


        // bullet과 target의 거리가 10보다 작을 경우 불렛 비활성화
        distance = (targetPos - bulletPos).sqrMagnitude;

        //Debug.Log("distance= " + (Mathf.Round(distance)));
        if ((Mathf.Round(distance)) < bulletMaxDistance)
        {
            SystemManager.Instance.PrefabCacheSystem.DisablePrefabCache(filePath, gameObject);
            // 다중 타겟인 경우, 이펙트 출력            


            GameObject target;
            target = attackTarget.transform.parent.gameObject;
            
            
            if (attackTarget.tag == "Enemy")
            {
                Enemy enemy = target.GetComponent<Enemy>();

                Turret attacker = attackOwner.GetComponent<Turret>();
                
                enemy.DecreseHP(attacker.power);
            }
            else if(attackTarget.tag == "Turret")
            {
                Turret turret = target.GetComponent<Turret>();

                Enemy attacker = attackOwner.GetComponent<Enemy>();

                turret.DecreseHP(attacker.power);
            }
                                                   
        }
    }
}
