using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CaptureMod.Utils;
using HarmonyLib;

namespace CaptureMod.Path
{
    //[HarmonyPatch]
    public static class MultiPath
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            
            var m = AccessTools.GetDeclaredMethods(typeof(CEIOGGEDKAN));
            //m.AddRange(AccessTools.GetDeclaredMethods(typeof(BLCGIFOPMIA)));

            foreach (var methodInfo in m)
            {
                if (!methodInfo.Name.Contains("get_") 
                    && !methodInfo.Name.Contains("set_")
                    && methodInfo.GetParameters().Any(info => info.ParameterType == typeof(IDLLMBGOEPL))
                    //&& methodInfo.ReturnType == typeof(string)
                    )
                {
                    methodInfo.ToString().Log();
                    yield return methodInfo;
                }
            }
          /*  
            var i = 1;
            foreach (var methodInfo in m)
            {
                var args = methodInfo.GetParameters();
                if (!methodInfo.Name.Contains("set_") && args.Any(p => p.ParameterType == typeof(string)))
                {
                    methodInfo.Log();
                    yield return methodInfo;
                }
            }*/

            //yield return AccessTools.Method(typeof(TextRenderer), "JLPGHNHIKKD");
            //yield return AccessTools.Method(typeof(TextRenderer), "KGFABCHDKKJ");
            //yield return AccessTools.Method(typeof(BNAOOLKPIBG), "LINKMGEDLME");
            //yield return AccessTools.Method(typeof(BNAOOLKPIBG), "DNICLBENBHI");

        }
        [HarmonyPostfix]
        public static void Call(MethodBase __originalMethod)
        {
            try
            {
                var normalMethod = __originalMethod as MethodInfo;
                if (normalMethod != null)
                {
                    $"call>{normalMethod.ReturnType.Name} {normalMethod.Name}({normalMethod.GetParameters().Aggregate("",(s, pi) => $"{s}, {pi.ParameterType.Name} {pi.Name}")})".Log();
                }
                else
                {
                    (__originalMethod.Name + " called ").Log();
                }
            }
            catch
            {
                // ignored
            }
        }
    
    }
    
}