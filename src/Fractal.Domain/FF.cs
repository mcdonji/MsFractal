using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Fractal.Common;
using Microsoft.CSharp;

namespace Fractal.Domain
{
    public abstract class Fun
    {
        private MethodInfo methodInfo = null;
        public void SetMethodInfo(string methodName)
        {
            methodInfo = GetType().GetMethod(methodName);
        }

        public object InvokeMethod(string methodName, params object[] p)
        {
           return methodInfo.Invoke(this, p);
        }
    }

    public class FF
    {
        public static string FFUNC = "FFunc";
        public static string HANDLE = "Handle";

        static Dictionary<string, Fun> functions = new Dictionary<string, Fun>();
        static List<string> references = new List<string>() { "System.dll","System.Core.dll", "System.Data.dll", "mscorlib.dll"};
        private static List<string> qualifiedReferences = null; 

        private const string CodeStart = @"using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Fractal.Domain;

namespace Fractal.DynamicDomain
{
    public class DynamicFor";
        private const string CodeInherit = @" : Fun
    {
";
     private const string CodeEnd = @"
    }
}
";

        public static void Clear ()
        {
            functions.Clear();
        }

        public static void Clear(string name)
        {
            if (functions.ContainsKey(name))
            {
                functions.Remove(name);
            }
        }

        public static void AddFunction(string dcName, string functionName, string methodSource)
        {
            string fullHandle = dcName + "_" + functionName;
            if (functions.ContainsKey(fullHandle))
                throw new InvalidOperationException(string.Format("There is already a function called '{0}'",
                    functionName));
            StringBuilder src = new StringBuilder(CodeStart);
            src.Append(fullHandle);
            src.Append(CodeInherit);
            src.Append(methodSource);
            src.Append(CodeEnd);
            Assembly assembly = Compile(src.ToString());
            Fun dynamicBase = assembly.CreateInstance("Fractal.DynamicDomain.DynamicFor" + fullHandle) as Fun;
            dynamicBase.SetMethodInfo(functionName);
            functions[fullHandle] = dynamicBase;
        }

        private static Assembly Compile (string sourceCode)
        {
            CompilerParameters cp = new CompilerParameters();
            if (qualifiedReferences == null)
            {
                qualifiedReferences = CreateQualifiedReferences();
            }

            cp.ReferencedAssemblies.AddRange(qualifiedReferences.ToArray());
//            cp.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().ManifestModule.FullyQualifiedName);
            cp.CompilerOptions = "/target:library /optimize";
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            CompilerResults cr = (new CSharpCodeProvider()).CompileAssemblyFromSource(cp, sourceCode);
            CompilerErrorCollection compilerErrorCollection = cr.Errors;
            if (compilerErrorCollection.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (CompilerError error in compilerErrorCollection)
                {
                    stringBuilder.AppendLine(error.ErrorText);
                }
                throw new ApplicationException("OMG Compiler ERROR!!!" + stringBuilder.ToString());
            }
            return cr.CompiledAssembly;
        }

        private static List<string> CreateQualifiedReferences()
        {
            List<string> list = new List<string>();
            list.AddRange(references);
            string entityFrameworkPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(DbContext)).CodeBase);
            list.Add(entityFrameworkPath.Substring(8) + "\\EntityFramework.dll");
            string fractalPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(DomainConcept)).CodeBase);
            list.Add(fractalPath.Substring(8) + "\\Fractal.Domain.dll");
            string fractalCommonPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Clock)).CodeBase);
            list.Add(fractalCommonPath.Substring(8) + "\\Fractal.Common.dll");
            return list;
        }

        public static void AddFunction (DomainConceptInstance func)
        {
            AddFunction(func.DomainConceptName, func[HANDLE], func[FFUNC]);
        }

        public static object Rf (string handle, params object[] parms)
        {
            string key = "Func_" + handle;
            if (!functions.ContainsKey(key))
            {
                AddFunction(FractalDb.SelectOne("Func", FractalDb.Where(HANDLE, handle)));
            }
            return functions[key].InvokeMethod(handle, parms);
        }


        public static object Rf(DomainConceptInstance functionable, params object[] parms)
        {
            string full_handle = functionable.DomainConceptName + "_" + functionable[HANDLE];
            if (!functions.ContainsKey(full_handle))
            {
                AddFunction(functionable);
            }
            return functions[full_handle].InvokeMethod(functionable[HANDLE], parms);
 
        }


        public static object Rf (string functionableDcName, string handle, params object[] parms)
        {
            string full_handle = functionableDcName + "_" + handle;
            if (!functions.ContainsKey(full_handle))
            {
                AddFunction(FractalDb.SelectOne(functionableDcName, FractalDb.Where(HANDLE, handle)));
            }
            return functions[full_handle].InvokeMethod(handle, parms);
        }
    }
}