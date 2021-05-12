using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x0200006D RID: 109
	public class Hediff_AvaliHasEgg : HediffWithComps
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000223 RID: 547 RVA: 0x00002FFC File Offset: 0x000011FC
		// (set) Token: 0x06000224 RID: 548 RVA: 0x00003004 File Offset: 0x00001204
		public float GestationProgress
		{
			get
			{
				return this.Severity;
			}
			private set
			{
				this.Severity = value;
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000225 RID: 549 RVA: 0x000119DC File Offset: 0x0000FBDC
		private bool IsSeverelyWounded
		{
			get
			{
				float num = 0f;
				List<Hediff> hediffs = this.pawn.health.hediffSet.hediffs;
				for (int i = 0; i < hediffs.Count; i++)
				{
					if (hediffs[i] is Hediff_Injury && !hediffs[i].IsPermanent())
					{
						num += hediffs[i].Severity;
					}
				}
				List<Hediff_MissingPart> missingPartsCommonAncestors = this.pawn.health.hediffSet.GetMissingPartsCommonAncestors();
				for (int j = 0; j < missingPartsCommonAncestors.Count; j++)
				{
					if (missingPartsCommonAncestors[j].IsFreshNonSolidExtremity)
					{
						num += missingPartsCommonAncestors[j].Part.def.GetMaxHealth(this.pawn);
					}
				}
				return num > 38f * this.pawn.RaceProps.baseHealthScale;
			}
		}

		// Token: 0x06000226 RID: 550 RVA: 0x00011AB4 File Offset: 0x0000FCB4
		public override void Tick()
		{
			this.ageTicks++;
			if (this.pawn.IsHashIntervalTick(1000))
			{
				if (this.pawn.needs.food != null && this.pawn.needs.food.CurCategory == HungerCategory.Starving && Rand.MTBEventOccurs(0.5f, 60000f, 1000f))
				{
					if (this.Visible && PawnUtility.ShouldSendNotificationAbout(this.pawn))
					{
						Messages.Message("MessageMiscarriedStarvation".Translate(this.pawn.LabelIndefinite()).CapitalizeFirst(), this.pawn, MessageTypeDefOf.NegativeHealthEvent, true);
					}
					this.Miscarry();
					return;
				}
				if (this.IsSeverelyWounded && Rand.MTBEventOccurs(0.5f, 60000f, 1000f))
				{
					if (this.Visible && PawnUtility.ShouldSendNotificationAbout(this.pawn))
					{
						Messages.Message("MessageMiscarriedPoorHealth".Translate(this.pawn.LabelIndefinite()).CapitalizeFirst(), this.pawn, MessageTypeDefOf.NegativeHealthEvent, true);
					}
					this.Miscarry();
					return;
				}
				if (this.GestationProgress >= 1f)
				{
					Building_Bed building_Bed = RestUtility.FindBedFor(this.pawn);
					Job newJob;
					if (building_Bed != null)
					{
						newJob = new Job(JobDefOfAvali.GotoLayAvaliEgg, this.father, building_Bed);
					}
					else
					{
						IntVec3 c = RCellFinder.RandomWanderDestFor(this.pawn, this.pawn.Position, 5f, null, Danger.Some);
						newJob = new Job(JobDefOfAvali.GotoLayAvaliEgg, this.father, c);
					}
					this.pawn.jobs.StartJob(newJob, JobCondition.InterruptForced, null, false, true, null, null, false, false);
				}
			}
			if (this.GestationProgress < 1f)
			{
				this.GestationProgress += 1f / (this.pawn.RaceProps.gestationPeriodDays * 60000f);
			}
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000300D File Offset: 0x0000120D
		private void Miscarry()
		{
			this.pawn.health.RemoveHediff(this);
		}

		// Token: 0x06000228 RID: 552 RVA: 0x00003020 File Offset: 0x00001220
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look<Pawn>(ref this.father, "father", false);
		}

		// Token: 0x06000229 RID: 553 RVA: 0x00011CCC File Offset: 0x0000FECC
		public override string DebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.DebugString());
			stringBuilder.AppendLine("Gestation progress: " + this.GestationProgress.ToStringPercent());
			stringBuilder.AppendLine("Time left: " + ((int)((1f - this.GestationProgress) * this.pawn.RaceProps.gestationPeriodDays * 60000f)).ToStringTicksToPeriod(true, false, true, true));
			return stringBuilder.ToString();
		}

		// Token: 0x040001E8 RID: 488
		private const int PawnStateCheckInterval = 1000;

		// Token: 0x040001E9 RID: 489
		private const float MTBMiscarryStarvingDays = 0.5f;

		// Token: 0x040001EA RID: 490
		private const float MTBMiscarryWoundedDays = 0.5f;

		// Token: 0x040001EB RID: 491
		public Pawn father;
	}
}
