using System;
using System.Collections.Generic;
using AlienRace;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000004 RID: 4
	public class CompCanBePutOn : ThingComp
	{
		// Token: 0x0600000E RID: 14 RVA: 0x000020A4 File Offset: 0x000002A4
		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (selPawn.CanReserveAndReach(this.parent, PathEndMode.ClosestTouch, Danger.Deadly, 1, -1, null, false) && !(this.parent.GetType() != typeof(Apparel)) && selPawn.IsColonist && selPawn.Awake())
			{
				List<Thing> avalablePawns = AvaliUtility.FindAllThingsOnMapAtRange(this.parent, null, typeof(Pawn), null, float.MaxValue, int.MaxValue, true, true);
				if (avalablePawns.Count != 0)
				{
					int num;
					for (int i = 0; i < avalablePawns.Count; i = num + 1)
					{
						Pawn p = avalablePawns[i] as Pawn;
						if (p != null && p != selPawn && (p.IsColonist || p.IsPrisonerOfColony || (!p.RaceProps.IsMechanoid && p.RaceProps.ToolUser && (p.Downed || !p.HostileTo(selPawn)))))
						{
							ThingDef_AlienRace thingDef_AlienRace = p.def as ThingDef_AlienRace;
							if (thingDef_AlienRace != null)
							{
								List<ThingDef> apparelList = thingDef_AlienRace.alienRace.raceRestriction.apparelList;
								if (apparelList.Count > 0)
								{
									apparelList.AddRange(thingDef_AlienRace.alienRace.raceRestriction.whiteApparelList);
									bool flag = false;
									for (int j = 0; j < apparelList.Count; j++)
									{
										ThingDef thingDef = apparelList[j];
										if (this.parent.def.defName == "apparel")
										{
											flag = true;
											break;
										}
									}
									if (!flag)
									{
										goto IL_2A0;
									}
								}
							}
							yield return new FloatMenuOption("PutOnLabel".Translate(p.Label), delegate()
							{
								Job job = new Job(JobDefOfAvali.PutOn, this.parent, p, selPawn.Position);
								job.count = 1;
								selPawn.jobs.TryTakeOrderedJob(job, JobTag.MiscWork);
							}, MenuOptionPriority.Default, null, null, 0f, null, null);
						}
						IL_2A0:
						num = i;
					}
				}
				avalablePawns = null;
				avalablePawns = null;
			}
			yield break;
		}
	}
}
