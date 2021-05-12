using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Avali
{
	// Token: 0x0200002C RID: 44
	public class CompRangedWeaponAvali : ThingComp
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x000028C4 File Offset: 0x00000AC4
		public CompProperties_WeaponAvali Props
		{
			get
			{
				return (CompProperties_WeaponAvali)this.props;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000EA RID: 234 RVA: 0x000028D1 File Offset: 0x00000AD1
		protected virtual string FloatMenuOptionLabel
		{
			get
			{
				return string.Format(this.Props.useLabel, this.parent.Label);
			}
		}

		// Token: 0x060000EB RID: 235 RVA: 0x00008C98 File Offset: 0x00006E98
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			if (this.shootBinded == null)
			{
				List<VerbProperties> verbs = this.parent.def.Verbs;
				for (int i = 0; i < verbs.Count<VerbProperties>(); i++)
				{
					VerbProperties verbProperties = verbs[i];
					if (verbProperties != null && verbProperties.verbClass == typeof(Verb_ShootBinded))
					{
						this.shootBinded = verbProperties.verbClass;
						break;
					}
				}
			}
			if (this.shootBinded != null && this.workLeft <= 0 && (this.currentBindMode == CompRangedWeaponAvali.bindMode.OwnerPawnOnly.ToString() || this.currentBindMode == CompRangedWeaponAvali.bindMode.AnyPawnInFaction.ToString()) && (this.workLeft <= 0 || this.workLeft > this.Props.workLeft))
			{
				this.workLeft = this.Props.workLeft;
			}
		}

		// Token: 0x060000EC RID: 236 RVA: 0x00008D7C File Offset: 0x00006F7C
		public override void PostDeSpawn(Map map)
		{
			if (this.shootBinded == null)
			{
				List<VerbProperties> verbs = this.parent.def.Verbs;
				for (int i = 0; i < verbs.Count<VerbProperties>(); i++)
				{
					VerbProperties verbProperties = verbs[i];
					if (verbProperties != null && verbProperties.verbClass == typeof(Verb_ShootBinded))
					{
						this.shootBinded = verbProperties.verbClass;
						break;
					}
				}
			}
			if (this.shootBinded != null)
			{
				this.compEquippable = this.parent.GetComp<CompEquippable>();
				if (this.compEquippable == null)
				{
					Log.Error(this.parent + " not have CompEquippable which is requred.", false);
					return;
				}
				if ((this.currentBindMode == CompRangedWeaponAvali.bindMode.OwnerPawnOnly.ToString() || this.currentBindMode == CompRangedWeaponAvali.bindMode.AnyPawnInFaction.ToString()) && (this.workLeft <= 0 || this.workLeft > this.Props.workLeft))
				{
					this.workLeft = this.Props.workLeft;
				}
				if (this.debug)
				{
					Log.Message(string.Concat(new object[]
					{
						this.parent,
						" currentBindMode = ",
						this.currentBindMode,
						"; ownerPawn = ",
						this.ownerPawn
					}), false);
				}
			}
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00008ED0 File Offset: 0x000070D0
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<string>(ref this.currentBindMode, "currentBindMode", CompRangedWeaponAvali.bindMode.OwnerPawnOnly.ToString(), false);
			Scribe_Values.Look<int>(ref this.workLeft, "workLeft", 0, false);
			Scribe_References.Look<Pawn>(ref this.ownerPawn, "ownerPawn", true);
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00008F24 File Offset: 0x00007124
		public override void PostSplitOff(Thing piece)
		{
			CompRangedWeaponAvali comp = ((ThingWithComps)piece).GetComp<CompRangedWeaponAvali>();
			comp.currentBindMode = this.currentBindMode;
			comp.workLeft = this.workLeft;
			comp.ownerPawn = this.ownerPawn;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00008F64 File Offset: 0x00007164
		public override string GetDescriptionPart()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.ownerPawn != null)
			{
				stringBuilder.Append("WeaponOwnerPawn".Translate() + this.ownerPawn.Name);
			}
			else
			{
				stringBuilder.Append("WeaponOwnerPawn".Translate() + "None".Translate());
			}
			if (this.currentBindMode != CompRangedWeaponAvali.bindMode.None.ToString() && this.Props.workLeft > 0 && this.workLeft > 0)
			{
				float num = (float)(this.Props.workLeft - this.workLeft);
				if (num > 0f)
				{
					num = 100f / ((float)this.Props.workLeft / num);
				}
				if (num >= 0.1f)
				{
					stringBuilder.Append("\n\n" + "HackProgress".Translate() + num + "%");
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x00009070 File Offset: 0x00007270
		public override string CompInspectStringExtra()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (this.ownerPawn != null)
			{
				stringBuilder.Append("WeaponOwnerPawn".Translate() + this.ownerPawn.Name);
			}
			else
			{
				stringBuilder.Append("WeaponOwnerPawn".Translate() + "None".Translate());
			}
			if (this.currentBindMode != CompRangedWeaponAvali.bindMode.None.ToString() && this.Props.workLeft > 0 && this.workLeft > 0)
			{
				float num = (float)(this.Props.workLeft - this.workLeft);
				if (num > 0f)
				{
					num = 100f / ((float)this.Props.workLeft / num);
				}
				if (num >= 0.1f)
				{
					stringBuilder.Append("\n" + "HackProgress".Translate() + num + "%");
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000917C File Offset: 0x0000737C
		public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			if (this.debug)
			{
				Log.Message(this.parent + " workLeft = " + this.workLeft, false);
			}
			if (this.workLeft != 0 && selPawn != null && this.ownerPawn != null && !selPawn.Drafted && selPawn.RaceProps.ToolUser)
			{
				if (this.currentBindMode == CompRangedWeaponAvali.bindMode.OwnerPawnOnly.ToString())
				{
					if (this.ownerPawn == selPawn)
					{
						goto IL_620;
					}
				}
				else if (this.currentBindMode == CompRangedWeaponAvali.bindMode.AnyPawnInFaction.ToString() && this.ownerPawn.Faction == selPawn.Faction)
				{
					goto IL_620;
				}
				if (selPawn.CanReserve(this.parent, 1, -1, null, false))
				{
					if (this.Props.hackWorkSkill != null && selPawn.skills.GetSkill(this.Props.hackWorkSkill).Level < this.Props.hackMinSkillLevel)
					{
						yield return new FloatMenuOption("CantHackBindedThing".Translate() + this.Props.hackWorkSkill + "SkillLevelToSmall".Translate() + this.Props.hackMinSkillLevel + ".", null, MenuOptionPriority.Default, null, null, 0f, null, null);
					}
					else if (!selPawn.CanReach(this.parent, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
					{
						yield return new FloatMenuOption(selPawn + "CantReach".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null);
					}
					else
					{
						Predicate<Thing> validator = (Thing t) => t.def == this.Props.workTable && t.IsPowered() && selPawn.CanReserve(t, 1, -1, null, false);
						Thing workTable = GenClosest.ClosestThingReachable(selPawn.Position, selPawn.Map, ThingRequest.ForDef(this.Props.workTable), PathEndMode.Touch, TraverseParms.For(selPawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
						if (workTable == null)
						{
							string facilities = "";
							for (int i = 0; i < this.Props.requredFacilities.Count; i++)
							{
								if (facilities == "")
								{
									facilities = this.Props.requredFacilities[i].LabelCap;
								}
								else
								{
									facilities += ", " + this.Props.requredFacilities[i].LabelCap;
								}
							}
							if (facilities == "")
							{
								yield return new FloatMenuOption(string.Format("RequiredUnoccupiedWorkTable".Translate(), this.Props.workTable.LabelCap) + ".", null, MenuOptionPriority.Default, null, null, 0f, null, null);
							}
							else
							{
								yield return new FloatMenuOption(string.Format("RequiredUnoccupiedWorkTable".Translate(), this.Props.workTable.LabelCap) + string.Format("WithFacilities".Translate(), facilities) + ".", null, MenuOptionPriority.Default, null, null, 0f, null, null);
							}
						}
						else if (!workTable.def.hasInteractionCell)
						{
							Log.Error(workTable + " not have interaction cell.", false);
						}
						else
						{
							FloatMenuOption useopt = new FloatMenuOption(this.FloatMenuOptionLabel, delegate()
							{
								if (selPawn.CanReserveAndReach(this.parent, PathEndMode.Touch, Danger.Deadly, 1, -1, null, false))
								{
									foreach (CompUseEffect compUseEffect in this.parent.GetComps<CompUseEffect>())
									{
										if (compUseEffect.SelectedUseOption(selPawn))
										{
											return;
										}
									}
									this.TryStartUseJob(selPawn, workTable);
								}
							}, MenuOptionPriority.Default, null, null, 0f, null, null);
							yield return useopt;
						}
					}
				}
				else
				{
					yield return new FloatMenuOption(this.FloatMenuOptionLabel + " (" + "Reserved".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
				}
			}
			IL_620:
			yield break;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x000091A0 File Offset: 0x000073A0
		public void TryStartUseJob(Pawn selPawn, Thing workTable)
		{
			Job job = new Job(this.Props.useJob, this.parent, workTable, workTable.OccupiedRect().ClosestCellTo(workTable.InteractionCell));
			job.count = 1;
			selPawn.jobs.TryTakeOrderedJob(job, JobTag.MiscWork);
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x00009200 File Offset: 0x00007400
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			foreach (Gizmo c in base.CompGetGizmosExtra())
			{
				yield return c;
			}
			if (!(this.shootBinded == null))
			{
				yield return new Command_Action
				{
					action = delegate()
					{
						this.EraseOwnerPawnInfo();
					},
					disabled = true,
					disabledReason = "ShouldBeEquiped".Translate(),
					defaultDesc = "EraseOwnerPawnInfoDesc".Translate(),
					icon = ContentFinder<Texture2D>.Get("UI/Commands/EraseOwnerPawnInfo", true),
					hotKey = KeyBindingDefOf.Misc5
				};
			}
			yield break;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00009220 File Offset: 0x00007420
		public void ChangeBindMode()
		{
			if (this.currentBindMode == CompRangedWeaponAvali.bindMode.OwnerPawnOnly.ToString())
			{
				this.currentBindMode = CompRangedWeaponAvali.bindMode.None.ToString();
				this.workLeft = 0;
				return;
			}
			if (this.currentBindMode == CompRangedWeaponAvali.bindMode.AnyPawnInFaction.ToString())
			{
				this.currentBindMode = CompRangedWeaponAvali.bindMode.OwnerPawnOnly.ToString();
				this.workLeft = this.Props.workLeft;
				return;
			}
			this.currentBindMode = CompRangedWeaponAvali.bindMode.AnyPawnInFaction.ToString();
			this.workLeft = this.Props.workLeft;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000028EE File Offset: 0x00000AEE
		public void EraseOwnerPawnInfo()
		{
			this.ownerPawn = null;
		}

		// Token: 0x04000097 RID: 151
		public bool debug;

		// Token: 0x04000098 RID: 152
		public Type shootBinded;

		// Token: 0x04000099 RID: 153
		public string currentBindMode = CompRangedWeaponAvali.bindMode.OwnerPawnOnly.ToString();

		// Token: 0x0400009A RID: 154
		public int workLeft;

		// Token: 0x0400009B RID: 155
		public CompEquippable compEquippable;

		// Token: 0x0400009C RID: 156
		public Pawn ownerPawn;

		// Token: 0x0200002D RID: 45
		public enum bindMode : byte
		{
			// Token: 0x0400009E RID: 158
			None,
			// Token: 0x0400009F RID: 159
			AnyPawnInFaction,
			// Token: 0x040000A0 RID: 160
			OwnerPawnOnly
		}
	}
}
