using System;
using System.Windows.Forms;
using System.Drawing;
using SharpDX.XInput;
using System.Threading;

namespace JoyStickControllor
{
    /* Main window form */
    public partial class form_main_config : Form
    {
        /* Initialize */
        public form_main_config()
        {
            InitializeComponent();
        }

        /* Create X-input device controller when GUI load */
        private void form_main_config_Load(object sender, EventArgs e)
        {
            try
            {
                /* Create X-input instance */
                global_info.Xinput_device = new Controller(UserIndex.One);
                /* Make message transmitter between UI thread and control thread */
                global_info.SyncContext = SynchronizationContext.Current;
                /* Create background control thread */
                global_info.ControlThread = new Thread(control_thread);
                global_info.ControlThread.IsBackground = true;
                /* Enable all buttons in the main window form */
                all_button_enable();
                /* Background control thread start */
                global_info.ControlThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("       " + "Load device information error!", "ERROR");
            }
        }

        /* Release all of the resource when GUI close */
        private void form_main_config_FormClosing(object sender, FormClosingEventArgs e)
        {
            /* Background thread exit */
            global_info.BackgroundExists = false;
            while ( ( ThreadState.Stopped != global_info.ControlThread.ThreadState) 
                    && (ThreadState.Aborted != global_info.ControlThread.ThreadState))  
            {
                Thread.Sleep(100);  
            }
            /* Release resource */
            global_info.Xinput_device = null;
            global_info.SyncContext = null;
            global_info.ControlThread = null;
        }

        /* Recover the main window form when double click the notify icon  */
        private void notifyicon_DoubleClick(object sender, EventArgs e)
        {
            /* Check if the window size is Minimize */
            if (FormWindowState.Minimized == WindowState)
            {
                /* Recover the original window */
                WindowState = FormWindowState.Normal;
                /* Active the window */
                this.Activate();
                /* Show icon in the toolbar */
                this.ShowInTaskbar = true;
                /* Disable the notify icon */
                notifyicon.Visible = false;
            }
        }

        /* Hide when minimized */
        private void form_main_config_SizeChanged(object sender, EventArgs e)
        {
            /* Check if the window size is Minimize */
            if (FormWindowState.Minimized == WindowState)
            {
                /* Hide the icon in toolbar */
                this.ShowInTaskbar = false;
                /* Enable the notify icon */
                notifyicon.Visible = true;
            }
        }

        /* Indicate user motion in UI when receive device status refresh message from background control thread */
        private void JoyStickSafePost(object sender)
        {
            /* Get current X-input device status */
            State current_state = (State)sender;
            /* Indicate user motion in UI */
            control_indicate(current_state);
        }

        /* Background control thread */
        private void control_thread()
        {
            /* Polling device status (50Hz) and send message to UI thread when status changed */
            while (global_info.BackgroundExists)
            {
                try
                {
                    /* Delay 20 millisecond */
                    Thread.Sleep(20);
                    /* Polling status data */
                    State current_state = global_info.Xinput_device.GetState();
                    if (current_state.Equals(global_info.Xinput_state))
                    {
                        /* No status change , do nothing */
                        continue;
                    }
                    if (global_info.Form_actived)
                    {
                        /* Send message to UI thread to refresh UI */
                        object msg = (object)current_state;
                        global_info.SyncContext.Post(JoyStickSafePost, msg);
                    }
                    else
                    {
                        /* Do the action that wanted to be done when X-input device is operated */
                        control_action(current_state);
                    }
                    /* Refresh the status data */
                    global_info.Xinput_state = current_state;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    MessageBox.Show("       " + "Get device status error!", "ERROR");
                }
            }
        }

        /* Set flag when main window form acquire the focus */
        private void form_main_config_Activated(object sender, EventArgs e)
        {
            global_info.Form_actived = true;
        }

        /* Clear flag when main window form lose the focus */
        private void form_main_config_Deactivate(object sender, EventArgs e)
        {
            global_info.Form_actived = false;
        }

        /* Indicate user motion of X-input device in UI */
        private void control_indicate(State device_state)
        {
            /* button X */
            button_indicate((device_state.Gamepad.Buttons & GamepadButtonFlags.X), button_X);
            /* button Y */
            button_indicate((device_state.Gamepad.Buttons & GamepadButtonFlags.Y), button_Y);
            /* button A */
            button_indicate((device_state.Gamepad.Buttons & GamepadButtonFlags.A), button_A);
            /* button B */
            button_indicate((device_state.Gamepad.Buttons & GamepadButtonFlags.B), button_B);
            /* button stick up */
            button_indicate((device_state.Gamepad.Buttons & GamepadButtonFlags.DPadUp), button_Stick_up);
            /* button stick left */
            button_indicate((device_state.Gamepad.Buttons & GamepadButtonFlags.DPadLeft), button_Stick_left);
            /* button stick right */
            button_indicate((device_state.Gamepad.Buttons & GamepadButtonFlags.DPadRight), button_Stick_right);
            /* button stick down */
            button_indicate((device_state.Gamepad.Buttons & GamepadButtonFlags.DPadDown), button_Stick_down);
            /* button select */
            button_indicate((device_state.Gamepad.Buttons & GamepadButtonFlags.Back), button_Select);
            /* button start */
            button_indicate((device_state.Gamepad.Buttons & GamepadButtonFlags.Start), button_Start);
            /* button LB */
            button_indicate((device_state.Gamepad.Buttons & GamepadButtonFlags.LeftShoulder), button_LB);
            /* button RB */
            button_indicate((device_state.Gamepad.Buttons & GamepadButtonFlags.RightShoulder), button_RB);
            /* button L3 */
            button_indicate((device_state.Gamepad.Buttons & GamepadButtonFlags.LeftThumb), button_L3);
            /* button R3 */
            button_indicate((device_state.Gamepad.Buttons & GamepadButtonFlags.RightThumb), button_R3);
            /* LT */
            trigger_indicate(device_state.Gamepad.LeftTrigger, button_LT);
            /* RT */
            trigger_indicate(device_state.Gamepad.RightTrigger, button_RT);
            /* L_Stick X */
            thumb_indicate((int)device_state.Gamepad.LeftThumbX, global_info.LeftThumb_deadzone, button_L_Pov_right, button_L_Pov_left);
            /* L_Stick Y */
            thumb_indicate((int)device_state.Gamepad.LeftThumbY, global_info.LeftThumb_deadzone, button_L_Pov_up, button_L_Pov_down);
            /* R_Stick X */
            thumb_indicate((int)device_state.Gamepad.RightThumbX, global_info.RightThumb_deadzone, button_R_Pov_right, button_R_Pov_left);
            /* R_Stick Y */
            thumb_indicate((int)device_state.Gamepad.RightThumbY, global_info.RightThumb_deadzone, button_R_Pov_up, button_R_Pov_down);
        }
        
        /* Highlight/recover the appropriate button in GUI when control the X-input device */
        private void button_indicate(GamepadButtonFlags status, Button gui_button)
        {
            if (0 < status)
            {
                gui_button.BackColor = SystemColors.Highlight;
            }
            else
            {
                gui_button.BackColor = SystemColors.Control;
                gui_button.UseVisualStyleBackColor = true;
            }
        }
        
        /* Highlight/recover the appropriate button in GUI when control the X-input device */
        private void trigger_indicate(byte status, Button gui_button)
        {
            if (100 < status)
            {
                gui_button.BackColor = SystemColors.Highlight;
            }
            else
            {
                gui_button.BackColor = SystemColors.Control;
                gui_button.UseVisualStyleBackColor = true;
            }
        }
        
        /* Highlight/recover the appropriate button in GUI when control the X-input device */
        private void thumb_indicate(int status, int deadzone, Button gui_button_negative, Button gui_button_positive)
        {
            if (deadzone < Math.Abs(status))
            {
                if (0 > status)
                {
                    gui_button_positive.BackColor = SystemColors.Highlight;
                    gui_button_negative.BackColor = SystemColors.Control;
                    gui_button_negative.UseVisualStyleBackColor = true;
                }
                else
                {
                    gui_button_negative.BackColor = SystemColors.Highlight;
                    gui_button_positive.BackColor = SystemColors.Control;
                    gui_button_positive.UseVisualStyleBackColor = true;
                }
            }
            else
            {
                gui_button_negative.BackColor = SystemColors.Control;
                gui_button_negative.UseVisualStyleBackColor = true;
                gui_button_positive.BackColor = SystemColors.Control;
                gui_button_positive.UseVisualStyleBackColor = true;
            }
        }

        /* Enable all of the buttons in GUI */
        private void all_button_enable()
        {
            button_load.Enabled = true;
            button_LT.Enabled = true;
            button_RT.Enabled = true;
            button_LB.Enabled = true;
            button_RB.Enabled = true;
            button_Select.Enabled = true;
            button_Start.Enabled = true;
            button_L_Pov_up.Enabled = true;
            button_L_Pov_left.Enabled = true;
            button_L_Pov_right.Enabled = true;
            button_L_Pov_down.Enabled = true;
            button_L3.Enabled = true;
            button_R_Pov_up.Enabled = true;
            button_R_Pov_left.Enabled = true;
            button_R_Pov_right.Enabled = true;
            button_R_Pov_down.Enabled = true;
            button_R3.Enabled = true;
            button_Stick_up.Enabled = true;
            button_Stick_left.Enabled = true;
            button_Stick_right.Enabled = true;
            button_Stick_down.Enabled = true;
            button_X.Enabled = true;
            button_Y.Enabled = true;
            button_A.Enabled = true;
            button_B.Enabled = true;
        }
        
        /* Action when X-input device is operated */
        private void control_action(State device_state)
        {
            /* Do your own action */
        }
    }
}
