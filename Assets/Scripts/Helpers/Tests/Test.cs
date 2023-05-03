using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Helpers.Tests
{
    public class Test : MonoBehaviour
    {
        public event Action TestEvent;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TestEvent?.Invoke();
            }
        }
    }
}





