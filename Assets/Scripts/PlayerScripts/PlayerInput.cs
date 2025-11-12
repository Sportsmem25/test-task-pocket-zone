using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public bool FirePressed { get; private set; }
    public JoystickUI joystick;
    public event System.Action OnFirePressed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if(joystick  == null)
            joystick = FindObjectOfType<JoystickUI>(true);
    }

    private void Update()
    {
        Movement();
        FireInput();
    }

    public void SetFirePressed(bool isPressed) 
    {
        FirePressed = isPressed;
        if(isPressed)
            OnFirePressed?.Invoke();
    }

    private void Movement()
    {
        if( joystick != null && joystick.Direction.magnitude > 0.1f)
        {
            MoveInput = joystick.Direction;
            if(MoveInput.magnitude < 0.12f)
                MoveInput = Vector2.zero;
            return;
        }
        //keyboard
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        MoveInput = new Vector2(h, v);
        if (MoveInput.sqrMagnitude > 1f) MoveInput.Normalize();
    }

    private void FireInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) FirePressed = true;
        if (Input.GetKeyUp(KeyCode.Space)) FirePressed = false;
        Debug.Log(FirePressed);
    }
}