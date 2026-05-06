using UnityEngine;

namespace MyNetGame.ClientScenesOBJ
{
    public class ClientBulltControl : ClientScenesObjectBase
    {
        public Rigidbody rb;
        public Collider col;
        public float damageValue = 30;
        public float BulltVelocity = 10;

        public override void InitOBJ()
        {
            base.InitOBJ();
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();

            if (rb != null)
            {
                rb.isKinematic = true;
            }

            if (col != null)
            {
                col.enabled = false;
            }
        }
    }
}
