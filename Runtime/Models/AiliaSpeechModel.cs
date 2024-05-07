/* ailia.speech model class */
/* Copyright 2022-2024 AXELL CORPORATION */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading;
using System.Runtime.InteropServices;

using ailia;

namespace ailiaSpeech{

public class AiliaSpeechModel : IDisposable
{
    // instance
    IntPtr net = IntPtr.Zero;
    private bool live_mode = false;
    private bool post_process_mode = false;

    // settings
    private const float THRESHOLD_VOLUME = 0.01f;
    private const float THRESHOLD_VAD = 0.5f;
    private const float SPEECH_SEC = 1.0f;
    private const float NO_SPEECH_SEC = 1.0f;

    //-----------------------------------------------------------------
    // Error check
    //-----------------------------------------------------------------

    private bool m_error = false;
    private string m_error_detail = "";

    private void Check(int status, string tag){
        if (status != 0 && m_error == false){
            m_error_detail = tag + " " + status + " " + Marshal.PtrToStringAnsi(AiliaSpeech.ailiaSpeechGetErrorDetail(net));
            if (status == Ailia.AILIA_STATUS_LICENSE_NOT_FOUND){
                m_error_detail += "\nLicense file not found. Please place license file.";
            }
            if (status == Ailia.AILIA_STATUS_LICENSE_EXPIRED){
                m_error_detail += "\nLicense file expired. Please place new license file.";
            }
            Debug.Log(m_error_detail);
            m_error = true;
        }
    }

    /**
    * \~japanese
    * @brief エラーが発生したか確認します。
    * @return
    *   エラーが発生した場合はtrue、発生していない場合はfalseを返す。
    *   
    * \~english
    * @brief    Check is error occured
    * @return
    *   If error is occured, it returns  true  , or  false  otherwise.
    */
    public bool IsError(){
        return m_error;
    }

    /**
    * \~japanese
    * @brief エラーの詳細を取得します。
    * @return
    *   エラーの詳細を示す文字列。
    *   
    * \~english
    * @brief    Get error detail.
    * @return
    *   The error detail string.
    */
    public string GetErrorDetail(){
        return m_error_detail;
    }

    /****************************************************************
     * モデル
     */

    /**
    * \~japanese
    * @brief インスタンスを作成します。
    * @param encoder_path   エンコーダのONNXファイルヘのパス
    * @param decoder_path   デコーダのONNXファイルヘのパス
    * @param env_id         実行環境 (Ailia.AILIA_ENVIRONMENT_ID_AUTOで自動選択)
    * @param memory_mode    メモリモード (Ailia.AILIA_MEMORY_REDUCE_CONSTANT | Ailia.AILIA_MEMORY_REDUCE_CONSTANT_WITH_INPUT_INITIALIZER | Ailia.AILIA_MEMORY_REUSE_INTERSTAGE など)
    * @param model_type   　モデル種別（AiliaSpeech.AILIA_SPEECH_MODEL_TYPE_*)
    * @param task           タスク種別（AiliaSpeech.AILIA_SPEECH_TASK_*)
    * @param flag           フラグの論理和（AiliaSpeech.AILIA_SPEECH_FLAG_*)
    * @param language       言語（jaやenなど、autoの場合は自動選択）
    * @return
    *   成功した場合はtrue、失敗した場合はfalseを返す。
    *   
    * \~english
    * @brief Create a instance.
    * @param encoder_path   Encoder onnx file path
    * @param decoder_path   Decoder onnx file path
    * @param env_id         Runtime environment (Ailia.AILIA_ENVIRONMENT_ID_AUTO for automatic selection)
    * @param memory_mode    Memory mode (Ailia.AILIA_MEMORY_REDUCE_CONSTANT | Ailia.AILIA_MEMORY_REDUCE_CONSTANT_WITH_INPUT_INITIALIZER | Ailia.AILIA_MEMORY_REUSE_INTERSTAGE etc)
    * @param model_type     Model type (AiliaSpeech.AILIA_SPEECH_MODEL_TYPE_*)
    * @param task           Task (AiliaSpeech.AILIA_SPEECH_TASK_*)
    * @param flag           OR of flags (AiliaSpeech.AILIA_SPEECH_FLAG_*)
    * @param language       Language (ja or en or etc. auto is automatic selection)
    * @return
    *   If this function is successful, it returns  true  , or  false  otherwise.
    */
    public bool Open(string encoder_path, string decoder_path, int env_id, int memory_mode, int model_type, int task, int flag, string language){
        AiliaLicense.CheckAndDownloadLicense();

        if (net != null){
            Close();
        }

        AiliaSpeech.AILIASpeechApiCallback callback = AiliaSpeech.GetCallback();

        int status = AiliaSpeech.ailiaSpeechCreate(ref net, env_id, Ailia.AILIA_MULTITHREAD_AUTO, memory_mode, task, flag, callback, AiliaSpeech.AILIA_SPEECH_API_CALLBACK_VERSION);
        Check(status, "ailiaSpeechCreate");
        if (status != 0){
            return false;
        }

        status = AiliaSpeech.ailiaSpeechOpenModelFile(net, encoder_path, decoder_path, model_type);
        Check(status, "ailiaSpeechOpenModelFile");
        if (status != 0){
            return false;
        }

        if (language != "auto"){
            status = AiliaSpeech.ailiaSpeechSetLanguage(net, language);
            Check(status, "ailiaSpeechSetLanguage");
            if (status != 0){
                return false;
            }
        }

        status = AiliaSpeech.ailiaSpeechSetSilentThreshold(net, THRESHOLD_VOLUME, SPEECH_SEC, NO_SPEECH_SEC);
        Check(status, "ailiaSpeechSetSilentThreshold");
        if (status != 0){
            return false;
        }

        CreateInterrupt();

        status = AiliaSpeech.ailiaSpeechSetIntermediateCallback(net, IntermediateCallback, m_interrupt_ptr);
        Check(status, "ailiaSpeechSetIntermediateCallback");
        if (status != 0){
            return false;
        }

        CreateThread();

        m_error = false;
        m_error_detail = "";

        if ((flag & AiliaSpeech.AILIA_SPEECH_FLAG_LIVE) != 0){
            live_mode = true;
        }else{
            live_mode = false;
        }

        return true;
    }

    /**
    * \~japanese
    * @brief VADファイルを開きます。
    * @param vad_path     VADのONNXファイルヘのパス
    * @param vad_type     VAD種別（AiliaSpeech.AILIA_SPEECH_VAD_TYPE_*)
    * @return
    *   成功した場合はtrue、失敗した場合はfalseを返す。
    *   
    * \~english
    * @brief Open VAD file.
    * @param vad_path     VAD onnx file path
    * @param vad_type     VAD type (AiliaSpeech.AILIA_SPEECH_VAD_TYPE_*)
    * @return
    *   If this function is successful, it returns  true  , or  false  otherwise.
    */
    public bool OpenVad(string vad_path, int vad_type){
        if (net == null){
            return false;
        }
        int status = AiliaSpeech.ailiaSpeechOpenVadFile(net, vad_path, vad_type);
        Check(status, "ailiaSpeechOpenVadFile");
        if (status != 0){
            return false;
        }
        status = AiliaSpeech.ailiaSpeechSetSilentThreshold(net, THRESHOLD_VAD, SPEECH_SEC, NO_SPEECH_SEC);
        Check(status, "ailiaSpeechSetSilentThreshold");
        if (status != 0){
            return false;
        }
        return true;
    }

    /**
    * \~japanese
    * @brief 辞書ファイルを開きます。
    * @param dictionary_path     辞書ファイルヘのパス
    * @param dictionary_type     辞書種別（AiliaSpeech.AILIA_SPEECH_DICTIONARY_TYPE_*)
    * @return
    *   成功した場合はtrue、失敗した場合はfalseを返す。
    *   
    * \~english
    * @brief Open dictionary file.
    * @param dictionary_path     dictionary file path
    * @param dictionary_type     dictionary type (AiliaSpeech.AILIA_SPEECH_DICTIONARY_TYPE_*)
    * @return
    *   If this function is successful, it returns  true  , or  false  otherwise.
    */
    public bool OpenDictionary(string dictionary_path, int dictionary_type){
        if (net == null){
            return false;
        }
        int status = AiliaSpeech.ailiaSpeechOpenDictionaryFile(net, dictionary_path, dictionary_type);
        Check(status, "ailiaSpeechOpenDictionaryFile");
        if (status != 0){
            return false;
        }
        return true;
    }

    /**
    * \~japanese
    * @brief ポストプロセスファイルを開きます。
    * @param encoder_path onnxファイルのパス名
    * @param decoder_path onnxファイルのパス名
    * @param source_path Tokenizerのmodelファイルのパス名
    * @param target_path Tokenizerのmodelファイルのパス名
    * @param prefix      T5のprefix (UTF8)、FuguMTの場合はnull
    * @param post_process_type AILIA_SPEECH_POST_PROCESS_TYPE_*
    * @return
    *   成功した場合はtrue、失敗した場合はfalseを返す。
    *   
    * \~english
    * @brief Open PostProcess file.
    * @param encoder_path The path name to the onnx file
    * @param decoder_path The path name to the onnx file
    * @param source_path The path name to the tokenizer model file
    * @param target_path The path name to the tokenizer model file
    * @param prefix      The prefix of T5 (UTF8), null for FuguMT
    * @param post_process_type AILIA_SPEECH_POST_PROCESS_TYPE_*
    * @return
    *   If this function is successful, it returns  true  , or  false  otherwise.
    */
    public bool OpenPostProcess(string encoder_path, string decoder_path, string source_path, string target_path, string prefix, int type){
        if (net == null){
            return false;
        }
        int status;
        if (prefix == null){
            status = AiliaSpeech.ailiaSpeechOpenPostProcessFile(net, encoder_path, decoder_path, source_path, target_path, IntPtr.Zero, type);
        }else{
            byte[] text = System.Text.Encoding.UTF8.GetBytes(prefix+"\u0000");
            GCHandle handle = GCHandle.Alloc(text, GCHandleType.Pinned);
            IntPtr prefix_ptr = handle.AddrOfPinnedObject();
            status = AiliaSpeech.ailiaSpeechOpenPostProcessFile(net, encoder_path, decoder_path, source_path, target_path, prefix_ptr, type);
            handle.Free();
        }
        Check(status, "ailiaSpeechOpenPostProcessFile");
        if (status != 0){
            return false;
        }
        post_process_mode = true;
        return true;
    }

    /****************************************************************
     * 開放する
     */
    /**
    * \~japanese
    * @brief インスタンスを破棄します。
    * @details
    *   インスタンスを破棄し、初期化します。
    *   
    *  \~english
    * @brief   Destroys instance
    * @details
    *   Destroys and initializes the instance.
    */
    public virtual void Close()
    {
        DestroyThread();
        DestroyInterrupt();
        if (net != IntPtr.Zero){
            AiliaSpeech.ailiaSpeechDestroy(net);
            net = IntPtr.Zero;
        }
    }

    /**
    * \~japanese
    * @brief リソースを解放します。
    *   
    *  \~english
    * @brief   Release resources.
    */
    public virtual void Dispose()
    {
        Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing){
            // release managed resource
        }
        Close(); // release unmanaged resource
    }

    ~AiliaSpeechModel(){
        Dispose(false);
    }

    /****************************************************************
     * Promptの設定
     */

    /**
    * \~japanese
    * @brief プロンプトの設定を行います。
    * @param prompt promptとなるテキスト(UTF8)
    * @return
    *   成功した場合はtrue、失敗した場合はfalseを返す。
    *
    * \~english
    * @brief Set prompt.
    * @param prompt The text of prompt (UTF8)
    * @return
    *   If this function is successful, it returns  true  , or  false  otherwise.
    */

    public bool SetPrompt(string prompt){
        byte[] text = System.Text.Encoding.UTF8.GetBytes(prompt+"\u0000");
        GCHandle handle = GCHandle.Alloc(text, GCHandleType.Pinned);
        IntPtr input = handle.AddrOfPinnedObject();
        int status = AiliaSpeech.ailiaSpeechSetPrompt(net, input);
        handle.Free();
        if (status != 0){
            return false;
        }
        return true;
    }

    /****************************************************************
     * 制約の設定
     */

    /**
    * \~japanese
    * @brief 制約の設定を行います。
    * @param constraint constraintとなるテキスト(UTF8)
    * @param constraint_type AILIA_SPEECH_CONSTRAINT_*
    * @return
    *   成功した場合はtrue、失敗した場合はfalseを返す。
    *
    * \~english
    * @brief Set constraint.
    * @param constraint The text of constraint (UTF8)
    * @param constraint_type AILIA_SPEECH_CONSTRAINT_*
    * @return
    *   If this function is successful, it returns  true  , or  false  otherwise.
    */

    public bool SetConstraint(string constraint, int constraint_type){
        byte[] text = System.Text.Encoding.UTF8.GetBytes(constraint+"\u0000");
        GCHandle handle = GCHandle.Alloc(text, GCHandleType.Pinned);
        IntPtr input = handle.AddrOfPinnedObject();
        int status = AiliaSpeech.ailiaSpeechSetConstraint(net, input, constraint_type);
        handle.Free();
        if (status != 0){
            return false;
        }
        return true;
    }

    /****************************************************************
     * 結果取得
     */

    private string GetDate(float cur_time, float next_time, float confidence){
        //string header = $"{(int)cur_time/60%60:00}:{(int)cur_time%60:00}.{(int)(cur_time*1000)%1000:000} --> {(int)next_time/60%60:00}:{(int)next_time%60:00}.{(int)(next_time*1000)%1000:000}";
        //string header = $"{(int)cur_time/60%60:00}:{(int)cur_time%60:00} --> {(int)next_time/60%60:00}:{(int)next_time%60:00}";
        string header = $"{(int)cur_time/60%60:00}:{(int)cur_time%60:00} --> {(int)next_time/60%60:00}:{(int)next_time%60:00} {confidence:f2}";
        return header;
    }

    private void PushResults(){
        uint count = 0;
        int status = AiliaSpeech.ailiaSpeechGetTextCount(net, ref count);
        if(status!=0){
            Check(status, "ailiaSpeechGetTextCount");
        }

        for (uint idx = 0; idx < count; idx++){
            AiliaSpeech.AILIASpeechText text = new AiliaSpeech.AILIASpeechText();
            status = AiliaSpeech.ailiaSpeechGetText(net, text, AiliaSpeech.AILIA_SPEECH_TEXT_VERSION, idx);
            if(status!=0){
                Check(status, "ailiaSpeechGetText");
            }

            float cur_time = text.time_stamp_begin;
            float next_time = text.time_stamp_end;
            float confidence = text.confidence;

            string header = GetDate(cur_time, next_time, confidence);

            string display_text = "[" + header + "] " + Marshal.PtrToStringAnsi(text.text);
            m_results.Add(display_text);
        }
    }

    /****************************************************************
     * スレッド制御
     */

    private UnityEngine.Object m_lock_async = new UnityEngine.Object();
    private Thread m_thread = null;
    private AutoResetEvent m_auto_event = null;
    private bool m_thread_abort = false;
    private int [] m_interrupt = new int [] {0};
    private GCHandle m_interrupt_handle;
    private IntPtr m_interrupt_ptr = IntPtr.Zero;
    private bool m_processing = false;
    private bool m_decoding = false;
    private bool m_complete = false;
    private List<string> m_results = new List<string>();

    private List<float[]> threadWaveQueue = new List<float[]>();
    private uint threadChannels = 0;
    private uint threadFrequency = 0;
    private bool threadComplete = false;

    private static UnityEngine.Object m_intermediate_lock_async = new UnityEngine.Object();
    private static string m_intermediate_text = "";

    [AOT.MonoPInvokeCallback(typeof(AiliaSpeech.ailiaIntermediateCallback))]
    public static int IntermediateCallback(IntPtr handle, IntPtr text){
        lock (m_intermediate_lock_async){
            try{
                 string decoded_text = Marshal.PtrToStringAnsi(text);
                 m_intermediate_text = decoded_text;
            }catch(Exception e){
            }
        }
        if (Marshal.ReadInt32(handle) != 0){
            return -1; // 中断
        }
        return 0;
    }

    private void CreateInterrupt()
    {
        m_interrupt_handle = GCHandle.Alloc(m_interrupt, GCHandleType.Pinned);
        m_interrupt_ptr = m_interrupt_handle.AddrOfPinnedObject();
    }

    private void DestroyInterrupt()
    {
        if (m_interrupt_ptr != IntPtr.Zero){
            m_interrupt_handle.Free();
            m_interrupt_ptr = IntPtr.Zero;
        }
    }

    private void CreateThread()
    {
        // reset thread state
        threadWaveQueue = new List<float[]>();
        threadChannels = 0;
        threadFrequency = 0;
        threadComplete = false;

        // reset interface
        m_processing = false;
        m_decoding = false;
        m_complete = false;
        m_intermediate_text = "";
        m_results = new List<string>();

        // create thread
        m_auto_event = new AutoResetEvent(false);
        m_thread = new Thread(Worker);
        m_thread.Start();
    }

    private void DestroyThread()
    {
        if (m_thread == null){
            return;
        }
        while (m_processing)
        {
            Thread.Sleep(1);
        }
        if (m_thread != null)
        {
            m_thread_abort = true;
            Marshal.WriteInt32(m_interrupt_ptr, 0, 1);
            m_auto_event.Set();
            while (m_thread.IsAlive)
            {
                Thread.Sleep(1);
            }
            m_thread_abort = false;
            Marshal.WriteInt32(m_interrupt_ptr, 0, 0);
            m_thread = null;
        }
    }

    private void Worker(object arguments)
    {
        while (true)
        {
            m_auto_event.WaitOne();
            if (m_thread_abort)
            {
                return;
            }

            int status;

            int samples = 0;
            float[] samples_buf = null;

            lock (m_lock_async)
            {
                for (int i = 0; i < threadWaveQueue.Count; i++){
                    float[] waveData = threadWaveQueue[i];
                    samples += waveData.Length;
                }
                if (samples > 0){
                    samples_buf = new float[samples];

                    int p = 0;
                    for (int i = 0; i < threadWaveQueue.Count; i++){
                        float[] waveData = threadWaveQueue[i];
                        for (int j = 0; j < waveData.Length; j++){
                            samples_buf[p + j] = waveData[j];
                        }
                        p = p + waveData.Length;
                    }
                }
                threadWaveQueue = new List<float[]>();
            }

            if (samples > 0) {
                status = AiliaSpeech.ailiaSpeechPushInputData(net, samples_buf, threadChannels, (uint)samples_buf.Length / threadChannels, threadFrequency);
                Check(status, "ailiaSpeechPushInputData");
            }

            if (threadComplete){
                status = AiliaSpeech.ailiaSpeechFinalizeInputData(net);
                Check(status, "ailiaSpeechFinalizeInputData");
            }

            while(true){
                uint buffered = 0;
                status = AiliaSpeech.ailiaSpeechBuffered(net, ref buffered);
                Check(status, "ailiaSpeechBuffered");

                if (buffered == 0){
                    break;
                }

                lock (m_lock_async)
                {
                    m_decoding = true;
                }

                status = AiliaSpeech.ailiaSpeechTranscribe(net);
                Check(status, "ailiaSpeechTranscribe");

                lock (m_lock_async)
                {
                    PushResults();
                    lock (m_intermediate_lock_async)
                    {
                        if (live_mode == false){
                            m_intermediate_text = "";
                        }
                    }
                }

                if (post_process_mode){
                    status = AiliaSpeech.ailiaSpeechPostProcess(net);
                    Check(status, "ailiaSpeechPostProcess");

                    lock (m_lock_async)
                    {
                        PushResults();
                    }
                }

                lock (m_lock_async)
                {
                    m_decoding = false;
                }
            }

            uint complete = 0;
            status = AiliaSpeech.ailiaSpeechComplete(net, ref complete);
            Check(status, "ailiaSpeechComplete");
            if (threadComplete){
                if (complete == 0){
                    Check(-1, "ailiaSpeechComplete must be true");
                }
            }

            lock (m_lock_async)
            {
                m_processing = false;
                if (threadComplete){
                    m_complete = true;
                }
            }
        }
    }

    /****************************************************************
     * 実行と結果取得
     */

    /**
    * \~japanese
    * @brief 音声認識を実行します。
    * @param waveQueue    入力PCM
    * @param frequency    入力PCMの周波数
    * @param channels     入力PCMのチャンネル数
    * @param tail        　入力が最後かどうか
    * @return
    *   成功した場合はtrue、失敗した場合はfalseを返す。
    * @detail
    *   音声認識を実行します。
    *   ノンブロッキングAPIです。
    *   実行が完了するとIsTranscribed APIがTrueを返します。
    *   実行結果はGetResults APIで取得可能です。
    *   実行の途中結果はGetIntermediateText APIで取得可能です。
    *   
    * \~english
    * @brief   Perform speech recognition
    * @param waveQueue    Input PCM
    * @param frequency    Frequency of PCM
    * @param channels     Number of channels of PCM
    * @param tail       　Is last input
    * @return
    *   If this function is successful, it returns  true  , or  false  otherwise.
    * @detail
    *   Run speech recognition.
    *   Non -blocked API.
    *   When the execution is completed, the IsTranscribed API returns True.
    *   Execution results can be obtained with the GetResults API.
    *   The result of execution can be obtained with the GetIntermediateText API.
    */
    public bool Transcribe(float[] waveData, uint frequency, uint channels, bool tail)
    {
        if (waveData.Length == 0){
            return false;
        }
        lock (m_lock_async)
        {
            threadChannels = channels;
            threadFrequency = frequency;
            threadWaveQueue.Add(waveData);
            threadComplete = tail;
            m_processing = true;
            m_auto_event.Set();
        }
        return true;
    }

    /**
    * \~japanese
    * @brief サブスレッドが実行中かどうか取得します。
    * @return
    *   実行中はtrue、それ以外の場合はfalseを返す。
    *   
    * \~english
    * @brief Check is processing sub thread.
    * @return
    *   If sub thread is processing, it returns  true  , or  false  otherwise.
    */
    public bool IsProcessing(){
        lock (m_lock_async)
        {
            return m_processing;
        }
    }

    /**
    * \~japanese
    * @brief Speech2Textを実行中かどうか取得します。
    * @return
    *   実行中はtrue、それ以外の場合はfalseを返す。
    *   
    * \~english
    * @brief Check is running Speech2Text.
    * @return
    *   If Speech2Text is running, it returns  true  , or  false  otherwise.
    */
    public bool IsTranscribing(){
        lock (m_lock_async)
        {
            return m_decoding;
        }
    }

    /**
    * \~japanese
    * @brief 全ての音声の処理が完了したかどうか取得します。
    * @return
    *   完了した場合はtrue、それ以外の場合はfalseを返す。
    *   
    * \~english
    * @brief Gets whether all audio processing is complete.
    * @return
    *   If Speech2Text is complete, it returns  true  , or  false  otherwise.
    */
    public bool IsCompleted(){
        lock (m_lock_async)
        {
            return m_complete;
        }
    }

    /**
    * \~japanese
    * @brief Speech2Textの実行結果を取得してクリアします。
    * @return
    *   認識結果を返す。
    *   
    * \~english
    * @brief Get results and clear of Speech2Text.
    * @return
    *   Transcribe results.
    */
    public List<string> GetResults(){
        lock (m_lock_async)
        {
            List<string> results = new List<string>(m_results);
            m_results.Clear();
            return results;
        }
    }

    /**
    * \~japanese
    * @brief Speech2Textの途中のテキストを取得します。
    * @return
    *   認識結果を返す。
    *   
    * \~english
    * @brief Get the intermediate result of Speech2Text.
    * @return
    *   Transcribe results.
    */
    public string GetIntermediateText(){
        lock (m_intermediate_lock_async){
            return m_intermediate_text;
        }
    }

    /**
    * \~japanese
    * @brief Speech2Textのステートを初期化します。
    * @return
    *   成功した場合はtrue、失敗した場合はfalseを返す。
    *   
    * \~english
    * @brief Initialize the Speech2Text state.
    * @return
    *   If this function is successful, it returns  true  , or  false  otherwise.
    */
    public bool ResetTranscribeState(){
        int status = AiliaSpeech.ailiaSpeechResetTranscribeState(net);
        lock (m_lock_async)
        {
            m_complete = false;
        }
        if (status == 0){
            return true;
        }
        return false;
    }
}

} // namespace ailiaSpeech
