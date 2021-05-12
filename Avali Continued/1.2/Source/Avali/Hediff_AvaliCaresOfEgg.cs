using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000040 RID: 64
	public class Hediff_AvaliCaresOfEgg : HediffWithComps
	{
		// Token: 0x06000159 RID: 345 RVA: 0x0000CEB8 File Offset: 0x0000B0B8
		public override void Tick()
		{
			base.Tick();
			if (!this.pawn.IsColonistPlayerControlled)
			{
				return;
			}
			if (this.pawn.IsHashIntervalTick(200000))
			{
				Predicate<Thing> validator = (Thing t) => t.def.defName == ThingDefOfAvali.AvaliEgg.ToString() && (t.TryGetComp<CompHatcher>().hatcheeParent == this.pawn || t.TryGetComp<CompHatcher>().otherParent == this.pawn);
				Thing thing = GenClosest.ClosestThingReachable(this.pawn.Position, this.pawn.Map, ThingRequest.ForDef(this.pawn.def), PathEndMode.Touch, TraverseParms.For(this.pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
				if (thing != null)
				{
					Pawn pawn = thing.TryGetComp<CompHatcher>().hatcheeParent;
					if (pawn == null)
					{
						pawn = thing.TryGetComp<CompHatcher>().otherParent;
						if (pawn == null)
						{
							this.pawn.health.RemoveHediff(this);
							return;
						}
					}
					if (pawn.CurrentBed() == null)
					{
						return;
					}
					Job newJob = new Job(JobDefOfAvali.CheckAvaliEgg, thing, pawn.CurrentBed(), pawn.CurrentBed().OccupiedRect().CenterCell);
					this.pawn.jobs.StartJob(newJob, JobCondition.InterruptForced, null, true, false, null, null, false, false);
				}
			}
		}

		// Token: 0x0400012E RID: 302
		private const int pawnStateCheckInterval = 200000;
	}
}
