using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Avali
{
	// Token: 0x02000032 RID: 50
	[StaticConstructorOnStartup]
	public class CompHoloTV : CompGlower
	{
		// Token: 0x0600010E RID: 270 RVA: 0x00009C54 File Offset: 0x00007E54
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front1);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front2);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front3);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front4);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front5);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front6);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front7);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front8);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front9);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front10);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front11);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front12);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front13);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front14);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front15);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front16);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front17);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front18);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front19);
			CompHoloTV.Hologram_southSheet.Add(CompHoloTV.Hologram_front20);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back1);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back2);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back3);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back4);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back5);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back6);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back7);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back8);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back9);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back10);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back11);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back12);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back13);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back14);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back15);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back16);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back17);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back18);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back19);
			CompHoloTV.Hologram_northSheet.Add(CompHoloTV.Hologram_back20);
			this.compPowerTrader = this.parent.TryGetComp<CompPowerTrader>();
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00002990 File Offset: 0x00000B90
		public override void PostDraw()
		{
			base.PostDraw();
			if (this.powered)
			{
				this.DrawCurrentFrame();
			}
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00009ED4 File Offset: 0x000080D4
		public override void CompTick()
		{
			base.CompTick();
			if (this.parent.IsHashIntervalTick(8))
			{
				if (this.compPowerTrader != null && (this.compPowerTrader.PowerNet.CurrentStoredEnergy() <= 0f || this.parent.IsBrokenDown() || !FlickUtility.WantsToBeOn(this.parent)))
				{
					this.powered = false;
					return;
				}
				this.powered = true;
				if (this.currFrame >= 19)
				{
					this.currFrame = -1;
				}
				this.currFrame++;
			}
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00009F5C File Offset: 0x0000815C
		private void DrawCurrentFrame()
		{
			Matrix4x4 matrix = default(Matrix4x4);
			int asInt = this.parent.Rotation.AsInt;
			Vector3 pos = this.parent.TrueCenter();
			pos.y = 5.3f;
			if (asInt == 2)
			{
				matrix.SetTRS(pos, Quaternion.identity, new Vector3(1f, 1f, 1f));
				Graphics.DrawMesh(MeshPool.plane20, matrix, CompHoloTV.Hologram_southSheet[this.currFrame], 0);
				return;
			}
			if (asInt == 0)
			{
				matrix.SetTRS(pos, Quaternion.identity, new Vector3(1f, 1f, 1f));
				Graphics.DrawMesh(MeshPool.plane20, matrix, CompHoloTV.Hologram_northSheet[this.currFrame], 0);
				return;
			}
			if (asInt == 1)
			{
				matrix.SetTRS(pos, Quaternion.identity, new Vector3(1f, 1f, 1f));
				Graphics.DrawMesh(MeshPool.plane20, matrix, CompHoloTV.Hologram_east, 0);
				return;
			}
			if (asInt == 3)
			{
				matrix.SetTRS(pos, Quaternion.identity, new Vector3(1f, 1f, 1f));
				Graphics.DrawMesh(MeshPool.plane20, matrix, CompHoloTV.Hologram_east, 0);
			}
		}

		// Token: 0x040000B8 RID: 184
		private static readonly string Folder_south = "Things/Building/Joy/TV/Hologram_south";

		// Token: 0x040000B9 RID: 185
		private static readonly string Folder_north = "Things/Building/Joy/TV/Hologram_north";

		// Token: 0x040000BA RID: 186
		private static readonly Material Hologram_east = MaterialPool.MatFrom("Things/Building/Joy/TV/Hologram_east");

		// Token: 0x040000BB RID: 187
		private static readonly Material Hologram_front1 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/1");

		// Token: 0x040000BC RID: 188
		private static readonly Material Hologram_front2 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/2");

		// Token: 0x040000BD RID: 189
		private static readonly Material Hologram_front3 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/3");

		// Token: 0x040000BE RID: 190
		private static readonly Material Hologram_front4 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/4");

		// Token: 0x040000BF RID: 191
		private static readonly Material Hologram_front5 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/5");

		// Token: 0x040000C0 RID: 192
		private static readonly Material Hologram_front6 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/6");

		// Token: 0x040000C1 RID: 193
		private static readonly Material Hologram_front7 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/7");

		// Token: 0x040000C2 RID: 194
		private static readonly Material Hologram_front8 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/8");

		// Token: 0x040000C3 RID: 195
		private static readonly Material Hologram_front9 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/9");

		// Token: 0x040000C4 RID: 196
		private static readonly Material Hologram_front10 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/10");

		// Token: 0x040000C5 RID: 197
		private static readonly Material Hologram_front11 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/11");

		// Token: 0x040000C6 RID: 198
		private static readonly Material Hologram_front12 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/12");

		// Token: 0x040000C7 RID: 199
		private static readonly Material Hologram_front13 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/13");

		// Token: 0x040000C8 RID: 200
		private static readonly Material Hologram_front14 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/14");

		// Token: 0x040000C9 RID: 201
		private static readonly Material Hologram_front15 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/15");

		// Token: 0x040000CA RID: 202
		private static readonly Material Hologram_front16 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/16");

		// Token: 0x040000CB RID: 203
		private static readonly Material Hologram_front17 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/17");

		// Token: 0x040000CC RID: 204
		private static readonly Material Hologram_front18 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/18");

		// Token: 0x040000CD RID: 205
		private static readonly Material Hologram_front19 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/19");

		// Token: 0x040000CE RID: 206
		private static readonly Material Hologram_front20 = MaterialPool.MatFrom(CompHoloTV.Folder_south + "/20");

		// Token: 0x040000CF RID: 207
		private static readonly Material Hologram_back1 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/1");

		// Token: 0x040000D0 RID: 208
		private static readonly Material Hologram_back2 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/2");

		// Token: 0x040000D1 RID: 209
		private static readonly Material Hologram_back3 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/3");

		// Token: 0x040000D2 RID: 210
		private static readonly Material Hologram_back4 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/4");

		// Token: 0x040000D3 RID: 211
		private static readonly Material Hologram_back5 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/5");

		// Token: 0x040000D4 RID: 212
		private static readonly Material Hologram_back6 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/6");

		// Token: 0x040000D5 RID: 213
		private static readonly Material Hologram_back7 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/7");

		// Token: 0x040000D6 RID: 214
		private static readonly Material Hologram_back8 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/8");

		// Token: 0x040000D7 RID: 215
		private static readonly Material Hologram_back9 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/9");

		// Token: 0x040000D8 RID: 216
		private static readonly Material Hologram_back10 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/10");

		// Token: 0x040000D9 RID: 217
		private static readonly Material Hologram_back11 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/11");

		// Token: 0x040000DA RID: 218
		private static readonly Material Hologram_back12 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/12");

		// Token: 0x040000DB RID: 219
		private static readonly Material Hologram_back13 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/13");

		// Token: 0x040000DC RID: 220
		private static readonly Material Hologram_back14 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/14");

		// Token: 0x040000DD RID: 221
		private static readonly Material Hologram_back15 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/15");

		// Token: 0x040000DE RID: 222
		private static readonly Material Hologram_back16 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/16");

		// Token: 0x040000DF RID: 223
		private static readonly Material Hologram_back17 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/17");

		// Token: 0x040000E0 RID: 224
		private static readonly Material Hologram_back18 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/18");

		// Token: 0x040000E1 RID: 225
		private static readonly Material Hologram_back19 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/19");

		// Token: 0x040000E2 RID: 226
		private static readonly Material Hologram_back20 = MaterialPool.MatFrom(CompHoloTV.Folder_north + "/20");

		// Token: 0x040000E3 RID: 227
		private static readonly List<Material> Hologram_southSheet = new List<Material>();

		// Token: 0x040000E4 RID: 228
		private static readonly List<Material> Hologram_northSheet = new List<Material>();

		// Token: 0x040000E5 RID: 229
		private int currFrame = 1;

		// Token: 0x040000E6 RID: 230
		private bool powered;

		// Token: 0x040000E7 RID: 231
		private CompPowerTrader compPowerTrader;
	}
}
