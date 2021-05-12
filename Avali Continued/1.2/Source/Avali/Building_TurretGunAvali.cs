using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace Avali
{
	// Token: 0x02000028 RID: 40
	[StaticConstructorOnStartup]
	public class Building_TurretGunAvali : Building_Turret
	{
		// Token: 0x060000AE RID: 174 RVA: 0x00002578 File Offset: 0x00000778
		public Building_TurretGunAvali()
		{
			this.top = new TurretTop(this);
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000AF RID: 175 RVA: 0x000025A2 File Offset: 0x000007A2
		public CompEquippable GunCompEq
		{
			get
			{
				return this.gun.TryGetComp<CompEquippable>();
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000B0 RID: 176 RVA: 0x000025AF File Offset: 0x000007AF
		public override LocalTargetInfo CurrentTarget
		{
			get
			{
				return this.currentTargetInt;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x000025B7 File Offset: 0x000007B7
		private bool WarmingUp
		{
			get
			{
				return this.burstWarmupTicksLeft > 0;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x000025C2 File Offset: 0x000007C2
		public override Verb AttackVerb
		{
			get
			{
				return this.GunCompEq.PrimaryVerb;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x000025CF File Offset: 0x000007CF
		private bool PlayerControlled
		{
			get
			{
				return (base.Faction == Faction.OfPlayer || this.MannedByColonist) && !this.MannedByNonColonist;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x000025F1 File Offset: 0x000007F1
		private bool CanSetForcedTarget
		{
			get
			{
				return this.PlayerControlled;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x000025F1 File Offset: 0x000007F1
		private bool CanToggleHoldFire
		{
			get
			{
				return this.PlayerControlled;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x000025F9 File Offset: 0x000007F9
		private bool IsMortar
		{
			get
			{
				return this.def.building.IsMortar;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x0000260B File Offset: 0x0000080B
		private bool IsMortarOrProjectileFliesOverhead
		{
			get
			{
				return this.AttackVerb.ProjectileFliesOverhead() || this.IsMortar;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000B8 RID: 184 RVA: 0x0000775C File Offset: 0x0000595C
		private bool CanExtractShell
		{
			get
			{
				if (!this.PlayerControlled)
				{
					return false;
				}
				CompChangeableProjectile compChangeableProjectile = this.gun.TryGetComp<CompChangeableProjectile>();
				return compChangeableProjectile != null && compChangeableProjectile.Loaded;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00002622 File Offset: 0x00000822
		private bool MannedByColonist
		{
			get
			{
				return this.mannableComp != null && this.mannableComp.ManningPawn != null && this.mannableComp.ManningPawn.Faction == Faction.OfPlayer;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000BA RID: 186 RVA: 0x00002652 File Offset: 0x00000852
		private bool MannedByNonColonist
		{
			get
			{
				return this.mannableComp != null && this.mannableComp.ManningPawn != null && this.mannableComp.ManningPawn.Faction != Faction.OfPlayer;
			}
		}

		// Token: 0x060000BB RID: 187 RVA: 0x0000778C File Offset: 0x0000598C
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			this.powerComp = base.GetComp<CompPowerTrader>();
			this.mannableComp = base.GetComp<CompMannable>();
			this.defaultDelay = this.def.building.turretBurstCooldownTime;
			if (this.turretBurstCooldownTime > -1f)
			{
				this.def.building.turretBurstCooldownTime = this.turretBurstCooldownTime;
				return;
			}
			this.turretBurstCooldownTime = this.defaultDelay;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00002685 File Offset: 0x00000885
		public override void PostMake()
		{
			base.PostMake();
			this.MakeGun();
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00002693 File Offset: 0x00000893
		public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
		{
			base.DeSpawn(mode);
			this.ResetCurrentTarget();
		}

		// Token: 0x060000BE RID: 190 RVA: 0x00007800 File Offset: 0x00005A00
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.burstCooldownTicksLeft, "burstCooldownTicksLeft", 0, false);
			Scribe_Values.Look<int>(ref this.burstWarmupTicksLeft, "burstWarmupTicksLeft", 0, false);
			Scribe_TargetInfo.Look(ref this.currentTargetInt, "currentTarget");
			Scribe_Values.Look<bool>(ref this.holdFire, "holdFire", false, false);
			Scribe_Deep.Look<Thing>(ref this.gun, "gun", new object[0]);
			Scribe_Values.Look<float>(ref this.turretBurstCooldownTime, "turretBurstCooldownTime", -1f, false);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				if (this.gun == null)
				{
					this.MakeGun();
				}
				this.UpdateGunVerbs();
			}
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000026A2 File Offset: 0x000008A2
		public override bool ClaimableBy(Faction by)
		{
			return base.ClaimableBy(by) && (this.mannableComp == null || this.mannableComp.ManningPawn == null) && (this.powerComp == null || !this.powerComp.PowerOn);
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x000078A4 File Offset: 0x00005AA4
		public override void OrderAttack(LocalTargetInfo targ)
		{
			if (!targ.IsValid)
			{
				if (this.forcedTarget.IsValid)
				{
					this.ResetForcedTarget();
				}
				return;
			}
			if ((targ.Cell - base.Position).LengthHorizontal < this.AttackVerb.verbProps.EffectiveMinRange(targ, this))
			{
				Messages.Message("MessageTargetBelowMinimumRange".Translate(), this, MessageTypeDefOf.RejectInput, false);
				return;
			}
			if ((targ.Cell - base.Position).LengthHorizontal > this.AttackVerb.verbProps.range)
			{
				Messages.Message("MessageTargetBeyondMaximumRange".Translate(), this, MessageTypeDefOf.RejectInput, false);
				return;
			}
			if (this.forcedTarget != targ)
			{
				this.forcedTarget = targ;
				if (this.burstCooldownTicksLeft <= 0)
				{
					this.TryStartShootSomething(false);
				}
			}
			if (this.holdFire)
			{
				Messages.Message("MessageAvaliTurretWontFireBecauseHoldFire".Translate(this.def.label), this, MessageTypeDefOf.RejectInput, false);
			}
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x000079C8 File Offset: 0x00005BC8
		public override void Tick()
		{
			base.Tick();
			if (this.forcedTarget.IsValid && !this.CanSetForcedTarget)
			{
				this.ResetForcedTarget();
			}
			if (!this.CanToggleHoldFire)
			{
				this.holdFire = false;
			}
			if (this.forcedTarget.ThingDestroyed)
			{
				this.ResetForcedTarget();
			}
			bool flag = (this.powerComp == null || this.powerComp.PowerOn) && (this.mannableComp == null || this.mannableComp.MannedNow);
			if (flag && base.Spawned)
			{
				this.GunCompEq.verbTracker.VerbsTick();
				if (!this.stunner.Stunned && this.AttackVerb.state != VerbState.Bursting)
				{
					if (this.WarmingUp)
					{
						this.burstWarmupTicksLeft--;
						if (this.burstWarmupTicksLeft == 0)
						{
							this.BeginBurst();
						}
					}
					else
					{
						if (this.burstCooldownTicksLeft > 0)
						{
							this.burstCooldownTicksLeft--;
						}
						if (this.burstCooldownTicksLeft <= 0 && this.IsHashIntervalTick(10))
						{
							this.TryStartShootSomething(true);
						}
					}
					this.top.TurretTopTick();
					return;
				}
			}
			else
			{
				this.ResetCurrentTarget();
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00007AEC File Offset: 0x00005CEC
		protected void TryStartShootSomething(bool canBeginBurstImmediately)
		{
			if (!base.Spawned || (this.holdFire && this.CanToggleHoldFire) || (this.AttackVerb.ProjectileFliesOverhead() && base.Map.roofGrid.Roofed(base.Position)) || !this.AttackVerb.Available())
			{
				this.ResetCurrentTarget();
				return;
			}
			bool isValid = this.currentTargetInt.IsValid;
			if (this.forcedTarget.IsValid)
			{
				this.currentTargetInt = this.forcedTarget;
			}
			else
			{
				this.currentTargetInt = this.TryFindNewTarget();
			}
			if (!isValid && this.currentTargetInt.IsValid)
			{
				SoundDefOf.TurretAcquireTarget.PlayOneShot(new TargetInfo(base.Position, base.Map, false));
			}
			if (!this.currentTargetInt.IsValid)
			{
				this.ResetCurrentTarget();
				return;
			}
			if (this.def.building.turretBurstWarmupTime > 0f)
			{
				this.burstWarmupTicksLeft = this.def.building.turretBurstWarmupTime.SecondsToTicks();
				return;
			}
			if (canBeginBurstImmediately)
			{
				this.BeginBurst();
				return;
			}
			this.burstWarmupTicksLeft = 1;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00007C08 File Offset: 0x00005E08
		protected LocalTargetInfo TryFindNewTarget()
		{
			IAttackTargetSearcher attackTargetSearcher = this.TargSearcher();
			Faction faction = attackTargetSearcher.Thing.Faction;
			float range = this.AttackVerb.verbProps.range;
			Building t;
			if (Rand.Value < 0.5f && this.AttackVerb.ProjectileFliesOverhead() && faction.HostileTo(Faction.OfPlayer) && base.Map.listerBuildings.allBuildingsColonist.Where(delegate(Building x)
			{
				float num = this.AttackVerb.verbProps.EffectiveMinRange(x, this);
				float num2 = (float)x.Position.DistanceToSquared(this.Position);
				return num2 > num * num && num2 < range * range;
			}).TryRandomElement(out t))
			{
				return t;
			}
			TargetScanFlags targetScanFlags = TargetScanFlags.NeedThreat;
			if (!this.AttackVerb.ProjectileFliesOverhead())
			{
				targetScanFlags |= TargetScanFlags.NeedLOSToAll;
				targetScanFlags |= TargetScanFlags.LOSBlockableByGas;
			}
			if (this.AttackVerb.IsIncendiary())
			{
				targetScanFlags |= TargetScanFlags.NeedNonBurning;
			}
			return (Thing)AttackTargetFinder.BestShootTargetFromCurrentPosition(attackTargetSearcher, targetScanFlags, new Predicate<Thing>(this.IsValidTarget), 0f, 9999f);
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x000026DC File Offset: 0x000008DC
		private IAttackTargetSearcher TargSearcher()
		{
			if (this.mannableComp != null && this.mannableComp.MannedNow)
			{
				return this.mannableComp.ManningPawn;
			}
			return this;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00007CFC File Offset: 0x00005EFC
		private bool IsValidTarget(Thing t)
		{
			Pawn pawn = t as Pawn;
			if (pawn != null)
			{
				if (this.AttackVerb.ProjectileFliesOverhead())
				{
					RoofDef roofDef = base.Map.roofGrid.RoofAt(t.Position);
					if (roofDef != null && roofDef.isThickRoof)
					{
						return false;
					}
				}
				if (this.mannableComp == null)
				{
					return !GenAI.MachinesLike(base.Faction, pawn);
				}
				if (pawn.RaceProps.Animal && pawn.Faction == Faction.OfPlayer)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00002700 File Offset: 0x00000900
		protected void BeginBurst()
		{
			this.AttackVerb.TryStartCastOn(this.CurrentTarget, false, true);
			base.OnAttackedTarget(this.CurrentTarget);
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00002722 File Offset: 0x00000922
		protected void BurstComplete()
		{
			this.burstCooldownTicksLeft = this.BurstCooldownTime().SecondsToTicks();
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00002735 File Offset: 0x00000935
		protected float BurstCooldownTime()
		{
			if (this.def.building.turretBurstCooldownTime >= 0f)
			{
				return this.def.building.turretBurstCooldownTime;
			}
			return this.AttackVerb.verbProps.defaultCooldownTime;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00007D7C File Offset: 0x00005F7C
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string inspectString = base.GetInspectString();
			if (!inspectString.NullOrEmpty())
			{
				stringBuilder.AppendLine(inspectString);
			}
			if (this.AttackVerb.verbProps.minRange > 0f)
			{
				stringBuilder.AppendLine("MinimumRange".Translate() + ": " + this.AttackVerb.verbProps.minRange.ToString("F0"));
			}
			if (base.Spawned && this.IsMortarOrProjectileFliesOverhead && base.Position.Roofed(base.Map))
			{
				stringBuilder.AppendLine("CannotFire".Translate() + ": " + "Roofed".Translate().CapitalizeFirst());
			}
			else if (base.Spawned && this.burstCooldownTicksLeft > 0 && this.BurstCooldownTime() > 5f)
			{
				stringBuilder.AppendLine("CanFireIn".Translate() + ": " + this.burstCooldownTicksLeft.ToStringSecondsFromTicks());
			}
			CompChangeableProjectile compChangeableProjectile = this.gun.TryGetComp<CompChangeableProjectile>();
			if (compChangeableProjectile != null)
			{
				if (compChangeableProjectile.Loaded)
				{
					stringBuilder.AppendLine("ShellLoaded".Translate(compChangeableProjectile.LoadedShell.LabelCap, compChangeableProjectile.LoadedShell));
				}
				else
				{
					stringBuilder.AppendLine("ShellNotLoaded".Translate());
				}
			}
			if (this.def.defName == "AvaliTurretLarge")
			{
				stringBuilder.AppendLine("TurretShootDelay".Translate() + ": " + this.def.building.turretBurstCooldownTime);
			}
			if (base.TargetCurrentlyAimingAt.Thing != null)
			{
				stringBuilder.AppendLine("TrackedTarget".Translate() + ": " + base.TargetCurrentlyAimingAt.Label);
			}
			return stringBuilder.ToString().TrimEndNewlines();
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000276F File Offset: 0x0000096F
		public override void Draw()
		{
			this.top.DrawTurret();
			base.Draw();
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00007FA8 File Offset: 0x000061A8
		public override void DrawExtraSelectionOverlays()
		{
			float range = this.AttackVerb.verbProps.range;
			if (range <= 50f)
			{
				GenDraw.DrawRadiusRing(base.Position, range);
			}
			float num = this.AttackVerb.verbProps.EffectiveMinRange(false);
			if (num <= 50f && num > 0.1f)
			{
				GenDraw.DrawRadiusRing(base.Position, num);
			}
			if (this.WarmingUp)
			{
				int degreesWide = (int)((float)this.burstWarmupTicksLeft * 0.5f);
				GenDraw.DrawAimPie(this, this.CurrentTarget, degreesWide, (float)this.def.size.x * 0.5f);
			}
			if (this.forcedTarget.IsValid && (!this.forcedTarget.HasThing || this.forcedTarget.Thing.Spawned))
			{
				Vector3 b;
				if (this.forcedTarget.HasThing)
				{
					b = this.forcedTarget.Thing.TrueCenter();
				}
				else
				{
					b = this.forcedTarget.Cell.ToVector3Shifted();
				}
				Vector3 a = this.TrueCenter();
				b.y = AltitudeLayer.WorldClipper.AltitudeFor();
				a.y = b.y;
				GenDraw.DrawLineBetween(a, b, Building_TurretGun.ForcedTargetLineMat);
			}
		}

		// Token: 0x060000CC RID: 204 RVA: 0x000080D8 File Offset: 0x000062D8
		private void InterfaceChangeTurretShootDelay(float delay)
		{
			SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
			this.turretBurstCooldownTime = this.def.building.turretBurstCooldownTime;
			this.turretBurstCooldownTime += delay;
			this.turretBurstCooldownTime = (float)Math.Round((double)Mathf.Clamp(this.turretBurstCooldownTime, 0f, 1000f), 1);
			MoteMaker.ThrowText(this.TrueCenter() + new Vector3(0.5f, 0f, 0.5f), base.Map, this.turretBurstCooldownTime.ToString(), Color.white, -1f);
			this.def.building.turretBurstCooldownTime = this.turretBurstCooldownTime;
		}

		// Token: 0x060000CD RID: 205 RVA: 0x0000818C File Offset: 0x0000638C
		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo c in base.GetGizmos())
			{
				yield return c;
			}
			if (this.def.defName == "AvaliTurretLarge")
			{
				Command_Action command_Action = new Command_Action();
				command_Action.action = delegate()
				{
					this.InterfaceChangeTurretShootDelay(-1f);
				};
				command_Action.defaultLabel = "-1";
				command_Action.defaultDesc = "CommandDecreaseTurretShootDelayDesc".Translate();
				command_Action.hotKey = KeyBindingDefOf.Misc5;
				command_Action.icon = ContentFinder<Texture2D>.Get("UI/Commands/DecreaseDelay", true);
				yield return command_Action;
				Command_Action command_Action2 = new Command_Action();
				command_Action2.action = delegate()
				{
					this.InterfaceChangeTurretShootDelay(-0.1f);
				};
				command_Action2.defaultLabel = "-0.1";
				command_Action2.defaultDesc = "CommandDecreaseTurretShootDelayDesc".Translate();
				command_Action2.hotKey = KeyBindingDefOf.Misc4;
				command_Action2.icon = ContentFinder<Texture2D>.Get("UI/Commands/DecreaseDelay", true);
				yield return command_Action2;
				Command_Action command_Action3 = new Command_Action();
				command_Action3.action = delegate()
				{
					this.turretBurstCooldownTime = this.defaultDelay;
					this.def.building.turretBurstCooldownTime = this.turretBurstCooldownTime;
					SoundDefOf.Tick_Tiny.PlayOneShotOnCamera(null);
					MoteMaker.ThrowText(this.TrueCenter() + new Vector3(0.5f, 0f, 0.5f), base.Map, this.turretBurstCooldownTime.ToString(), Color.white, -1f);
				};
				command_Action3.defaultLabel = "CommandDefaultTurretShootDelay".Translate();
				command_Action3.defaultDesc = "CommandDefaultTurretShootDelayDesc".Translate();
				command_Action3.hotKey = KeyBindingDefOf.Misc1;
				command_Action3.icon = ContentFinder<Texture2D>.Get("UI/Commands/DefaultDelay", true);
				yield return command_Action3;
				Command_Action command_Action4 = new Command_Action();
				command_Action4.action = delegate()
				{
					this.InterfaceChangeTurretShootDelay(0.1f);
				};
				command_Action4.defaultLabel = "+0.1";
				command_Action4.defaultDesc = "CommandIncreaseTurretShootDelayDesc".Translate();
				command_Action4.hotKey = KeyBindingDefOf.Misc2;
				command_Action4.icon = ContentFinder<Texture2D>.Get("UI/Commands/IncreaseDelay", true);
				yield return command_Action4;
				Command_Action command_Action5 = new Command_Action();
				command_Action5.action = delegate()
				{
					this.InterfaceChangeTurretShootDelay(1f);
				};
				command_Action5.defaultLabel = "+1";
				command_Action5.defaultDesc = "CommandIncreaseTurretShootDelayDesc".Translate();
				command_Action5.hotKey = KeyBindingDefOf.Misc3;
				command_Action5.icon = ContentFinder<Texture2D>.Get("UI/Commands/IncreaseDelay", true);
				yield return command_Action5;
			}
			if (this.CanExtractShell)
			{
				CompChangeableProjectile changeableProjectile = this.gun.TryGetComp<CompChangeableProjectile>();
				yield return new Command_Action
				{
					defaultLabel = "CommandExtractShell".Translate(),
					defaultDesc = "CommandExtractShellDesc".Translate(),
					icon = changeableProjectile.LoadedShell.uiIcon,
					iconAngle = changeableProjectile.LoadedShell.uiIconAngle,
					iconOffset = changeableProjectile.LoadedShell.uiIconOffset,
					iconDrawScale = GenUI.IconDrawScale(changeableProjectile.LoadedShell),
					alsoClickIfOtherInGroupClicked = false,
					action = delegate()
					{
						GenPlace.TryPlaceThing(changeableProjectile.RemoveShell(), this.Position, this.Map, ThingPlaceMode.Near, null, null, default(Rot4));
					}
				};
			}
			if (this.CanSetForcedTarget)
			{
				Command_VerbTarget attack = new Command_VerbTarget();
				attack.defaultLabel = "CommandSetForceAttackTarget".Translate();
				attack.defaultDesc = "CommandSetForceAttackTargetDesc".Translate();
				attack.icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack", true);
				attack.verb = this.AttackVerb;
				attack.hotKey = KeyBindingDefOf.Misc4;
				if (base.Spawned && this.IsMortarOrProjectileFliesOverhead && base.Position.Roofed(base.Map))
				{
					attack.Disable("CannotFire".Translate() + ": " + "Roofed".Translate().CapitalizeFirst());
				}
				yield return attack;
			}
			if (this.forcedTarget.IsValid)
			{
				Command_Action stop = new Command_Action();
				stop.defaultLabel = "CommandStopForceAttack".Translate();
				stop.defaultDesc = "CommandStopForceAttackDesc".Translate();
				stop.icon = ContentFinder<Texture2D>.Get("UI/Commands/Halt", true);
				stop.action = delegate()
				{
					this.ResetForcedTarget();
					SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
				};
				if (!this.forcedTarget.IsValid)
				{
					stop.Disable("CommandStopAttackFailNotForceAttacking".Translate());
				}
				stop.hotKey = KeyBindingDefOf.Misc5;
				yield return stop;
			}
			if (this.CanToggleHoldFire)
			{
				Command_Toggle command_Toggle = new Command_Toggle();
				command_Toggle.defaultLabel = "CommandHoldFire".Translate();
				command_Toggle.defaultDesc = "CommandHoldFireDesc".Translate();
				command_Toggle.icon = ContentFinder<Texture2D>.Get("UI/Commands/HoldFire", true);
				command_Toggle.hotKey = KeyBindingDefOf.Misc6;
				command_Toggle.toggleAction = delegate()
				{
					this.holdFire = !this.holdFire;
					if (this.holdFire)
					{
						this.ResetForcedTarget();
					}
				};
				command_Toggle.isActive = (() => this.holdFire);
				yield return command_Toggle;
			}
			yield break;
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00002782 File Offset: 0x00000982
		private void ResetForcedTarget()
		{
			this.forcedTarget = LocalTargetInfo.Invalid;
			this.burstWarmupTicksLeft = 0;
			if (this.burstCooldownTicksLeft <= 0)
			{
				this.TryStartShootSomething(false);
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x000027A6 File Offset: 0x000009A6
		private void ResetCurrentTarget()
		{
			this.currentTargetInt = LocalTargetInfo.Invalid;
			this.burstWarmupTicksLeft = 0;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x000027BA File Offset: 0x000009BA
		public void MakeGun()
		{
			this.gun = ThingMaker.MakeThing(this.def.building.turretGunDef, null);
			this.UpdateGunVerbs();
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x000081AC File Offset: 0x000063AC
		private void UpdateGunVerbs()
		{
			List<Verb> allVerbs = this.gun.TryGetComp<CompEquippable>().AllVerbs;
			for (int i = 0; i < allVerbs.Count; i++)
			{
				Verb verb = allVerbs[i];
				verb.caster = this;
				verb.castCompleteCallback = new Action(this.BurstComplete);
			}
		}

		// Token: 0x0400006F RID: 111
		private const int TryStartShootSomethingIntervalTicks = 10;

		// Token: 0x04000070 RID: 112
		public float defaultDelay;

		// Token: 0x04000071 RID: 113
		public float turretBurstCooldownTime = -1f;

		// Token: 0x04000072 RID: 114
		protected int burstCooldownTicksLeft;

		// Token: 0x04000073 RID: 115
		protected int burstWarmupTicksLeft;

		// Token: 0x04000074 RID: 116
		protected LocalTargetInfo currentTargetInt = LocalTargetInfo.Invalid;

		// Token: 0x04000075 RID: 117
		private bool holdFire;

		// Token: 0x04000076 RID: 118
		public Thing gun;

		// Token: 0x04000077 RID: 119
		protected TurretTop top;

		// Token: 0x04000078 RID: 120
		protected CompPowerTrader powerComp;

		// Token: 0x04000079 RID: 121
		protected CompMannable mannableComp;

		// Token: 0x0400007A RID: 122
		public static Material ForcedTargetLineMat = MaterialPool.MatFrom(GenDraw.LineTexPath, ShaderDatabase.Transparent, new Color(1f, 0.5f, 0.5f));
	}
}
