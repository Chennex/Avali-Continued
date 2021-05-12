using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace Avali
{
	// Token: 0x0200006B RID: 107
	[StaticConstructorOnStartup]
	public class CompRunningGenerator : CompPowerTrader
	{
		// Token: 0x0600021C RID: 540 RVA: 0x000112B0 File Offset: 0x0000F4B0
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			CompRunningGenerator.RunningTrack_southSheet.Add(CompRunningGenerator.RunningTrack_south0);
			CompRunningGenerator.RunningTrack_southSheet.Add(CompRunningGenerator.RunningTrack_south1);
			CompRunningGenerator.RunningTrack_southSheet.Add(CompRunningGenerator.RunningTrack_south2);
			CompRunningGenerator.RunningTrack_southSheet.Add(CompRunningGenerator.RunningTrack_south3);
			CompRunningGenerator.RunningTrack_southSheet.Add(CompRunningGenerator.RunningTrack_south4);
			CompRunningGenerator.RunningTrack_southSheet.Add(CompRunningGenerator.RunningTrack_south5);
			CompRunningGenerator.RunningTrack_southSheet.Add(CompRunningGenerator.RunningTrack_south6);
			CompRunningGenerator.RunningTrack_southSheet.Add(CompRunningGenerator.RunningTrack_south7);
			CompRunningGenerator.RunningTrack_southSheet.Add(CompRunningGenerator.RunningTrack_south8);
			CompRunningGenerator.RunningTrack_northSheet.Add(CompRunningGenerator.RunningTrack_north0);
			CompRunningGenerator.RunningTrack_northSheet.Add(CompRunningGenerator.RunningTrack_north1);
			CompRunningGenerator.RunningTrack_northSheet.Add(CompRunningGenerator.RunningTrack_north2);
			CompRunningGenerator.RunningTrack_northSheet.Add(CompRunningGenerator.RunningTrack_north3);
			CompRunningGenerator.RunningTrack_northSheet.Add(CompRunningGenerator.RunningTrack_north4);
			CompRunningGenerator.RunningTrack_northSheet.Add(CompRunningGenerator.RunningTrack_north5);
			CompRunningGenerator.RunningTrack_northSheet.Add(CompRunningGenerator.RunningTrack_north6);
			CompRunningGenerator.RunningTrack_northSheet.Add(CompRunningGenerator.RunningTrack_north7);
			CompRunningGenerator.RunningTrack_northSheet.Add(CompRunningGenerator.RunningTrack_north8);
			CompRunningGenerator.RunningTrack_eastSheet.Add(CompRunningGenerator.RunningTrack_east0);
			CompRunningGenerator.RunningTrack_eastSheet.Add(CompRunningGenerator.RunningTrack_east1);
			CompRunningGenerator.RunningTrack_eastSheet.Add(CompRunningGenerator.RunningTrack_east2);
			CompRunningGenerator.RunningTrack_eastSheet.Add(CompRunningGenerator.RunningTrack_east3);
			CompRunningGenerator.RunningTrack_eastSheet.Add(CompRunningGenerator.RunningTrack_east4);
			CompRunningGenerator.RunningTrack_eastSheet.Add(CompRunningGenerator.RunningTrack_east5);
			CompRunningGenerator.RunningTrack_eastSheet.Add(CompRunningGenerator.RunningTrack_east6);
			CompRunningGenerator.RunningTrack_eastSheet.Add(CompRunningGenerator.RunningTrack_east7);
			CompRunningGenerator.RunningTrack_eastSheet.Add(CompRunningGenerator.RunningTrack_east8);
			this.compMannable = this.parent.TryGetComp<CompMannable>();
			if (base.Props.basePowerConsumption <= 0f && !this.parent.IsBrokenDown() && FlickUtility.WantsToBeOn(this.parent))
			{
				base.PowerOn = true;
			}
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00002FE6 File Offset: 0x000011E6
		public override void PostDraw()
		{
			base.PostDraw();
			this.DrawCurrentFrame();
		}

		// Token: 0x0600021E RID: 542 RVA: 0x000114A0 File Offset: 0x0000F6A0
		public override void CompTick()
		{
			base.CompTick();
			this.UpdateDesiredPowerOutput();
			if (this.compMannable == null)
			{
				return;
			}
			if (this.compMannable.MannedNow && this.parent.IsHashIntervalTick(1))
			{
				if (this.currFrame >= 8)
				{
					this.currFrame = -1;
				}
				this.currFrame++;
			}
		}

		// Token: 0x0600021F RID: 543 RVA: 0x000114FC File Offset: 0x0000F6FC
		public void UpdateDesiredPowerOutput()
		{
			if (this.parent.IsHashIntervalTick(60))
			{
				if (!this.parent.IsBrokenDown() && FlickUtility.WantsToBeOn(this.parent))
				{
					if (this.compMannable == null)
					{
						Log.Error(this.parent + "not have CompProperties_UseBuilding.", false);
						return;
					}
					if (this.compMannable.MannedNow)
					{
						base.PowerOutput = this.compMannable.ManningPawn.GetStatValue(StatDefOf.MoveSpeed, true) * 100f + (float)Rand.RangeInclusive(-100, 100);
						return;
					}
					base.PowerOutput = 0f;
					return;
				}
				else
				{
					base.PowerOutput = 0f;
				}
			}
		}

		// Token: 0x06000220 RID: 544 RVA: 0x000115A8 File Offset: 0x0000F7A8
		private void DrawCurrentFrame()
		{
			Matrix4x4 matrix = default(Matrix4x4);
			int asInt = this.parent.Rotation.AsInt;
			Vector3 pos = this.parent.TrueCenter();
			pos.y = 5.3f;
			if (asInt == 2)
			{
				matrix.SetTRS(pos, Quaternion.identity, new Vector3(1f, 1f, 2f));
				Graphics.DrawMesh(MeshPool.plane10, matrix, CompRunningGenerator.RunningTrack_southSheet[this.currFrame], 0);
				return;
			}
			if (asInt == 0)
			{
				matrix.SetTRS(pos, Quaternion.identity, new Vector3(1f, 1f, 2f));
				Graphics.DrawMesh(MeshPool.plane10, matrix, CompRunningGenerator.RunningTrack_northSheet[this.currFrame], 0);
				return;
			}
			if (asInt == 1)
			{
				matrix.SetTRS(pos, Quaternion.identity, new Vector3(2f, 1f, 1f));
				Graphics.DrawMesh(MeshPool.plane10, matrix, CompRunningGenerator.RunningTrack_eastSheet[this.currFrame], 0);
				return;
			}
			if (asInt == 3)
			{
				matrix.SetTRS(pos, Quaternion.identity, new Vector3(2f, 1f, 1f));
				Graphics.DrawMesh(MeshPool.plane10Flip, matrix, CompRunningGenerator.RunningTrack_eastSheet[this.currFrame], 0);
			}
		}

		// Token: 0x040001B7 RID: 439
		private static readonly string Folder_south = "Things/Building/Joy/RunningTrack/RunningTrack_south";

		// Token: 0x040001B8 RID: 440
		private static readonly string Folder_north = "Things/Building/Joy/RunningTrack/RunningTrack_north";

		// Token: 0x040001B9 RID: 441
		private static readonly string Folder_east = "Things/Building/Joy/RunningTrack/RunningTrack_east";

		// Token: 0x040001BA RID: 442
		private static readonly Material RunningTrack_south0 = MaterialPool.MatFrom(CompRunningGenerator.Folder_south + "/0");

		// Token: 0x040001BB RID: 443
		private static readonly Material RunningTrack_south1 = MaterialPool.MatFrom(CompRunningGenerator.Folder_south + "/1");

		// Token: 0x040001BC RID: 444
		private static readonly Material RunningTrack_south2 = MaterialPool.MatFrom(CompRunningGenerator.Folder_south + "/2");

		// Token: 0x040001BD RID: 445
		private static readonly Material RunningTrack_south3 = MaterialPool.MatFrom(CompRunningGenerator.Folder_south + "/3");

		// Token: 0x040001BE RID: 446
		private static readonly Material RunningTrack_south4 = MaterialPool.MatFrom(CompRunningGenerator.Folder_south + "/4");

		// Token: 0x040001BF RID: 447
		private static readonly Material RunningTrack_south5 = MaterialPool.MatFrom(CompRunningGenerator.Folder_south + "/5");

		// Token: 0x040001C0 RID: 448
		private static readonly Material RunningTrack_south6 = MaterialPool.MatFrom(CompRunningGenerator.Folder_south + "/6");

		// Token: 0x040001C1 RID: 449
		private static readonly Material RunningTrack_south7 = MaterialPool.MatFrom(CompRunningGenerator.Folder_south + "/7");

		// Token: 0x040001C2 RID: 450
		private static readonly Material RunningTrack_south8 = MaterialPool.MatFrom(CompRunningGenerator.Folder_south + "/8");

		// Token: 0x040001C3 RID: 451
		private static readonly Material RunningTrack_north0 = MaterialPool.MatFrom(CompRunningGenerator.Folder_north + "/0");

		// Token: 0x040001C4 RID: 452
		private static readonly Material RunningTrack_north1 = MaterialPool.MatFrom(CompRunningGenerator.Folder_north + "/1");

		// Token: 0x040001C5 RID: 453
		private static readonly Material RunningTrack_north2 = MaterialPool.MatFrom(CompRunningGenerator.Folder_north + "/2");

		// Token: 0x040001C6 RID: 454
		private static readonly Material RunningTrack_north3 = MaterialPool.MatFrom(CompRunningGenerator.Folder_north + "/3");

		// Token: 0x040001C7 RID: 455
		private static readonly Material RunningTrack_north4 = MaterialPool.MatFrom(CompRunningGenerator.Folder_north + "/4");

		// Token: 0x040001C8 RID: 456
		private static readonly Material RunningTrack_north5 = MaterialPool.MatFrom(CompRunningGenerator.Folder_north + "/5");

		// Token: 0x040001C9 RID: 457
		private static readonly Material RunningTrack_north6 = MaterialPool.MatFrom(CompRunningGenerator.Folder_north + "/6");

		// Token: 0x040001CA RID: 458
		private static readonly Material RunningTrack_north7 = MaterialPool.MatFrom(CompRunningGenerator.Folder_north + "/7");

		// Token: 0x040001CB RID: 459
		private static readonly Material RunningTrack_north8 = MaterialPool.MatFrom(CompRunningGenerator.Folder_north + "/8");

		// Token: 0x040001CC RID: 460
		private static readonly Material RunningTrack_east0 = MaterialPool.MatFrom(CompRunningGenerator.Folder_east + "/0");

		// Token: 0x040001CD RID: 461
		private static readonly Material RunningTrack_east1 = MaterialPool.MatFrom(CompRunningGenerator.Folder_east + "/1");

		// Token: 0x040001CE RID: 462
		private static readonly Material RunningTrack_east2 = MaterialPool.MatFrom(CompRunningGenerator.Folder_east + "/2");

		// Token: 0x040001CF RID: 463
		private static readonly Material RunningTrack_east3 = MaterialPool.MatFrom(CompRunningGenerator.Folder_east + "/3");

		// Token: 0x040001D0 RID: 464
		private static readonly Material RunningTrack_east4 = MaterialPool.MatFrom(CompRunningGenerator.Folder_east + "/4");

		// Token: 0x040001D1 RID: 465
		private static readonly Material RunningTrack_east5 = MaterialPool.MatFrom(CompRunningGenerator.Folder_east + "/5");

		// Token: 0x040001D2 RID: 466
		private static readonly Material RunningTrack_east6 = MaterialPool.MatFrom(CompRunningGenerator.Folder_east + "/6");

		// Token: 0x040001D3 RID: 467
		private static readonly Material RunningTrack_east7 = MaterialPool.MatFrom(CompRunningGenerator.Folder_east + "/7");

		// Token: 0x040001D4 RID: 468
		private static readonly Material RunningTrack_east8 = MaterialPool.MatFrom(CompRunningGenerator.Folder_east + "/8");

		// Token: 0x040001D5 RID: 469
		private static readonly List<Material> RunningTrack_southSheet = new List<Material>();

		// Token: 0x040001D6 RID: 470
		private static readonly List<Material> RunningTrack_northSheet = new List<Material>();

		// Token: 0x040001D7 RID: 471
		private static readonly List<Material> RunningTrack_eastSheet = new List<Material>();

		// Token: 0x040001D8 RID: 472
		private CompMannable compMannable;

		// Token: 0x040001D9 RID: 473
		private int currFrame;
	}
}
