using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;

public class CloudPostBuild : MonoBehaviour
{
	#if UNITY_IOS
	[PostProcessBuild]
	public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject) {

		if (buildTarget == BuildTarget.iOS) {

			// Get pbx
			string projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
			PBXProject pbxProject = new PBXProject ();
			pbxProject.ReadFromFile (projectPath);
			string target = pbxProject.TargetGuidByName ("Unity-iPhone");
			// set enable bitcode off
			pbxProject.SetBuildProperty (target, "ENABLE_BITCODE", "NO");
			pbxProject.WriteToFile (projectPath);

			// Get plist
			string plistPath = pathToBuiltProject + "/Info.plist";
			PlistDocument plist = new PlistDocument ();
			plist.ReadFromString (File.ReadAllText (plistPath));

			// Get root
			PlistElementDict rootDict = plist.root;

			// Change value of CFBundleVersion in Xcode plist
			var buildKey = "CFBundleVersion";
			rootDict.SetString (buildKey, "2.3.4");
			rootDict.SetString ("NSCameraUsageDescription", "Use to assess AR");
			rootDict.SetString ("NSPhotoLibraryUsageDescription", "Use to save screenshot");

			// Write to file
			File.WriteAllText (plistPath, plist.WriteToString ());
		}
	}
	#endif
}