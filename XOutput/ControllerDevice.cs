﻿using System;
using SlimDX.DirectInput;

namespace XOutput
{
    public struct OutputState
    {
        public byte LX, LY, RX, RY, L2, R2;
        public bool A, B, X, Y, Start, Back, L1, R1, L3, R3, Home;
        public bool DpadUp, DpadRight, DpadDown, DpadLeft;
    }

    public class ControllerDevice
    {
        public const int XINPUT_CONTROL_COUNT = 21;
        public Joystick joystick;
        int deviceNumber;
        public string name;
        public bool enabled = true;

        public OutputState cOutput;
        public InputControl[] controlMappings = new InputControl[XINPUT_CONTROL_COUNT];
        bool[] buttons;
        int[] dPads;
        public int[] analogs;


        delegate byte input(ControlSubTypeEnum subType, byte num);

        public ControllerDevice(Joystick joy, int num)
        {
            joystick = joy;
            deviceNumber = num;
            name = joystick.Information.InstanceName;
            cOutput = new OutputState();

            //initialize values
            joystick.Poll();
            JoystickState jState = joystick.GetCurrentState();
            buttons = jState.GetButtons();
            dPads = jState.GetPointOfViewControllers();
            analogs = GetAxes(jState);
            
            //TODO: delete this code.  Initial value is already blank.
            //for (int i = 0; i < controlMappings.Length; i++)
            //{
            //    controlMappings[i] = 255; //Changed default mapping to blank
            //}

            var saveData = SaveManager.LoadDeviceControls(joy.Information.ProductName.ToString());
            if (saveData != null)
            {
                controlMappings = saveData;
            }
        }

        #region Utility Functions

        public void Save()
        {
            SaveManager.SaveDeviceControls(joystick.Information.ProductName, controlMappings);
        }

        private int[] GetAxes(JoystickState jstate)
        {
            return new int[] { jstate.X, jstate.Y, jstate.Z, jstate.RotationX, jstate.RotationY, jstate.RotationZ };
        }

        private unsafe byte toByte(bool n)
        {
            return *((byte*)(&n));
        }

        private bool[] getPov(byte n)
        {
            bool[] b = new bool[4];
            int i = dPads[n];
            switch (i)
            {
                case -1: b[0] = false; b[1] = false; b[2] = false; b[3] = false; break;
                case 0: b[0] = true; b[1] = false; b[2] = false; b[3] = false; break;
                case 4500: b[0] = true; b[1] = false; b[2] = false; b[3] = true; break;
                case 9000: b[0] = false; b[1] = false; b[2] = false; b[3] = true; break;
                case 13500: b[0] = false; b[1] = true; b[2] = false; b[3] = true; break;
                case 18000: b[0] = false; b[1] = true; b[2] = false; b[3] = false; break;
                case 22500: b[0] = false; b[1] = true; b[2] = true; b[3] = false; break;
                case 27000: b[0] = false; b[1] = false; b[2] = true; b[3] = false; break;
                case 31500: b[0] = true; b[1] = false; b[2] = true; b[3] = false; break;
            }
            return b;
        }

        public void changeNumber(int n)
        {
            deviceNumber = n;
        }

        #endregion

        #region Input Types

        byte Button(ControlSubTypeEnum subType, byte num)
        {
            int i = (int)toByte(buttons[num]) * 255;
            return (byte)i;
        }

        byte Analog(ControlSubTypeEnum subType, byte num)
        {
            int p = analogs[num];
            switch (subType)
            {
                case ControlSubTypeEnum.Normal:
                    return (byte)(p / 256);
                case ControlSubTypeEnum.Inverted:
                    return (byte)((65535 - p) / 256);
                case ControlSubTypeEnum.Half:
                    int m = (p - 32767) / 129;
                    if (m < 0)
                    {
                        m = 0;
                    }
                    return (byte)m;
                case ControlSubTypeEnum.InvertedHalf:
                    m = (p - 32767) / 129;
                    if (-m < 0)
                    {
                        m = 0;
                    }
                    return (byte)-m;
            }
            return 0;
        }

        byte DPad(ControlSubTypeEnum subType, byte num)
        {
            int i = (int)toByte(getPov(num)[(int)subType-4]) * 255;
            return (byte)i;
        }

        #endregion

        private void updateInput()
        {
            joystick.Poll();
            JoystickState jState = joystick.GetCurrentState();
            buttons = jState.GetButtons();
            dPads = jState.GetPointOfViewControllers();
            analogs = GetAxes(jState);

            input funcButton = Button;
            input funcAnalog = Analog;
            input funcDPad = DPad;
            input[] funcArray = new input[] { funcButton, funcAnalog, funcDPad };

            byte[] output = new byte[XINPUT_CONTROL_COUNT];
            for (int i = 0; i < XINPUT_CONTROL_COUNT; i++)
            {
                if (controlMappings[i].Type == ControlTypeEnum.Disabled)
                {
                    continue;
                }
                var type = controlMappings[i].Type;
                var subtype = controlMappings[i].SubType;
                var num = controlMappings[i].InputIndex;
                output[i] = funcArray[(byte)type - 1](subtype, num);
            }

            cOutput.A = output[0] != 0;
            cOutput.B = output[1] != 0;
            cOutput.X = output[2] != 0;
            cOutput.Y = output[3] != 0;

            cOutput.DpadUp = output[4] != 0;
            cOutput.DpadDown = output[5] != 0;
            cOutput.DpadLeft = output[6] != 0;
            cOutput.DpadRight = output[7] != 0;

            cOutput.L2 = output[9];
            cOutput.R2 = output[8];

            cOutput.L1 = output[10] != 0;
            cOutput.R1 = output[11] != 0;

            cOutput.L3 = output[12] != 0;
            cOutput.R3 = output[13] != 0;

            cOutput.Home = output[14] != 0;
            cOutput.Start = output[15] != 0;
            cOutput.Back = output[16] != 0;

            cOutput.LY = output[17];
            cOutput.LX = output[18];
            cOutput.RY = output[19];
            cOutput.RX = output[20];
            
        }


        public byte[] getoutput()
        {
            updateInput();
            byte[] Report = new byte[64];
            Report[1] = 0x02;
            Report[2] = 0x05;
            Report[3] = 0x12;

            Report[10] = (byte)(
                ((cOutput.Back ? 1 : 0) << 0) |
                ((cOutput.L3 ? 1 : 0) << 1) |
                ((cOutput.R3 ? 1 : 0) << 2) |
                ((cOutput.Start ? 1 : 0) << 3) |
                ((cOutput.DpadUp ? 1 : 0) << 4) |
                ((cOutput.DpadRight ? 1 : 0) << 5) |
                ((cOutput.DpadDown ? 1 : 0) << 6) |
                ((cOutput.DpadLeft ? 1 : 0) << 7));

            Report[11] = (byte)(
                ((cOutput.L1 ? 1 : 0) << 2) |
                ((cOutput.R1 ? 1 : 0) << 3) |
                ((cOutput.Y ? 1 : 0) << 4) |
                ((cOutput.B ? 1 : 0) << 5) |
                ((cOutput.A ? 1 : 0) << 6) |
                ((cOutput.X ? 1 : 0) << 7));

            //Guide
            Report[12] = (byte)(cOutput.Home ? 0xFF : 0x00);


            Report[14] = cOutput.LX; //Left Stick X


            Report[15] = cOutput.LY; //Left Stick Y


            Report[16] = cOutput.RX; //Right Stick X


            Report[17] = cOutput.RY; //Right Stick Y

            Report[26] = cOutput.R2;
            Report[27] = cOutput.L2;

            return Report;
        }


    }
}
