using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Helpers.Tests
{
    public class Test : MonoBehaviour
    {
        private IEnumerator _myRoutine;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(AnotherRoutine());
            }
        }

        private IEnumerator AnotherRoutine()
        {
            yield return GetPreparedMyRoutine();
        }
        
        private IEnumerator GetPreparedMyRoutine()
        {
            Debug.Log("Prepared");
            return MyRoutine();
        }
        
        private IEnumerator MyRoutine()
        {
            Debug.Log("Started");
            yield return null;
        }
    }
}





