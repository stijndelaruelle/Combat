using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace XBOXParty
{
    public enum ButtonState
    {
        None,
        OnPress,   //Only fire the frame we're pressed
        OnRelease, //Only fire the frame we released
        Pressed,   //Fire all the time while pressed
        Released   //Fire all the time while released
    }

    public class InputManager : Singleton<InputManager>
    {
        private class ButtonInputCommand
        {
            public ButtonInputCommand(int controllerIndex, KeyCode keyCode, ControllerButtonCode buttonCode, ButtonState buttonState)
            {
                this.ControllerIndex = controllerIndex;
                this.KeyCode = keyCode;
                this.ButtonCode = buttonCode;
                this.ButtonState = buttonState;
            }

            //Add alt inputs!
            private int m_ControllerIndex;
            public int ControllerIndex
            {
                get { return m_ControllerIndex; }
                set { m_ControllerIndex = value; }
            }

            private KeyCode m_KeyCode;
            public KeyCode KeyCode
            {
                get { return m_KeyCode; }
                set { m_KeyCode = value; }
            }

            private ControllerButtonCode m_ButtonCode;
            public ControllerButtonCode ButtonCode
            {
                get { return m_ButtonCode; }
                set { m_ButtonCode = value; }
            }

            private ButtonState m_ButtonState;
            public ButtonState ButtonState
            {
                get { return m_ButtonState; }
                set { m_ButtonState = value; }
            }
        }

        private class AxisInputCommand
        {
            public AxisInputCommand(int controllerIndex, KeyCode positiveKeyCode, KeyCode negativeKeyCode, ControllerAxisCode axisCode, ControllerButtonCode positiveButtonCode, ControllerButtonCode negativeButtonCode)
            {
                this.ControllerIndex = controllerIndex;
                this.PositiveKeyCode = positiveKeyCode;
                this.NegativeKeyCode = negativeKeyCode;
                this.AxisCode = axisCode;
                this.PositiveButtonCode = positiveButtonCode;
                this.NegativeButtonCode = negativeButtonCode;
            }

            private int m_ControllerIndex;
            public int ControllerIndex
            {
                get { return m_ControllerIndex; }
                set { m_ControllerIndex = value; }
            }

            private KeyCode m_PositiveKeyCode;
            public KeyCode PositiveKeyCode
            {
                get { return m_PositiveKeyCode; }
                set { m_PositiveKeyCode = value; }
            }

            private KeyCode m_NegativeKeyCode;
            public KeyCode NegativeKeyCode
            {
                get { return m_NegativeKeyCode; }
                set { m_NegativeKeyCode = value; }
            }

            private ControllerAxisCode m_AxisCode;
            public ControllerAxisCode AxisCode
            {
                get { return m_AxisCode; }
                set { m_AxisCode = value; }
            }

            private ControllerButtonCode m_PositiveButtonCode;
            public ControllerButtonCode PositiveButtonCode
            {
                get { return m_PositiveButtonCode; }
                set { m_PositiveButtonCode = value; }
            }

            private ControllerButtonCode m_NegativeButtonCode;
            public ControllerButtonCode NegativeButtonCode
            {
                get { return m_NegativeButtonCode; }
                set { m_NegativeButtonCode = value; }
            }
        }

        private Dictionary<string, AxisInputCommand> m_AxisInputCommands;
        private Dictionary<string, ButtonInputCommand> m_ButtonInputCommands;

        protected override void Awake()
        {
            base.Awake();
            m_AxisInputCommands = new Dictionary<string, AxisInputCommand>();
            m_ButtonInputCommands = new Dictionary<string, ButtonInputCommand>();
        }

        protected override void OnDestroy()
        {
            m_AxisInputCommands.Clear();
            m_ButtonInputCommands.Clear();
        }


        private void Update()
        {
            //The keyboard updates trough unity.
            ControllerInput.UpdateState();
        }


        public void BindButton(string name, KeyCode keyCode, ButtonState buttonState)
        {
            if (m_ButtonInputCommands.ContainsKey(name))
            {
                ButtonInputCommand inputCommand = m_ButtonInputCommands[name];
                inputCommand.KeyCode = keyCode;
                inputCommand.ButtonState = buttonState;
                return;
            }

            m_ButtonInputCommands.Add(name, new ButtonInputCommand(0, keyCode, ControllerButtonCode.None, buttonState));
        }

        public void BindButton(string name, int controllerIndex, ControllerButtonCode buttonCode, ButtonState buttonState)
        {
            if (m_ButtonInputCommands.ContainsKey(name))
            {
                ButtonInputCommand inputCommand = m_ButtonInputCommands[name];
                inputCommand.ControllerIndex = controllerIndex;
                inputCommand.ButtonCode = buttonCode;
                inputCommand.ButtonState = buttonState;
                return;
            }

            m_ButtonInputCommands.Add(name, new ButtonInputCommand(controllerIndex, KeyCode.None, buttonCode, buttonState));
        }

        public void BindAxis(string name, KeyCode positiveKeyCode, KeyCode negativeKeyCode)
        {
            if (m_AxisInputCommands.ContainsKey(name))
            {
                AxisInputCommand inputCommand = m_AxisInputCommands[name];
                inputCommand.PositiveKeyCode = positiveKeyCode;
                inputCommand.NegativeKeyCode = negativeKeyCode;
                return;
            }

            m_AxisInputCommands.Add(name, new AxisInputCommand(0, positiveKeyCode, negativeKeyCode, ControllerAxisCode.None, ControllerButtonCode.None, ControllerButtonCode.None));
        }

        public void BindAxis(string name, int controllerIndex, ControllerButtonCode positiveButtonCode, ControllerButtonCode negativeButtonCode)
        {
            if (m_AxisInputCommands.ContainsKey(name))
            {
                AxisInputCommand inputCommand = m_AxisInputCommands[name];
                inputCommand.ControllerIndex = controllerIndex;
                inputCommand.PositiveButtonCode = positiveButtonCode;
                inputCommand.NegativeButtonCode = negativeButtonCode;
                return;
            }

            m_AxisInputCommands.Add(name, new AxisInputCommand(controllerIndex, KeyCode.None, KeyCode.None, ControllerAxisCode.None, positiveButtonCode, negativeButtonCode));
        }

        public void BindAxis(string name, int controllerIndex, ControllerAxisCode axisCode)
        {
            if (m_AxisInputCommands.ContainsKey(name))
            {
                AxisInputCommand inputCommand = m_AxisInputCommands[name];
                inputCommand.ControllerIndex = controllerIndex;
                inputCommand.AxisCode = axisCode;
                return;
            }

            m_AxisInputCommands.Add(name, new AxisInputCommand(controllerIndex, KeyCode.None, KeyCode.None, axisCode, ControllerButtonCode.None, ControllerButtonCode.None));
        }

        public void UnbindButton(string name)
        {
            if (m_ButtonInputCommands.ContainsKey(name))
            {
                m_ButtonInputCommands.Remove(name);
            }
        }

        public void UnbindAxis(string name)
        {
            if (m_AxisInputCommands.ContainsKey(name))
            {
                m_AxisInputCommands.Remove(name);
            }
        }


        public bool GetButton(string name)
        {
            if (m_ButtonInputCommands.ContainsKey(name))
            {
                ButtonInputCommand inputCommand = m_ButtonInputCommands[name];

                //Check for keyboard input first (only return on success)
                bool result = false;
                if (inputCommand.KeyCode != KeyCode.None)
                {
                    switch (inputCommand.ButtonState)
                    {
                        case ButtonState.OnPress:
                            result = Input.GetKeyDown(inputCommand.KeyCode);
                            break;

                        case ButtonState.OnRelease:
                            result = Input.GetKeyUp(inputCommand.KeyCode);
                            break;

                        case ButtonState.Pressed:
                            result = Input.GetKey(inputCommand.KeyCode);
                            break;

                        case ButtonState.Released:
                            result = (!Input.GetKey(inputCommand.KeyCode));
                            break;

                        default:
                            result = false;
                            break;
                    }

                    if (result)
                        return true;
                }

                //Check for controller, always return regardless of success
                if (inputCommand.ButtonCode != ControllerButtonCode.None)
                {
                    switch (inputCommand.ButtonState)
                    {
                        case ButtonState.OnPress:   return ControllerInput.GetButtonDown(inputCommand.ControllerIndex, inputCommand.ButtonCode);
                        case ButtonState.OnRelease: return ControllerInput.GetButtonUp(inputCommand.ControllerIndex, inputCommand.ButtonCode);
                        case ButtonState.Pressed:   return ControllerInput.GetButton(inputCommand.ControllerIndex, inputCommand.ButtonCode);
                        case ButtonState.Released:  return (!ControllerInput.GetButton(inputCommand.ControllerIndex, inputCommand.ButtonCode));

                        default:
                            break;
                    }
                }

                return false;
            }

            Debug.Log("No button with name: " + name + " was found!");
            return false;
        }

        public float GetAxis(string name)
        {
            if (m_AxisInputCommands.ContainsKey(name))
            {
                AxisInputCommand inputCommand = m_AxisInputCommands[name];

                float value = 0.0f;

                //Check buttons
                if (inputCommand.PositiveKeyCode != KeyCode.None && inputCommand.NegativeKeyCode != KeyCode.None)
                {
                    if (Input.GetKey(inputCommand.PositiveKeyCode)) value += 1.0f;
                    if (Input.GetKey(inputCommand.NegativeKeyCode)) value -= 1.0f;

                    if (value != 0.0f)
                        return value;
                }

                if (inputCommand.PositiveButtonCode != ControllerButtonCode.None && inputCommand.NegativeButtonCode != ControllerButtonCode.None)
                {
                    if (ControllerInput.GetButton(inputCommand.ControllerIndex, inputCommand.PositiveButtonCode)) value += 1.0f;
                    if (ControllerInput.GetButton(inputCommand.ControllerIndex, inputCommand.NegativeButtonCode)) value -= 1.0f;

                    if (value != 0.0f)
                        return value;
                }

                //Check controller axis
                if (inputCommand.AxisCode != ControllerAxisCode.None)
                {
                    return ControllerInput.GetAxis(inputCommand.ControllerIndex, inputCommand.AxisCode);
                }

                return 0.0f;
            }

            Debug.Log("No axis with name: " + name + " was found!");
            return 0.0f;
        }


        public bool IsUsed(KeyCode keyCode)
        {
            foreach (KeyValuePair<string, ButtonInputCommand> valuePair in m_ButtonInputCommands)
            {
                if (valuePair.Value.KeyCode == keyCode)
                    return true;
            }

            return false;
        }

        public bool IsUsed(ControllerButtonCode buttonCode)
        {
            foreach (KeyValuePair<string, ButtonInputCommand> valuePair in m_ButtonInputCommands)
            {
                if (valuePair.Value.ButtonCode == buttonCode)
                    return true;
            }

            return false;
        }
    }
}