using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using PokeNPCS.NPCs;
using PokeNPCS.Events;

namespace PokeNPCS
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class PokeNPCS : Mod
	{

		public override void Load()
		{
			NurseJoy.HeadIndex = AddNPCHeadTexture(ModContent.NPCType<NurseJoy>(), "PokeNPCS/Sprites/NPCs/NurseJoy_Head");
			Giovanni.HeadIndex = AddNPCHeadTexture(ModContent.NPCType<Giovanni>(), "PokeNPCS/Sprites/NPCs/Giovanni_Head");
			Spyral.HeadIndex = AddNPCHeadTexture(ModContent.NPCType<Spyral>(), "PokeNPCS/Sprites/NPCs/Spyral_Head");
			Lillie.HeadIndex = AddNPCHeadTexture(ModContent.NPCType<Lillie>(), "PokeNPCS/Sprites/NPCs/Lillie_Head");
		}

	}
}