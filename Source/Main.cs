using HarmonyLib;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace LineOfSightFix
{
	[StaticConstructorOnStartup]
	public class Main
	{
		static Main()
		{
			var harmony = new Harmony("net.pardeike.harmony.LineOfSightFix");
			harmony.PatchAll();
		}
	}

	[HarmonyPatch(typeof(AttackTargetFinder), nameof(AttackTargetFinder.CanSee))]
	public class AttackTargetFinder_CanSee_Patch
	{
		public static void CalcShootableCellsOfFixed(List<IntVec3> outCells, Thing t)
		{
			if (t is Pawn)
			{
				outCells.Clear();
				outCells.Add(t.Position);
				return;
			}
			else
				ShootLeanUtility.CalcShootableCellsOf(outCells, t);
		}

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var from = SymbolExtensions.GetMethodInfo(() => ShootLeanUtility.CalcShootableCellsOf(null, null));
			var to = SymbolExtensions.GetMethodInfo(() => CalcShootableCellsOfFixed(null, null));
			return instructions.MethodReplacer(from, to);
		}
	}
}
