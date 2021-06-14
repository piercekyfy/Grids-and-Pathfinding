using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PWH.Utils
{
    public abstract class BasicSingleton<T> : MonoBehaviour where T : BasicSingleton<T>
    {
        public static T instance;

        protected virtual void Awake()
        {
            if (instance) { Destroy(this); return; }

            instance = (T)this;
        }
    }
}


