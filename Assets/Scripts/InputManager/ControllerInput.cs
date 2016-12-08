using UnityEngine;
using XInputDotNetPure;

namespace XBOXParty
{
    public enum ControllerAxisCode
    {
        None = 0,
        LeftStickX = 1,
        LeftStickY = 2,
        RightStickX = 3,
        RightStickY = 4,
        LeftTrigger = 5,
        RightTrigger = 6
    }

    public enum ControllerButtonCode
    {
        None = 0,
        A = 1,
        B = 2,
        X = 3,
        Y = 4,
        LeftShoulder = 5,
        RightShoulder = 6,
        LeftStick = 7,
        RightStick = 8,
        Start = 9,
        Back = 10,
        Guide = 11,
        Up = 12,
        Down = 13,
        Left = 14,
        Right = 15
    }

    //Chose a static class to mimic the unity Input class.
    public sealed class ControllerInput
    {
        private static int NUMBER_OF_CONTROLLERS = 4; //This is ALWAYS the case, XInput only supports 4 controllers
        private static bool m_IsInitialized = false;

        private static XboxController[] m_XboxControllers = new XboxController[4];

        private static void Initialize()
        {
            for (int i = 0; i < NUMBER_OF_CONTROLLERS; ++i)
            {
                m_XboxControllers[i] = new XboxController();
                m_XboxControllers[i].ControllerIndex = (PlayerIndex)i;

                m_IsInitialized = true;
            }

            UpdateState();
        }

        public static void DeInitialize()
        {
            //If rumble is on when we close the application, it doesn't stop rumbling.
            for (int i = 0; i < NUMBER_OF_CONTROLLERS; ++i)
            {
                m_XboxControllers[i] = null;
            }
        }

        public static void UpdateState()
        {
            if (!m_IsInitialized)
                Initialize();

            for (int i = 0; i < NUMBER_OF_CONTROLLERS; ++i)
            {
                m_XboxControllers[i].UpdateState();
            }
        }

        public static float GetAxis(int controllerIndex, ControllerAxisCode axisCode)
        {
            if (!m_IsInitialized)
                Initialize();

            return m_XboxControllers[controllerIndex].GetAxis(axisCode);
        }

        public static bool GetButton(int controllerIndex, ControllerButtonCode keyCode)
        {
            if (!m_IsInitialized)
                Initialize();

            return m_XboxControllers[controllerIndex].GetButton(keyCode);
        }

        public static bool GetButtonDown(int controllerIndex, ControllerButtonCode keyCode)
        {
            if (!m_IsInitialized)
                Initialize();

            return m_XboxControllers[controllerIndex].GetButtonDown(keyCode);
        }

        public static bool GetButtonUp(int controllerIndex, ControllerButtonCode keyCode)
        {
            if (!m_IsInitialized)

                Initialize();
            return m_XboxControllers[controllerIndex].GetButtonUp(keyCode);
        }

        public static void SetVibration(int controllerIndex, float leftValue, float rightvalue, float time)
        {
            m_XboxControllers[controllerIndex].SetVibration(leftValue, rightvalue, time);
        }

        public static bool IsConnected(int controllerIndex)
        {
            return m_XboxControllers[controllerIndex].IsConnected();
        }
    }

    public class XboxController
    {
        private PlayerIndex m_ControllerIndex;
        public PlayerIndex ControllerIndex
        {
            get { return m_ControllerIndex; }
            set { m_ControllerIndex = value; }
        }

        private GamePadState m_CurrentState;
        public GamePadState CurrentState
        {
            get { return m_CurrentState; }
        }

        private GamePadState m_PreviousState;
        public GamePadState PreviousState
        {
            get { return m_PreviousState; }
        }

        private float m_RumbleTimeLeft = 0.0f;

        public void UpdateState()
        {
            m_PreviousState = m_CurrentState;
            m_CurrentState = GamePad.GetState(m_ControllerIndex);

            UpdateRumble();
        }

        private void UpdateRumble()
        {
            if (m_RumbleTimeLeft > 0.0f)
            {
                m_RumbleTimeLeft -= Time.deltaTime;
            }

            if (m_RumbleTimeLeft < 0.0f)
            {
                m_RumbleTimeLeft = 0.0f;
                SetVibration(0.0f, 0.0f, 0.0f);
            }
        }

        public float GetAxis(ControllerAxisCode axisCode)
        {
            switch (axisCode)
            {
                case ControllerAxisCode.LeftStickX:
                    return m_CurrentState.ThumbSticks.Left.X;

                case ControllerAxisCode.LeftStickY:
                    return m_CurrentState.ThumbSticks.Left.Y;

                case ControllerAxisCode.RightStickX:
                    return m_CurrentState.ThumbSticks.Right.X;

                case ControllerAxisCode.RightStickY:
                    return m_CurrentState.ThumbSticks.Right.Y;

                case ControllerAxisCode.LeftTrigger:
                    return m_CurrentState.Triggers.Left;

                case ControllerAxisCode.RightTrigger:
                    return m_CurrentState.Triggers.Right;

                default:
                    return 0.0f;
            }
        }

        public bool GetButton(ControllerButtonCode keyCode)
        {
            switch (keyCode)
            {
                case ControllerButtonCode.A:
                    return (m_CurrentState.Buttons.A == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.B:
                    return (m_CurrentState.Buttons.B == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.X:
                    return (m_CurrentState.Buttons.X == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Y:
                    return (m_CurrentState.Buttons.Y == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.LeftShoulder:
                    return (m_CurrentState.Buttons.LeftShoulder == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.RightShoulder:
                    return (m_CurrentState.Buttons.RightShoulder == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.LeftStick:
                    return (m_CurrentState.Buttons.LeftStick == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.RightStick:
                    return (m_CurrentState.Buttons.RightStick == XInputDotNetPure.ButtonState.Pressed);

                //Central buttons
                case ControllerButtonCode.Start:
                    return (m_CurrentState.Buttons.Start == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Back:
                    return (m_CurrentState.Buttons.Back == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Guide:
                    return (m_CurrentState.Buttons.Guide == XInputDotNetPure.ButtonState.Pressed);

                //D pad
                case ControllerButtonCode.Up:
                    return (m_CurrentState.DPad.Up == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Down:
                    return (m_CurrentState.DPad.Down == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Left:
                    return (m_CurrentState.DPad.Left == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Right:
                    return (m_CurrentState.DPad.Right == XInputDotNetPure.ButtonState.Pressed);

                default:
                    return false;
            }
        }

        public bool GetButtonDown(ControllerButtonCode keyCode)
        {
            switch (keyCode)
            {
                case ControllerButtonCode.A:
                    return (m_CurrentState.Buttons.A == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.A != XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.B:
                    return (m_CurrentState.Buttons.B == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.B != XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.X:
                    return (m_CurrentState.Buttons.X == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.X != XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Y:
                    return (m_CurrentState.Buttons.Y == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.Y != XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.LeftShoulder:
                    return (m_CurrentState.Buttons.LeftShoulder == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.LeftShoulder != XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.RightShoulder:
                    return (m_CurrentState.Buttons.RightShoulder == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.RightShoulder != XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.LeftStick:
                    return (m_CurrentState.Buttons.LeftStick == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.LeftStick != XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.RightStick:
                    return (m_CurrentState.Buttons.RightStick == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.RightStick != XInputDotNetPure.ButtonState.Pressed);

                //Central buttons
                case ControllerButtonCode.Start:
                    return (m_CurrentState.Buttons.Start == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.Start != XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Back:
                    return (m_CurrentState.Buttons.Back == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.Back != XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Guide:
                    return (m_CurrentState.Buttons.Guide == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.Guide != XInputDotNetPure.ButtonState.Pressed);

                //D pad
                case ControllerButtonCode.Up:
                    return (m_CurrentState.DPad.Up == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.DPad.Up != XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Down:
                    return (m_CurrentState.DPad.Down == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.DPad.Down != XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Left:
                    return (m_CurrentState.DPad.Left == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.DPad.Left != XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Right:
                    return (m_CurrentState.DPad.Right == XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.DPad.Right != XInputDotNetPure.ButtonState.Pressed);

                default:
                    return false;
            }
        }

        public bool GetButtonUp(ControllerButtonCode keyCode)
        {
            switch (keyCode)
            {
                case ControllerButtonCode.A:
                    return (m_CurrentState.Buttons.A != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.A == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.B:
                    return (m_CurrentState.Buttons.B != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.B == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.X:
                    return (m_CurrentState.Buttons.X != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.X == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Y:
                    return (m_CurrentState.Buttons.Y != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.Y == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.LeftShoulder:
                    return (m_CurrentState.Buttons.LeftShoulder != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.LeftShoulder == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.RightShoulder:
                    return (m_CurrentState.Buttons.RightShoulder != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.RightShoulder == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.LeftStick:
                    return (m_CurrentState.Buttons.LeftStick != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.LeftStick == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.RightStick:
                    return (m_CurrentState.Buttons.RightStick != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.RightStick == XInputDotNetPure.ButtonState.Pressed);

                //Central buttons
                case ControllerButtonCode.Start:
                    return (m_CurrentState.Buttons.Start != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.Start == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Back:
                    return (m_CurrentState.Buttons.Back != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.Back == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Guide:
                    return (m_CurrentState.Buttons.Guide != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.Buttons.Guide == XInputDotNetPure.ButtonState.Pressed);

                //D pad
                case ControllerButtonCode.Up:
                    return (m_CurrentState.DPad.Up != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.DPad.Up == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Down:
                    return (m_CurrentState.DPad.Down != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.DPad.Down == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Left:
                    return (m_CurrentState.DPad.Left != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.DPad.Left == XInputDotNetPure.ButtonState.Pressed);

                case ControllerButtonCode.Right:
                    return (m_CurrentState.DPad.Right != XInputDotNetPure.ButtonState.Pressed &&
                            m_PreviousState.DPad.Right == XInputDotNetPure.ButtonState.Pressed);

                default:
                    return false;
            }
        }

        public void SetVibration(float leftValue, float rightValue, float time)
        {
            if (time < 0.0f)
                return;

            m_RumbleTimeLeft = time;
            GamePad.SetVibration(m_ControllerIndex, leftValue, rightValue);
        }

        public bool IsConnected()
        {
            return m_CurrentState.IsConnected;
        }
    }
}
