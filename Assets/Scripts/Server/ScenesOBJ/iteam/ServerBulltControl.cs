using UnityEngine;

namespace MyNetGame.ServerScenesOBJ
{
    public class ServerBulltControl : ServerScenesObjectBase
    {
        public Rigidbody rb;
        public Collider col;
        public float damageValue = 30;
        public float BulltVelocity = 10;

        /// <summary>
        /// 初始化子弹对象的刚体、碰撞器与生命周期设置。
        /// </summary>
        public override void InitOBJ()
        {
            base.InitOBJ();
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();

            Destroy(gameObject, 5f);

            if (col != null)
            {
                col.isTrigger = true;
            }
        }

        /// <summary>
        /// 按固定帧推进子弹，并检测命中目标后结算伤害。
        /// </summary>
        private void FixedUpdate()
        {
            if (rb == null)
            {
                return;
            }

            float moveDistance = BulltVelocity * Time.fixedDeltaTime;
            Vector3 direction = transform.forward;
            const float bulletRadius = 0.3f;

            if (Physics.SphereCast(transform.position, bulletRadius, direction, out RaycastHit hit, moveDistance))
            {
                global::HPBase hpBase = hit.collider.GetComponent<global::HPBase>();
                if (hpBase != null)
                {
                    transform.position = hit.point;
                    hpBase.TakeDamage(damageValue);
                    Destroy(gameObject);
                    Debug.Log("射线检测命中:" + hpBase.gameObject.name);
                }
            }
            else
            {
                transform.position += direction * moveDistance;
            }
        }
    }
}
