using UnityEngine;
using DitzeGames.MobileJoystick;

namespace DitzeGames.MobileJoystick.Example {
    public class BoxMover : MonoBehaviour {
        private Joystick _joystick;
        private Button _button;
        private TouchField _touchField;

        // Use this for initialization
        void Awake() {
            _joystick = FindObjectOfType<Joystick>();
            _button = FindObjectOfType<Button>();
            _touchField = FindObjectOfType<TouchField>();
        }

        // Update is called once per frame
        void Update() {
            transform.position = new Vector3(transform.position.x + _joystick.AxisNormalized.x * Time.deltaTime * 3f,
                _button.Pressed ? 2 : 1, transform.position.z + _joystick.AxisNormalized.y * Time.deltaTime * 3f);
            transform.Rotate(Vector3.up, _touchField.TouchDist.x);
            transform.Rotate(Vector3.left, _touchField.TouchDist.y);
        }
    }
}