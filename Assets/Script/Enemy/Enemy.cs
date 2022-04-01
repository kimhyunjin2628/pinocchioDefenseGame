using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    enum EnemyState
    {
        Walk,   //�ʵ��� �����͸� ���� �̵�
        Battle, //�ͷ��� ����
        Dead    //�̵�X, ��Ȱ��ȭ ó��
    }
    [SerializeField]
    EnemyState enemyState = EnemyState.Walk;
    
    //Enemy
    [SerializeField]
    int speed;  //�̵��ӵ�

    //�̵� ����
    [SerializeField]
    GameObject[] targetTile;    //Ÿ�ϸ� ���� �ִ� �̵� Ÿ��

    int targetTileIndex = 0;    //Ÿ�ϸ� Ÿ�� �ε���

    GameObject currentTarget;   //���� Ÿ��

    Vector3 dirVec; //�̵�ó���� ���⺤��

    //test
    [SerializeField]
    GameObject[] turret;

    /// <summary>
    /// �ʱ�ȭ �Լ� : ������
    /// </summary>
    protected override void Initialize()
    {
        base.Initialize();

        currentTarget = targetTile[targetTileIndex];
        dirVec = FindDirVec(currentTarget);
    }

    /// <summary>
    /// this��ü�� ��ġ���� target������ ���⺤�͸� ���� ��ȯ : ������
    /// </summary>
    /// <param name="target">�������� �� Ÿ��</param>
    /// <returns>������ ���⺤��</returns>
    Vector3 FindDirVec(GameObject target)
    {
        if (target == null)
            return Vector3.zero;

        Vector3 dirVec = Vector3.zero;
        dirVec = target.transform.position - transform.position;
        dirVec.Normalize();
        return dirVec;
    }

    /// <summary>
    /// �ǽð� this��ü�� ���º� ���� : ������
    /// </summary>
    protected override void UpdateActor()
    {
        switch (enemyState)
        {
            case EnemyState.Walk:
                CheckArrive();
                UpdateMove(dirVec);
                DetectTarget(turret);
                break;
            case EnemyState.Battle:
                UpdateBattle();
                break;

        }
    }

    #region Walk - �̵��� �� ����

    /// <summary>
    /// ��ǥ �������� ���� �ߴ��� Ȯ�� : ������
    /// </summary>
    void CheckArrive()
    {
        //����ó��
        if (currentTarget == null)
            return;
        if (targetTileIndex >= targetTile.Length - 1)
            return;

            //Ÿ�ٿ� �������� �ʾ��� ���
            if (Vector3.Distance(transform.position, currentTarget.transform.position) > 0.5f)
        {
            float rotY = Mathf.Round(transform.localEulerAngles.y);
            //����ó��, �ӵ��� ���� distance�� �������� ������ ��� ���⺰ ����ó��
            if (!((rotY == 0f && transform.position.z < currentTarget.transform.position.z)//����
                || (rotY == 90f && transform.position.x < currentTarget.transform.position.x)//������
                || (rotY == 180f && transform.position.z > currentTarget.transform.position.z)//����
                || (rotY == 270f && transform.position.x > currentTarget.transform.position.x)))//����
            {
                return;
            }
        }

        //�̵� Ÿ�� ����
        transform.position = new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z);
        
        currentTarget = targetTile[++targetTileIndex];

        dirVec = FindDirVec(currentTarget);
    }

    /// <summary>
    /// ���� ���ͷ� �ǽð� this��ü�� ��ġ�� ȸ���� ����: ������ 
    /// </summary>
    /// <param name="dirVec">�̵��� ȸ���� ���⺤��</param>
    void UpdateMove(Vector3 dirVec)
    {
        //����ó��
        if (dirVec == Vector3.zero)
            return;

        Vector3 updateVec = new Vector3(dirVec.x * speed * Time.deltaTime, 0 , dirVec.z * speed * Time.deltaTime);
        transform.position += updateVec;
        Quaternion rotation = Quaternion.LookRotation(-dirVec);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 0.3f);
    }

    /// <summary>
    /// this��ü�� ��Ÿ� �ȿ��ִ� Ÿ���� ������ ���� ������ Ÿ���� ���� : ������
    /// </summary>
    protected override void DetectTarget(GameObject[] target)
    {
        base.DetectTarget(target);
    }

    #endregion

    #region Attack - ������ �� ����

    /// <summary>
    /// this��ü�� ���¸� �������� ���� : ������
    /// </summary>
    protected override void Attack()
    {
        base.Attack();

        //���� ���·� ���� 
        this.enemyState = EnemyState.Battle;

        //���� 
        animator.SetBool("attack", true);
        animator.SetBool("finAttack", false);
    }

    /// <summary>
    /// �ǽð����� ������ �������� �ȳ��������� �Ǻ��ϰ� ��������� 
    /// ���� �������� �������� �ٸ� ���·� ���������� ���� : ������
    /// </summary>
    protected override void UpdateBattle()
    {
        base.UpdateBattle();

        //attackSpeed�ʿ� 1�� ����
        if (Time.time - attackTimer > attackSpeed)
        {
            //������ ����� ���� ������ ���� ���� ��ȭ
            if (attackTarget == null || !(attackTarget.activeSelf))
            {
                enemyState = EnemyState.Walk;

                //���� ����
                animator.SetBool("finAttack", true);
            }
            else
            {
                //���ݽð� ���� ���� �ʱ�ȭ
                attackTimer = Time.time;

                //���� ����
                animator.SetBool("attack", true);
            }
        }
    }

    #endregion
}