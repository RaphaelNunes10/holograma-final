using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Collections.Generic;

namespace RogoDigital.Lipsync.AutoSync
{
	public class AutoSync
	{
		private int index;
		private AutoSyncModule[] moduleSequence;
		private ASProcessDelegate onFinishedCallback;
		private bool silentMode = false;
		private ASProcessDelegateData finalData;

		public void RunSequence (AutoSyncModule[] moduleSequence, ASProcessDelegate onFinishedCallback, LipSyncData inputData, bool silent = false)
		{
			index = 0;
			this.moduleSequence = moduleSequence;
			this.onFinishedCallback = onFinishedCallback;
			silentMode = silent;

			finalData = new ASProcessDelegateData(true, "", ClipFeatures.None);

			if (moduleSequence.Length > 0)
			{
				RunModuleSafely(moduleSequence[index], inputData, ProcessNext, silent);
			}
		}

		public void RunModuleSafely (AutoSyncModule module, LipSyncData data, ASProcessDelegate callback, bool silent = false)
		{
			if(finalData == null)
			{
				finalData = new ASProcessDelegateData(true, "", ClipFeatures.None);
			}

			if (AutoSyncUtility.CheckIsClipCompatible(data, module))
			{
				if (!silent)
				{
					if (moduleSequence != null)
					{
						EditorUtility.DisplayProgressBar("AutoSync", string.Format("Processing module {0}/{1}. Please Wait.", index + 1, moduleSequence.Length), index + 1f / (float)moduleSequence.Length);
					}
					else
					{
						EditorUtility.DisplayProgressBar("AutoSync", "Processing module, Please Wait.", 1f);
					}
				}

				module.Process(data, callback);
			}
			else
			{
				EditorUtility.ClearProgressBar();
				callback.Invoke(data, new ASProcessDelegateData(false, "Module failed compatibility check.", ClipFeatures.None));
				moduleSequence = null;
				finalData = null;
			}
		}

		private void ProcessNext (LipSyncData inputData, ASProcessDelegateData data)
		{
			index++;

			finalData.addedFeatures |= data.addedFeatures;

			if (data.success == false)
			{
				finalData.success = data.success;
				finalData.message = data.message;

				if (onFinishedCallback != null)
					onFinishedCallback.Invoke(null, finalData);

				moduleSequence = null;
				finalData = null;
				EditorApplication.Beep();
				EditorUtility.ClearProgressBar();
				return;
			}

			if (index >= moduleSequence.Length)
			{
				if (onFinishedCallback != null)
					onFinishedCallback.Invoke(inputData, finalData);

				moduleSequence = null;
				finalData = null;
				EditorUtility.ClearProgressBar();
				return;
			}
			else
			{
				RunModuleSafely(moduleSequence[index], inputData, ProcessNext, silentMode);
			}
		}

		/// <summary>
		/// Delegate function used for returning data from AutoSync
		/// </summary>
		/// <param name="outputClip"></param>
		/// <param name="success"></param>
		/// <param name="customData"></param>
		public delegate void ASProcessDelegate (LipSyncData outputClip, ASProcessDelegateData data);

		public class ASProcessDelegateData
		{
			public bool success;
			public string message;
			public ClipFeatures addedFeatures;

			public ASProcessDelegateData (bool success, string message, ClipFeatures addedFeatures)
			{
				this.success = success;
				this.message = message;
				this.addedFeatures = addedFeatures;
			}
		}

		#region Obsolete Members
		[Obsolete("Please read lipsync.rogodigital.com/documentation for info on how the new AutoSync methods work.", true)]
		public delegate void AutoSyncDataReadyDelegate (AudioClip clip, List<PhonemeMarker> markers);

		[Obsolete("Please read lipsync.rogodigital.com/documentation for info on how the new AutoSync methods work.", true)]
		public delegate void AutoSyncFailedDelegate (string error);

		[Obsolete("Use AutoSyncUtility.VerifyProgramAtPath instead.")]
		public static bool CheckSoX ()
		{
			string soXPath = EditorPrefs.GetString("LipSync_SoXPath");
			return AutoSyncUtility.VerifyProgramAtPath(soXPath, "SoX");
		}

		[Obsolete("Please read lipsync.rogodigital.com/documentation for info on how the new AutoSync methods work.", true)]
		public static void ProcessAudio (AudioClip clip, AutoSyncDataReadyDelegate dataReadyCallback, AutoSyncFailedDelegate failedCallback, string progressPrefix, AutoSyncOptions options)
		{
		}

		[Obsolete("Please read lipsync.rogodigital.com/documentation for info on how the new AutoSync methods work.", true)]
		public static void ProcessAudio (AudioClip clip, AutoSyncDataReadyDelegate callback, AutoSyncFailedDelegate failedCallback, AutoSyncOptions options)
		{
		}

		[Obsolete("Please read lipsync.rogodigital.com/documentation for info on how the new AutoSync methods work.", true)]
		public static List<PhonemeMarker> CleanupOutput (List<PhonemeMarker> data, float aggressiveness)
		{
			throw new NotImplementedException();
		}

		[Obsolete("Use AutoSyncDataReadyDelegate instead.")]
		public delegate void AutoSyncDataReady (AudioClip clip, List<PhonemeMarker> markers);

		[Obsolete("Use ASPocketSphinxOptionsPreset instead.")]
		public enum AutoSyncOptionsPreset
		{
			Default,
			HighQuality,
		}

		[Obsolete("Use ASPocketSphinxOptions instead.")]
		public struct AutoSyncOptions
		{
			public string languageModel;
			public bool useAudioConversion;
			public bool allphone_ciEnabled;
			public bool backtraceEnabled;
			public int beamExponent;
			public int pbeamExponent;
			public float lwValue;
			public bool doCleanup;
			public float cleanupAggression;

			public AutoSyncOptions (string languageModel, bool useAudioConversion, bool allphone_ciEnabled, bool backtraceEnabled, int beamExponent, int pbeamExponent, float lwValue, bool doCleanup, float cleanupAggression)
			{
				this.languageModel = languageModel;
				this.useAudioConversion = useAudioConversion;
				this.allphone_ciEnabled = allphone_ciEnabled;
				this.backtraceEnabled = backtraceEnabled;
				this.beamExponent = beamExponent;
				this.pbeamExponent = pbeamExponent;
				this.lwValue = lwValue;
				this.doCleanup = doCleanup;
				this.cleanupAggression = cleanupAggression;
			}

			public AutoSyncOptions (string languageModel, bool useAudioConversion, AutoSyncOptionsPreset preset)
			{
				this.languageModel = languageModel;
				this.useAudioConversion = useAudioConversion;

				if (preset == AutoSyncOptionsPreset.HighQuality)
				{
					this.allphone_ciEnabled = false;
					this.backtraceEnabled = true;
					this.beamExponent = -40;
					this.pbeamExponent = -40;
					this.lwValue = 15f;
					this.doCleanup = true;
					this.cleanupAggression = 0.003f;
				}
				else
				{
					this.allphone_ciEnabled = EditorPrefs.GetBool("LipSync_Allphone_ciEnabled", true);
					this.backtraceEnabled = EditorPrefs.GetBool("LipSync_BacktraceEnabled", false);
					this.beamExponent = EditorPrefs.GetInt("LipSync_BeamExponent", -20);
					this.pbeamExponent = EditorPrefs.GetInt("LipSync_PbeamExponent", -20);
					this.lwValue = EditorPrefs.GetFloat("LipSync_LwValue", 2.5f);
					this.doCleanup = EditorPrefs.GetBool("LipSync_DoCleanup", false);
					this.cleanupAggression = EditorPrefs.GetFloat("LipSync_CleanupAggression", 0);
				}
			}
		}
		#endregion
	}
}