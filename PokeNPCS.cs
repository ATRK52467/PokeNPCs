using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using PokeNPCS.NPCs;
using PokeNPCS.Events;
using Terraria.Audio;

namespace PokeNPCS
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class PokeNPCS : Mod
	{

		public static SoundStyle ArticunoSpawnSound;
		public static int ArticunoMusicSlot;
		public override void Load()
		{
			NurseJoy.HeadIndex = AddNPCHeadTexture(ModContent.NPCType<NurseJoy>(), "PokeNPCS/Sprites/NPCs/NurseJoy_Head");
			Giovanni.HeadIndex = AddNPCHeadTexture(ModContent.NPCType<Giovanni>(), "PokeNPCS/Sprites/NPCs/Giovanni_Head");
			Spyral.HeadIndex = AddNPCHeadTexture(ModContent.NPCType<Spyral>(), "PokeNPCS/Sprites/NPCs/Spyral_Head");
			Lillie.HeadIndex = AddNPCHeadTexture(ModContent.NPCType<Lillie>(), "PokeNPCS/Sprites/NPCs/Lillie_Head");
			ArticunoSpawnSound = new SoundStyle($"PokeNPCS/Sounds/Bosses/ArticunoSpawn")
			{
				Volume = 1.0f,
				PitchVariance = 0.1f
			};
			ArticunoMusicSlot = MusicLoader.GetMusicSlot(this, "PokeNPCS/Sounds/Bosses/ArticunoTheme");
		}

	}
}