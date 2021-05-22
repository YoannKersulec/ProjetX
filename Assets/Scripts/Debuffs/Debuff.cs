using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Debuffs
{
    public abstract class Debuff
    {

        public float MyDuration { get; set; }

        public float ProcChance { get; set; }

        private float elapsed;

        protected Character character;

        public virtual void Apply(Character character)
        {
            this.character = character;
            character.ApplyDebuff(this);
        }

        public virtual  void Remove()
        {
            character.RemoveDebuff(this);
            elapsed = 0;
        }

        public virtual void Update()
        {
            elapsed += Time.deltaTime;

            if (elapsed >= MyDuration)
            {
                Remove();
            }
        }
    }
}
