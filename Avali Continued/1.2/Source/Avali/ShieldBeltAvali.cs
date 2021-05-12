using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Avali
{
	// Token: 0x02000067 RID: 103
	[StaticConstructorOnStartup]
	public class ShieldBeltAvali : Apparel
	{
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000208 RID: 520 RVA: 0x00002F57 File Offset: 0x00001157
		private float EnergyMax
		{
			get
			{
				return this.GetStatValue(StatDefOf.EnergyShieldEnergyMax, true);
			}
		}

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000209 RID: 521 RVA: 0x00002F65 File Offset: 0x00001165
		private float EnergyGainPerTick
		{
			get
			{
				return this.GetStatValue(StatDefOf.EnergyShieldRechargeRate, true) / 60f;
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600020A RID: 522 RVA: 0x00002F79 File Offset: 0x00001179
		public float Energy
		{
			get
			{
				return this.energy;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600020B RID: 523 RVA: 0x00002F81 File Offset: 0x00001181
		public ShieldState ShieldState
		{
			get
			{
				if (this.ticksToReset > 0)
				{
					return ShieldState.Resetting;
				}
				return ShieldState.Active;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600020C RID: 524 RVA: 0x00010BF4 File Offset: 0x0000EDF4
		private bool ShouldDisplay
		{
			get
			{
				Pawn wearer = base.Wearer;
				return wearer.Spawned && !wearer.Dead && !wearer.Downed && (wearer.InAggroMentalState || wearer.Drafted || (wearer.Faction.HostileTo(Faction.OfPlayer) && !wearer.IsPrisoner) || Find.TickManager.TicksGame < this.lastKeepDisplayTick + this.KeepDisplayingTicks);
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x00010C68 File Offset: 0x0000EE68
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.energy, "energy", 0f, false);
			Scribe_Values.Look<int>(ref this.ticksToReset, "ticksToReset", -1, false);
			Scribe_Values.Look<int>(ref this.lastKeepDisplayTick, "lastKeepDisplayTick", 0, false);
		}

		// Token: 0x0600020E RID: 526 RVA: 0x00002F8F File Offset: 0x0000118F
		public override float GetSpecialApparelScoreOffset()
		{
			return this.EnergyMax * this.ApparelScorePerEnergyMax;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00010CB8 File Offset: 0x0000EEB8
		public override void Tick()
		{
			base.Tick();
			if (base.Wearer == null)
			{
				this.energy = 0f;
				return;
			}
			if (this.ShieldState == ShieldState.Resetting)
			{
				this.ticksToReset--;
				if (this.ticksToReset <= 0)
				{
					this.Reset();
					return;
				}
			}
			else if (this.ShieldState == ShieldState.Active)
			{
				this.energy += this.EnergyGainPerTick;
				if (this.energy > this.EnergyMax)
				{
					this.energy = this.EnergyMax;
				}
			}
		}

		// Token: 0x06000210 RID: 528 RVA: 0x00010D3C File Offset: 0x0000EF3C
		public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
		{
			if (this.ShieldState == ShieldState.Active && ((dinfo.Instigator != null && !dinfo.Instigator.Position.AdjacentTo8WayOrInside(base.Wearer.Position)) || dinfo.Def.isExplosive))
			{
				if (dinfo.Instigator != null)
				{
					AttachableThing attachableThing = dinfo.Instigator as AttachableThing;
					if (attachableThing != null && attachableThing.parent == base.Wearer)
					{
						return false;
					}
				}
				this.energy -= dinfo.Amount * this.EnergyLossPerDamage;
				if (dinfo.Def == DamageDefOf.EMP)
				{
					this.energy = -1f;
				}
				if (this.energy < 0f)
				{
					this.Break();
				}
				else
				{
					this.AbsorbedDamage(dinfo);
				}
				return true;
			}
			return false;
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00002F9E File Offset: 0x0000119E
		public void KeepDisplaying()
		{
			this.lastKeepDisplayTick = Find.TickManager.TicksGame;
		}

		// Token: 0x06000212 RID: 530 RVA: 0x00010E08 File Offset: 0x0000F008
		private void AbsorbedDamage(DamageInfo dinfo)
		{
			SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
			this.impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
			Vector3 loc = base.Wearer.TrueCenter() + this.impactAngleVect.RotatedBy(180f) * 0.5f;
			float num = Mathf.Min(10f, 2f + dinfo.Amount / 10f);
			MoteMaker.MakeStaticMote(loc, base.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, num);
			int num2 = (int)num;
			for (int i = 0; i < num2; i++)
			{
				MoteMaker.ThrowDustPuff(loc, base.Wearer.Map, Rand.Range(0.8f, 1.2f));
			}
			this.lastAbsorbDamageTick = Find.TickManager.TicksGame;
			this.KeepDisplaying();
		}

		// Token: 0x06000213 RID: 531 RVA: 0x00010EF8 File Offset: 0x0000F0F8
		private void Break()
		{
			SoundDefOf.EnergyShield_Broken.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
			MoteMaker.MakeStaticMote(base.Wearer.TrueCenter(), base.Wearer.Map, ThingDefOf.Mote_ExplosionFlash, 12f);
			for (int i = 0; i < 6; i++)
			{
				Vector3 loc = base.Wearer.TrueCenter() + Vector3Utility.HorizontalVectorFromAngle((float)Rand.Range(0, 360)) * Rand.Range(0.3f, 0.6f);
				MoteMaker.ThrowDustPuff(loc, base.Wearer.Map, Rand.Range(0.8f, 1.2f));
			}
			this.energy = 0f;
			this.ticksToReset = this.StartingTicksToReset;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x00010FD0 File Offset: 0x0000F1D0
		private void Reset()
		{
			if (base.Wearer.Spawned)
			{
				SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(base.Wearer.Position, base.Wearer.Map, false));
				MoteMaker.ThrowLightningGlow(base.Wearer.TrueCenter(), base.Wearer.Map, 3f);
			}
			this.ticksToReset = -1;
			this.energy = this.EnergyOnReset;
		}

		// Token: 0x06000215 RID: 533 RVA: 0x00011048 File Offset: 0x0000F248
		public override void DrawWornExtras()
		{
			if (this.ShieldState == ShieldState.Active && this.ShouldDisplay)
			{
				float num = Mathf.Lerp(1.2f, 1.55f, this.energy);
				Vector3 vector = base.Wearer.Drawer.DrawPos;
				vector.y = AltitudeLayer.Blueprint.AltitudeFor();
				int num2 = Find.TickManager.TicksGame - this.lastAbsorbDamageTick;
				if (num2 < 8)
				{
					float num3 = (float)(8 - num2) / 8f * 0.05f;
					vector += this.impactAngleVect * num3;
					num -= num3;
				}
				float angle = (float)Rand.Range(0, 360);
				Vector3 s = new Vector3(num, 1f, num);
				Matrix4x4 matrix = default(Matrix4x4);
				matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
				if (this.BubbleMat == null)
				{
					this.SetBubbleMatMat();
				}
				Graphics.DrawMesh(MeshPool.plane10, matrix, this.BubbleMat, 0);
			}
		}

		// Token: 0x06000216 RID: 534 RVA: 0x00002FB0 File Offset: 0x000011B0
		public override bool AllowVerbCast(IntVec3 root, Map map, LocalTargetInfo targ, Verb verb)
		{
			return !(verb is Verb_LaunchProjectile) || ReachabilityImmediate.CanReachImmediate(root, targ, map, PathEndMode.Touch, null);
		}

		// Token: 0x06000217 RID: 535 RVA: 0x00002FC7 File Offset: 0x000011C7
		public virtual void SetBubbleMatMat()
		{
			this.BubbleMat = MaterialPool.MatFrom("Things/Avali/Apparel/ShieldBelt/ShieldBubble", ShaderDatabase.Transparent);
		}

		// Token: 0x04000196 RID: 406
		private const float MinDrawSize = 1.2f;

		// Token: 0x04000197 RID: 407
		private const float MaxDrawSize = 1.55f;

		// Token: 0x04000198 RID: 408
		private const float MaxDamagedJitterDist = 0.05f;

		// Token: 0x04000199 RID: 409
		private const int JitterDurationTicks = 8;

		// Token: 0x0400019A RID: 410
		public Material BubbleMat;

		// Token: 0x0400019B RID: 411
		private float energy;

		// Token: 0x0400019C RID: 412
		private int ticksToReset = -1;

		// Token: 0x0400019D RID: 413
		private int lastKeepDisplayTick = -9999;

		// Token: 0x0400019E RID: 414
		private Vector3 impactAngleVect;

		// Token: 0x0400019F RID: 415
		private int lastAbsorbDamageTick = -9999;

		// Token: 0x040001A0 RID: 416
		private int StartingTicksToReset = 3200;

		// Token: 0x040001A1 RID: 417
		private float EnergyOnReset = 0.2f;

		// Token: 0x040001A2 RID: 418
		private float EnergyLossPerDamage = 0.033f;

		// Token: 0x040001A3 RID: 419
		private int KeepDisplayingTicks = 1000;

		// Token: 0x040001A4 RID: 420
		private float ApparelScorePerEnergyMax = 0.25f;
	}
}
