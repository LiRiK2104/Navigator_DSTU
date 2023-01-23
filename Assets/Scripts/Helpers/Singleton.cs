using UnityEditor;
using UnityEngine;

namespace Helpers
{
    public abstract class Singleton<T> : Singleton where T : MonoBehaviour
    {
        private static T _instance;

        private static readonly object Lock = new object();

        protected virtual void Awake()
        {
            if (_instance == null) 
                _instance = gameObject.GetComponent<T>();  
            else 
                Debug.LogError("[Singleton] Second instance of '" + typeof (T) + "' created!");
        }

        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                
                if (Application.isEditor && EditorApplication.isPlaying == false)
                    return null;
#endif

                if (IsQuitting)
                {
                    Debug.LogWarning("[Singleton] '" + typeof(T) + "' not returned cause application is quitting!");
                    return null;
                }

                lock (Lock)
                {
                    if (_instance != null)
                        return _instance;
                    
                    var foundInstances = (T[]) FindObjectsOfType(typeof(T));

                    if (foundInstances.Length > 0)
                    {
                        if (foundInstances.Length > 1)
                        {
                            Debug.LogError("[Singleton] '" + typeof(T) + "' many instances created!");
                        
                            for (int i = 1; i < foundInstances.Length; i++)
                                Destroy(foundInstances[i]);
                        }

                        return _instance = foundInstances[0];   
                    }
                    
                    Debug.LogError("[Singleton] '" + typeof(T) + "' an instance in the scene not exist!");
                        
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = "(singleton) " + typeof(T);
                    return _instance;
                }
            }
        }
    }

    public abstract class Singleton : MonoBehaviour
    {
        public static bool IsQuitting { get; private set; }

        private void OnApplicationQuit()
        {
            IsQuitting = true;
        }
    }
}