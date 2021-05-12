using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000057 RID: 87
	public class MentalState_ReturnsToPackmates : MentalState_RaceDependant
	{
		// Token: 0x060001D2 RID: 466 RVA: 0x0000F824 File Offset: 0x0000DA24
		public override void PreStart()
		{
			base.PreStart();
			List<Pawn> list = this.pawn.relations.RelatedPawns.ToList<Pawn>();
			for (int i = 0; i < list.Count<Pawn>(); i++)
			{
				Pawn pawn = list[i];
				if (pawn != null)
				{
					if (this.pawn.HaveLoveRelation(pawn))
					{
						this.pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true, true);
					}
					else if (this.pawn.HavePackRelation(pawn) && pawn.Faction != this.pawn.Faction)
					{
						return;
					}
				}
			}
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00002E48 File Offset: 0x00001048
		public override RandomSocialMode SocialModeMax()
		{
			return RandomSocialMode.Off;
		}
	}
}
