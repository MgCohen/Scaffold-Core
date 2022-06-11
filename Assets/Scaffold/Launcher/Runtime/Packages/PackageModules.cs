﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Scaffold.Launcher
{
    [CreateAssetMenu(menuName = "Package Module")]
    public class PackageModules : ScriptableObject
    {
        public PackagePath Launcher = new PackagePath();
        public List<PackagePath> Modules = new List<PackagePath>();

        public bool ContainModule(string packageName)
        {
            return Modules.Any(package => package.Key == packageName);
        }

        public PackagePath GetPackage(string packageName)
        {
            if (ContainModule(packageName))
            {
                return Modules.First(package => package.Key == packageName);
            }
            else
            {
                Debug.Log($"Missing Package Reference - {packageName}");
                return null;
            }
        }

        public List<PackagePath> GetPackageDependencies(PackagePath package)
        {
            if (!Modules.Contains(package))
            {
                return null;
            }

            List<PackagePath> dependencies = new List<PackagePath>();
            foreach (string packageName in package.dependencies)
            {
                PackagePath dependency = GetPackage(packageName);
                if(dependency == null)
                {
                    continue;
                }

                if (dependencies.Contains(dependency))
                {
                    continue;
                }

                dependencies.Add(dependency);
            }

            return dependencies;
        }
    }
}