/* ailia.speech translate model class */
/* Copyright 2024 AXELL CORPORATION */

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading;
using System.Runtime.InteropServices;

using ailia;

namespace ailiaSpeech{

public class AiliaSpeechTranslateModel : IDisposable
{

    // instance
    IntPtr net = IntPtr.Zero;

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
    * @param encoder_path onnxファイルのパス名
    * @param decoder_path onnxファイルのパス名
    * @param source_path Tokenizerのmodelファイルのパス名
    * @param target_path Tokenizerのmodelファイルのパス名
    * @param post_process_type AILIA_SPEECH_POST_PROCESS_TYPE_*
    * @param env_id         実行環境 (Ailia.AILIA_ENVIRONMENT_ID_AUTOで自動選択)
    * @param memory_mode    メモリモード (Ailia.AILIA_MEMORY_REDUCE_CONSTANT | Ailia.AILIA_MEMORY_REDUCE_CONSTANT_WITH_INPUT_INITIALIZER | Ailia.AILIA_MEMORY_REUSE_INTERSTAGE など)
    * @return
    *   成功した場合はtrue、失敗した場合はfalseを返す。
    *   
    * \~english
    * @brief Create a instance.
    * @brief Open PostProcess file.
    * @param encoder_path The path name to the onnx file
    * @param decoder_path The path name to the onnx file
    * @param source_path The path name to the tokenizer model file
    * @param target_path The path name to the tokenizer model file
    * @param post_process_type AILIA_SPEECH_POST_PROCESS_TYPE_*
    * @param env_id         Runtime environment (Ailia.AILIA_ENVIRONMENT_ID_AUTO for automatic selection)
    * @param memory_mode    Memory mode (Ailia.AILIA_MEMORY_REDUCE_CONSTANT | Ailia.AILIA_MEMORY_REDUCE_CONSTANT_WITH_INPUT_INITIALIZER | Ailia.AILIA_MEMORY_REUSE_INTERSTAGE etc)
    * @return
    *   If this function is successful, it returns  true  , or  false  otherwise.
    */
    public bool Open(string encoder_path, string decoder_path, string source_path, string target_path, int type, int env_id, int memory_mode){
        AiliaLicense.CheckAndDownloadLicense();

        if (net != null){
            Close();
        }

        AiliaSpeech.AILIASpeechApiCallback callback = AiliaSpeech.GetCallback();

        int task = AiliaSpeech.AILIA_SPEECH_TASK_TRANSCRIBE;
        int flag = AiliaSpeech.AILIA_SPEECH_FLAG_NONE;

        int status = AiliaSpeech.ailiaSpeechCreate(ref net, env_id, Ailia.AILIA_MULTITHREAD_AUTO, memory_mode, task, flag, callback, AiliaSpeech.AILIA_SPEECH_API_CALLBACK_VERSION);
        Check(status, "ailiaSpeechCreate");
        if (status != 0){
            return false;
        }


        status = AiliaSpeech.ailiaSpeechOpenPostProcessFile(net, encoder_path, decoder_path, source_path, target_path, IntPtr.Zero, type);
        Check(status, "ailiaSpeechOpenPostProcessFile");
        if (status != 0){
            return false;
        }

        m_error = false;
        m_error_detail = "";

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

    ~AiliaSpeechTranslateModel(){
        Dispose(false);
    }

    /****************************************************************
     * 翻訳の実行
     */

    /**
    * \~japanese
    * @brief 翻訳を実行します。
    * @param input_text    入力テキスト
    * @return
    *   成功した場合は翻訳後の文字列、失敗した場合はnullを返す。
    * @detail
    *   翻訳を実行します。
    *   ブロッキングAPIです。
    *   
    * \~english
    * @brief   Perform speech recognition
    * @param input_text    Input Text
    * @return
    *   If this function is successful, it returns  translated string  , or  null  otherwise.
    * @detail
    *   Run translate.
    *   Blocked API.
    */
    public string Translate(string input_text){
        AiliaSpeech.AILIASpeechText text = new AiliaSpeech.AILIASpeechText();

        byte[] input_text_byte = System.Text.Encoding.UTF8.GetBytes(input_text+"\u0000");
        GCHandle input_text_handle = GCHandle.Alloc(input_text_byte, GCHandleType.Pinned);
        IntPtr input_text_ptr = input_text_handle.AddrOfPinnedObject();

        byte[] language_text_byte = System.Text.Encoding.UTF8.GetBytes("None\u0000");
        GCHandle language_handle = GCHandle.Alloc(language_text_byte, GCHandleType.Pinned);
        IntPtr language_text_ptr = language_handle.AddrOfPinnedObject();

        text.text = input_text_ptr;
        text.time_stamp_begin = 0.0f;
        text.time_stamp_end = 0.0f;
        text.confidence = 0.0f;
        text.person_id = 0;
        text.language = language_text_ptr;

        uint idx = 0;
        int status = AiliaSpeech.ailiaSpeechSetText(net, text, AiliaSpeech.AILIA_SPEECH_TEXT_VERSION, idx);
        if(status!=0){
            Check(status, "ailiaSpeechSetText");
            return null;
        }

        status = AiliaSpeech.ailiaSpeechPostProcess(net);
        if(status!=0){
            Check(status, "ailiaSpeechPostProcess");
            return null;
        }

        status = AiliaSpeech.ailiaSpeechGetText(net, text, AiliaSpeech.AILIA_SPEECH_TEXT_VERSION, idx);
        if(status!=0){
            Check(status, "ailiaSpeechGetText");
            return null;
        }

        input_text_handle.Free();
        language_handle.Free();

        return Marshal.PtrToStringAnsi(text.text);
    }
}

} // namespace ailiaSpeech
