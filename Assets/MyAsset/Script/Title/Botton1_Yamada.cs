using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Title.Yamasample {
    internal sealed class Botton1_Yamada : MonoBehaviour
    {
        // クリック時にログ表示  
        public void OnEvent_pointerDown()
        {
            Debug.Log("Click LEFT");
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
