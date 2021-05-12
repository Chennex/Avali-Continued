using System;
using RimWorld;
using Verse;

namespace Avali
{
	// Token: 0x02000013 RID: 19
	public class CompSunGlower : CompGlower
	{
		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000052 RID: 82 RVA: 0x000022F3 File Offset: 0x000004F3
		public new CompProperties_Glower Props
		{
			get
			{
				return (CompProperties_Glower)this.props;
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000049CC File Offset: 0x00002BCC
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (this.parent.Spawned && this.glow == -1)
			{
				int num = (int)(255f * this.parent.Map.skyManager.CurSkyGlow);
				this.glow = num;
				this.Props.glowColor = new ColorInt(num, num, num);
				this.parent.Map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
				this.parent.Map.glowGrid.RegisterGlower(this);
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00004A60 File Offset: 0x00002C60
		public override void CompTick()
		{
			if (this.parent.Spawned && this.parent.IsHashIntervalTick(60))
			{
				int num = (int)(255f * this.parent.Map.skyManager.CurSkyGlow);
				if (this.glow != num)
				{
					this.glow = num;
					this.Props.glowColor = new ColorInt(num, num, num);
					this.parent.Map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
					this.parent.Map.glowGrid.DeRegisterGlower(this);
					this.parent.Map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
					this.parent.Map.glowGrid.RegisterGlower(this);
				}
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00002088 File Offset: 0x00000288
		public override void ReceiveCompSignal(string signal)
		{
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00002088 File Offset: 0x00000288
		public override void PostExposeData()
		{
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00002300 File Offset: 0x00000500
		public override void PostDeSpawn(Map map)
		{
			if (this.glow != -1)
			{
				map.mapDrawer.MapMeshDirty(this.parent.Position, MapMeshFlag.Things);
				map.glowGrid.DeRegisterGlower(this);
			}
		}

		// Token: 0x0400003B RID: 59
		public int glow = -1;
	}
}
