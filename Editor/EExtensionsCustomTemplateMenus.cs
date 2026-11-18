/*
 *  Copyright 2026 KamKamKee <kamkamkeedev@gmail.com>
 *
 *  This Source Code Form is subject to the terms of the Mozilla Public
 *  License, v. 2.0. If a copy of the MPL was not distributed with this
 *  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *  
 */

using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor;

public static class EExtensionsCustomTemplateMenus
{
    private const string propertyBehaviourTemplate = "Scripting Templates/PropertyBehaviourTemplate.txt";

    [MenuItem("Assets/Create/PropertyBehaviour Script", false, -221)]
    public static void CreatePropertyBehaviourScript()
    {
        string templatePath = GetSiblingFilePath(propertyBehaviourTemplate);
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "New PropertyBehaviour Script.cs");
    }

    private static string GetSiblingFilePath(string fileName, [CallerFilePath] string callerFilePath = "")
    {
        string folderPath = Path.GetDirectoryName(callerFilePath);
        return Path.Combine(folderPath, fileName);
    }
}
