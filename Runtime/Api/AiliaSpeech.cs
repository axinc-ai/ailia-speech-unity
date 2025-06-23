/* ailia Speech Unity Plugin Native Interface */
/* Copyright 2022 - 2025 AXELL CORPORATION */

using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Runtime.InteropServices;

using ailia;
using ailiaAudio;
using ailiaTokenizer;

namespace ailiaSpeech{
public class AiliaSpeech
{
	/* Native Binary 定義 */

	#if (UNITY_IPHONE && !UNITY_EDITOR) || (UNITY_WEBGL && !UNITY_EDITOR)
		public const String LIBRARY_NAME="__Internal";
	#else
		#if (UNITY_ANDROID && !UNITY_EDITOR) || (UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX)
			public const String LIBRARY_NAME="ailia_speech";
		#else
			public const String LIBRARY_NAME="ailia_speech";
		#endif
	#endif

	/****************************************************************
	* モデルタイプ定義
	**/

	/**
	* \~japanese
	* @def AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_TINY
	* @brief Whisper Tiny model
	*
	* \~english
	* @def AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_TINY
	* @brief Whisper Tiny model
	*/
	public const Int32 AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_TINY = (0);

	/**
	* \~japanese
	* @def AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_BASE
	* @brief Whisper Base model
	*
	* \~english
	* @def AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_BASE
	* @brief Whisper Base model
	*/
	public const Int32 AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_BASE = (1);

	/**
	* \~japanese
	* @def AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_SMALL
	* @brief Whisper Small model
	*
	* \~english
	* @def AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_SMALL
	* @brief Whisper Small model
	*/
	public const Int32 AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_SMALL = (2);

	/**
	* \~japanese
	* @def AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_MEDIUM
	* @brief Whisper Medium model
	*
	* \~english
	* @def AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_MEDIUM
	* @brief Whisper Medium model
	*/
	public const Int32 AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_MEDIUM = (3);

	/**
	* \~japanese
	* @def AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_LARGE
	* @brief Whisper Large model
	*
	* \~english
	* @def AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_LARGE
	* @brief Whisper Large model
	*/
	public const Int32 AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_LARGE = (4);

	/**
	* \~japanese
	* @def AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_LARGE_V3
	* @brief Whisper Large V3 model
	*
	* \~english
	* @def AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_LARGE_V3
	* @brief Whisper Large V3 model
	*/
	public const Int32 AILIA_SPEECH_MODEL_TYPE_WHISPER_MULTILINGUAL_LARGE_V3 = (5);

	/****************************************************************
	* タスク定義
	**/

	/**
	* \~japanese
	* @def AILIA_SPEECH_TASK_TRANSCRIBE
	* @brief Transcribe mode
	*
	* \~english
	* @def AILIA_SPEECH_TASK_TRANSCRIBE
	* @brief Transcribe mode
	*/
	public const Int32 AILIA_SPEECH_TASK_TRANSCRIBE = (0);

	/**
	* \~japanese
	* @def AILIA_SPEECH_TASK_TRANSLATE
	* @brief Translate mode
	*
	* \~english
	* @def AILIA_SPEECH_TASK_TRANSLATE
	* @brief Translate mode
	*/
	public const Int32 AILIA_SPEECH_TASK_TRANSLATE = (1);

	/****************************************************************
	* 制約定義
	**/

	/**
	* \~japanese
	* @def AILIA_SPEECH_CONSTRAINT_CHARACTERS
	* @brief 文字の制約を行います。
	*
	* \~english
	* @def AILIA_SPEECH_CONSTRAINT_CHARACTERS
	* @brief Constraint by characters
	*/
	public const Int32 AILIA_SPEECH_CONSTRAINT_CHARACTERS = (0);

	/**
	* \~japanese
	* @def AILIA_SPEECH_CONSTRAINT_WORDS
	* @brief 単語の制約を行います。単語はカンマで区切ります。
	*
	* \~english
	* @def AILIA_SPEECH_CONSTRAINT_WORDS
	* @brief Constraint by words. Separate words with commas.
	*/
	public const Int32 AILIA_SPEECH_CONSTRAINT_WORDS = (1);

	/****************************************************************
	* フラグ定義
	**/

	/**
	* \~japanese
	* @def AILIA_SPEECH_FLAG_NONE
	* @brief Default flag
	*
	* \~english
	* @def AILIA_SPEECH_FLAG_NONE
	* @brief Default flag
	*/
	public const Int32 AILIA_SPEECH_FLAG_NONE = (0);

	/**
	* \~japanese
	* @def AILIA_SPEECH_FLAG_LIVE
	* @brief Live mode
	*
	* \~english
	* @def AILIA_SPEECH_FLAG_LIVE
	* @brief Live mode
	*/
	public const Int32 AILIA_SPEECH_FLAG_LIVE = (1);

	/****************************************************************
	* VAD定義
	**/

	/**
	* \~japanese
	* @def AILIA_SPEECH_VAD_TYPE_SILERO
	* @brief SileroVAD
	*
	* \~english
	* @def AILIA_SPEECH_VAD_TYPE_SILERO
	* @brief SileroVAD
	*/
	public const Int32 AILIA_SPEECH_VAD_TYPE_SILERO = (0);

	/****************************************************************
	* 辞書定義
	**/

	/**
	* \~japanese
	* @def AILIA_SPEECH_DICTIONARY_TYPE_REPLACE
	* @brief 置換辞書
	*
	* \~english
	* @def AILIA_SPEECH_DICTIONARY_TYPE_REPLACE
	* @brief Dictionary for replace
	*/
	public const Int32 AILIA_SPEECH_DICTIONARY_TYPE_REPLACE = (0);

	/****************************************************************
	* 後処理定義
	**/

	/**
	* \~japanese
	* @def AILIA_SPEECH_POST_PROCESS_TYPE_T5
	* @brief T5
	*
	* \~english
	* @def AILIA_SPEECH_POST_PROCESS_TYPE_T5
	* @brief T5
	*/
	public const Int32 AILIA_SPEECH_POST_PROCESS_TYPE_T5 = (0);

	/**
	* \~japanese
	* @def AILIA_SPEECH_POST_PROCESS_TYPE_FUGUMT_EN_JA
	* @brief FuguMT EN JA
	*
	* \~english
	* @def AILIA_SPEECH_POST_PROCESS_TYPE_FUGUMT_EN_JA
	* @brief FuguMT EN JA
	*/
	public const Int32 AILIA_SPEECH_POST_PROCESS_TYPE_FUGUMT_EN_JA = (1);

	/**
	* \~japanese
	* @def AILIA_SPEECH_POST_PROCESS_TYPE_FUGUMT_JA_EN
	* @brief FuguMT JA EN
	*
	* \~english
	* @def AILIA_SPEECH_POST_PROCESS_TYPE_FUGUMT_JA_EN
	* @brief FuguMT JA EN
	*/
	public const Int32 AILIA_SPEECH_POST_PROCESS_TYPE_FUGUMT_JA_EN = (2);

	/****************************************************************
	* APIコールバック定義
	**/

	// API mapping via C#
	public delegate int ailiaCallbackAudioGetFrameLen(ref Int32 a, int b, int c, int d, int e);
	public delegate int ailiaCallbackAudioGetMelSpectrogram(IntPtr a, IntPtr b, int c, int d, int e, int f, int g, int h, int i, int j, float k, int l, float m, float n, int o, int p, int q);
	public delegate int ailiaCallbackAudioResample(IntPtr a, IntPtr b, int c, int d, int e, int f);
	public delegate int ailiaCallbackAudioGetResampleLen(IntPtr a, int b, int c, int d);

	public delegate int ailiaCallbackTokenizerCreate(IntPtr a, int b, int c);
	public delegate int ailiaCallbackTokenizerOpenModelFileA(IntPtr a, IntPtr b);
	public delegate int ailiaCallbackTokenizerOpenModelFileW(IntPtr a, IntPtr b);
	public delegate int ailiaCallbackTokenizerEncode(IntPtr a, IntPtr b);
	public delegate int ailiaCallbackTokenizerGetTokenCount(IntPtr a, IntPtr b);
	public delegate int ailiaCallbackTokenizerGetTokens(IntPtr a, IntPtr b, uint c);
	public delegate int ailiaCallbackTokenizerDecode(IntPtr a, IntPtr b, uint c);
	public delegate int ailiaCallbackTokenizerGetTextLength(IntPtr a, IntPtr b);
	public delegate int ailiaCallbackTokenizerGetText(IntPtr a, IntPtr b, uint c);
	public delegate void ailiaCallbackTokenizerDestroy(IntPtr a);
	public delegate int ailiaCallbackTokenizerUtf8ToUtf32(IntPtr a, IntPtr b, IntPtr c, uint d);
	public delegate int ailiaCallbackTokenizerUtf32ToUtf8(IntPtr a, IntPtr b, uint c);

	public delegate int ailiaCallbackCreate(IntPtr a, int b, int c);
	public delegate int ailiaCallbackOpenWeightFileA(IntPtr a, IntPtr b);
	public delegate int ailiaCallbackOpenWeightFileW(IntPtr a, IntPtr b);
	public delegate int ailiaCallbackOpenWeightMem(IntPtr a, IntPtr b, UInt32 c);
	public delegate int ailiaCallbackSetMemoryMode(IntPtr a, UInt32 b);
	public delegate void ailiaCallbackDestroy(IntPtr a);
	public delegate int ailiaCallbackUpdate(IntPtr a);
	public delegate int ailiaCallbackGetBlobIndexByInputIndex(IntPtr a, IntPtr b, uint c);
	public delegate int ailiaCallbackGetBlobIndexByOutputIndex(IntPtr a, IntPtr b, uint c);
	public delegate int ailiaCallbackGetBlobData(IntPtr a, IntPtr b, uint c, uint d);
	public delegate int ailiaCallbackSetInputBlobData(IntPtr a, IntPtr b, uint c, uint d);
	public delegate int ailiaCallbackSetInputBlobShape(IntPtr a, Ailia.AILIAShape  b, uint c, uint d);
	public delegate int ailiaCallbackGetBlobShape(IntPtr a, IntPtr b, uint c, uint d);
	public delegate IntPtr ailiaCallbackGetErrorDetail(IntPtr a);

	public delegate int ailiaCallbackCopyBlobData(IntPtr a, uint b, IntPtr c, uint d);
	public delegate int ailiaCallbackGetEnvironment(IntPtr a, uint b, uint d);

	// Intermediate
	public delegate int ailiaIntermediateCallback(IntPtr handle, IntPtr text);

	/****************************************************************
	 *  引数をスルーする系のAPIに変換
	 **/

	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaCreate(IntPtr net, int env_id, int num_thread);
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaOpenWeightFileW(IntPtr net, IntPtr path);
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaOpenWeightFileA(IntPtr net, IntPtr path);
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaOpenWeightMem(IntPtr net, IntPtr buf, uint buf_size);   
	[DllImport(AiliaAudio.LIBRARY_NAME)]
	public static extern int ailiaAudioGetMelSpectrogram(IntPtr dst, IntPtr src, int sample_n, int sample_rate, int fft_n, int hop_n, int win_n, int win_type, int max_frame_n, int center, float power, int fft_norm_type, float f_min, float f_max, int mel_n, int mel_norm_type, int mel_formula);
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaGetBlobIndexByInputIndex(IntPtr net, IntPtr blob_idx, UInt32 input_blob_idx);
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaGetBlobIndexByOutputIndex(IntPtr net, IntPtr blob_idx, UInt32 output_blob_idx);
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaGetBlobShape(IntPtr net, IntPtr shape, UInt32 blob_idx, UInt32 version);
	[DllImport(Ailia.LIBRARY_NAME)]
	public static extern int ailiaGetEnvironment(IntPtr net, UInt32 env_idx, UInt32 version);
    [DllImport(AiliaTokenizer.LIBRARY_NAME)]
    public static extern int ailiaTokenizerCreate(IntPtr net, int type, int flags);
    [DllImport(AiliaTokenizer.LIBRARY_NAME)]
    public static extern int ailiaTokenizerOpenModelFileA(IntPtr net, IntPtr utf8);
    [DllImport(AiliaTokenizer.LIBRARY_NAME)]
    public static extern int ailiaTokenizerOpenModelFileW(IntPtr net, IntPtr utf16);
    [DllImport(AiliaTokenizer.LIBRARY_NAME)]
    public static extern int ailiaTokenizerEncode(IntPtr net, IntPtr utf8);
    [DllImport(AiliaTokenizer.LIBRARY_NAME)]
    public static extern int ailiaTokenizerGetTokenCount(IntPtr net, IntPtr count);
    [DllImport(AiliaTokenizer.LIBRARY_NAME)]
    public static extern int ailiaTokenizerGetTextLength(IntPtr net, IntPtr len);
    [DllImport(AiliaTokenizer.LIBRARY_NAME)]
	public static extern int ailiaTokenizerUtf8ToUtf32(IntPtr a, IntPtr b, IntPtr c, uint d);
    [DllImport(AiliaTokenizer.LIBRARY_NAME)]
	public static extern int ailiaTokenizerUtf32ToUtf8(IntPtr a, IntPtr b, uint c);
    [DllImport(AiliaAudio.LIBRARY_NAME)]
	public static extern int ailiaAudioResample(IntPtr a, IntPtr b, int c, int d, int e, int f);
    [DllImport(AiliaAudio.LIBRARY_NAME)]
	public static extern int ailiaAudioGetResampleLen(IntPtr a, int b, int c, int d);

	/****************************************************************
	 *  IL2CPP用
	 *  以下のエラーを抑制するために、一度、C#空間でブリッジする
	 *  NotSupportedException: To marshal a managed method, please add an attribute named 'MonoPInvokeCallback' to the method definition.
	 **/

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackAudioGetFrameLen))]
	public static int ailiaCallbackAudioGetFrameLenBridge (ref Int32 a, int b, int c, int d, int e) {
		return AiliaAudio.ailiaAudioGetFrameLen(ref a, b, c, d, e);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackAudioGetMelSpectrogram))]
	public static int ailiaCallbackAudioGetMelSpectrogramBridge (IntPtr a, IntPtr b, int c, int d, int e, int f, int g, int h, int i, int j, float k, int l, float m, float n, int o, int p, int q) {
		return ailiaAudioGetMelSpectrogram(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackAudioResample))]
	public static int ailiaCallbackAudioResampleBridge (IntPtr a, IntPtr b, int c, int d, int e, int f) {
		return ailiaAudioResample(a, b, c, d, e, f);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackAudioGetResampleLen))]
	public static int ailiaCallbackAudioGetResampleLenBridge (IntPtr a, int b, int c, int d) {
		return ailiaAudioGetResampleLen(a, b, c, d);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackTokenizerCreate))]
	public static int ailiaCallbackTokenizerCreateBridge (IntPtr a, int b, int c) {
		return ailiaTokenizerCreate(a, b, c);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackTokenizerOpenModelFileA))]
	public static int ailiaCallbackTokenizerOpenModelFileABridge (IntPtr a, IntPtr b) {
		return ailiaTokenizerOpenModelFileA(a, b);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackTokenizerOpenModelFileW))]
	public static int ailiaCallbackTokenizerOpenModelFileWBridge (IntPtr a, IntPtr b) {
		return ailiaTokenizerOpenModelFileW(a, b);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackTokenizerEncode))]
	public static int ailiaCallbackTokenizerEncodeBridge (IntPtr a, IntPtr b){
		return ailiaTokenizerEncode(a, b);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackTokenizerGetTokenCount))]
	public static int ailiaCallbackTokenizerGetTokenCountBridge (IntPtr a, IntPtr b){
		return ailiaTokenizerGetTokenCount(a, b);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackTokenizerGetTokens))]
	public static int ailiaCallbackTokenizerGetTokensBridge (IntPtr a, IntPtr b, uint c){
		return AiliaTokenizer.ailiaTokenizerGetTokens(a, b, c);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackTokenizerDecode))]
	public static int ailiaCallbackTokenizerDecodeBridge (IntPtr a, IntPtr b, uint c){
		return AiliaTokenizer.ailiaTokenizerDecode(a, b, c);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackTokenizerGetTextLength))]
	public static int ailiaCallbackTokenizerGetTextLengthBridge (IntPtr a, IntPtr b){
		return ailiaTokenizerGetTextLength(a, b);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackTokenizerGetText))]
	public static int ailiaCallbackTokenizerGetTextBridge (IntPtr a, IntPtr b, uint c){
		return AiliaTokenizer.ailiaTokenizerGetText(a, b, c);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackTokenizerDestroy))]
	public static void ailiaCallbackTokenizerDestroyBridge (IntPtr a){
		AiliaTokenizer.ailiaTokenizerDestroy(a);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackTokenizerUtf8ToUtf32))]
	public static int ailiaCallbackTokenizerUtf8ToUtf32Bridge (IntPtr a, IntPtr b, IntPtr c, uint d){
		return ailiaTokenizerUtf8ToUtf32(a, b, c, d);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackTokenizerUtf32ToUtf8))]
	public static int ailiaCallbackTokenizerUtf32ToUtf8Bridge (IntPtr a, IntPtr b, uint c){
		return ailiaTokenizerUtf32ToUtf8(a, b, c);
	
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackCreate))]
	public static int ailiaCallbackCreateBridge (IntPtr a, int b, int c) {
		return ailiaCreate(a, b, c);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackOpenWeightFileA))]
	public static int ailiaCallbackOpenWeightFileABridge (IntPtr a, IntPtr b) {
		return ailiaOpenWeightFileA(a, b);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackOpenWeightFileW))]
	public static int ailiaCallbackOpenWeightFileWBridge (IntPtr a, IntPtr b) {
		return ailiaOpenWeightFileW(a, b);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackOpenWeightMem))]
	public static int ailiaCallbackOpenWeightMemBridge (IntPtr a, IntPtr b, uint c) {
		return ailiaOpenWeightMem(a, b, c);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackSetMemoryMode))]
	public static int ailiaCallbackSetMemoryModeBridge (IntPtr a, uint b) {
		return Ailia.ailiaSetMemoryMode(a, b);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackDestroy))]
	public static void ailiaCallbackDestroyBridge (IntPtr a) {
		Ailia.ailiaDestroy(a);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackUpdate))]
	public static int ailiaCallbackUpdateBridge (IntPtr a) {
		return Ailia.ailiaUpdate(a);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackGetBlobIndexByInputIndex))]
	public static int ailiaCallbackGetBlobIndexByInputIndexBridge (IntPtr a, IntPtr b, uint c) {
		return ailiaGetBlobIndexByInputIndex(a, b, c);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackGetBlobIndexByOutputIndex))]
	public static int ailiaCallbackGetBlobIndexByOutputIndexBridge (IntPtr a, IntPtr b, uint c) {
		return ailiaGetBlobIndexByOutputIndex(a, b, c);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackGetBlobData))]
	public static int ailiaCallbackGetBlobDataBridge (IntPtr a, IntPtr b, uint c, uint d) {
		return Ailia.ailiaGetBlobData(a, b, c, d);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackSetInputBlobData))]
	public static int ailiaCallbackSetInputBlobDataBridge (IntPtr a, IntPtr b, uint c, uint d) {
		return Ailia.ailiaSetInputBlobData(a, b, c, d);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackSetInputBlobShape))]
	public static int ailiaCallbackSetInputBlobShapeBridge (IntPtr a, Ailia.AILIAShape  b, uint c, uint d) {
		return Ailia.ailiaSetInputBlobShape(a, b, c, d);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackGetBlobShape))]
	public static int ailiaCallbackGetBlobShapeBridge (IntPtr a, IntPtr b, uint c, uint d) {
		return ailiaGetBlobShape(a, b, c, d);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackGetErrorDetail))]
	public static IntPtr ailiaCallbackGetErrorDetailBridge (IntPtr a) {
		return Ailia.ailiaGetErrorDetail(a);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackCopyBlobData))]
	public static int ailiaCallbackCopyBlobDataBridge (IntPtr a, uint b, IntPtr c, uint d) {
		return Ailia.ailiaCopyBlobData(a, b, c, d);
	}

	[AOT.MonoPInvokeCallback(typeof(ailiaCallbackGetEnvironment))]
	public static int ailiaCallbackGetEnvironmentBridge (IntPtr a, uint b, uint c) {
		return ailiaGetEnvironment(a, b, c);
	}

	/**
	* \~japanese
	* @def AILIA_SPEECH_API_CALLBACK_VERSION
	* @brief 構造体バージョン
	*
	* \~english
	* @def AILIA_SPEECH_API_CALLBACK_VERSION
	* @brief Struct version
	*/
	public const int AILIA_SPEECH_API_CALLBACK_VERSION = (6);

	/* APIコールバック関数構造体 */
	[StructLayout(LayoutKind.Sequential)]
	public struct AILIASpeechApiCallback
	{
		public ailiaCallbackAudioGetFrameLen ailiaAudioGetFrameLen;
		public ailiaCallbackAudioGetMelSpectrogram ailiaAudioGetMelSpectrogram;
		public ailiaCallbackAudioResample ailiaAudioResample;
		public ailiaCallbackAudioGetResampleLen ailiaAudioGetResampleLen;
		public ailiaCallbackTokenizerCreate ailiaTokenizerCreate;
		public ailiaCallbackTokenizerOpenModelFileA ailiaTokenizerOpenModelFileA;
		public ailiaCallbackTokenizerOpenModelFileW ailiaTokenizerOpenModelFileW;
		public ailiaCallbackTokenizerEncode ailiaTokenizerEncode;
		public ailiaCallbackTokenizerGetTokenCount ailiaTokenizerGetTokenCount;
		public ailiaCallbackTokenizerGetTokens ailiaTokenizerGetTokens;
		public ailiaCallbackTokenizerDecode ailiaTokenizerDecode;
		public ailiaCallbackTokenizerGetTextLength ailiaTokenizerGetTextLength;
		public ailiaCallbackTokenizerGetText ailiaTokenizerGetText;
		public ailiaCallbackTokenizerDestroy ailiaTokenizerDestroy;
		public ailiaCallbackTokenizerUtf8ToUtf32 ailiaTokenizerUtf8ToUtf32;
		public ailiaCallbackTokenizerUtf32ToUtf8 ailiaTokenizerUtf32ToUtf8;
		public ailiaCallbackCreate ailiaCreate;
		public ailiaCallbackOpenWeightFileA ailiaOpenWeightFileA;
		public ailiaCallbackOpenWeightFileW ailiaOpenWeightFileW;
		public ailiaCallbackOpenWeightMem ailiaOpenWeightMem;
		public ailiaCallbackSetMemoryMode ailiaSetMemoryMode;
		public ailiaCallbackDestroy ailiaDestroy;
		public ailiaCallbackUpdate ailiaUpdate;
		public ailiaCallbackGetBlobIndexByInputIndex ailiaGetBlobIndexByInputIndex;
		public ailiaCallbackGetBlobIndexByOutputIndex ailiaGetBlobIndexByOutputIndex;
		public ailiaCallbackGetBlobData ailiaGetBlobData;
		public ailiaCallbackSetInputBlobData ailiaSetInputBlobData;
		public ailiaCallbackSetInputBlobShape ailiaSetInputBlobShape;
		public ailiaCallbackGetBlobShape ailiaGetBlobShape;
		public ailiaCallbackGetErrorDetail ailiaGetErrorDetail;
		public ailiaCallbackCopyBlobData ailiaCopyBlobData;
		public ailiaCallbackGetEnvironment ailiaGetEnvironment;
	};

	public static AiliaSpeech.AILIASpeechApiCallback GetCallback(){
		AiliaSpeech.AILIASpeechApiCallback callback=new AiliaSpeech.AILIASpeechApiCallback();

		// C# pass through API Mapping
		callback.ailiaAudioGetFrameLen=ailiaCallbackAudioGetFrameLenBridge;
		callback.ailiaAudioGetMelSpectrogram=ailiaCallbackAudioGetMelSpectrogramBridge;
		callback.ailiaAudioResample=ailiaCallbackAudioResampleBridge;
		callback.ailiaAudioGetResampleLen=ailiaCallbackAudioGetResampleLenBridge;

		callback.ailiaTokenizerCreate = ailiaCallbackTokenizerCreateBridge;
		callback.ailiaTokenizerOpenModelFileA = ailiaCallbackTokenizerOpenModelFileABridge;
		callback.ailiaTokenizerOpenModelFileW = ailiaCallbackTokenizerOpenModelFileWBridge;
		callback.ailiaTokenizerEncode = ailiaCallbackTokenizerEncodeBridge;
		callback.ailiaTokenizerGetTokenCount = ailiaCallbackTokenizerGetTokenCountBridge;
		callback.ailiaTokenizerGetTokens = ailiaCallbackTokenizerGetTokensBridge;
		callback.ailiaTokenizerDecode = ailiaCallbackTokenizerDecodeBridge;
		callback.ailiaTokenizerGetTextLength = ailiaCallbackTokenizerGetTextLengthBridge;
		callback.ailiaTokenizerGetText = ailiaCallbackTokenizerGetTextBridge;
		callback.ailiaTokenizerDestroy = ailiaCallbackTokenizerDestroyBridge;
		callback.ailiaTokenizerUtf8ToUtf32 = ailiaCallbackTokenizerUtf8ToUtf32Bridge;
		callback.ailiaTokenizerUtf32ToUtf8 = ailiaCallbackTokenizerUtf32ToUtf8Bridge;

		callback.ailiaCreate=ailiaCallbackCreateBridge;
		callback.ailiaOpenWeightFileA=ailiaCallbackOpenWeightFileABridge;
		callback.ailiaOpenWeightFileW=ailiaCallbackOpenWeightFileWBridge;
		callback.ailiaOpenWeightMem=ailiaCallbackOpenWeightMemBridge;
		callback.ailiaSetMemoryMode=ailiaCallbackSetMemoryModeBridge;
		callback.ailiaDestroy=ailiaCallbackDestroyBridge;
		callback.ailiaUpdate=ailiaCallbackUpdateBridge;
		callback.ailiaGetBlobIndexByInputIndex=ailiaCallbackGetBlobIndexByInputIndexBridge;
		callback.ailiaGetBlobIndexByOutputIndex=ailiaCallbackGetBlobIndexByOutputIndexBridge;
		callback.ailiaGetBlobData=ailiaCallbackGetBlobDataBridge;
		callback.ailiaSetInputBlobData=ailiaCallbackSetInputBlobDataBridge;
		callback.ailiaSetInputBlobShape=ailiaCallbackSetInputBlobShapeBridge;
		callback.ailiaGetBlobShape=ailiaCallbackGetBlobShapeBridge;
		callback.ailiaGetErrorDetail=ailiaCallbackGetErrorDetailBridge;
		callback.ailiaCopyBlobData=ailiaCallbackCopyBlobDataBridge;
		callback.ailiaGetEnvironment=ailiaCallbackGetEnvironmentBridge;
		
		return callback;
	}

	/****************************************************************
	* ネットワークオブジェクトのインスタンス
	**/

	/**
	* \~japanese
	* @def AILIA_SPEECH_TEXT_VERSION
	* @brief 構造体バージョン
	*
	* \~english
	* @def AILIA_SPEECH_TEXT_VERSION
	* @brief Struct version
	*/
	public const int AILIA_SPEECH_TEXT_VERSION = (2);

	[StructLayout(LayoutKind.Sequential)]
	public class AILIASpeechText{
		public IntPtr text;
		public float time_stamp_begin;
		public float time_stamp_end;
		public uint person_id;
		public IntPtr language;
		public float confidence;
	};

	/****************************************************************
	* Speech2Text API
	**/

	/**
	* \~japanese
	* @brief ネットワークオブジェクトを作成します。
	* @param net ネットワークオブジェクトポインタへのポインタ
	* @param env_id 計算に利用する推論実行環境のID( ailiaGetEnvironment() で取得)  \ref AILIA_ENVIRONMENT_ID_AUTO
	* にした場合は自動で選択する
	* @param num_thread スレッド数の上限(  \ref AILIA_MULTITHREAD_AUTO  にした場合は自動で設定)
	* @param memory_mode メモリモード(AILIA_MEMORY_MODE_*)
	* @param task AILIA_SPEECH_TASK_*
	* @param flag AILIA_SPEECH_FLAG_*の論理和
	* @param api_callback ailiaのAPIへのコールバック
	* @param version AILIA_SPEECH_API_CALLBACK_VERSION
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	* @details
	*   ネットワークオブジェクトを作成します。
	*
	* \~english
	* @brief Creates a network instance.
	* @param net A pointer to the network instance pointer
	* @param env_id The ID of the inference backend used for computation (obtained by  ailiaGetEnvironment() ). It is
	* selected automatically if  \ref AILIA_ENVIRONMENT_ID_AUTO  is specified.
	* @param num_thread The upper limit on the number of threads (It is set automatically if  \ref AILIA_MULTITHREAD_AUTO
	* @param memory_mode The memory mode (AILIA_MEMORY_MODE_*)
	* @param task AILIA_SPEECH_TASK_*
	* @param flag OR of AILIA_SPEECH_FLAG_*
	* @param api_callback The callback for ailia API
	* @param version AILIA_SPEECH_API_CALLBACK_VERSION
	* is specified.)
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	* @details
	*   Creates a network instance.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechCreate(ref IntPtr net, int env_id, int num_thread, int memory_mode, int task, int flag, AILIASpeechApiCallback callback, int version);

	/**
	* \~japanese
	* @brief モデルを指定します。
	* @param net ネットワークオブジェクトポインタ
	* @param encoder_path onnxファイルのパス名
	* @param decoder_path onnxファイルのパス名
	* @param model_type AILIA_SPEECH_MODEL_TYPE_*
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Set models into a network instance.
	* @param net A network instance pointer
	* @param encoder_path The path name to the onnx file
	* @param decoder_path The path name to the onnx file
	* @param model_type AILIA_SPEECH_MODEL_TYPE_*
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
	[DllImport(LIBRARY_NAME, EntryPoint = "ailiaSpeechOpenModelFileW", CharSet=CharSet.Unicode)]
	public static extern int ailiaSpeechOpenModelFile(IntPtr net, string encoder_path, string decoder_path, int model_type);
#else
	[DllImport(LIBRARY_NAME, EntryPoint = "ailiaSpeechOpenModelFileA", CharSet=CharSet.Ansi)]
	public static extern int ailiaSpeechOpenModelFile(IntPtr net, string encoder_path, string decoder_path, int model_type);
#endif

	/**
	* \~japanese
	* @brief 無音検知に適用するVADモデルを指定します。
	* @param net ネットワークオブジェクトポインタ
	* @param vad_path onnxファイルのパス名
	* @param vad_type AILIA_SPEECH_VAD_TYPE_*
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Set vad model for voice activity detection.
	* @param net A network instance pointer
	* @param vad_path The path name to the onnx file
	* @param vad_type AILIA_SPEECH_VAD_TYPE_*
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
	[DllImport(LIBRARY_NAME, EntryPoint = "ailiaSpeechOpenVadFileW", CharSet=CharSet.Unicode)]
	public static extern int ailiaSpeechOpenVadFile(IntPtr net, string vad_path, int vad_type);
#else
	[DllImport(LIBRARY_NAME, EntryPoint = "ailiaSpeechOpenVadFileA", CharSet=CharSet.Ansi)]
	public static extern int ailiaSpeechOpenVadFile(IntPtr net,  string vad_path, int vad_type);
#endif

	/**
	* \~japanese
	* @brief 誤り訂正辞書を指定します。
	* @param net ネットワークオブジェクトポインタ
	* @param dictionary_path 辞書ファイルのパス名
	* @param dictionary_type AILIA_SPEECH_DICTIONARY_TYPE_*
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Set dictionary for error correction.
	* @param net A network instance pointer
	* @param dictionary_path The path name to the dictionary file
	* @param dictionary_type AILIA_SPEECH_DICTIONARY_TYPE_*
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
	[DllImport(LIBRARY_NAME, EntryPoint = "ailiaSpeechOpenDictionaryFileW", CharSet=CharSet.Unicode)]
	public static extern int ailiaSpeechOpenDictionaryFile(IntPtr net, string dictionary_path, int dictionary_type);
#else
	[DllImport(LIBRARY_NAME, EntryPoint = "ailiaSpeechOpenDictionaryFileA", CharSet=CharSet.Ansi)]
	public static extern int ailiaSpeechOpenDictionaryFile(IntPtr net,  string dictionary_path, int dictionary_type);
#endif

	/**
	* \~japanese
	* @brief 後処理に適用するAIモデルを指定します。
	* @param net ネットワークオブジェクトポインタ
	* @param encoder_path onnxファイルのパス名
	* @param decoder_path onnxファイルのパス名
	* @param source_path Tokenizerのmodelファイルのパス名
	* @param target_path Tokenizerのmodelファイルのパス名
	* @param prefix      T5のprefix (UTF8)、FuguMTの場合はNULL
	* @param post_process_type AILIA_SPEECH_POST_PROCESS_TYPE_*
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	* @detail
	*   prefixにはUTF8の文字列を与える必要があります。下記のようにUTF8の文字列を取得可能です。
	*    byte[] text = System.Text.Encoding.UTF8.GetBytes(utf8+"\u0000");
	*    GCHandle handle = GCHandle.Alloc(text, GCHandleType.Pinned);
	*    IntPtr prefix = handle.AddrOfPinnedObject();
	*    ailiaSpeechOpenPostProcessFile(...,prefix,...);
	*    handle.Free();
	*
	* \~english
	* @brief Set AI model for post process
	* @param net A network instance pointer
	* @param encoder_path The path name to the onnx file
	* @param decoder_path The path name to the onnx file
	* @param source_path The path name to the tokenizer model file
	* @param target_path The path name to the tokenizer model file
	* @param prefix      The prefix of T5 (UTF8), NULL for FuguMT
	* @param post_process_type AILIA_SPEECH_POST_PROCESS_TYPE_*
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	* @detail
	*   You need to provide a UTF8 string as the prefix. You can obtain a UTF8 string as follows.
	*    byte[] text = System.Text.Encoding.UTF8.GetBytes(utf8+"\u0000");
	*    GCHandle handle = GCHandle.Alloc(text, GCHandleType.Pinned);
	*    IntPtr prefix = handle.AddrOfPinnedObject();
	*    ailiaSpeechOpenPostProcessFile(...,prefix,...);
	*    handle.Free();
	*/
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
	[DllImport(LIBRARY_NAME, EntryPoint = "ailiaSpeechOpenPostProcessFileW", CharSet=CharSet.Unicode)]
	public static extern int ailiaSpeechOpenPostProcessFile(IntPtr net, string encoder_path, string decoder_path, string source_path, string target_path, IntPtr prefix, int post_process_type);
#else
	[DllImport(LIBRARY_NAME, EntryPoint = "ailiaSpeechOpenPostProcessFileA", CharSet=CharSet.Ansi)]
	public static extern int ailiaSpeechOpenPostProcessFile(IntPtr net, string encoder_path, string decoder_path, string source_path, string target_path, IntPtr prefix, int post_process_type);
#endif

	/**
	* \~japanese
	* @brief 話者分離に適用するAIモデルを指定します。
	* @param net ネットワークオブジェクトポインタ
	* @param segmentation_path onnxファイルのパス名
	* @param embedding_path onnxファイルのパス名
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Set AI model for speaker diarization 
	* @param net A network instance pointer
	* @param segmentation_path The path name to the onnx file
	* @param embedding_path The path name to the onnx file
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
	[DllImport(LIBRARY_NAME, EntryPoint = "ailiaSpeechOpenDiarizationFileW", CharSet=CharSet.Unicode)]
	public static extern int ailiaSpeechOpenDiarizationFile(IntPtr net, string segmentation_path, string embedding_path);
#else
	[DllImport(LIBRARY_NAME, EntryPoint = "ailiaSpeechOpenDiarizationFileA", CharSet=CharSet.Ansi)]
	public static extern int ailiaSpeechOpenDiarizationFile(IntPtr net, string segmentation_path, string embedding_path);
#endif

	/**
	* \~japanese
	* @brief 音声をキューに投入します。
	* @param net ネットワークオブジェクトポインタ
	* @param src PCMデータ（チャンネルインタリーブ、LRLR、-1.0 to 1.0）
	* @param channels チャンネル数
	* @param samples チャンネルあたりのサンプル数
	* @param sampling_rate サンプリングレート（Hz）
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Push PCM data to queue
	* @param net A network instance pointer
	* @param src The input pcm data (channel interleave, LRLR, -1.0 to 1.0 range)
	* @param channels The number of pcm channels
	* @param samples The number of pcm samples per channel
	* @param sampling_rate The sampling rate (Hz)
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechPushInputData(IntPtr net, float [] src, uint channels, uint samples, uint sampling_rate);

	/**
	* \~japanese
	* @brief 音声のキューへの投入を終了します。
	* @param net ネットワークオブジェクトポインタ
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	* @details
	*   音声ファイルの末尾であることを通知することで、30秒分のデータが存在しなくてもailiaSpeechBufferedが1を返すようになります。
	*   ailiaSpeechFinalizeInputDataを実行後、ailiaSpeechPushInputDataの実行前に、ailiaSpeechResetTranscribeStateを呼び出す必要があります。
	*
	* \~english
	* @brief Finalize input PCM data to queue
	* @param net A network instance pointer
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	* @details
	*   By signaling the end of the audio file, ailiaSpeechBuffered will return 1 even if 30 seconds worth of data does not exist.
	*   You must call ailiaSpeechResetTranscribeState after executing ailiaSpeechFinalizeInputData and before executing ailiaSpeechPushInputData.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechFinalizeInputData(IntPtr net);

   /**
	* \~japanese
	* @brief 音声認識を行うためのデータが存在するかどうかを判定します。
	* @param net ネットワークオブジェクトポインタ
	* @param buffered 存在フラグ
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Is processing data exist
	* @param net A network instance pointer
	* @param buffered Is data exist
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechBuffered(IntPtr net, ref uint buffered);

	/**
	* \~japanese
	* @brief 全てのデータを処理したかどうかを判定します。
	* @param net ネットワークオブジェクトポインタ
	* @param complete 完了フラグ
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Is processed all data
	* @param net A network instance pointer
	* @param complete Is complete
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechComplete(IntPtr net, ref uint complete);

	/**
	* \~japanese
	* @brief プロンプトの設定を行います。
	* @param net ネットワークオブジェクトポインタ
	* @param prompt promptとなるテキスト(UTF8)
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す
	*
	* \~english
	* @brief Set prompt.
	* @param net A network instance pointer
	* @param prompt The text of prompt (UTF8)
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechSetPrompt(IntPtr net, IntPtr prompt);

	/**
	* \~japanese
	* @brief 制約の設定を行います。
	* @param net ネットワークオブジェクトポインタ
	* @param constraint 制約となるテキスト(UTF8)
	* @param type 制約モード (AILIA_SPEECH_CONSTRAINT_*)
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す
	*
	* \~english
	* @brief Set constraint.
	* @param net A network instance pointer
	* @param constraint The text of constraint (UTF8)
	* @param type The type of constraint (AILIA_SPEECH_CONSTRAINT_*)
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int  ailiaSpeechSetConstraint(IntPtr net, IntPtr constraint, int type);

	/**
	* \~japanese
	* @brief 言語設定を行います。
	* @param net ネットワークオブジェクトポインタ
	* @param language 言語コード（en, jaなど）
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す
	*   languageにautoを指定すると自動選択になる。
	*
	* \~english
	* @brief Performs the inferences and provides the inference result.
	* @param net A network instance pointer
	* @param language Language code (en, ja, etc)
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*   If you set auto to language, language will automatically detected.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechSetLanguage(IntPtr net, string language);

	/**
	* \~japanese
	* @brief 認識の途中結果を取得するコールバックを設定します。
	* @param net ネットワークオブジェクトポインタ
	* @param callback コールバック
	* @param handle コールバックに提供されるハンドル
	* @return
	*   返値は解放する必要はありません。
	*   文字列の有効期間は次にailiaSpeechのAPIを呼ぶまでです。
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Set a callback to get intermediate results of recognition.
	* @param net A network instance pointer
	* @param callback callback
	* @param handle handle for callback
	* @return
	*   The return value does not have to be released.
	*   The string is valid until the next ailiaSpeech API function is called.
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechSetIntermediateCallback(IntPtr net, ailiaIntermediateCallback callback, IntPtr handle);

	/**
	* \~japanese
	* @brief 音声認識を行います。
	* @param net ネットワークオブジェクトポインタ
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Speech recognition
	* @param net A network instance pointer
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechTranscribe(IntPtr net);

	/**
	* \~japanese
	* @brief 後処理を行います。
	* @param net ネットワークオブジェクトポインタ
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	* @details
	*   認識した結果はailiaSpeechGetText APIで取得します。
	*
	* \~english
	* @brief Execute post process
	* @param net A network instance pointer
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	* @details
	*   Get the recognition result with ailiaSpeechGetText API.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechPostProcess(IntPtr net);
	
	/**
	* \~japanese
	* @brief 認識したテキストの数を取得します。
	* @param net ネットワークオブジェクトポインタ
	* @param count テキストの数
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Get text count
	* @param net A network instance pointer
	* @param count Text count
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechGetTextCount(IntPtr net, ref uint count);

	/**
	* \~japanese
	* @brief 認識したテキストを取得します。
	* @param net ネットワークオブジェクトポインタ
	* @param text テキスト
	* @param version AILIA_SPEECH_TEXT_VERSION
	* @param idx テキストのインデックス
	* @return
	*   返値は解放する必要はありません。
	*   文字列の有効期間は次にailiaSpeechのAPIを呼ぶまでです。
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	*
	* \~english
	* @brief Get text length
	* @param net A network instance pointer
	* @param text Text
	* @param version AILIA_SPEECH_TEXT_VERSION
	* @param idx Text index
	* @return
	*   The return value does not have to be released.
	*   The string is valid until the next ailiaSpeech API function is called.
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechGetText(IntPtr net, [In,Out] AILIASpeechText text, uint version, uint idx);

	/**
	* \~japanese
	* @brief ポストプロセス対象のテキストを設定します。
	* @param net ネットワークオブジェクトポインタ
	* @param text テキスト
	* @param version AILIA_SPEECH_TEXT_VERSION
	* @param idx テキストのインデックス
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	* @details
	*   音声認識を使用せず、ポストプロセスのみを使用する場合に使用します。
	*   文字列は内部バッファにコピーされるため、呼び出し後に解放することができます。
	*   idxがailiaSpeechGetTextCountよりも大きい場合、自動的に内部バッファが拡張されます。
	*
	* \~english
	* @brief Set postprocess text
	* @param net A network instance pointer
	* @param text Text
	* @param version AILIA_SPEECH_TEXT_VERSION
	* @param idx Text index
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	* @details
	*   Used when using only post-processing without using speech recognition.
	*   Since the string is copied to the internal buffer, it can be released after the call.
	*   If idx is larger than ailiaSpeechGetTextCount, the internal buffer will be automatically expanded.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechSetText(IntPtr net, [In] AILIASpeechText text, uint version, uint idx);

	/**
	* \~japanese
	* @brief ネットワークオブジェクトを破棄します。
	* @param net ネットワークオブジェクトポインタ
	*
	* \~english
	* @brief It destroys the network instance.
	* @param net A network instance pointer
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern void ailiaSpeechDestroy(IntPtr net);

	/**
	* \~japanese
	* @brief エラーの詳細を返します
	* @param net   ネットワークオブジェクトポインタ
	* @return
	*   エラー詳細
	* @details
	*   返値は解放する必要はありません。
	*   文字列の有効期間は次にailiaSpeechのAPIを呼ぶまでです。
	*   モデルが暗号化されている場合は空文字を返します。
	*   取得したポイントから以下のように文字列に変換して下さい。
	*   @code
	*   Marshal.PtrToStringAnsi(Ailia.ailiaGetErrorDetail(net))
	*   @endcode
	*
	* \~english
	* @brief Returns the details of errors.
	* @param net   The network instance pointer
	* @return
	*   Error details
	* @details
	*   The return value does not have to be released.
	*   The string is valid until the next ailiaSpeech API function is called.
	*   If model is encrypted, this function returns empty string.
	*   Convert from the point obtained to a string as follows
	*   @code
	*   Marshal.PtrToStringAnsi(Ailia.ailiaGetErrorDetail(net))
	*   @endcode
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern IntPtr ailiaSpeechGetErrorDetail(IntPtr net);

	/**
	* \~japanese
	* @brief 無音判定の閾値を設定します。
	* @param net ネットワークオブジェクトポインタ
	* @param silent_threshold  有音判定のしきい値
	* @param speech_sec    有音区間の時間
	* @param no_speech_sec 無音区間の時間
	* @return
	*   有音区間が一定以上存在する場合に無音区間が一定時間以上続いた場合に30secを待たずに滞留しているバッファを処理します。
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す
	*   languageにautoを指定すると自動選択になる。
	*
	* \~english
	* @brief Set silent threshold.
	* @param net A network instance pointer
	* @param silent_threshold  volume threshold
	* @param speech_sec    speech time
	* @param no_speech_sec no_speech time
	* @return
	*   If there are more than a certain number of sounded sections, and if the silent section lasts for a certain amount of time or more,
	*   the remaining buffer is processed without waiting for 30 seconds.
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	*   If you set auto to language, language will automatically detected.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechSetSilentThreshold(IntPtr net, float silent_threshold, float speech_sec, float no_speech_sec);

	/**
	* \~japanese
	* @brief ネットワークオブジェクトの内部状態を初期化します。
	* @param net ネットワークオブジェクトポインタ
	* @return
	*   成功した場合は \ref AILIA_STATUS_SUCCESS 、そうでなければエラーコードを返す。
	* @details
	*   このAPIを呼び出すことで、前回のデコード結果などの内部状態を初期化します。
	*   このAPIを呼び出した後、モデルを再び開く必要はありません。
	*   ailiaSpeechOpenModelFile、ailiaSpeechSetIntermediateCallback、ailiaSpeechSetLanguage、ailiaSpeechSetSilentThreshold、ailiaSpeechSetPromptのステートは保持されます。
	*
	* \~english
	* @brief It resets the network instance.
	* @param net A network instance pointer
	* @return
	*   If this function is successful, it returns  \ref AILIA_STATUS_SUCCESS , or an error code otherwise.
	* @details
	*   By calling this API, the internal state such as the previous decoding result is initialized.
	*   There is no need to reopen the model after calling this API.
	*   The states of ailiaSpeechOpenModelFile, ailiaSpeechSetIntermediateCallback, ailiaSpeechSetLanguage, ailiaSpeechSetSilentThreshold, ailiaSpeechSetPrompt are preserved.
	*/
	[DllImport(LIBRARY_NAME)]
	public static extern int ailiaSpeechResetTranscribeState(IntPtr net);
}
} // namespace ailiaSpeech