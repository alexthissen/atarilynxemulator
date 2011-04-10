﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KillerApps.Emulation.Core;
using KillerApps.Emulation.Processors;
using System.Diagnostics;
using System.IO;

namespace KillerApps.Emulation.Atari.Lynx
{
	public class LynxHandheld: ILynxDevice
	{
		public RomCart Cartridge { get; set; }
		public Ram64KBMemory Ram { get; private set; }
		public RomBootMemory Rom { get; private set; }
		internal MemoryManagementUnit Mmu { get; private set; }
		public Mikey Mikey { get; private set; }
		public Suzy Suzy { get; private set; }
		public Cmos65SC02 Cpu { get; private set; }
		public Clock SystemClock { get; private set; }
		public byte[] LcdScreenDma;

		public Stream BootRomImage { get; set; }
		public Stream CartRomImage { get; set; }

		public bool CartridgePowerOn { get; set; }
		public ulong NextTimerEvent { get; set; }

		private static TraceSwitch GeneralSwitch = new TraceSwitch("General", "General trace switch", "Error");

		public void Initialize()
		{
			Ram = new Ram64KBMemory();
			Rom = new RomBootMemory();
			Rom.LoadBootImage(BootRomImage);
			
			Mikey = new Mikey(this);
			Suzy = new Suzy(this);
			Suzy.Initialize();

			// Pass all hardware that have memory access to MMU
			Mmu = new MemoryManagementUnit(Rom, Ram, Mikey, Suzy);
			SystemClock = new Clock();
			LcdScreenDma = new byte[0x3FC0 * 4];

			// Finally construct processor
			Cpu = new Cmos65SC02(Mmu, SystemClock);

			Mikey.Initialize();

			Reset();
		}

		public void Reset()
		{
			Mikey.Reset();
			Suzy.Reset();
			Mmu.Reset();
			Cpu.Reset();
		}

		public void Update(ulong cyclesToExecute)
		{
			ulong executedCycles = 0;

			while (cyclesToExecute > executedCycles)
			{
				GenerateInterrupts();
				executedCycles += ExecuteCpu();
				SynchronizeTime();
			}
		}

		private ulong ExecuteCpu()
		{
			ulong executedCycles = Cpu.Execute(1);
			if (Cpu.IsAsleep) SystemClock.CompatibleCycleCount = NextTimerEvent;
			return executedCycles;
		}

		private void GenerateInterrupts() 
		{
			// Mikey is only source of interrupts. It contains all timers (regular, audio and UART)
			if (SystemClock.CompatibleCycleCount >= NextTimerEvent) 
				Mikey.Update();
			Debug.WriteLineIf(GeneralSwitch.TraceVerbose, "LynxHandheld::GenerateInterrupts");
		}

		private void SynchronizeTime()
		{
			Debug.WriteLineIf(GeneralSwitch.TraceVerbose, String.Format("LynxHandheld::SynchronizeTime: Current time is {0}", SystemClock.CompatibleCycleCount));
		}

		
		public void UpdateJoystickState(JoyStickStates state)
		{

		}
	}
}
