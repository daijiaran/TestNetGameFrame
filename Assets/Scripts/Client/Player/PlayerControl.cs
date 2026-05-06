using System;
using Shared.DJRNetLib;
using Shared.DJRNetLib.Packet;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [Header("Player")]
    public Collider collider;
    public Rigidbody rigidbody;
    public TextMeshProUGUI PlayerName;
    public float moveSpeed = 5f;
    public bool isCurrentPlayer = false;
    public bool isActive = false;
    

    [Header("Camera")]
    public Camera camera;
    public float mouseSpeed = 3f;
    public float zoomSpeed = 5f;
    public float minDistance = 2f;
    public float maxDistance = 15f;

    private float currentX;
    private float currentY = 20f;
    private float distance = 6f;
    
    [Header("枪口的面向方向")]
    public Transform FaceDirection;
    
    
    private void Start()
    {
        
    }

    public void Init()
    {
        collider = GetComponent<Collider>();
        rigidbody = GetComponent<Rigidbody>();

        Debug.Log("Init()方法测试设置当前玩家,PlayerSelf.isCurrentPlayer:"+isCurrentPlayer);

        if (isCurrentPlayer)
        {
            camera = Camera.main;
            //将相机设为玩家子物体
            camera.transform.SetParent(transform);
        
            //设置相机在玩家头顶的高度（例如往上偏移 15 米）
            // x=0, z=0 保证相机在正上方，y 是高度
            camera.transform.localPosition = new Vector3(0, 15f, 0);
        
            //设置旋转：绕 X 轴旋转 90 度，使其垂直向下看
            camera.transform.localRotation = Quaternion.Euler(85f, 0, 0);
        
            //允许 HandleCameraRotation 控制
            //同步初始的变量值
            currentX = transform.eulerAngles.y;
            currentY = 85f; // 初始俯视角度
            distance = maxDistance; // 对应 y 的高度
            //激活玩家实例，开始更新
            isActive = true;
            Debug.Log("当前玩家初始化成功！！");
        }
    }

    private void FixedUpdate()
    {
        if (!isCurrentPlayer) return;
        if(!isActive) return;
        Move();
    }

    private void Update()
    {
        if (!isCurrentPlayer) return;
        if(!isActive) return;
        HandleCameraRotation();
        HandleCameraZoom();
        FaceToMouse();
        Attack();
    }






    /// <summary>
    ///  玩家面向方向
    /// </summary>
    public void FaceToMouse()
    {
        //安全检查：确保鼠标设备存在
        if (Mouse.current == null) return;

        //获取新版输入系统的鼠标位置
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        Ray ray = camera.ScreenPointToRay(mouseScreenPosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 lookTarget = hit.point;
            lookTarget.y = FaceDirection.position.y; 

            FaceDirection.LookAt(lookTarget);
        }
    }


    private float attackTimer = 0f; // 攻击计时器
    public float attackInterval = 0.1f; // 攻击间隔（秒），例如0.2秒发一次子弹

    /// <summary>
    /// 攻击信号发送（支持持续按住）
    /// </summary>
    public void Attack()
    {
        if (Mouse.current == null) return;

        // 1. 计时器累加（使用Time.deltaTime确保时间平滑）
        attackTimer += Time.deltaTime;

        // 2. 检测鼠标左键是否处于“按住”状态
        if (Mouse.current.leftButton.isPressed)
        {
            // 3. 判断是否达到了攻击间隔
            if (attackTimer >= attackInterval)
            {
                // 执行攻击逻辑
                SendAttack();
            
                // 重置计时器
                attackTimer = 0f;
            }
        }
    }

    //发送逻辑
    private void SendAttack()
    {
        // 构造发射子弹的类型
        UserAttackPacket attackPacket = new UserAttackPacket(1, "Bullt1");
        // 发送消息
        NetworkPlayerManager.Instance.SendAttackToServer(attackPacket);
    }


    public void Died()
    {
        if (isCurrentPlayer)
        {
            ClientRoot.Instance.GameOverEvent?.Invoke();
            camera.transform.SetParent(null);
        }

        Destroy(gameObject);
    }
    
    
    
    #region ===== 玩家移动（网络） =====
    private void Move()
    {
        float h = 0;
        float v = 0;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) h -= 1;
            if (Keyboard.current.dKey.isPressed) h += 1;
            if (Keyboard.current.sKey.isPressed) v -= 1;
            if (Keyboard.current.wKey.isPressed) v += 1;
        }

        
        Vector3 dir = GetPlayerMoveDirection();
        
        //创建并且写入数据
        UserMovePacket movePacket = new UserMovePacket(h, v,dir.x, dir.y, dir.z);
        movePacket.Attack_x = FaceDirection.forward.x;
        movePacket.Attack_y = FaceDirection.forward.y;
        movePacket.Attack_z = FaceDirection.forward.z;
        
        
        //发送数据
        NetworkPlayerManager.Instance.SendMoveToSever(movePacket);
        
    }
    #endregion

    
    
    
    
    #region ===== 相机控制 =====

    /// <summary>
    /// 右键旋转相机（绕玩家）
    /// </summary>
    private void HandleCameraRotation()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            currentX += Mouse.current.delta.ReadValue().x * mouseSpeed * Time.deltaTime;
            currentY -= Mouse.current.delta.ReadValue().y * mouseSpeed * Time.deltaTime;
            currentY = Mathf.Clamp(currentY, -20f, 80f);
        }

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 dir = new Vector3(0, 0, -distance);

        camera.transform.position = transform.position + rotation * dir;
        camera.transform.LookAt(transform.position + Vector3.up * 1.5f);
    }

    
    /// <summary>
    /// 获取相机的面向方向
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPlayerMoveDirection()
    {
        Vector3 direction = camera.transform.forward;
        direction.y = 0;
        if (direction.sqrMagnitude > 0.001f)
        {
            direction.Normalize();
        }
        else
        {
            direction = transform.forward;
        }
        return direction;
    }

    

    /// <summary>
    /// 滚轮缩放
    /// </summary>
    private void HandleCameraZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll == 0) return;

        distance -= scroll * zoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    #endregion

    public void OnDisable()
    {
        isActive = false;
    }
}
