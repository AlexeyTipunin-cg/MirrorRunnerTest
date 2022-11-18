using UnityEngine;

public class ApplicationController : MonoBehaviour
{

    private static bool _blockInput;
    public static bool blockInput => _blockInput;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            _blockInput = !_blockInput;
            Cursor.lockState = _blockInput ? CursorLockMode.None: CursorLockMode.Confined;
        }
    }
}
