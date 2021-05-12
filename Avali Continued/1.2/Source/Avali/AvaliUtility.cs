using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x02000024 RID: 36
	public static class AvaliUtility
	{
		// Token: 0x06000093 RID: 147 RVA: 0x00006B94 File Offset: 0x00004D94
		public static bool BothPawnsReproductiveOrNotReproductive(Pawn pawn, Pawn pawn2)
		{
			return (pawn.ageTracker.CurLifeStage.reproductive && pawn2.ageTracker.CurLifeStage.reproductive) || (!pawn.ageTracker.CurLifeStage.reproductive && !pawn2.ageTracker.CurLifeStage.reproductive);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x000024AD File Offset: 0x000006AD
		public static bool HavePackRelation(this Pawn pawn, Pawn pawn2)
		{
			return pawn.relations.DirectRelationExists(PawnRelationDefOfAvali.Packmate, pawn2) || pawn.relations.DirectRelationExists(PawnRelationDefOfAvali.PackLeader, pawn2);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000024D8 File Offset: 0x000006D8
		public static bool HaveLoveRelation(this Pawn pawn, Pawn pawn2)
		{
			return pawn.relations.DirectRelationExists(PawnRelationDefOf.Lover, pawn2) || pawn.relations.DirectRelationExists(PawnRelationDefOf.Spouse, pawn2) || pawn.relations.DirectRelationExists(PawnRelationDefOf.Fiance, pawn2);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00006BEC File Offset: 0x00004DEC
		public static bool PawnListed(this Pawn pawn, List<Pawn> relatedPawns)
		{
			for (int i = 0; i < relatedPawns.Count; i++)
			{
				Pawn pawn2 = relatedPawns[i];
				if (pawn2 != null && pawn2 == pawn)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00006C1C File Offset: 0x00004E1C
		public static int GetSkillLevel(this Pawn pawn, SkillDef skillDef)
		{
			if (pawn.skills == null)
			{
				return 0;
			}
			SkillRecord skill = pawn.skills.GetSkill(skillDef);
			if (skill != null && !skill.TotallyDisabled && skill.Level > 0)
			{
				return skill.Level;
			}
			return 0;
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00006C5C File Offset: 0x00004E5C
		public static int GetTotalSkillLevel(this Pawn pawn)
		{
			if (pawn.skills == null)
			{
				return 0;
			}
			int num = 0;
			List<SkillRecord> list = new List<SkillRecord>();
			list.Add(pawn.skills.GetSkill(SkillDefOf.Animals));
			list.Add(pawn.skills.GetSkill(SkillDefOf.Artistic));
			list.Add(pawn.skills.GetSkill(SkillDefOf.Construction));
			list.Add(pawn.skills.GetSkill(SkillDefOf.Cooking));
			list.Add(pawn.skills.GetSkill(SkillDefOf.Crafting));
			list.Add(pawn.skills.GetSkill(SkillDefOf.Plants));
			list.Add(pawn.skills.GetSkill(SkillDefOf.Intellectual));
			list.Add(pawn.skills.GetSkill(SkillDefOf.Medicine));
			list.Add(pawn.skills.GetSkill(SkillDefOf.Melee));
			list.Add(pawn.skills.GetSkill(SkillDefOf.Mining));
			list.Add(pawn.skills.GetSkill(SkillDefOf.Shooting));
			list.Add(pawn.skills.GetSkill(SkillDefOf.Social));
			for (int i = 0; i < list.Count; i++)
			{
				SkillRecord skillRecord = list[i];
				if (skillRecord != null && !skillRecord.TotallyDisabled && skillRecord.Level > 0)
				{
					num += skillRecord.Level;
				}
			}
			return num;
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00006DBC File Offset: 0x00004FBC
		public static SkillRecord GetHighestPackSkill(this Pawn thisPawn, List<Pawn> pack)
		{
			pack.Add(thisPawn);
			SkillRecord result = null;
			int num = 0;
			for (int i = 0; i < pack.Count; i++)
			{
				Pawn pawn = pack[i];
				if (pawn.skills != null && (thisPawn == pawn || thisPawn.HavePackRelation(pawn)))
				{
					List<SkillRecord> list = new List<SkillRecord>();
					list.Add(pawn.skills.GetSkill(SkillDefOf.Animals));
					list.Add(pawn.skills.GetSkill(SkillDefOf.Artistic));
					list.Add(pawn.skills.GetSkill(SkillDefOf.Construction));
					list.Add(pawn.skills.GetSkill(SkillDefOf.Cooking));
					list.Add(pawn.skills.GetSkill(SkillDefOf.Crafting));
					list.Add(pawn.skills.GetSkill(SkillDefOf.Plants));
					list.Add(pawn.skills.GetSkill(SkillDefOf.Intellectual));
					list.Add(pawn.skills.GetSkill(SkillDefOf.Medicine));
					list.Add(pawn.skills.GetSkill(SkillDefOf.Melee));
					list.Add(pawn.skills.GetSkill(SkillDefOf.Mining));
					list.Add(pawn.skills.GetSkill(SkillDefOf.Shooting));
					list.Add(pawn.skills.GetSkill(SkillDefOf.Social));
					for (int j = 0; j < list.Count; j++)
					{
						SkillRecord skillRecord = list[j];
						if (skillRecord != null && !skillRecord.TotallyDisabled && skillRecord.Level > num)
						{
							result = skillRecord;
							num = skillRecord.Level;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00006F70 File Offset: 0x00005170
		public static HediffDef TryRemovePackHediffsAndAddPackHediff(this Pawn pawn, HediffDef newPackHediff)
		{
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				if (hediffs[i].def == newPackHediff)
				{
					return newPackHediff;
				}
			}
			for (int j = 0; j < hediffs.Count; j++)
			{
				Hediff hediff = hediffs[j];
				if (hediff != null && (hediff.def == HediffDefOfAvali.AvaliPackExploration || hediff.def == HediffDefOfAvali.AvaliPackMilitary || hediff.def == HediffDefOfAvali.AvaliPackHunting || hediff.def == HediffDefOfAvali.AvaliPackScientific || hediff.def == HediffDefOfAvali.AvaliPackIndustrial || hediff.def == HediffDefOfAvali.AvaliPackArtistical))
				{
					pawn.health.RemoveHediff(hediff);
				}
			}
			pawn.health.AddHediff(newPackHediff, null, null, null);
			return newPackHediff;
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00002516 File Offset: 0x00000716
		public static void TryAddDirectRelation(this Pawn pawn1, Pawn pawn2, PawnRelationDef relation)
		{
			if (!pawn1.relations.DirectRelationExists(relation, pawn2))
			{
				pawn1.relations.AddDirectRelation(relation, pawn2);
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00007040 File Offset: 0x00005240
		public static void TryMakeImmune(this Pawn pawn, float minResist, float reqResist, HediffDef immunityHediff)
		{
			Hediff hediff = pawn.health.hediffSet.hediffs.Find((Hediff x) => x.def == immunityHediff);
			if (minResist >= reqResist && hediff == null)
			{
				pawn.health.AddHediff(immunityHediff, null, null, null);
				return;
			}
			if (minResist < reqResist && hediff != null)
			{
				pawn.health.RemoveHediff(hediff);
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000070B4 File Offset: 0x000052B4
		public static List<Thing> FindAllThingsOnMapAtRange(Thing thing, ThingDef thingDef = null, Type thingType = null, List<Thing> thingsList = null, float range = 3.40282347E+38f, int maxOutputListThings = 2147483647, bool shouldBeReachable = false, bool shouldBeReservable = false)
		{
			List<Thing> list = new List<Thing>();
			IntVec3 position = thing.Position;
			if (range != 3.40282347E+38f)
			{
				range /= 100f;
			}
			if (thingsList == null)
			{
				thingsList = thing.Map.spawnedThings.ToList<Thing>();
			}
			for (int i = 0; i < thingsList.Count; i++)
			{
				Thing thing2 = thingsList[i];
				if (thing2.def != null)
				{
					if (thingType == null)
					{
						if (thingDef == null)
						{
							break;
						}
						if (thing2.def != thingDef)
						{
							goto IL_13D;
						}
					}
					else if (thing2.def.thingClass != thingType)
					{
						goto IL_13D;
					}
					if (shouldBeReachable || shouldBeReservable)
					{
						Pawn pawn = thing as Pawn;
						if (pawn != null && ((shouldBeReachable && !pawn.CanReach(thing2, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn)) || (shouldBeReservable && !pawn.CanReserve(thing2, 1, -1, null, false))))
						{
							goto IL_13D;
						}
					}
					IntVec3 position2 = thing2.Position;
					if (range != 3.40282347E+38f)
					{
						double num = Math.Sqrt(Math.Pow((double)(position2.x - position.x), 2.0) + Math.Pow((double)(position2.z - position.z), 2.0));
						if (num <= (double)range)
						{
							list.Add(thing2);
						}
					}
					else
					{
						list.Add(thing2);
					}
					if (list.Count > maxOutputListThings)
					{
						break;
					}
				}
				IL_13D:;
			}
			List<Thing> list2 = new List<Thing>();
			while (list.Count > 0)
			{
				double num2 = double.MaxValue;
				Thing item = list.First<Thing>();
				for (int j = 0; j < list.Count; j++)
				{
					Thing thing3 = list[j];
					IntVec3 position3 = thing3.Position;
					double num3 = Math.Sqrt(Math.Pow((double)(position3.x - position.x), 2.0) + Math.Pow((double)(position3.z - position.z), 2.0));
					if (num3 < num2)
					{
						num2 = num3;
						item = thing3;
					}
				}
				list2.Add(item);
				list.Remove(item);
			}
			return list2;
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00002534 File Offset: 0x00000734
		public static bool IsSlave(this Pawn pawn)
		{
			return !pawn.IsColonist && pawn.kindDef == PawnKindDef.Named("AvaliSlave");
		}

		// Token: 0x0600009F RID: 159 RVA: 0x000072C8 File Offset: 0x000054C8
		public static bool IsPowered(this Thing building)
		{
			CompPowerTrader compPowerTrader = building.TryGetComp<CompPowerTrader>();
			return compPowerTrader == null || compPowerTrader.PowerOn;
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00002553 File Offset: 0x00000753
		public static bool IsOnAndNotBrokenDown(this ThingWithComps thing)
		{
			return FlickUtility.WantsToBeOn(thing) && !thing.IsBrokenDown();
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000072EC File Offset: 0x000054EC
		public static Pawn GetUserPawn(this ThingWithComps thing)
		{
			List<Thing> thingList = thing.InteractionCell.GetThingList(thing.Map);
			for (int i = 0; i < thingList.Count; i++)
			{
				Pawn pawn = thingList[i] as Pawn;
				if (pawn != null)
				{
					Job curJob = pawn.CurJob;
					if (curJob != null)
					{
						LocalTargetInfo a = curJob.targetA.Thing;
						LocalTargetInfo a2 = curJob.targetB.Thing;
						LocalTargetInfo a3 = curJob.targetC.Thing;
						if (a == thing || a2 == thing || a3 == thing)
						{
							return pawn;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x000073A4 File Offset: 0x000055A4
		public static Thing FindClosestSittableUnoccupiedThing(Pawn pawn, ThingDef thingDef, float maxSearchDistance = 9999f)
		{
			Predicate<Thing> validator = delegate(Thing t)
			{
				Thing thing = t as Building;
				return thing.def.building.isSittable && pawn.CanReserve(thing, 1, -1, null, false);
			};
			return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(thingDef), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), maxSearchDistance, validator, null, 0, -1, false, RegionType.Set_Passable, false);
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00007408 File Offset: 0x00005608
		public static Thing FindClosestUnoccupiedThing(Pawn pawn, ThingDef thingDef, float maxSearchDistance = 9999f, bool shouldBePowered = false)
		{
			Predicate<Thing> validator = delegate(Thing t)
			{
				Thing thing = t as Building;
				if (shouldBePowered)
				{
					shouldBePowered = thing.IsPowered();
				}
				else
				{
					shouldBePowered = true;
				}
				return pawn.CanReserve(thing, 1, -1, null, false) && shouldBePowered;
			};
			return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(thingDef), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), maxSearchDistance, validator, null, 0, -1, false, RegionType.Set_Passable, false);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00007470 File Offset: 0x00005670
		public static Thing SpecifiedThingAtCellWithDefName(List<Thing> thingsAtCell, string thingDefName)
		{
			for (int i = 0; i < thingsAtCell.Count; i++)
			{
				Thing thing = thingsAtCell[i];
				if (thing.def != null && thing.def.defName == thingDefName)
				{
					return thing;
				}
			}
			return null;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000074B4 File Offset: 0x000056B4
		public static bool LinkedToRequredFacilities(this Thing building, List<ThingDef> requredFacilities)
		{
			if (requredFacilities.Count == 0)
			{
				return true;
			}
			CompAffectedByFacilities compAffectedByFacilities = building.TryGetComp<CompAffectedByFacilities>();
			if (compAffectedByFacilities == null)
			{
				return false;
			}
			List<Thing> linkedFacilitiesListForReading = compAffectedByFacilities.LinkedFacilitiesListForReading;
			if (linkedFacilitiesListForReading.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < requredFacilities.Count; i++)
			{
				ThingDef thingDef = requredFacilities[i];
				for (int j = 0; j < linkedFacilitiesListForReading.Count; j++)
				{
					Thing thing = linkedFacilitiesListForReading[j];
					if (thingDef == thing.def)
					{
						linkedFacilitiesListForReading.Remove(thing);
					}
				}
			}
			return linkedFacilitiesListForReading.Count == 0;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00007540 File Offset: 0x00005740
		public static Building BuildingInPosition(Map map, IntVec3 position, BuildableDef buildableDef = null)
		{
			List<Thing> list = map.thingGrid.ThingsAt(position).ToList<Thing>();
			for (int i = 0; i < list.Count<Thing>(); i++)
			{
				Building building = list[i] as Building;
				if (building != null)
				{
					if (buildableDef != null && building.def.altitudeLayer == buildableDef.altitudeLayer)
					{
						return building;
					}
					if (building.def.holdsRoof || building.def.blockLight)
					{
						return building;
					}
				}
			}
			return null;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000075B8 File Offset: 0x000057B8
		public static List<Pawn> FindPawnsForLovinInRoom(this Pawn pawn, Room room, bool giveThought = false)
		{
			if (room.PsychologicallyOutdoors)
			{
				return null;
			}
			List<Pawn> allPawnsSpawned = room.Map.mapPawns.AllPawnsSpawned;
			bool flag = pawn.story.traits.HasTrait(TraitDefOf.Gay);
			for (int i = 0; i < allPawnsSpawned.Count; i++)
			{
				Pawn pawn2 = allPawnsSpawned[i];
				if (pawn2.Downed || pawn2.Dead || pawn2.health.HasHediffsNeedingTend(false))
				{
					return null;
				}
				if ((pawn2.RaceProps.Humanlike && !pawn.HavePackRelation(pawn2) && !pawn.HaveLoveRelation(pawn2)) || (pawn.relations.OpinionOf(pawn2) < 30 && pawn2.relations.OpinionOf(pawn) < 30))
				{
					if (giveThought)
					{
						pawn.AddMemoryOfDef(ThoughtDefOfAvali.AvaliNeedPrivacy);
					}
					return null;
				}
				if (pawn2.Drafted)
				{
					allPawnsSpawned.Remove(pawn2);
				}
				else
				{
					bool flag2 = pawn2.story.traits.HasTrait(TraitDefOf.Gay);
					if (pawn.gender == pawn2.gender && (!flag || !flag2))
					{
						allPawnsSpawned.Remove(pawn2);
					}
				}
			}
			return allPawnsSpawned;
		}
	}
}
