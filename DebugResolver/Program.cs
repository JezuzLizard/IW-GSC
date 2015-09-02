using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Resolver;


namespace DebugResolver
{
    class Program
    {
        static void Main(string[] args)
        {
            var resolver = new BaseResolver(false, Game.Ghosts);

            ushort functionIndex, methodIndex, fieldIndex, stringIndex, OPCodeIndex;
            functionIndex = 100;
            methodIndex = 33523;
            fieldIndex = 50;
            stringIndex = 50;
            OPCodeIndex = 30;

            Opcode testOp = resolver.ResolveOpcodes();

            string testFunction, testMethod, testField, testString, testOPCode;
            testFunction = "setdvar";
            testMethod = "giveweapon";
            testField = "name";
            testString = "j_head";
            testOPCode = "OpReturn";

            Console.WriteLine("Resolver - ResolveStringById");
            Console.WriteLine(String.Format("functionIndex: {0} | methodIndex: {1} | fieldIndex: {2} | stringIndex {3} | OPCodeIndex: {4}", functionIndex, methodIndex, fieldIndex, stringIndex, OPCodeIndex));
            Console.WriteLine();
        
            Console.WriteLine(String.Format("Function: {0}", resolver.ResolveFunctionNameById(functionIndex)));
            Console.WriteLine(String.Format("Method: {0}", resolver.ResolveMethodNameById(methodIndex)));
            Console.WriteLine(String.Format("Field: {0}", resolver.ResolveFieldNameById(fieldIndex)));
            Console.WriteLine(String.Format("String: {0}", resolver.ResolveStringNamegById(fieldIndex)));
            Console.WriteLine(String.Format("OPCode: {0}", resolver.ResolveOpcodeNameById(OPCodeIndex)));

            Console.WriteLine();
            Console.WriteLine("Resolver - ResolveIdByString");
            Console.WriteLine(String.Format("functionName: {0} | methodName: {1} | fieldName: {2} | stringName: {3} | OPCodeName: {4}", testFunction, testMethod, testField, testString, testOPCode));
            Console.WriteLine();

            Console.WriteLine(String.Format("Function: {0}", resolver.ResolveIdOfFunction(testFunction)));
            Console.WriteLine(String.Format("Method: {0}", resolver.ResolveIdOfMethod(testMethod)));
            Console.WriteLine(String.Format("Field: {0}", resolver.ResolveIdOfField(testField)));
            Console.WriteLine(String.Format("String: {0}", resolver.ResolveIdOfString(testString)));
            Console.WriteLine(String.Format("OPCode: {0}", resolver.ResolveIdOfOpcodeString(testOPCode)));

            Console.ReadKey();
        }
    }
}
