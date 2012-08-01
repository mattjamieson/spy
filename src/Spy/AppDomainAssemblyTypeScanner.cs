namespace Spy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Extensions;

    /// <summary>
    /// Scans the app domain for assemblies and types
    /// </summary>
    public static class AppDomainAssemblyTypeScanner
    {
        static AppDomainAssemblyTypeScanner()
        {
            LoadSpyAssemblies();
        }

        /// <summary>
        /// Spy core assembly
        /// </summary>
        private static readonly Assembly SpyAssembly = typeof (AppDomainAssemblyTypeScanner).Assembly;

        /// <summary>
        /// Indicates whether the Spy assemblies have already been loaded
        /// </summary>
        private static bool _spyAssembliesLoaded;

        /// <summary>
        /// Set of rules for ignoring assemblies while scanning through them
        /// </summary>
        private static IEnumerable<Func<Assembly, bool>> _ignoredAssemblies;

        /// <summary>
        /// Gets or sets a set of rules for ignoring assemblies while scanning through them
        /// </summary>
        public static IEnumerable<Func<Assembly, bool>> IgnoredAssemblies
        {
            private get
            {
                return _ignoredAssemblies;
            }
            set
            {
                _ignoredAssemblies = value;
                UpdateTypes();
            }
        }

        /// <summary>
        /// Gets app domain types
        /// </summary>
        public static IEnumerable<Type> Types { get; private set; }

        /// <summary>
        /// Gets app domain assemblies
        /// </summary>
        public static IEnumerable<Assembly> Assemblies { get; private set; }

        /// <summary>
        /// Load assemblies from a the app domain base directory matching a given wildcard.
        /// Assemblies will only be loaded if they aren't already in the appdomain.
        /// </summary>
        /// <param name="wildcardFilename">Wildcard to match the assemblies to load</param>
        public static void LoadAssemblies(string wildcardFilename)
        {
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory, wildcardFilename);
        }

        /// <summary>
        /// Load assemblies from a given directory matching a given wildcard.
        /// Assemblies will only be loaded if they aren't already in the appdomain.
        /// </summary>
        /// <param name="containingDirectory">Directory containing the assemblies</param>
        /// <param name="wildcardFilename">Wildcard to match the assemblies to load</param>
        public static void LoadAssemblies(string containingDirectory, string wildcardFilename)
        {
            UpdateAssemblies();

            var existingAssemblyPaths = Assemblies.Select(a => a.Location).ToArray();

            var unloadedAssemblies = Directory.GetFiles(containingDirectory, wildcardFilename)
                .Where(f => !existingAssemblyPaths.Contains(f, StringComparer.InvariantCultureIgnoreCase));

            foreach (var unloadedAssembly in unloadedAssemblies)
            {
                Assembly.Load(AssemblyName.GetAssemblyName(unloadedAssembly));
            }

            UpdateTypes();
        }

        /// <summary>
        /// Refreshes the type cache if additional assemblies have been loaded.
        /// Note: This is called automatically if assemblies are loaded using LoadAssemblies.
        /// </summary>
        public static void UpdateTypes()
        {
            UpdateAssemblies();
            Types = Assemblies.SelectMany(a => a.SafeGetExportedTypes()).Where(t => !t.IsAbstract).ToArray();
        }

        /// <summary>
        /// Updates the assembly cache from the appdomain
        /// </summary>
        private static void UpdateAssemblies()
        {
            Assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => IgnoredAssemblies == null || !IgnoredAssemblies.Any(i => i(a)))
                .Where(a => !a.IsDynamic && !a.ReflectionOnly)
                .ToArray();
        }

        /// <summary>
        /// Loads any Spy*.dll assemblies in the app domain base directory
        /// </summary>
        public static void LoadSpyAssemblies()
        {
            if (_spyAssembliesLoaded) { return; }
            LoadAssemblies(@"Spy*.dll");
            _spyAssembliesLoaded = true;
        }

        /// <summary>
        /// Gets all types implementing a particular interface/base class
        /// </summary>
        /// <typeparam name="TType">Type to search for</typeparam>
        /// <param name="excludeInternalTypes">Whether to exclude types inside the core Spy assembly</param>
        /// <returns>IEnumerable of types</returns>
        public static IEnumerable<Type> TypesOf<TType>(bool excludeInternalTypes = false)
        {
            var returnTypes = Types.Where(t => typeof (TType).IsAssignableFrom(t));
            if (excludeInternalTypes) { returnTypes = returnTypes.Where(t => t.Assembly != SpyAssembly); }
            return returnTypes;
        }
    }

    public static class AppDomainAssemblyTypeScannerExtensions
    {
        public static IEnumerable<Type> NotOfType<TType>(this IEnumerable<Type> types)
        {
            return types.Where(t => !typeof (TType).IsAssignableFrom(t));
        }

        public static IEnumerable<Type> HavingAttribute<TAttribute>(this IEnumerable<Type> types)
        {
            return types.Where(t => t.GetCustomAttributes(typeof (TAttribute), true).Length > 0);
        }
    }
}