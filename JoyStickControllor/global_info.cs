using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpDX.XInput;
using System.Threading;

namespace JoyStickControllor
{
    class global_info
    {
        /* Used as a positive and negative value to filter the left thumbstick input */
        private static int                      _LeftThumb_deadzone     =   7849;
        /* Used as a positive and negative value to filter the right thumbstick input */
        private static int                      _RightThumb_deadzone    =   8689;
        /* Flag used to check if main window form acquire the focus */
        private static bool                     _Form_actived           =   true;
        /* X-input device instance */
        private static Controller               _Xinput_device          =   null;
        /* Message transmitter between UI thread and control thread */
        private static SynchronizationContext   _SyncContext            =   null;
        /* Background control thread */
        private static Thread                   _ControlThread          =   null;
        /* Background thread exit flag */
        private static bool                     _BackgroundExists       =   true;
        /* X-input device status */
        private static State                    _Xinput_state;
        
        public static int LeftThumb_deadzone
        {
            get {   return  _LeftThumb_deadzone;    }
        }
        
        public static int RightThumb_deadzone
        {
            get {   return  _RightThumb_deadzone;   }
        }
        
        public static bool Form_actived
        {
            get {   return  _Form_actived;          }
            set {   _Form_actived = value;          }
        }
        
        public static Controller Xinput_device
        {
            get {   return  _Xinput_device;         }
            set {   _Xinput_device = value;         }
        }
        
        public static SynchronizationContext SyncContext
        {
            get {   return  _SyncContext;           }
            set {   _SyncContext = value;           }
        }
        
        public static Thread ControlThread
        {
            get {   return  _ControlThread;         }
            set {   _ControlThread = value;         }
        }
        
        public static bool BackgroundExists
        {
            get {   return  _BackgroundExists;      }
            set {   _BackgroundExists = value;      }
        }
        
        public static State Xinput_state
        {
            get {   return  _Xinput_state;          }
            set {   _Xinput_state = value;          }
        }
        
    }
}
