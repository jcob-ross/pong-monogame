namespace PongMonoGame
{
  using System;
  using System.Collections.Generic;
  using Microsoft.Xna.Framework;
  using Microsoft.Xna.Framework.Audio;
  using Microsoft.Xna.Framework.Content;

  /// <summary>
  ///   Class for managing and playing sounds.
  /// </summary>
  public class SoundPlayer : IDisposable
  {
    private readonly ContentManager _contentManager;
    private float _masterVolume = .3f;
    private readonly Dictionary<string, SoundEffect> _soundEffects = new Dictionary<string, SoundEffect>(4);
    private readonly Dictionary<string, SoundEffectInstance> _soundInstances = new Dictionary<string, SoundEffectInstance>(8);

    /// <summary>
    ///   Master vlume modifier. Accepts values fro 0 (no sound) to 1 (maximum volume).
    /// </summary>
    public float MasterVolume { get { return _masterVolume; } set { _masterVolume = MathHelper.Clamp(value, 0f, 1f); } }

    /// <summary>
    ///   Maximum number of sound effect instances that can play concurrently.
    /// </summary>
    public int MaxConcurrentSounds { get; set; } = 4;

    /// <summary>
    ///   Creates new instance of <see cref="SoundPlayer"/> class.
    /// </summary>
    /// <param name="contentManager">game's <see cref="ContentManager"/></param>
    /// <exception cref="ArgumentNullException">when <paramref name="contentManager"/> is null</exception>
    public SoundPlayer(ContentManager contentManager)
    {
      if (null == contentManager)
        throw new ArgumentNullException(nameof(contentManager));

      _contentManager = contentManager;
    }

    /// <summary>
    ///   Loads sound effect assets.
    /// </summary>
    public void LoadSounds()
    {
      const string plopName = "8bit_plop";
      var plop = _contentManager.Load<SoundEffect>(plopName);
      _soundEffects.Add(plopName, plop);
    }
    /// <summary>
    ///   Unloads and clears all sound effects and playing sounds.
    /// </summary>
    public void UnloadSounds()
    {
      foreach (var pair in _soundInstances)
      {
        pair.Value.Stop();
        pair.Value.Dispose();
      }
      _soundInstances.Clear();

      foreach (var pair in _soundEffects)
      {
        pair.Value.Dispose();
      }
      _soundEffects.Clear();
    }

    /// <summary>
    ///   Plays sound specified by asset's name.
    /// </summary>
    /// <remarks>
    ///   No sound will be played if asset with name specified by <paramref name="soundName"/> is not found.
    ///   If <paramref name="uniqueId"/> encounters collision. Previous sound effect parameters will be modified.
    /// </remarks>
    /// <param name="soundName">The key of the loaded asset.</param>
    /// <param name="uniqueId">Unique identifier of the sound to use as key in collection of currently playing sound effects.</param>
    /// <param name="pitch">Pitch adjustment, ranging from -1.0f (down one octave) to 1.0f (up one octave). 0.0f is unity (normal) pitch.</param>
    /// <param name="volume">Volume, ranging from 0.0f (silence) to 1.0f (full volume). 1.0f is full volume relative to <see cref="SoundPlayer.MasterVolume"/>.</param>
    /// <param name="pan">Panning, ranging from -1.0f (full left) to 1.0f (full right). 0.0f is centered.</param>
    /// <param name="loop">Indicates whether looping is enabled for the sound effect instance.</param>
    /// <exception cref="ArgumentNullException"> when <paramref name="uniqueId"/> is null or whitespace.</exception>
    public void PlaySound(string soundName, string uniqueId, float pitch = 0f, float volume = 1f, float pan = 0f, bool loop = false)
    {
      ClearFinishedSounds();

      if (String.IsNullOrWhiteSpace(soundName))
        return;
      if (String.IsNullOrWhiteSpace(uniqueId))
        throw new ArgumentNullException(nameof(uniqueId));

      if (MaxConcurrentSounds < _soundInstances.Count)
        return;
      if (! _soundEffects.ContainsKey(soundName))
        return;

      pitch = MathHelper.Clamp(pitch, -1f, 1f);
      volume = MathHelper.Clamp(volume, 0f, 1f);
      pan = MathHelper.Clamp(pan, -1f, 1f);

      if (_soundInstances.ContainsKey(uniqueId))
      {
        var oldInstance = _soundInstances[uniqueId];
        oldInstance.Pitch = pitch;
        oldInstance.Volume = volume;
        oldInstance.Pan = pan;
        return;
      }

      var instance = _soundEffects[soundName].CreateInstance();
      instance.Pitch = pitch;
      instance.Volume = _masterVolume * volume;
      instance.Pan = pan;

      instance.Play();
      _soundInstances.Add(uniqueId, instance);
    }

    private readonly List<string> _instancesToRemove = new List<string>();
    private void ClearFinishedSounds(bool clearLoopingSounds = false)
    {
      foreach (var pair in _soundInstances)
      {
        if (clearLoopingSounds && pair.Value.IsLooped)
          pair.Value.Stop();
        if (pair.Value.State == SoundState.Stopped)
        {
          pair.Value.Dispose();
          _instancesToRemove.Add(pair.Key);
        }
      }

      for (var i = 0; i < _instancesToRemove.Count; i++)
        _soundInstances.Remove(_instancesToRemove[i]);

      _instancesToRemove.Clear();
    }
    
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    private bool _disposed;
    private void Dispose(bool disposing)
    {
      if (! _disposed)
      {
        if (disposing)
        {
          UnloadSounds();
        }
        _disposed = true;
      }
    }
  }
}