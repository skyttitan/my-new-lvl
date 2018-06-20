#define FREE_VERSION
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using CF.CameraBot.Parts;

namespace CF.CameraBot
{
	[Serializable]
    public class PositionOverrider
    {
		/// <summary>For developer view all addon in one panel.</summary>
		/// <see cref="PositionOverriderDrawer"/>
		public string m_AddonList;
		public int m_AddonCount;
#if FREE_VERSION
		public void Update(Preset preset) { }
#endif
	}
}