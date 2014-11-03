﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KillerApps.Emulation.Core
{
	public interface IMemoryAccess<TAddress, TData>
	{
		void Poke(TAddress address, TData value);
		TData Peek(TAddress address);
	}
}
