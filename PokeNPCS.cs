using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using PokeNPCS.NPCs;

namespace PokeNPCS
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class PokeNPCS : Mod
	{
		public override void Load()
		{
			NurseJoy.HeadIndex = AddNPCHeadTexture(ModContent.NPCType<NurseJoy>(), "PokeNPCS/Sprites/NPCs/NurseJoy_Head");
		}

	}
}