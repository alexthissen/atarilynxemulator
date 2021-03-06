using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KillerApps.Emulation.AtariLynx;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace KillerApps.Gaming.MonoGame
{
	public class InputHandler : DrawableGameComponent
	{
		protected JoystickStates joystick;

		public InputHandler(Game game): base (game) { }

		public JoystickStates Joystick
		{
			get
			{
				JoystickStates result = JoystickStates.None;
				result = BuildJoystickState();
				return result;
			}
		}

		public virtual bool ExitGame { get { return false; } }

		protected virtual JoystickStates BuildJoystickState()
		{
			return JoystickStates.None;
		}
	}
}
