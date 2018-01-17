
using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace UmbMapper.Tests.Benchmarks
{
    public class TypeCodeSwitch
    {
        [Params(typeof(int), typeof(float), typeof(ulong))]
        public Type Type { get; set; }

        [Benchmark(Description = "MultipleIf", Baseline = true)]
        public string MultipleIf()
        {
            Type type = this.Type;
            if (type == typeof(int))
            {
                return "int";
            }

            if (type == typeof(long))
            {
                return "long";
            }

            if (type == typeof(bool))
            {
                return "bool";
            }

            if (type == typeof(short))
            {
                return "short";
            }

            if (type == typeof(double))
            {
                return "double";
            }

            if (type == typeof(float))
            {
                return "float";
            }

            if (type == typeof(char))
            {
                return "char";
            }

            if (type == typeof(byte))
            {
                return "byte";
            }

            if (type == typeof(sbyte))
            {
                return "sbyte";
            }

            if (type == typeof(uint))
            {
                return "uint";
            }

            if (type == typeof(ushort))
            {
                return "ushort";
            }

            if (type == typeof(ulong))
            {
                return "ulong";
            }

            return string.Empty;
        }

        [Benchmark(Description = "MultipleTypeCodeSwitch")]
        public string MultipleTypeCodeSwitch()
        {
            Type type = this.Type;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int32:
                    return "int";

                case TypeCode.Int64:
                    return "long";

                case TypeCode.Boolean:
                    return "bool";

                case TypeCode.Int16:
                    return "short";

                case TypeCode.Double:
                    return "double";

                case TypeCode.Single:
                    return "float";

                case TypeCode.Char:
                    return "char";

                case TypeCode.Byte:
                    return "byte";

                case TypeCode.SByte:
                    return "sbyte";

                case TypeCode.UInt32:
                    return "uint";

                case TypeCode.UInt16:
                    return "ushort";

                case TypeCode.UInt64:
                    return "ulong";
            }

            return string.Empty;
        }

        [Benchmark(Description = "MultipleTypeNameSwitch")]
        public string MultipleTypeNameSwitch()
        {
            Type type = this.Type;
            switch (type.Name)
            {
                case "Int32":
                    return "int";

                case "Int64":
                    return "long";

                case "Boolean":
                    return "bool";

                case "Int16":
                    return "short";

                case "Double":
                    return "double";

                case "Single":
                    return "float";

                case "Char":
                    return "char";

                case "Byte":
                    return "byte";

                case "SByte":
                    return "sbyte";

                case "UInt32":
                    return "uint";

                case "UInt16":
                    return "ushort";

                case "UInt64":
                    return "ulong";
            }

            return string.Empty;
        }

        [Benchmark(Description = "MultipleIfTypeCodeSwitchMix")]
        public string MultipleIfTypeCodeSwitchMix()
        {
            Type type = this.Type;

            if (type == typeof(int))
            {
                return "int";
            }

            if (type == typeof(long))
            {
                return "long";
            }

            if (type == typeof(bool))
            {
                return "bool";
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Int16:
                    return "short";

                case TypeCode.Double:
                    return "double";

                case TypeCode.Single:
                    return "float";

                case TypeCode.Char:
                    return "char";

                case TypeCode.Byte:
                    return "byte";

                case TypeCode.SByte:
                    return "sbyte";

                case TypeCode.UInt32:
                    return "uint";

                case TypeCode.UInt16:
                    return "ushort";

                case TypeCode.UInt64:
                    return "ulong";
            }

            return string.Empty;
        }
    }
}
