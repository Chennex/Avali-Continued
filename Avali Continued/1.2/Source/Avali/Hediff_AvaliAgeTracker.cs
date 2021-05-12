using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Avali
{
	// Token: 0x02000041 RID: 65
	public class Hediff_AvaliAgeTracker : HediffWithComps
	{
		// Token: 0x0600015C RID: 348 RVA: 0x0000D03C File Offset: 0x0000B23C
		public override void Tick()
		{
			base.Tick();
			if (this.pawn.IsHashIntervalTick(1000))
			{
				if (this.hasCaretaker)
				{
					using (IEnumerator<Pawn> enumerator = this.pawn.relations.RelatedPawns.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Pawn pawn = enumerator.Current;
							if (this.pawn.relations.DirectRelationExists(PawnRelationDefOfAvali.Caretaker, pawn) && (pawn.Dead || pawn == null))
							{
								this.pawn.relations.TryRemoveDirectRelation(PawnRelationDefOfAvali.Caretaker, pawn);
								this.hasCaretaker = false;
							}
						}
						goto IL_19E;
					}
				}
				foreach (Pawn pawn2 in this.pawn.Map.mapPawns.FreeHumanlikesOfFaction(this.pawn.Faction))
				{
					if (pawn2.ageTracker.CurLifeStage.reproductive && pawn2 != this.pawn && this.pawn.relations.OpinionOf(pawn2) >= 25 && pawn2.relations.OpinionOf(pawn2) >= 25)
					{
						this.pawn.relations.AddDirectRelation(PawnRelationDefOfAvali.Caretaker, pawn2);
						pawn2.relations.AddDirectRelation(PawnRelationDefOfAvali.Kit, this.pawn);
						PawnRelationDef def = this.pawn.relations.GetDirectRelation(PawnRelationDefOfAvali.Kit, this.pawn).def;
						def.label += this.pawn;
						this.hasCaretaker = true;
					}
				}
				IL_19E:
				float ageBiologicalYearsFloat = this.pawn.ageTracker.AgeBiologicalYearsFloat;
				if (ageBiologicalYearsFloat >= 0f && ageBiologicalYearsFloat < 0.5f)
				{
					this.pawn.workSettings.DisableAll();
					this.pawn.health.AddHediff(HediffDefOfAvali.CantTalk, null, null, null);
					this.pawn.story.traits.allTraits.Clear();
					this.GenerateTraits();
					return;
				}
				if (ageBiologicalYearsFloat >= 0.5f && ageBiologicalYearsFloat < 1f)
				{
					this.GiveTraits();
					if (this.pawnAge < ageBiologicalYearsFloat)
					{
						Hediff hediff = this.pawn.health.hediffSet.hediffs.Find((Hediff x) => x.def == HediffDefOfAvali.CantTalk);
						if (hediff != null)
						{
							this.pawn.health.RemoveHediff(hediff);
						}
						this.pawnAge = this.pawn.ageTracker.AgeBiologicalYearsFloat;
						return;
					}
				}
				else if (ageBiologicalYearsFloat >= 1f && ageBiologicalYearsFloat < 3f)
				{
					this.GiveTraits();
					if (this.pawnAge < ageBiologicalYearsFloat)
					{
						this.pawn.skills.GetSkill(SkillDefOf.Animals).passion.ToString();
						this.pawnAge = this.pawn.ageTracker.AgeBiologicalYearsFloat;
						return;
					}
				}
				else if (ageBiologicalYearsFloat >= 3f && ageBiologicalYearsFloat < 5f)
				{
					this.GiveTraits();
					if (this.pawnAge < ageBiologicalYearsFloat)
					{
						this.pawnAge = this.pawn.ageTracker.AgeBiologicalYearsFloat;
						return;
					}
				}
				else if (ageBiologicalYearsFloat >= 5f)
				{
					this.GiveTraits();
					if (this.pawnAge < ageBiologicalYearsFloat)
					{
						this.pawnAge = this.pawn.ageTracker.AgeBiologicalYearsFloat;
						this.pawn.health.RemoveHediff(this);
					}
				}
			}
		}

		// Token: 0x0600015D RID: 349 RVA: 0x0000D3D0 File Offset: 0x0000B5D0
		public void GiveTraits()
		{
			if (this.pawn.story.traits.allTraits.Count >= 3)
			{
				return;
			}
			float value = this.pawn.records.GetValue(RecordDefOf.TimesInMentalState);
			if (value >= 3f)
			{
				this.ChangeTraitDegreeIfConditionReached(value);
			}
			else if (value >= 6f)
			{
				this.ChangeTraitDegreeIfConditionReached(value);
			}
			else if (value >= 9f)
			{
				this.ChangeTraitDegreeIfConditionReached(value);
			}
			else if (value >= 12f)
			{
				this.ChangeTraitDegreeIfConditionReached(value);
			}
			this.pawn.records.GetValue(RecordDefOf.TimesInMentalState);
			this.GiveTraitIfConditionReached("Kind", RecordDefOf.PrisonersChatted, 1000f, 0);
			this.GiveTraitIfConditionReached("Kind", RecordDefOf.PrisonersRecruited, 10f, 0);
			this.GiveTraitIfConditionReached("Masochist", RecordDefOf.DamageTaken, 500f, 0);
			this.GiveTraitIfConditionReached("TooSmart", RecordDefOf.ResearchPointsResearched, 4000f, 0);
			this.GiveTraitIfConditionReached("FearsFire", RecordDefOf.TimesOnFire, 3f, 0);
			this.GiveTraitIfConditionReached("Nerves", RecordDefOf.TimesInMentalState, 3f, -1);
			this.GiveTraitIfConditionReached("Bloodlust", RecordDefOf.AnimalsSlaughtered, 40f, 0);
			this.GiveTraitIfConditionReached("Bloodlust", RecordDefOf.KillsHumanlikes, 40f, 0);
			this.GiveTraitIfConditionReached("ShootingAccuracy", RecordDefOf.Headshots, 10f, 1);
			this.GiveTraitIfConditionReached("ShootingAccuracy", RecordDefOf.ShotsFired, 1000f, -1);
			this.GiveTraitIfConditionReached("GreenThumb", RecordDefOf.PlantsSown, 100f, 0);
		}

		// Token: 0x0600015E RID: 350 RVA: 0x0000D564 File Offset: 0x0000B764
		public void ChangeTraitDegreeIfConditionReached(float timesInMentalState)
		{
			if (this.pawn.story.traits.DegreeOfTrait(TraitDef.Named("Nerves")) == -1)
			{
				this.pawn.story.traits.allTraits.Remove(new Trait(TraitDefOf.Nerves, -1, false));
				this.pawn.story.traits.GainTrait(new Trait(TraitDefOf.Nerves, -2, false));
			}
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000D5DC File Offset: 0x0000B7DC
		public void GiveTraitIfConditionReached(string traitName, RecordDef record, float recordsCount, int degree)
		{
			if (this.pawn.story.traits.allTraits.Count >= 3)
			{
				return;
			}
			if (!this.pawn.story.traits.HasTrait(TraitDef.Named(traitName)) && this.pawn.records.GetValue(record) > recordsCount)
			{
				this.pawn.story.traits.GainTrait(new Trait(TraitDef.Named(traitName), degree, false));
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x0000D65C File Offset: 0x0000B85C
		public void GenerateTraits()
		{
			if (Rand.Chance(0.95f))
			{
				this.pawn.story.traits.GainTrait(new Trait(TraitDef.Named("Abrasive"), 0, false));
			}
			if (Rand.Chance(0.4f))
			{
				this.pawn.story.traits.GainTrait(new Trait(TraitDef.Named("FearsFire"), 0, false));
			}
			this.GiveTraitWithChance("Brawler");
			this.GiveTraitWithChance("Bloodlust");
			this.GiveTraitWithChance("CreepyBreathing");
			this.GiveTraitWithChance("FastLearner");
			this.GiveTraitWithChance("Ascetic");
			this.GiveTraitWithChance("Gay");
			this.GiveTraitWithChance("GreenThumb");
			this.GiveTraitWithChance("NaturalMood");
			this.GiveTraitWithChance("Nudist");
			this.GiveTraitWithChance("PsychicSensitivity");
			this.GiveTraitWithChance("Psychopath");
			this.GiveTraitWithChance("TooSmart");
			this.GiveTraitWithChance("Nimble");
			this.GiveTraitWithChance("Pyromaniac");
			this.GiveTraitWithChance("SuperImmune");
			this.GiveTraitWithChance("FastLearner");
			this.GiveTraitWithChance("Masochist");
			this.GiveTraitWithChance("NightOwl");
			this.GiveTraitWithChance("Wimp");
			this.GiveTraitWithChanceInRange("SpeedOffset", -1, 2);
			this.GiveTraitWithChanceInRange("DrugDesire", -1, 2);
			this.GiveTraitWithChanceInRange("NaturalMood", -2, 2);
			this.GiveTraitWithChanceInRange("Nerves", -2, 2);
			this.GiveTraitWithChanceInRange("Neurotic", -2, 2);
			this.GiveTraitWithChanceInRange("Industriousness", -2, 2);
			this.GiveTraitWithChanceInRange("PsychicSensitivity", -2, 2);
			this.GiveTraitWithChanceInRange("Beauty", 1, 2);
			if (this.pawn.story.traits.HasTrait(TraitDefOf.Abrasive))
			{
				if (Rand.Chance(0.25f))
				{
					this.pawn.story.traits.GainTrait(new Trait(TraitDefOf.Kind, 0, false));
				}
				if (this.pawn.story.traits.allTraits.Count >= 3)
				{
					return;
				}
			}
			this.GiveTraitWithChanceIfNull("DislikesWomen", "DislikesMen");
			this.GiveTraitWithChanceIfNull("DislikesMen", "DislikesWomen");
		}

		// Token: 0x06000161 RID: 353 RVA: 0x0000D894 File Offset: 0x0000BA94
		public void GiveTraitWithChance(string traitName)
		{
			if (this.pawn.story.traits.allTraits.Count >= 3)
			{
				return;
			}
			if (Rand.Chance(0.01f))
			{
				this.pawn.story.traits.GainTrait(new Trait(TraitDef.Named(traitName), 0, false));
			}
		}

		// Token: 0x06000162 RID: 354 RVA: 0x0000D8F0 File Offset: 0x0000BAF0
		public void GiveTraitWithChanceInRange(string traitName, int min, int max)
		{
			if (this.pawn.story.traits.allTraits.Count >= 3)
			{
				return;
			}
			if (Rand.Chance(0.01f))
			{
				int num = Rand.Range(min, max);
				if (num == 0)
				{
					if (Rand.Chance(0.5f))
					{
						num = -1;
					}
					else
					{
						num = 1;
					}
				}
				this.pawn.story.traits.GainTrait(new Trait(TraitDef.Named(traitName), num, false));
			}
		}

		// Token: 0x06000163 RID: 355 RVA: 0x0000D968 File Offset: 0x0000BB68
		public void GiveTraitWithChanceIfNull(string nullTrait, string traitToGive)
		{
			if (this.pawn.story.traits.allTraits.Count >= 3)
			{
				return;
			}
			if (!this.pawn.story.traits.HasTrait(TraitDef.Named(nullTrait)) && Rand.Chance(0.01f))
			{
				this.pawn.story.traits.GainTrait(new Trait(TraitDef.Named(traitToGive), 0, false));
			}
		}

		// Token: 0x0400012F RID: 303
		public const int pawnStateCheckInterval = 1000;

		// Token: 0x04000130 RID: 304
		public const float chance = 0.01f;

		// Token: 0x04000131 RID: 305
		public const int opinionNeeded = 25;

		// Token: 0x04000132 RID: 306
		private float pawnAge;

		// Token: 0x04000133 RID: 307
		private bool hasCaretaker;
	}
}
