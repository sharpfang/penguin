/**
 * Copyright (c) 2018 Enzien Audio, Ltd.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice,
 *    this list of conditions, and the following disclaimer.
 * 
 * 2. Redistributions in binary form must reproduce the phrase "powered by heavy",
 *    the heavy logo, and a hyperlink to https://enzienaudio.com, all in a visible
 *    form.
 * 
 *   2.1 If the Application is distributed in a store system (for example,
 *       the Apple "App Store" or "Google Play"), the phrase "powered by heavy"
 *       shall be included in the app description or the copyright text as well as
 *       the in the app itself. The heavy logo will shall be visible in the app
 *       itself as well.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using AOT;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Hv_PenguinSoundEngine_AudioLib))]
public class Hv_PenguinSoundEngine_Editor : Editor {

  [MenuItem("Heavy/PenguinSoundEngine")]
  static void CreateHv_PenguinSoundEngine() {
    GameObject target = Selection.activeGameObject;
    if (target != null) {
      target.AddComponent<Hv_PenguinSoundEngine_AudioLib>();
    }
  }
  
  private Hv_PenguinSoundEngine_AudioLib _dsp;

  private void OnEnable() {
    _dsp = target as Hv_PenguinSoundEngine_AudioLib;
  }

  public override void OnInspectorGUI() {
    bool isEnabled = _dsp.IsInstantiated();
    if (!isEnabled) {
      EditorGUILayout.LabelField("Press Play!",  EditorStyles.centeredGreyMiniLabel);
    }
    // events
    GUI.enabled = isEnabled;
    EditorGUILayout.Space();
    // Blue
    if (GUILayout.Button("Blue")) {
      _dsp.SendEvent(Hv_PenguinSoundEngine_AudioLib.Event.Blue);
    }
    
    // Cyan
    if (GUILayout.Button("Cyan")) {
      _dsp.SendEvent(Hv_PenguinSoundEngine_AudioLib.Event.Cyan);
    }
    
    // Green
    if (GUILayout.Button("Green")) {
      _dsp.SendEvent(Hv_PenguinSoundEngine_AudioLib.Event.Green);
    }
    
    // Magenda
    if (GUILayout.Button("Magenda")) {
      _dsp.SendEvent(Hv_PenguinSoundEngine_AudioLib.Event.Magenda);
    }
    
    // MoveOff
    if (GUILayout.Button("MoveOff")) {
      _dsp.SendEvent(Hv_PenguinSoundEngine_AudioLib.Event.Moveoff);
    }
    
    // MoveOn
    if (GUILayout.Button("MoveOn")) {
      _dsp.SendEvent(Hv_PenguinSoundEngine_AudioLib.Event.Moveon);
    }
    
    // Red
    if (GUILayout.Button("Red")) {
      _dsp.SendEvent(Hv_PenguinSoundEngine_AudioLib.Event.Red);
    }
    
    // Yellow
    if (GUILayout.Button("Yellow")) {
      _dsp.SendEvent(Hv_PenguinSoundEngine_AudioLib.Event.Yellow);
    }
    
    // transmitFail
    if (GUILayout.Button("transmitFail")) {
      _dsp.SendEvent(Hv_PenguinSoundEngine_AudioLib.Event.Transmitfail);
    }
    
    // transmitOn
    if (GUILayout.Button("transmitOn")) {
      _dsp.SendEvent(Hv_PenguinSoundEngine_AudioLib.Event.Transmiton);
    }
    
    // transmitSuccess
    if (GUILayout.Button("transmitSuccess")) {
      _dsp.SendEvent(Hv_PenguinSoundEngine_AudioLib.Event.Transmitsuccess);
    }
    
    GUILayout.EndVertical();

    // parameters
    GUI.enabled = true;
    GUILayout.BeginVertical();
    EditorGUILayout.Space();
  }
}
#endif // UNITY_EDITOR

[RequireComponent (typeof (AudioSource))]
public class Hv_PenguinSoundEngine_AudioLib : MonoBehaviour {
  
  // Events are used to trigger bangs in the patch context (thread-safe).
  // Example usage:
  /*
    void Start () {
        Hv_PenguinSoundEngine_AudioLib script = GetComponent<Hv_PenguinSoundEngine_AudioLib>();
        script.SendEvent(Hv_PenguinSoundEngine_AudioLib.Event.Blue);
    }
  */
  public enum Event : uint {
    Blue = 0x6697C2E2,
    Cyan = 0x7F9A9509,
    Green = 0xB34D3150,
    Magenda = 0xF589E050,
    Moveoff = 0xAF8AD8D9,
    Moveon = 0x518FDDC7,
    Red = 0x8BE2D2C,
    Yellow = 0x9E534D0A,
    Transmitfail = 0x27B92B1B,
    Transmiton = 0x8A69D7A3,
    Transmitsuccess = 0xCA94BB9E,
  }
  
  // Delegate method for receiving float messages from the patch context (thread-safe).
  // Example usage:
  /*
    void Start () {
        Hv_PenguinSoundEngine_AudioLib script = GetComponent<Hv_PenguinSoundEngine_AudioLib>();
        script.RegisterSendHook();
        script.FloatReceivedCallback += OnFloatMessage;
    }

    void OnFloatMessage(Hv_PenguinSoundEngine_AudioLib.FloatMessage message) {
        Debug.Log(message.receiverName + ": " + message.value);
    }
  */
  public class FloatMessage {
    public string receiverName;
    public float value;

    public FloatMessage(string name, float x) {
      receiverName = name;
      value = x;
    }
  }
  public delegate void FloatMessageReceived(FloatMessage message);
  public FloatMessageReceived FloatReceivedCallback;

  // internal state
  private Hv_PenguinSoundEngine_Context _context;

  public bool IsInstantiated() {
    return (_context != null);
  }

  public void RegisterSendHook() {
    _context.RegisterSendHook();
  }
  
  // see Hv_PenguinSoundEngine_AudioLib.Event for definitions
  public void SendEvent(Hv_PenguinSoundEngine_AudioLib.Event e) {
    if (IsInstantiated()) _context.SendBangToReceiver((uint) e);
  }
  
  public void FillTableWithMonoAudioClip(string tableName, AudioClip clip) {
    if (clip.channels > 1) {
      Debug.LogWarning("Hv_PenguinSoundEngine_AudioLib: Only loading first channel of '" +
          clip.name + "' into table '" +
          tableName + "'. Multi-channel files are not supported.");
    }
    float[] buffer = new float[clip.samples]; // copy only the 1st channel
    clip.GetData(buffer, 0);
    _context.FillTableWithFloatBuffer(tableName, buffer);
  }

  public void FillTableWithFloatBuffer(string tableName, float[] buffer) {
    _context.FillTableWithFloatBuffer(tableName, buffer);
  }

  private void Awake() {
    _context = new Hv_PenguinSoundEngine_Context((double) AudioSettings.outputSampleRate);
  }
  
  private void Update() {
    // retreive sent messages
    if (_context.IsSendHookRegistered()) {
      Hv_PenguinSoundEngine_AudioLib.FloatMessage tempMessage;
      while ((tempMessage = _context.msgQueue.GetNextMessage()) != null) {
        FloatReceivedCallback(tempMessage);
      }
    }
  }

  private void OnAudioFilterRead(float[] buffer, int numChannels) {
    Assert.AreEqual(numChannels, _context.GetNumOutputChannels()); // invalid channel configuration
    _context.Process(buffer, buffer.Length / numChannels); // process dsp
  }
}

class Hv_PenguinSoundEngine_Context {

#if UNITY_IOS && !UNITY_EDITOR
  private const string _dllName = "__Internal";
#else
  private const string _dllName = "Hv_PenguinSoundEngine_AudioLib";
#endif

  // Thread-safe message queue
  public class SendMessageQueue {
    private readonly object _msgQueueSync = new object();
    private readonly Queue<Hv_PenguinSoundEngine_AudioLib.FloatMessage> _msgQueue = new Queue<Hv_PenguinSoundEngine_AudioLib.FloatMessage>();

    public Hv_PenguinSoundEngine_AudioLib.FloatMessage GetNextMessage() {
      lock (_msgQueueSync) {
        return (_msgQueue.Count != 0) ? _msgQueue.Dequeue() : null;
      }
    }

    public void AddMessage(string receiverName, float value) {
      Hv_PenguinSoundEngine_AudioLib.FloatMessage msg = new Hv_PenguinSoundEngine_AudioLib.FloatMessage(receiverName, value);
      lock (_msgQueueSync) {
        _msgQueue.Enqueue(msg);
      }
    }
  }

  public readonly SendMessageQueue msgQueue = new SendMessageQueue();
  private readonly GCHandle gch;
  private readonly IntPtr _context; // handle into unmanaged memory
  private SendHook _sendHook = null;

  [DllImport (_dllName)]
  private static extern IntPtr hv_PenguinSoundEngine_new_with_options(double sampleRate, int poolKb, int inQueueKb, int outQueueKb);

  [DllImport (_dllName)]
  private static extern int hv_processInlineInterleaved(IntPtr ctx,
      [In] float[] inBuffer, [Out] float[] outBuffer, int numSamples);

  [DllImport (_dllName)]
  private static extern void hv_delete(IntPtr ctx);

  [DllImport (_dllName)]
  private static extern double hv_getSampleRate(IntPtr ctx);

  [DllImport (_dllName)]
  private static extern int hv_getNumInputChannels(IntPtr ctx);

  [DllImport (_dllName)]
  private static extern int hv_getNumOutputChannels(IntPtr ctx);

  [DllImport (_dllName)]
  private static extern void hv_setSendHook(IntPtr ctx, SendHook sendHook);

  [DllImport (_dllName)]
  private static extern void hv_setPrintHook(IntPtr ctx, PrintHook printHook);

  [DllImport (_dllName)]
  private static extern int hv_setUserData(IntPtr ctx, IntPtr userData);

  [DllImport (_dllName)]
  private static extern IntPtr hv_getUserData(IntPtr ctx);

  [DllImport (_dllName)]
  private static extern void hv_sendBangToReceiver(IntPtr ctx, uint receiverHash);

  [DllImport (_dllName)]
  private static extern void hv_sendFloatToReceiver(IntPtr ctx, uint receiverHash, float x);

  [DllImport (_dllName)]
  private static extern uint hv_msg_getTimestamp(IntPtr message);

  [DllImport (_dllName)]
  private static extern bool hv_msg_hasFormat(IntPtr message, string format);

  [DllImport (_dllName)]
  private static extern float hv_msg_getFloat(IntPtr message, int index);

  [DllImport (_dllName)]
  private static extern bool hv_table_setLength(IntPtr ctx, uint tableHash, uint newSampleLength);

  [DllImport (_dllName)]
  private static extern IntPtr hv_table_getBuffer(IntPtr ctx, uint tableHash);

  [DllImport (_dllName)]
  private static extern float hv_samplesToMilliseconds(IntPtr ctx, uint numSamples);

  [DllImport (_dllName)]
  private static extern uint hv_stringToHash(string s);

  private delegate void PrintHook(IntPtr context, string printName, string str, IntPtr message);

  private delegate void SendHook(IntPtr context, string sendName, uint sendHash, IntPtr message);

  public Hv_PenguinSoundEngine_Context(double sampleRate, int poolKb=10, int inQueueKb=2, int outQueueKb=2) {
    gch = GCHandle.Alloc(msgQueue);
    _context = hv_PenguinSoundEngine_new_with_options(sampleRate, poolKb, inQueueKb, outQueueKb);
    hv_setPrintHook(_context, new PrintHook(OnPrint));
    hv_setUserData(_context, GCHandle.ToIntPtr(gch));
  }

  ~Hv_PenguinSoundEngine_Context() {
    hv_delete(_context);
    GC.KeepAlive(_context);
    GC.KeepAlive(_sendHook);
    gch.Free();
  }

  public void RegisterSendHook() {
    // Note: send hook functionality only applies to messages containing a single float value
    if (_sendHook == null) {
      _sendHook = new SendHook(OnMessageSent);
      hv_setSendHook(_context, _sendHook);
    }
  }

  public bool IsSendHookRegistered() {
    return (_sendHook != null);
  }

  public double GetSampleRate() {
    return hv_getSampleRate(_context);
  }

  public int GetNumInputChannels() {
    return hv_getNumInputChannels(_context);
  }

  public int GetNumOutputChannels() {
    return hv_getNumOutputChannels(_context);
  }

  public void SendBangToReceiver(uint receiverHash) {
    hv_sendBangToReceiver(_context, receiverHash);
  }

  public void SendFloatToReceiver(uint receiverHash, float x) {
    hv_sendFloatToReceiver(_context, receiverHash, x);
  }

  public void FillTableWithFloatBuffer(string tableName, float[] buffer) {
    uint tableHash = hv_stringToHash(tableName);
    if (hv_table_getBuffer(_context, tableHash) != IntPtr.Zero) {
      hv_table_setLength(_context, tableHash, (uint) buffer.Length);
      Marshal.Copy(buffer, 0, hv_table_getBuffer(_context, tableHash), buffer.Length);
    } else {
      Debug.Log(string.Format("Table '{0}' doesn't exist in the patch context.", tableName));
    }
  }

  public uint StringToHash(string s) {
    return hv_stringToHash(s);
  }

  public int Process(float[] buffer, int numFrames) {
    return hv_processInlineInterleaved(_context, buffer, buffer, numFrames);
  }

  [MonoPInvokeCallback(typeof(PrintHook))]
  private static void OnPrint(IntPtr context, string printName, string str, IntPtr message) {
    float timeInSecs = hv_samplesToMilliseconds(context, hv_msg_getTimestamp(message)) / 1000.0f;
    Debug.Log(string.Format("{0} [{1:0.000}]: {2}", printName, timeInSecs, str));
  }

  [MonoPInvokeCallback(typeof(SendHook))]
  private static void OnMessageSent(IntPtr context, string sendName, uint sendHash, IntPtr message) {
    if (hv_msg_hasFormat(message, "f")) {
      SendMessageQueue msgQueue = (SendMessageQueue) GCHandle.FromIntPtr(hv_getUserData(context)).Target;
      msgQueue.AddMessage(sendName, hv_msg_getFloat(message, 0));
    }
  }
}
