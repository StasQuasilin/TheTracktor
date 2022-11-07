using DitzeGames.MobileJoystick;
using UnityEngine;

namespace Controllers
{
    public class JoystickController : AbstractController
    {
        private Joystick _joystick;
        private TouchField _touchField;
        
        private void Awake() {
            _joystick = FindObjectOfType<Joystick>();
            _touchField = FindObjectOfType<TouchField>();
        }
        
        public override MainController.ControlType GetControlType()
        {
            return MainController.ControlType.Joystick;
        }

        private Vector2 _input;
        public override Vector2 GetInput()
        {
            _input.x = _joystick.AxisNormalized.x;
            _input.y = _joystick.AxisNormalized.y;

            return _input;
        }
    }
}
