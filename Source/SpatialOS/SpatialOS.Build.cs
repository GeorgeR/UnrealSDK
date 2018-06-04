// Copyright (c) Improbable Worlds Ltd, All Rights Reserved

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using UnrealBuildTool;

public class SpatialOS : ModuleRules
{
#if !WITH_FORWARDED_MODULE_RULES_CTOR
    // Backwards compatibility with Unreal 4.15
    public SpatialOS(TargetInfo Target)
#else
    // Unreal 4.16+
    public SpatialOS(ReadOnlyTargetRules Target) : base(Target)
#endif
    {
        PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;
        bFasterWithoutUnity = true;

        PublicIncludePaths.AddRange(
            new string[]
            {
                "SpatialOS/Public",
                "SpatialOS/Public/WorkerSdk",
                "SpatialOS/Public/Generated/Usr",
                "SpatialOS/Public/Generated/Std",
                "SpatialOS/Public/Generated/UClasses",
            });

        PrivateIncludePaths.AddRange( new string[] { "SpatialOS/Private" });

        PublicDependencyModuleNames.AddRange(
            new string[]
            {
                "Core",
                "Projects",
                "CoreUObject",
                "Engine"
            });

        var CoreSdkLibraryDir = Path.GetFullPath(Path.Combine(ModuleDirectory, "..", "..", "Binaries", "ThirdParty", "Improbable", Target.Platform.ToString()));
        string CoreSdkShared;

        if (!Directory.Exists(CoreSdkLibraryDir))
            throw new DirectoryNotFoundException(CoreSdkLibraryDir);

        if (Target.Platform == UnrealTargetPlatform.Win32 || Target.Platform == UnrealTargetPlatform.Win64)
        {
            CoreSdkShared = Path.Combine(CoreSdkLibraryDir, "CoreSdkDll.dll");
            var lib = Path.Combine(CoreSdkLibraryDir, "CoreSdkDll.lib");
            PublicAdditionalLibraries.AddRange(new[] { lib });

            if (!File.Exists(CoreSdkShared))
                throw new FileNotFoundException(CoreSdkShared);

            if(!File.Exists(lib))
                throw new FileNotFoundException(lib);
        }
        else
        {
            CoreSdkShared = Path.Combine(CoreSdkLibraryDir, "libCoreSdkDll.so");
            PublicAdditionalLibraries.AddRange(new[] { CoreSdkShared });

            if(!File.Exists(CoreSdkShared))
                throw new FileNotFoundException(CoreSdkShared);
        }

        RuntimeDependencies.Add(CoreSdkShared, StagedFileType.NonUFS);

        PublicLibraryPaths.Add(CoreSdkLibraryDir);
        PublicDelayLoadDLLs.Add("CoreSdkDll.dll");

        // Point generated code to the correct API spec.
        PublicDefinitions.Add("IMPROBABLE_DLL_API=SPATIALOS_API");
    }
}
