using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

namespace Korn.Utils
{
    public static class SystemVariablesUtils
    {
        readonly static EnvironmentVariableTarget Target = EnvironmentVariableTarget.Machine;

        public static string GetVariable(string name) => Environment.GetEnvironmentVariable(name, Target);
        public static void SetVariable(string name, string value) => Environment.SetEnvironmentVariable(name, value, Target);

        public static bool IsExistVariable(string name) => GetVariable(name) != null;
        public static void AddVariable(string name, string value) => SetVariable(name, value);
        public static void DeleteVariable(string name) => SetVariable(name, null);

        public static void AppendVariableValue(string name, string value)
        {
            var rawVariable = GetVariable(name);

            if (rawVariable is null)
            {
                AddVariable(name, value);
                return;
            }

            var collection = SystemVariablesCollection.Parse(rawVariable);
            collection.AddIfNotExists(value);
            rawVariable = collection.Serialize();

            SetVariable(name, rawVariable);
        }

        public static void RemoveVariableValue(string name, string value)
        {
            var rawVariable = GetVariable(name);
            if (rawVariable is null)
                return;

            var collection = SystemVariablesCollection.Parse(rawVariable);
            collection.Remove(value);
            rawVariable = collection.Serialize();

            SetVariable(name, rawVariable);
        }

        class SystemVariablesCollection
        {
            SystemVariablesCollection(string[] entries) : this(entries.ToList()) { }
            SystemVariablesCollection(List<string> entries) => Entries = entries;

            public List<string> Entries { get; private set; }

            public void Add(string value) => Entries.Add(value);

            public void AddIfNotExists(string value)
            {
                if (Entries.Contains(value))
                    return;

                Add(value);
            }

            public void Remove(string value) => Entries.Remove(value);

            public string Serialize()
            {
                var length = Entries.Sum(e => e.Length) + Entries.Count;
                var builder = new StringBuilder(length);

                foreach (var entry in Entries)
                {
                    builder.Append(entry);
                    builder.Append(';');
                }

                return builder.ToString();
            }

            public static SystemVariablesCollection Parse(string text)
                => new SystemVariablesCollection(text.SplitEx(';', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}