using UnityEngine;

namespace Controllers
{
    public class KeyboardController : AbstractController
    {
        public override MainController.ControlType GetControlType()
        {
            return MainController.ControlType.Keyboard;
        }

        private Vector2 _input;
        public override Vector2 GetInput()
        {
            _input.y = Input.GetAxis("Vertical");
            _input.x = Input.GetAxis("Horizontal") * 0.5f;
            return _input;
        }
    }
}
