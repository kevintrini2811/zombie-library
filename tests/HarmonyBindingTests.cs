using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using HarmonyLib;

// Read the actual product and installed SDK as metadata: no IL2CPP initializers
// or game methods execute in this offline test.
string root = Path.GetFullPath(args.FirstOrDefault() ?? ".");
string game = Environment.GetEnvironmentVariable("PVZ_GAME_DIR") ?? "/mnt/FastGaming1/SteamLibrary/steamapps/common/PVZ Replanted";
var files = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll")
    .Concat(Directory.GetFiles(Path.Combine(game, "MelonLoader/Il2CppAssemblies"), "*.dll"))
    .Concat(Directory.GetFiles(Path.Combine(game, "MelonLoader/net6"), "*.dll"))
    .Append(Path.Combine(root, "bin/Release/net6.0/ZombieLibrary.dll"));
using var metadata = new MetadataLoadContext(new PathAssemblyResolver(files), "System.Private.CoreLib");
var product = metadata.LoadFromAssemblyPath(Path.Combine(root, "bin/Release/net6.0/ZombieLibrary.dll"));
string[] names = { "NativeArmoredSpawnPatch", "NativeArmoredLoadPatch", "NativeArmoredBobsledDamagePatch",
    "NativeArmorMagnetSelectionPatch", "NativeArmorBoardDisposePatch", "NativeArmorTrackPatch", "ZombieInitializeMarkerResetPatch", "NativeBobsledWearUpdatePatch", "NativeBobsledHitScopePatch", "NativeBobsledCrashedHatPatch", "NativeArmorShieldHitPatch", "NativeArmorShieldResultPatch", "NativeArmorShieldForwardPatch" };
static Type ValueType(Type type) => type.IsByRef ? type.GetElementType()! : type;
static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
int checkedArguments = 0;
foreach (string name in names)
{
    var patch = product.GetType("ZombieLibrary." + name, true)!;
    var annotation = patch.GetCustomAttributesData().Single(x => x.AttributeType.FullName == "HarmonyLib.HarmonyPatch");
    var owner = (Type)annotation.ConstructorArguments[0].Value!;
    string methodName = (string)annotation.ConstructorArguments[1].Value!;
    var original = owner.GetMethods(All).Single(m => m.Name == methodName);
    var arguments = original.GetParameters();
    Type? stateType = null;
    foreach (var hook in patch.GetMethods(All | BindingFlags.DeclaredOnly)
        .Where(m => m.Name is "Prefix" or "Postfix" or "Finalizer"))
    foreach (var parameter in hook.GetParameters())
    {
        string parameterName = parameter.Name!;
        Type actual = ValueType(parameter.ParameterType);
        if (parameterName == "__instance")
            Check(actual.IsAssignableFrom(owner), name + " has wrong instance type");
        else if (parameterName == "__result")
            Check(actual == ValueType(original.ReturnType), name + " has wrong result type");
        else if (parameterName == "__state")
        {
            Check(stateType == null || stateType == actual, name + " has mismatched hook state");
            stateType = actual;
        }
        else
        {
            var argument = parameterName.StartsWith("__", StringComparison.Ordinal)
                && int.TryParse(parameterName[2..], out int position)
                ? arguments.ElementAtOrDefault(position)
                : arguments.SingleOrDefault(p => p.Name == parameterName);
            Check(argument != null, $"{name}.{hook.Name}: SDK argument '{parameterName}' does not exist on {owner.Name}.{methodName}");
            Check(actual == ValueType(argument!.ParameterType), name + " has incompatible argument type");
        }
        ++checkedArguments;
    }
    Console.WriteLine($"SDK binding PASS {name} -> {owner.Name}.{methodName}({string.Join(", ", arguments.Select(p => p.Name))})");
}

// Reconstruct only the binding boundary using names from the real SDK and
// compiled prefix, then let real Harmony bind it on this host. The shim never
// models zombie behavior or pretends to execute IL2CPP rendering.
var originalTrack = metadata.LoadFromAssemblyPath(Path.Combine(game, "MelonLoader/Il2CppAssemblies/Assembly-CSharp.dll"))
    .GetType("Il2CppReloaded.Gameplay.Zombie", true)!.GetMethod("GetTrackPosition")!;
var prefixTrack = product.GetType("ZombieLibrary.NativeArmorTrackPatch", true)!.GetMethod("Prefix", All)!;
var module = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("NativeArmorBindingShim"), AssemblyBuilderAccess.Run)
    .DefineDynamicModule("Main");
var targetType = module.DefineType("ZombieShim", TypeAttributes.Public);
var target = targetType.DefineMethod("GetTrackPosition", MethodAttributes.Public, typeof(TrackPoint), new[] { typeof(string) });
target.DefineParameter(1, ParameterAttributes.None, originalTrack.GetParameters()[0].Name);
var il = target.GetILGenerator(); var result = il.DeclareLocal(typeof(TrackPoint));
il.Emit(OpCodes.Ldloca, result); il.Emit(OpCodes.Initobj, typeof(TrackPoint)); il.Emit(OpCodes.Ldloc, result); il.Emit(OpCodes.Ret);
var runtimeType = targetType.CreateType()!;
var runtimeTarget = runtimeType.GetMethod("GetTrackPosition")!;
MethodInfo Prefix(string typeName, string[] parameterNames)
{
    var type = module.DefineType(typeName, TypeAttributes.Public);
    var method = type.DefineMethod("Prefix", MethodAttributes.Public | MethodAttributes.Static, typeof(bool),
        new[] { typeof(object), typeof(string), typeof(TrackPoint).MakeByRefType() });
    for (int i = 0; i < parameterNames.Length; ++i) method.DefineParameter(i + 1, ParameterAttributes.None, parameterNames[i]);
    var body = method.GetILGenerator(); body.Emit(OpCodes.Ldarg_1);
    body.Emit(OpCodes.Call, typeof(Capture).GetMethod(nameof(Capture.Track))!); body.Emit(OpCodes.Ret);
    return type.CreateType()!.GetMethod("Prefix")!;
}
var harmony = new Harmony("ZombieLibrary.NativeArmorBindingRegression");
try
{
    var broken = Prefix("BrokenPrefix", new[] { "__instance", "theTrackName", "__result" });
    bool rejected = false;
    try { harmony.Patch(runtimeTarget, prefix: new HarmonyMethod(broken)); }
    catch (Exception error) when (error.ToString().Contains("theTrackName", StringComparison.Ordinal)) { rejected = true; }
    Check(rejected, "negative control failed to reproduce old Harmony name-binding error");
    harmony.UnpatchAll(harmony.Id);
    var current = Prefix("ProductPrefix", prefixTrack.GetParameters().Select(p => p.Name!).ToArray());
    harmony.Patch(runtimeTarget, prefix: new HarmonyMethod(current));
    runtimeTarget.Invoke(Activator.CreateInstance(runtimeType), new object[] { "anim_bucket" });
    Check(Capture.Last == "anim_bucket", "Harmony did not deliver actual SDK argument to compiled prefix signature");
}
finally { harmony.UnpatchAll(harmony.Id); }
Console.WriteLine($"PASS {names.Length} compiled patch classes / {checkedArguments} hook arguments against actual SDK; real Harmony rejects old name and binds current positional prefix.");

public struct TrackPoint { public float X, Y; }
public static class Capture { public static string? Last; public static bool Track(string value) { Last = value; return true; } }
