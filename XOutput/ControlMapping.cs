using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput
{
    public struct InputControl
    {
        public ControlTypeEnum Type { get; set; }
        public ControlSubTypeEnum SubType { get; set; }
        public byte InputIndex { get; set; }

        public override string ToString()
        {
            switch (this.Type)
            {
                case ControlTypeEnum.Disabled:
                    return "Disabled";
                case ControlTypeEnum.Axis:
                    return $"{this.SubType.ToShortString()}Axis {((ControlAxesEnum)this.InputIndex)}";
                case ControlTypeEnum.Button:
                    return $"Button {this.InputIndex + 1}";
                case ControlTypeEnum.Hat:
                    return $"D-Pad {this.InputIndex + 1} {this.SubType.ToShortString()}";
                default:
                    throw new ArgumentException($"Nonexistent {nameof(this.Type)}.", nameof(this.Type));
            }
        }
    }

    public struct ControlMapping
    {
        public ControlMapping(ControlTypeEnum type = ControlTypeEnum.Disabled, ControlSubTypeEnum subType = ControlSubTypeEnum.Normal, byte inputIndex = 0, byte outputIndex = 0)
        {
            InputControl = new InputControl() { Type = type, SubType = subType, InputIndex = inputIndex };
            OutputIndex = outputIndex;
        }
        public InputControl InputControl;
        public byte OutputIndex;

        public override string ToString()
        {
            return InputControl.ToString();
        }
    }

    public enum ControlTypeEnum : byte
    {
        Disabled = 0,
        Button,
        Axis,
        Hat
    }

    public enum ControlSubTypeEnum : byte
    {
        Normal = 0,
        Inverted,
        Half,
        InvertedHalf,
        HatUp,
        HatDown,
        HatLeft,
        HatRight
    }

    public static class EnumExtensionMethods
    {
        public static string ToShortString(this ControlSubTypeEnum subType)
        {
            switch (subType)
            {
                case ControlSubTypeEnum.Normal:
                    return "";
                case ControlSubTypeEnum.Inverted:
                    return "I";
                case ControlSubTypeEnum.Half:
                    return "H";
                case ControlSubTypeEnum.InvertedHalf:
                    return "IH";
                case ControlSubTypeEnum.HatUp:
                    return "Up";
                case ControlSubTypeEnum.HatDown:
                    return "Down";
                case ControlSubTypeEnum.HatLeft:
                    return "Left";
                case ControlSubTypeEnum.HatRight:
                    return "Right";
                default:
                    throw new ArgumentException("Nonexistent subtype.", nameof(subType));
            }
        }
    }

    public enum ControlAxesEnum : byte
    {
        X = 0,
        Y,
        Z,
        RotationX,
        RotationY,
        RotationZ
    };
}
