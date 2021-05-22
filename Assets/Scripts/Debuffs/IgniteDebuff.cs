using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Debuffs
{
    class IgniteDebuff : Debuff
    {
        public float MyTickDamage { get; set; }

        private float elapsed;

        public  IgniteDebuff()
        {
            MyDuration = 20;
            MyTickDamage = 5;
        }

        public override void Update()
        {
            elapsed += Time.deltaTime;

            if (elapsed >= MyDuration/MyTickDamage)
            {
                character.TakeDamage(MyTickDamage, null);
                elapsed = 0;
            }

           base.Update();
        }

        public override void Remove()
        {
            elapsed = 0;
            base.Remove();
        }
    }
}
