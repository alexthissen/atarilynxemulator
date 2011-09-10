﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace KillerApps.Emulation.Atari.Lynx
{
	public class SpriteCollisionNumber
	{
		// "The other 3 are currently ignored by the hardware, but ought to be set to '0' for future compatibility."
		// "B7,B6 = 0"
		// "B4 = 0"
		public byte ByteData 
		{
			get { return (byte)(byteData & 0x2F); }
			set
			{ 
				byteData = value;
				byteData &= 0x2F; // B7, B6, B4 are set to zero
			}
		}

		public SpriteCollisionNumber(byte sprcoll)
		{
			this.ByteData = sprcoll;
		}

		// "One of the upper 4 bits is used by the hardware to disable collision activity for this sprite."
		// "B5 = dont collide. 1=dont collide with this sprite."
		public bool DontCollide 
		{
			get { return (ByteData & DontCollideMask) == DontCollideMask; }
			set { if (value) ByteData |= DontCollideMask; else ByteData &= (DontCollideMask ^ 0xFF); }
		}

		// "The software must assign this collision number for each use of each sprite."
		// "B3,B2,B1,B0 = number"
		public byte Number 
		{ 
			get { return (byte)(ByteData & 0x0F); }
			set 
			{
				//Debug.WriteLineIf(value > 0x0F, "Collision number should be 0 to 15.");
				ByteData &= DontCollideMask; // Zero out previous collision number
				ByteData |= (byte)(value & 0x0F);
			}
		}

		private const byte DontCollideMask = 0x20;
		private byte byteData;
	}
}
