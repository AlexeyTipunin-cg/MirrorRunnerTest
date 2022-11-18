using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class EnterNameUI : MonoBehaviour
    {
        [SerializeField] private InputField _inputField;
        // Use this for initialization
        void Start()
        {
            _inputField.onSubmit.AddListener(OnSubmit);
        }


        private void OnSubmit(string name)
        {
            //string
        }

    }
}