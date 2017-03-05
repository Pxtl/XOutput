using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace XOutput
{
    public partial class ControllerOptions : Form
    {
        ControllerDevice dev;
        public ControllerOptions(ControllerDevice device)
        {
            InitializeComponent();
            dev = device;
            int outputIndex = 0;

            foreach (MultiLevelComboBox m in this.Controls.OfType<MultiLevelComboBox>()) {
                m.Items[0] = getBindingText(outputIndex); //Change combobox text according to saved binding
                m.addOption("Disabled",
                    tag: new ControlMapping(ControlTypeEnum.Disabled)
                );
                ToolStripMenuItem axes = m.addMenu("Axes");
                ToolStripMenuItem buttons = m.addMenu("Buttons");
                ToolStripMenuItem dpads = m.addMenu("D-Pads");
                ToolStripMenuItem iaxes = m.addMenu("Inverted Axes", axes);
                ToolStripMenuItem haxes = m.addMenu("Half Axes", axes);
                ToolStripMenuItem ihaxes = m.addMenu("Inverted Half Axes", axes);
                for (int i = 1; i <= dev.joystick.Capabilities.ButtonCount; i++)
                {
                    m.addOption(null, buttons,
                        new ControlMapping(ControlTypeEnum.Button, ControlSubTypeEnum.Normal, (byte)(i - 1), (byte)outputIndex)
                    );
                }
                for (int i = 1; i <= dev.joystick.Capabilities.PovCount; i += 1)
                {
                    m.addOption(null, dpads,
                        new ControlMapping(ControlTypeEnum.Hat, ControlSubTypeEnum.HatUp, (byte)(i - 1), (byte)outputIndex)
                    );
                    m.addOption(null, dpads,
                        new ControlMapping(ControlTypeEnum.Hat, ControlSubTypeEnum.HatDown, (byte)(i - 1), (byte)outputIndex)
                    );
                    m.addOption(null, dpads,
                        new ControlMapping(ControlTypeEnum.Hat, ControlSubTypeEnum.HatLeft, (byte)(i - 1), (byte)outputIndex)
                    );
                    m.addOption(null, dpads,
                        new ControlMapping(ControlTypeEnum.Hat, ControlSubTypeEnum.HatRight, (byte)(i - 1), (byte)outputIndex)
                    );
                }
                for (int i = 0; i <= dev.analogs.Length - 1; i += 1)
                {
                    if (dev.analogs[i] != 0)
                    {
                        int labelNum = i + 1;
                        m.addOption(null, axes,
                            new ControlMapping(ControlTypeEnum.Axis, ControlSubTypeEnum.Normal, (byte)(i), (byte)outputIndex)
                        );
                        m.addOption(null, iaxes,
                            new ControlMapping(ControlTypeEnum.Axis, ControlSubTypeEnum.Inverted, (byte)(i), (byte)outputIndex)
                        );
                        m.addOption(null, haxes,
                            new ControlMapping(ControlTypeEnum.Axis, ControlSubTypeEnum.Half, (byte)(i), (byte)outputIndex)
                        );
                        m.addOption(null, ihaxes,
                            new ControlMapping(ControlTypeEnum.Axis, ControlSubTypeEnum.InvertedHalf, (byte)(i), (byte)outputIndex)
                        );
                    }
                }
                m.SelectionChangeCommitted += new System.EventHandler(SelectionChanged);
                outputIndex +=1;
            }
        }

        private string getBindingText(InputControl mapping)
        {
            return mapping.ToString();
        }

        private string getBindingText(int i)
        {
            return getBindingText(dev.controlMappings[i]);
        }

        private void SelectionChanged(object sender, EventArgs e) {
            ToolStripMenuItem i = (ToolStripMenuItem)sender;
            var mapping = (ControlMapping)i.Tag;
            dev.controlMappings[mapping.OutputIndex] = mapping.InputControl;
            dev.Save();
        }

        private void onClose(object sender, EventArgs e) {
            dev.Save();
        }

    }
}
