using System.Runtime.InteropServices;
using static VBExtension;
using static VBConstants;
using Microsoft.VisualBasic;
using System;
using System.Windows;
using System.Windows.Controls;
using static System.DateTime;
using static System.Math;
using System.Linq;
using static Microsoft.VisualBasic.Collection;
using static Microsoft.VisualBasic.Constants;
using static Microsoft.VisualBasic.Conversion;
using static Microsoft.VisualBasic.DateAndTime;
using static Microsoft.VisualBasic.ErrObject;
using static Microsoft.VisualBasic.FileSystem;
using static Microsoft.VisualBasic.Financial;
using static Microsoft.VisualBasic.Information;
using static Microsoft.VisualBasic.Interaction;
using static Microsoft.VisualBasic.Strings;
using static Microsoft.VisualBasic.VBMath;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using SappySharp.Forms;
using static modSappy;
using static FMod;
using static mdlFile;
using static SapPlayer;
using static MidiLib;
using static mColorUtils;
using static mTrace;
using static SappySharp.Forms.frmSappy;
using static SappySharp.Forms.frmTakeTrax;
using static SappySharp.Forms.frmMakeTrax;
using static SappySharp.Forms.frmAbout;
using static SappySharp.Forms.frmTakeSamp;
using static SappySharp.Forms.frmAssembler;
using static SappySharp.Forms.frmOptions;
using static SappySharp.Forms.frmMidiMapper;
using static SappySharp.Forms.frmSelectMidiOut;
using static SappySharp.Forms.frmInputBox;
using static SappySharp.Classes.cNoStatusBar;
using static SappySharp.Classes.SChannels;
using static SappySharp.Classes.SNotes;
using static SappySharp.Classes.NoteInfo;
using static SappySharp.Classes.SChannel;
using static SappySharp.Classes.SNote;
using static SappySharp.Classes.SSubroutines;
using static SappySharp.Classes.SSubroutine;
using static SappySharp.Classes.SappyEventQueue;
using static SappySharp.Classes.SappyEvent;
using static SappySharp.Classes.NoteInfos;
using static SappySharp.Classes.SSamples;
using static SappySharp.Classes.SSample;
using static SappySharp.Classes.SDirects;
using static SappySharp.Classes.SDirect;
using static SappySharp.Classes.SDrumKit;
using static SappySharp.Classes.SDrumKits;
using static SappySharp.Classes.SInstruments;
using static SappySharp.Classes.SInstrument;
using static SappySharp.Classes.SKeyMaps;
using static SappySharp.Classes.SKeyMap;
using static SappySharp.Classes.clsSappyDecoder;
using static SappySharp.Classes.gCommonDialog;
using static SappySharp.Classes.pcMemDC;
using static SappySharp.Classes.cVBALImageList;
using static SappySharp.Classes.cRegistry;

/// <summary>
/// FMOD VB6 Module
/// </summary>
static partial class FMod
{
    public const decimal FMOD_VERSION = 3.74m;

    // ************
    // * Enums
    // ************


    // FMOD_ERRORS
    // On failure of commands in FMOD, use FSOUND_GetError to attain what happened.

    public enum FMOD_ERRORS
    {
        FMOD_ERR_NONE // No errors
    , FMOD_ERR_BUSY // Cannot call this command after FSOUND_Init.  Call FSOUND_Close first.
    , FMOD_ERR_UNINITIALIZED // This command failed because FSOUND_Init was not called
    , FMOD_ERR_INIT // Error initializing output device.
    , FMOD_ERR_ALLOCATED // Error initializing output device, but more specifically, the output device is already in use and cannot be reused.
    , FMOD_ERR_PLAY // Playing the sound failed.
    , FMOD_ERR_OUTPUT_FORMAT // Soundcard does not support the features needed for this soundsystem (16bit stereo output)
    , FMOD_ERR_COOPERATIVELEVEL // Error setting cooperative level for hardware.
    , FMOD_ERR_CREATEBUFFER // Error creating hardware sound buffer.
    , FMOD_ERR_FILE_NOTFOUND // File not found
    , FMOD_ERR_FILE_FORMAT // Unknown file format
    , FMOD_ERR_FILE_BAD // Error loading file
    , FMOD_ERR_MEMORY // Not enough memory
    , FMOD_ERR_VERSION // The version number of this file format is not supported
    , FMOD_ERR_INVALID_PARAM // An invalid parameter was passed to this function
    , FMOD_ERR_NO_EAX // Tried to use an EAX command on a non EAX enabled channel or output.
    , FMOD_ERR_CHANNEL_ALLOC // Failed to allocate a new channel
    , FMOD_ERR_RECORD // Recording is not supported on this machine
    , FMOD_ERR_MEDIAPLAYER // Windows Media Player not installed so cannot play wma or use internet streaming.
    , FMOD_ERR_CDDEVICE // An error occured trying to open the specified CD device
    }


    // FSOUND_OUTPUTTYPES
    // These output types are used with FSOUND_SetOutput, to choose which output driver to use.
    // FSOUND_OUTPUT_DSOUND will not support hardware 3d acceleration if the sound card driver
    // does not support DirectX 6 Voice Manager Extensions.
    // FSOUND_OUTPUT_WINMM is recommended for NT and CE.

    public enum FSOUND_OUTPUTTYPES
    {
        FSOUND_OUTPUT_NOSOUND // NoSound driver, all calls to this succeed but do nothing.
    , FSOUND_OUTPUT_WINMM // Windows Multimedia driver.
    , FSOUND_OUTPUT_DSOUND // DirectSound driver.  You need this to get EAX2 or EAX3 support, or FX api support.
    , FSOUND_OUTPUT_A3D // A3D driver.

    , FSOUND_OUTPUT_OSS // Linux/Unix OSS (Open Sound System) driver, i.e. the kernel sound drivers.
    , FSOUND_OUTPUT_ESD // Linux/Unix ESD (Enlightment Sound Daemon) driver.
    , FSOUND_OUTPUT_ALSA // Linux Alsa driver.

    , FSOUND_OUTPUT_ASIO // Low latency ASIO driver
    , FSOUND_OUTPUT_XBOX // Xbox driver
    , FSOUND_OUTPUT_PS2 // PlayStation 2 driver
    , FSOUND_OUTPUT_MAC // Mac SoundMager driver
    , FSOUND_OUTPUT_GC // Gamecube driver

    , FSOUND_OUTPUT_NOSOUND_NONREALTIME // This is the same as nosound, but the sound generation is driven by FSOUND_Update
    }


    // FSOUND_MIXERTYPES
    // These mixer types are used with FSOUND_SetMixer, to choose which mixer to use, or to act
    // upon for other reasons using FSOUND_GetMixer.
    // It is not nescessary to set the mixer.  FMOD will autodetect the best mixer for you.

    public enum FSOUND_MIXERTYPES
    {
        FSOUND_MIXER_AUTODETECT // CE/PS2 Only - Non interpolating/low quality mixer
    , FSOUND_MIXER_BLENDMODE // removed / obsolete.
    , FSOUND_MIXER_MMXP5 // removed / obsolete.
    , FSOUND_MIXER_MMXP6 // removed / obsolete.

    , FSOUND_MIXER_QUALITY_AUTODETECT // All platforms - Autodetect the fastest quality mixer based on your cpu.
    , FSOUND_MIXER_QUALITY_FPU // Win32/Linux only - Interpolating/volume ramping FPU mixer.
    , FSOUND_MIXER_QUALITY_MMXP5 // Win32/Linux only - Interpolating/volume ramping FPU mixer.
    , FSOUND_MIXER_QUALITY_MMXP6 // Win32/Linux only - Interpolating/volume ramping ppro+ MMX mixer.

    , FSOUND_MIXER_MONO // CE/PS2 only - MONO non interpolating/low quality mixer. For speed
    , FSOUND_MIXER_QUALITY_MONO // CE/PS2 only - MONO Interpolating mixer.  For speed
    }


    // FMUSIC_TYPES
    // These definitions describe the type of song being played.
    // See FMUSIC_GetType

    public enum FMUSIC_TYPES
    {
        FMUSIC_TYPE_NONE
    , FMUSIC_TYPE_MOD // Protracker / Fasttracker
    , FMUSIC_TYPE_S3M // ScreamTracker 3
    , FMUSIC_TYPE_XM // FastTracker 2
    , FMUSIC_TYPE_IT // Impulse Tracker.
    , FMUSIC_TYPE_MIDI // MIDI file
    , FMUSIC_TYPE_FSB // FMOD Sample Bank file
    }


    // FSOUND_DSP_PRIORITIES
    // These default priorities are used by FMOD internal system DSP units.  They describe the
    // position of the DSP chain, and the order of how audio processing is executed.
    // You can actually through the use of FSOUND_DSP_GetxxxUnit (where xxx is the name of the DSP
    // unit), disable or even change the priority of a DSP unit.

    public enum FSOUND_DSP_PRIORITIES
    {
        FSOUND_DSP_DEFAULTPRIORITY_CLEARUNIT = 0 // DSP CLEAR unit - done first
    , FSOUND_DSP_DEFAULTPRIORITY_SFXUNIT = 100 // DSP SFX unit - done second
    , FSOUND_DSP_DEFAULTPRIORITY_MUSICUNIT = 200 // DSP MUSIC unit - done third
    , FSOUND_DSP_DEFAULTPRIORITY_USER = 300 // User priority, use this as reference for your own dsp units
    , FSOUND_DSP_DEFAULTPRIORITY_FFTUNIT = 900 // This reads data for FSOUND_DSP_GetSpectrum, so it comes after user units
    , FSOUND_DSP_DEFAULTPRIORITY_CLIPANDCOPYUNIT = 1000 // DSP CLIP AND COPY unit - last
    }


    // FSOUND_CAPS
    // Driver description bitfields.  Use FSOUND_Driver_GetCaps to determine if a driver enumerated
    // has the settings you are after.  The enumerated driver depends on the output mode, see
    // FSOUND_OUTPUTTYPES

    public enum FSOUND_CAPS
    {
        FSOUND_CAPS_HARDWARE = 0x1 // This driver supports hardware accelerated 3d sound.
    , FSOUND_CAPS_EAX2 = 0x2 // This driver supports EAX 2 reverb
    , FSOUND_CAPS_EAX3 = 0x10 // This driver supports EAX 3 reverb
    }


    // FSOUND_MODES
    // Sample description bitfields, OR them together for loading and describing samples.
    // NOTE.  If the file format being loaded already has a defined format, such as WAV or MP3, then
    // trying to override the pre-defined format with a new set of format flags will not work.  For
    // example, an 8 bit WAV file will not load as 16bit if you specify FSOUND_16BITS.  It will just
    // ignore the flag and go ahead loading it as 8bits.  For these type of formats the only flags
    // you can specify that will really alter the behaviour of how it is loaded, are the following.

    // Looping behaviour - FSOUND_LOOP_OFF, FSOUND_LOOP_NORMAL, FSOUND_LOOP_BIDI
    // Load destination - FSOUND_HW3D, FSOUND_HW2D, FSOUND_2D
    // Loading behaviour - FSOUND_NONBLOCKING, FSOUND_LOADMEMORY, FSOUND_LOADRAW, FSOUND_MPEGACCURATE, FSOUND_MPEGHALFRATE, FSOUND_FORCEMONO
    // Playback behaviour - FSOUND_STREAMABLE, FSOUND_ENABLEFX
    // PlayStation 2 only - FSOUND_USECORE0, FSOUND_USECORE1, FSOUND_LOADMEMORYIOP

    // See flag descriptions for what these do.
    [Flags]
    public enum FSOUND_MODES : uint
    {
        FSOUND_LOOP_OFF = 0x1 // For non looping samples.
    , FSOUND_LOOP_NORMAL = 0x2 // For forward looping samples.
    , FSOUND_LOOP_BIDI = 0x4 // For bidirectional looping samples.  (no effect if in hardware).
    , FSOUND_8BITS = 0x8 // For 8 bit samples.
    , FSOUND_16BITS = 0x10 // For 16 bit samples.
    , FSOUND_MONO = 0x20 // For mono samples.
    , FSOUND_STEREO = 0x40 // For stereo samples.
    , FSOUND_UNSIGNED = 0x80 // For source data containing unsigned samples.
    , FSOUND_SIGNED = 0x100 // For source data containing signed data.
    , FSOUND_DELTA = 0x200 // For source data stored as delta values.
    , FSOUND_IT214 = 0x400 // For source data stored using IT214 compression.
    , FSOUND_IT215 = 0x800 // For source data stored using IT215 compression.
    , FSOUND_HW3D = 0x1000 // Attempts to make samples use 3d hardware acceleration. (if the card supports it)
    , FSOUND_2D = 0x2000 // Ignores any 3d processing.  overrides FSOUND_HW3D.  Located in software.
    , FSOUND_STREAMABLE = 0x4000 // For realtime streamable samples.  If you dont supply this sound may come out corrupted.
    , FSOUND_LOADMEMORY = 0x8000 // For FSOUND_Sample_Load - name will be interpreted as a pointer to data
    , FSOUND_LOADRAW = 0x10000 // For FSOUND_Sample_Load/FSOUND_Stream_Open - will ignore file format and treat as raw pcm.
    , FSOUND_MPEGACCURATE = 0x20000 // For FSOUND_Stream_Open - scans MP2/MP3 (VBR also) for accurate FSOUND_Stream_GetLengthMs/FSOUND_Stream_SetTime.
    , FSOUND_FORCEMONO = 0x40000 // For forcing stereo streams and samples to be mono - needed with FSOUND_HW3D - incurs speed hit
    , FSOUND_HW2D = 0x80000 // 2d hardware sounds.  allows hardware specific effects
    , FSOUND_ENABLEFX = 0x100000 // Allows DX8 FX to be played back on a sound.  Requires DirectX 8 - Note these sounds cant be played more than once, or have a changing frequency
    , FSOUND_MPEGHALFRATE = 0x200000 // For FMODCE only - decodes mpeg streams using a lower quality decode, but faster execution
    , FSOUND_XADPCM = 0x400000 // For XBOX only - Describes a user sample that its contents are compressed as XADPCM
    , FSOUND_VAG = 0x800000 // For PS2 only - Describes a user sample that its contents are compressed as Sony VAG format.
    , FSOUND_NONBLOCKING = 0x1000000 // For FSOUND_Stream_Open - Causes stream to open in the background and not block the foreground app - stream plays only when ready.
    , FSOUND_GCADPCM = 0x2000000 // For Gamecube only - Contents are compressed as Gamecube DSP-ADPCM format
    , FSOUND_MULTICHANNEL = 0x4000000 // For PS2 only - Contents are interleaved into a multi-channel (more than stereo) format
    , FSOUND_USECORE0 = 0x8000000 // For PS2 only - Sample/Stream is forced to use hardware voices 00-23
    , FSOUND_USECORE1 = 0x10000000 // For PS2 only - Sample/Stream is forced to use hardware voices 24-47
    , FSOUND_LOADMEMORYIOP = 0x20000000 // For PS2 only - __S1 will be interpreted as a pointer to data for streaming and samples.  The address provided will be an IOP address
    , FSOUND_STREAM_NET = 0x80000000 // Specifies an internet stream

    , FSOUND_NORMAL = FSOUND_16BITS | FSOUND_SIGNED | FSOUND_MONO
    }


    // FSOUND_CDPLAYMODES
    // Playback method for a CD Audio track, with FSOUND_CD_SetPlayMode

    public enum FSOUND_CDPLAYMODES
    {
        FSOUND_CD_PLAYCONTINUOUS // Starts from the current track and plays to end of CD.
    , FSOUND_CD_PLAYONCE // Plays the specified track then stops.
    , FSOUND_CD_PLAYLOOPED // Plays the specified track looped, forever until stopped manually.
    , FSOUND_CD_PLAYRANDOM // Plays tracks in random order
    }


    // FSOUND_CHANNELSAMPLEMODE
    // Miscellaneous values for FMOD functions.
    // FSOUND_PlaySound, FSOUND_PlaySoundEx, FSOUND_Sample_Alloc, FSOUND_Sample_Load, FSOUND_SetPan

    public enum FSOUND_CHANNELSAMPLEMODE
    {
        FSOUND_FREE = -1 // definition for dynamically allocated channel or sample
    , FSOUND_UNMANAGED = -2 // definition for allocating a sample that is NOT managed by fsound
    , FSOUND_ALL = -3 // for a channel index or sample index, this flag affects ALL channels or samples available!  Not supported by all functions.
    , FSOUND_STEREOPAN = -1 // definition for full middle stereo volume on both channels
    , FSOUND_SYSTEMCHANNEL = -1000 // special channel ID for channel based functions that want to alter the global FSOUND software mixing output channel
    , FSOUND_SYSTEMSAMPLE = -1000 // special sample ID for all sample based functions that want to alter the global FSOUND software mixing output sample
    }


    // FSOUND_REVERB_PROPERTIES
    // FSOUND_Reverb_SetProperties, FSOUND_Reverb_GetProperties, FSOUND_REVERB_PROPERTYFLAGS

    public class FSOUND_REVERB_PROPERTIES
    {
        //                                                     MIN     MAX    DEFAULT DESCRIPTION
        public int Environment;                             // 0       25     0       sets all listener properties
        public decimal EnvSize;                             // 1.0     100.0  7.5     environment size in meters
        public decimal EnvDiffusion;                        // 0.0     1.0    1.0     environment diffusion
        public int Room;                                    // -10000  0      -1000   room effect level (at mid frequencies)
        public int RoomHF;                                  // -10000  0      -100    relative room effect level at high frequencies
        public int RoomLF;                                  // -10000  0      0       relative room effect level at low frequencies
        public decimal DecayTime;                           // 0.1     20.0   1.49    reverberation decay time at mid frequencies
        public decimal DecayHFRatio;                        // 0.1     2.0    0.83    high-frequency to mid-frequency decay time ratio
        public decimal DecayLFRatio;                        // 0.1     2.0    1.0     low-frequency to mid-frequency decay time ratio
        public int Reflections;                             // -10000  1000   -2602   early reflections level relative to room effect
        public decimal ReflectionsDelay;                    // 0.0     0.3    0.007   initial reflection delay time
        public decimal[] ReflectionsPan = new decimal[3];   //                0,0,0   early reflections panning vector
        public int Reverb;                                  // -1000   2000   200     late reverberation level relative to room effect
        public decimal ReverbDelay;                         // 0.0     0.1    0.011   late reverberation delay time relative to initial reflection
        public decimal[] ReverbPan = new decimal[3];        // 0,0,0   late reverberation panning vector
        public decimal EchoTime;                            // .075    0.25   0.25    echo time
        public decimal EchoDepth;                           // 0.0     1.0    0.0     echo depth
        public decimal ModulationTime;                      // 0.04    4.0    0.25    modulation time
        public decimal ModulationDepth;                     // 0.0     1.0    0.0     modulation depth
        public decimal AirAbsorptionHF;                     // -100    0.0    -5.0    change in level per meter at high frequencies
        public decimal HFReference;                         // 1000.0  20000  5000.0  reference high frequency (hz)
        public decimal LFReference;                         // 20.0    1000.0 250.0   reference low frequency (hz)
        public decimal RoomRolloffFactor;                   // 0.0     10.0   0.0     like FSOUND_3D_SetRolloffFactor but for room effect
        public decimal Diffusion;                           // 0.0     100.0  100.0   Value that controls the echo density in the late reverberation decay. (xbox only)
        public decimal Density;                             // 0.0     100.0  100.0   Value that controls the modal density in the late reverberation decay (xbox only)
        public int flags;                                   // modifies the behavior of above properties
    }


    // FSOUND_REVERB_FLAGS
    // Values for the Flags member of the FSOUND_REVERB_PROPERTIES structure.
    [Flags]
    public enum FSOUND_REVERB_PROPERTYFLAGS
    {
        FSOUND_REVERBFLAGS_DECAYTIMESCALE = 0x1 // EnvironmentSize affects reverberation decay time
    , FSOUND_REVERBFLAGS_REFLECTIONSSCALE = 0x2 // EnvironmentSize affects reflection level
    , FSOUND_REVERBFLAGS_REFLECTIONSDELAYSCALE = 0x4 // EnvironmentSize affects initial reflection delay time
    , FSOUND_REVERBFLAGS_REVERBSCALE = 0x8 // EnvironmentSize affects reflections level
    , FSOUND_REVERBFLAGS_REVERBDELAYSCALE = 0x10 // EnvironmentSize affects late reverberation delay time
    , FSOUND_REVERBFLAGS_DECAYHFLIMIT = 0x20 // AirAbsorptionHF affects DecayHFRatio
    , FSOUND_REVERBFLAGS_ECHOTIMESCALE = 0x40 // EnvironmentSize affects echo time
    , FSOUND_REVERBFLAGS_MODULATIONTIMESCALE = 0x80 // EnvironmentSize affects modulation time
    , FSOUND_REVERB_FLAGS_CORE0 = 0x100 // PS2 Only - Reverb is applied to CORE0 (hw voices 0-23)
    , FSOUND_REVERB_FLAGS_CORE1 = 0x200 // PS2 Only - Reverb is applied to CORE1 (hw voices 24-47)
    , FSOUND_REVERBFLAGS_DEFAULT = FSOUND_REVERBFLAGS_DECAYTIMESCALE | FSOUND_REVERBFLAGS_REFLECTIONSSCALE | FSOUND_REVERBFLAGS_REFLECTIONSDELAYSCALE | FSOUND_REVERBFLAGS_REVERBSCALE | FSOUND_REVERBFLAGS_REVERBDELAYSCALE | FSOUND_REVERBFLAGS_DECAYHFLIMIT | FSOUND_REVERB_FLAGS_CORE0 | FSOUND_REVERB_FLAGS_CORE1
    }


    // FSOUND_REVERB_CHANNELPROPERTIES
    // Structure defining the properties for a reverb source, related to a FSOUND channel.
    // FSOUND_Reverb_SetEnvironment, FSOUND_Reverb_SetEnvironmentAdvanced

    public class FSOUND_REVERB_CHANNELPROPERTIES
    {
        public int Direct; // direct path level (at low and mid frequencies)
        public int DirectHF; // relative direct path level at high frequencies
        public int Room; // room effect level (at low and mid frequencies)
        public int RoomHF; // relative room effect level at high frequencies
        public int Obstruction; // main obstruction control (attenuation at high frequencies)
        public decimal ObstructionLFRatio; // obstruction low-frequency level re. main control
        public int Occlusion; // main occlusion control (attenuation at high frequencies)
        public decimal OcclustionLFRatio; // occlusion low-frequency level re. main control
        public decimal OcclusionRoomRatio; // relative occlusion control for room effect
        public decimal OcclusionDirectRatio; // relative occlusion control for direct path
        public int Exclusion; // main exlusion control (attenuation at high frequencies)
        public decimal ExclusionLFRatio; // exclusion low-frequency level re. main control
        public int OutsideVolumeHF; // outside sound cone level at high frequencies
        public decimal DopplerFactor; // like DS3D flDopplerFactor but per source
        public decimal RolloffFactor; // like DS3D flRolloffFactor but per source
        public decimal RoomRolloffFactor; // like DS3D flRolloffFactor but for room effect
        public decimal AirAbsorptionFactor; // multiplies AirAbsorptionHF member of FSOUND_REVERB_PROPERTIES
        public int flags; // modifies the behavior of properties
    }


    // FSOUND_REVERB_CHANNELFLAGS
    // Values for the Flags member of the FSOUND_REVERB_CHANNELPROPERTIES structure.
    [Flags]
    public enum FSOUND_REVERB_CHANNELFLAGS
    {
        FSOUND_REVERB_CHANNELFLAGS_DIRECTHFAUTO = 0x1 // Automatic setting of Direct due to distance from listener
    , FSOUND_REVERB_CHANNELFLAGS_ROOMAUTO = 0x2 // Automatic setting of Room due to distance from listener
    , FSOUND_REVERB_CHANNELFLAGS_ROOMHFAUTO = 0x4 // Automatic setting of RoomHF due to distance from listener
    , FSOUND_REVERB_CHANNELFLAGS_DEFAULT = FSOUND_REVERB_CHANNELFLAGS_DIRECTHFAUTO | FSOUND_REVERB_CHANNELFLAGS_ROOMAUTO | FSOUND_REVERB_CHANNELFLAGS_ROOMHFAUTO
    }


    // FSOUND_FX_MODES
    // These values are used with FSOUND_FX_Enable to enable DirectX 8 FX for a channel.

    public enum FSOUND_FX_MODES
    {
        FSOUND_FX_CHORUS
    , FSOUND_FX_COMPRESSOR
    , FSOUND_FX_DISTORTION
    , FSOUND_FX_ECHO
    , FSOUND_FX_FLANGER
    , FSOUND_FX_GARGLE
    , FSOUND_FX_I3DL2REVERB
    , FSOUND_FX_PARAMEQ
    , FSOUND_FX_WAVES_REVERB
    }


    // FSOUND_SPEAKERMODES
    // These are speaker types defined for use with the FSOUND_SetSpeakerMode command.
    // Note - Only reliably works with FSOUND_OUTPUT_DSOUND or FSOUND_OUTPUT_XBOX output modes.  Other output modes will only
    // interpret FSOUND_SPEAKERMODE_MONO and set everything else to be stereo.
    // Using either DolbyDigital or DTS will use whatever 5.1 digital mode is available if destination hardware is unsure.

    public enum FSOUND_SPEAKERMODES
    {
        FSOUND_SPEAKERMODE_DOLBYDIGITAL // The audio is played through a speaker arrangement of surround speakers with a subwoofer.
    , FSOUND_SPEAKERMODE_HEADPHONE // The speakers are headphones.
    , FSOUND_SPEAKERMODE_MONO // The speakers are monaural.
    , FSOUND_SPEAKERMODE_QUAD // The speakers are quadraphonic.
    , FSOUND_SPEAKERMODE_STEREO // The speakers are stereo (default value).
    , FSOUND_SPEAKERMODE_SURROUND // The speakers are surround sound.
    , FSOUND_SPEAKERMODE_DTS // The audio is played through a speaker arrangement of surround speakers with a subwoofer.
    , FSOUND_SPEAKERMODE_PROLOGIC2 // Dolby Prologic 2.  Playstation 2 and Gamecube only
    }


    // FSOUND_INIT_FLAGS
    // Initialization flags.  Use them with FSOUND_Init in the flags parameter to change various behaviour.
    // FSOUND_INIT_ENABLESYSTEMCHANNELFX Is an init mode which enables the FSOUND mixer buffer to be affected by DirectX 8 effects.
    // Note that due to limitations of DirectSound, FSOUND_Init may fail if this is enabled because the buffersize is too small.
    // This can be fixed with FSOUND_SetBufferSize.  Increase the BufferSize until it works.
    // When it is enabled you can use the FSOUND_FX api, and use FSOUND_SYSTEMCHANNEL as the channel id when setting parameters.

    public enum FSOUND_INITMODES
    {
        FSOUND_INIT_USEDEFAULTMIDISYNTH = 0x1 // Causes MIDI playback to force software decoding.
    , FSOUND_INIT_GLOBALFOCUS = 0x2 // For DirectSound output - sound is not muted when window is out of focus.
    , FSOUND_INIT_ENABLESYSTEMCHANNELFX = 0x4 // For DirectSound output - Allows FSOUND_FX api to be used on global software mixer output!
    , FSOUND_INIT_ACCURATEVULEVELS = 0x8 // This latency adjusts FSOUND_GetCurrentLevels, but incurs a small cpu and memory hit
    , FSOUND_INIT_PS2_DISABLECORE0REVERB = 0x10 // PS2 only - Disable reverb on CORE 0 to regain SRAM
    , FSOUND_INIT_PS2_DISABLECORE1REVERB = 0x20 // PS2 only - Disable reverb on CORE 1 to regain SRAM
    , FSOUND_INIT_PS2_SWAPDMACORES = 0x40 // PS2 only - By default FMOD uses DMA CH0 for mixing, CH1 for uploads, this flag swaps them around
    , FSOUND_INIT_DONTLATENCYADJUST = 0x80 // Callbacks are not latency adjusted, and are called at mix time.  Also information functions are immediate
    , FSOUND_INIT_GC_INITLIBS = 0x100 // Gamecube only - Initializes GC audio libraries
    , FSOUND_INIT_STREAM_FROM_MAIN_THREAD = 0x200 // Turns off fmod streamer thread, and makes streaming update from FSOUND_Update called by the user
    }


    // FSOUND_STREAM_NET_STATUS
    // Status values for internet streams. Use FSOUND_Stream_Net_GetStatus to get the current status of an internet stream.

    public enum FSOUND_STREAM_NET_STATUS
    {
        FSOUND_STREAM_NET_NOTCONNECTED // Stream hasn't connected yet
    , FSOUND_STREAM_NET_CONNECTING // Stream is connecting to remote host
    , FSOUND_STREAM_NET_BUFFERING // Stream is buffering data
    , FSOUND_STREAM_NET_READY // Stream is ready to play
    , FSOUND_STREAM_NET_ERROR // Stream has suffered a fatal error
    }


    // FSOUND_TAGFIELD_TYPE
    // Describes the type of a particular tag field.
    // See FSOUND_Stream_GetNumTagFields, FSOUND_Stream_GetTagField, FSOUND_Stream_FindTagField

    public enum FSOUND_TAGFIELD_TYPE
    {
        FSOUND_TAGFIELD_VORBISCOMMENT = 0 // A vorbis comment
    , FSOUND_TAGFIELD_ID3V1 // Part of an ID3v1 tag
    , FSOUND_TAGFIELD_ID3V2 // An ID3v2 frame
    , FSOUND_TAGFIELD_SHOUTCAST // A SHOUTcast header line
    , FSOUND_TAGFIELD_ICECAST // An Icecast header line
    , FSOUND_TAGFIELD_ASF // An Advanced Streaming Format header line
    }


    // FSOUND_STATUS_FLAGS
    // These values describe the protocol and format of an internet stream. Use FSOUND_Stream_Net_GetStatus to retrieve this information for an open internet stream.

    public enum FSOUND_STATUS_FLAGS
    {
        FSOUND_PROTOCOL_SHOUTCAST = 0x1
    , FSOUND_PROTOCOL_ICECAST = 0x2
    , FSOUND_PROTOCOL_HTTP = 0x4
    , FSOUND_FORMAT_MPEG = 0x10000
    , FSOUND_FORMAT_OGGVORBIS = 0x20000
    }

    // FSOUND_TOC_TAG
    // FSOUND_Stream_Open, FSOUND_Stream_FindTagField

    public class FSOUND_TOC_TAG
    {
        public byte[] TagName = new byte[3]; // The string "TOC" (4th character is 0), just in case this structure is accidentally treated as a string.
        public int NumTracks; // The number of tracks on the CD.
        public int[] Min = new int[99]; // The start offset of each track in minutes.
        public int[] Sec = new int[99]; // The start offset of each track in seconds.
        public int[] Frame = new int[99]; // The start offset of each track in frames.
    }


    // /* ================================== */
    // /* Initialization / Global functions. */
    // /* ================================== */


    // PRE - FSOUND_Init functions. These cant be called after FSOUND_Init is
    // called (they will fail). They set up FMOD system functionality.


    [DllImport("fmod.dll", EntryPoint = "_FSOUND_SetOutput@4")]
    public static extern byte FSOUND_SetOutput(FSOUND_OUTPUTTYPES outputtype);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetDriver@4")]
    public static partial byte FSOUND_SetDriver(int driver);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_SetMixer@4")]
    public static extern byte FSOUND_SetMixer(FSOUND_MIXERTYPES mixer);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetBufferSize@4")]
    public static partial byte FSOUND_SetBufferSize(int lenms);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetHWND@4")]
    public static partial byte FSOUND_SetHWND(int hwnd);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetMinHardwareChannels@4")]
    public static partial byte FSOUND_SetMinHardwareChannels(int min);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetMaxHardwareChannels@4")]
    public static partial byte FSOUND_SetMaxHardwareChannels(int min);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetMemorySystem@20")]
    public static partial byte FSOUND_SetMemorySystem(int pool, int poollen, int useralloc, int userrealloc, int userfree);


    // Main initialization / closedown functions.
    // Note : Use FSOUND_INIT_USEDEFAULTMIDISYNTH with FSOUND_Init for software override
    // with MIDI playback.
    // : Use FSOUND_INIT_GLOBALFOCUS with FSOUND_Init to make sound audible no matter
    // which window is in focus. (FSOUND_OUTPUT_DSOUND only)


    [DllImport("fmod.dll", EntryPoint = "_FSOUND_Init@12")]
    public static extern byte FSOUND_Init(int mixrate, int maxchannels, FSOUND_INITMODES flags);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Close@0")]
    public static partial int FSOUND_Close();


    // Runtime system level functions


    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Update@0")]
    public static partial int FSOUND_Update();

    [DllImport("fmod.dll", EntryPoint = "_FSOUND_SetSpeakerMode@4")]
    public static extern int FSOUND_SetSpeakerMode(FSOUND_SPEAKERMODES speakermode);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetSFXMasterVolume@4")]
    public static partial int FSOUND_SetSFXMasterVolume(int volume);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_SetPanSeperation@4")]
    public static extern int FSOUND_SetPanSeperation(decimal pansep);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_File_SetCallbacks@20")]
    public static partial int FSOUND_File_SetCallbacks(int OpenCallback, int CloseCallback, int ReadCallback, int SeekCallback, int TellCallback);


    // System information functions.


    [DllImport("fmod.dll", EntryPoint = "_FSOUND_GetError@0")]
    public static extern FMOD_ERRORS FSOUND_GetError();
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_GetVersion@0")]
    public static extern decimal FSOUND_GetVersion();
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_GetOutput@0")]
    public static extern FSOUND_OUTPUTTYPES FSOUND_GetOutput();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetOutputHandle@0")]
    public static partial int FSOUND_GetOutputHandle();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetDriver@0")]
    public static partial int FSOUND_GetDriver();
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_GetMixer@0")]
    public static extern FSOUND_MIXERTYPES FSOUND_GetMixer();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetNumDrivers@0")]
    public static partial int FSOUND_GetNumDrivers();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetDriverName@4")]
    public static partial int FSOUND_GetDriverName(int id);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetDriverCaps@8")]
    public static partial byte FSOUND_GetDriverCaps(int id, ref int caps);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetOutputRate@0")]
    public static partial int FSOUND_GetOutputRate();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetMaxChannels@0")]
    public static partial int FSOUND_GetMaxChannels();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetMaxSamples@0")]
    public static partial int FSOUND_GetMaxSamples();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetSFXMasterVolume@0")]
    public static partial int FSOUND_GetSFXMasterVolume();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetNumHWChannels@12")]
    public static partial void FSOUND_GetNumHWChannels(ref int num2d, ref int num3d, ref int total);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetChannelsPlaying@0")]
    public static partial int FSOUND_GetChannelsPlaying();
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_GetCPUUsage@0")]
    public static extern decimal FSOUND_GetCPUUsage();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetMemoryStats@8")]
    public static partial void FSOUND_GetMemoryStats(ref int currentalloced, ref int maxalloced);

    // /* =================================== */
    // /* Sample management / load functions. */
    // /* =================================== */


    // Sample creation and management functions
    // Note : Use FSOUND_LOADMEMORY   flag with FSOUND_Sample_Load to load from memory.
    // Use FSOUND_LOADRAW      flag with FSOUND_Sample_Load to treat as as raw pcm data.


    [DllImport("fmod.dll", EntryPoint = "_FSOUND_Sample_Load@20")]
    public static extern int FSOUND_Sample_Load(int index, string name, FSOUND_MODES mode, int offset, int length);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_Alloc@28")]
    public static partial int FSOUND_Sample_Alloc(int index, int length, int mode, int deffreq, int defvol, int defpan, int defpri);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_Free@4")]
    public static partial int FSOUND_Sample_Free(int sptr);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_Upload@12")]
    public static partial byte FSOUND_Sample_Upload(int sptr, ref int srcdata, int mode);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_Lock@28")]
    public static partial byte FSOUND_Sample_Lock(int sptr, int offset, int length, ref int ptr1, ref int ptr2, ref int len1, ref int len2);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_Unlock@20")]
    public static partial byte FSOUND_Sample_Unlock(int sptr, int sptr1, int sptr2, int len1, int len2);


    // Sample control functions


    [DllImport("fmod.dll", EntryPoint = "_FSOUND_Sample_SetMode@8")]
    public static extern byte FSOUND_Sample_SetMode(int sptr, FSOUND_MODES mode);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_SetLoopPoints@12")]
    public static partial byte FSOUND_Sample_SetLoopPoints(int sptr, int loopstart, int loopend);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_SetDefaults@20")]
    public static partial byte FSOUND_Sample_SetDefaults(int sptr, int deffreq, int defvol, int defpan, int defpri);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_SetDefaultsEx@32")]
    public static partial byte FSOUND_Sample_SetDefaultsEx(int sptr, int deffreq, int defvol, int defpan, int defpri, int varfreq, int varvol, int varpan);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_Sample_SetMinMaxDistance@12")]
    public static extern byte FSOUND_Sample_SetMinMaxDistance(int sptr, decimal min, decimal max);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_SetMaxPlaybacks@8")]
    public static partial byte FSOUND_Sample_SetMaxPlaybacks(int sptr, int max);


    // Sample information functions


    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_Get@4")]
    public static partial int FSOUND_Sample_Get(int sampno);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_GetName@4")]
    public static partial int FSOUND_Sample_GetName(int sptr);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_GetLength@4")]
    public static partial int FSOUND_Sample_GetLength(int sptr);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_GetLoopPoints@12")]
    public static partial byte FSOUND_Sample_GetLoopPoints(int sptr, ref int loopstart, ref int loopend);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_GetDefaults@20")]
    public static partial byte FSOUND_Sample_GetDefaults(int sptr, ref int deffreq, ref int defvol, ref int defpan, ref int defpri);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_GetDefaultsEx@32")]
    public static partial byte FSOUND_Sample_GetDefaultsEx(int sptr, ref int deffreq, ref int defvol, ref int defpan, ref int defpri, ref int varfreq, ref int varvol, ref int varpan);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Sample_GetMode@4")]
    public static partial int FSOUND_Sample_GetMode(int sptr);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_Sample_GetMinMaxDistance@12")]
    public static extern byte FSOUND_Sample_GetMinMaxDistance(int sptr, ref decimal min, ref decimal max);

    // /* ============================ */
    // /* Channel control functions.   */
    // /* ============================ */


    // Playing and stopping sounds.
    // Note : Use FSOUND_FREE as the channel variable, to let FMOD pick a free channel for you.
    // Use FSOUND_ALL as the channel variable to control ALL channels with one function call!


    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_PlaySound@8")]
    public static partial int FSOUND_PlaySound(int channel, int sptr);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_PlaySoundEx@16")]
    public static partial int FSOUND_PlaySoundEx(int channel, int sptr, int dsp, byte startpaused);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_StopSound@4")]
    public static partial byte FSOUND_StopSound(int channel);


    // Functions to control playback of a channel.
    // Note : FSOUND_ALL can be used on most of these functions as a channel value.


    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetFrequency@8")]
    public static partial byte FSOUND_SetFrequency(int channel, int freq);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetVolume@8")]
    public static partial byte FSOUND_SetVolume(int channel, int Vol);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetVolumeAbsolute@8")]
    public static partial byte FSOUND_SetVolumeAbsolute(int channel, int Vol);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetPan@8")]
    public static partial byte FSOUND_SetPan(int channel, int pan);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetSurround@8")]
    public static partial byte FSOUND_SetSurround(int channel, int surround);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetMute@8")]
    public static partial byte FSOUND_SetMute(int channel, byte mute);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetPriority@8")]
    public static partial byte FSOUND_SetPriority(int channel, int Priority);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetReserved@8")]
    public static partial byte FSOUND_SetReserved(int channel, int reserved);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetPaused@8")]
    public static partial byte FSOUND_SetPaused(int channel, byte Paused);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetLoopMode@8")]
    public static partial byte FSOUND_SetLoopMode(int channel, int loopmode);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_SetCurrentPosition@8")]
    public static partial byte FSOUND_SetCurrentPosition(int channel, int offset);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_3D_SetAttributes@12")]
    public static extern byte FSOUND_3D_SetAttributes(int channel, ref decimal Pos, ref decimal vel);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_3D_SetMinMaxDistance@12")]
    public static extern byte FSOUND_3D_SetMinMaxDistance(int channel, decimal min, decimal max);


    // Channel information functions.


    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_IsPlaying@4")]
    public static partial byte FSOUND_IsPlaying(int channel);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetFrequency@4")]
    public static partial int FSOUND_GetFrequency(int channel);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetVolume@4")]
    public static partial int FSOUND_GetVolume(int channel);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetAmplitude@4")]
    public static partial int FSOUND_GetAmplitude(int channel);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetPan@4")]
    public static partial int FSOUND_GetPan(int channel);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetSurround@4")]
    public static partial byte FSOUND_GetSurround(int channel);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetMute@4")]
    public static partial byte FSOUND_GetMute(int channel);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetPriority@4")]
    public static partial int FSOUND_GetPriority(int channel);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetReserved@4")]
    public static partial byte FSOUND_GetReserved(int channel);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetPaused@4")]
    public static partial byte FSOUND_GetPaused(int channel);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetLoopMode@4")]
    public static partial int FSOUND_GetLoopMode(int channel);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetCurrentPosition@4")]
    public static partial int FSOUND_GetCurrentPosition(int channel);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetCurrentSample@4")]
    public static partial int FSOUND_GetCurrentSample(int channel);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_GetCurrentLevels@12")]
    public static extern byte FSOUND_GetCurrentLevels(int channel, ref decimal l, ref decimal r);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetNumSubChannels@4")]
    public static partial int FSOUND_GetNumSubChannels(int channel);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_GetSubChannel@8")]
    public static partial int FSOUND_GetSubChannel(int channel, int subchannel);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_3D_GetAttributes@12")]
    public static extern byte FSOUND_3D_GetAttributes(int channel, ref decimal Pos, ref decimal vel);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_3D_GetMinMaxDistance@12")]
    public static extern byte FSOUND_3D_GetMinMaxDistance(int channel, ref decimal min, ref decimal max);

    // /* ========================== */
    // /* Global 3D sound functions. */
    // /* ========================== */

    // See also 3d sample and channel based functions above.
    // Call FSOUND_Update once a frame to process 3d information.

    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_3D_Listener_SetCurrent@8")]
    public static partial int FSOUND_3D_Listener_SetCurrent(int current);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_3D_Listener_SetAttributes@32")]
    public static extern int FSOUND_3D_Listener_SetAttributes(decimal Pos, decimal vel, decimal fx, decimal fy, decimal fz, decimal tx, decimal ty, decimal tz);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_3D_Listener_GetAttributes@32")]
    public static extern int FSOUND_3D_Listener_GetAttributes(ref decimal Pos, ref decimal vel, ref decimal fx, ref decimal fy, ref decimal fz, ref decimal tx, ref decimal ty, ref decimal tz);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_3D_SetDopplerFactor@4")]
    public static extern int FSOUND_3D_SetDopplerFactor(decimal fscale);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_3D_SetDistanceFactor@4")]
    public static extern int FSOUND_3D_SetDistanceFactor(decimal fscale);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_3D_SetRolloffFactor@4")]
    public static extern int FSOUND_3D_SetRolloffFactor(decimal fscale);

    // /* =================== */
    // /* FX functions.       */
    // /* =================== */


    // Functions to control DX8 only effects processing.

    // - FX enabled samples can only be played once at a time, not multiple times at once.
    // - Sounds have to be created with FSOUND_HW2D or FSOUND_HW3D for this to work.
    // - FSOUND_INIT_ENABLESYSTEMCHANNELFX can be used to apply hardware effect processing to the
    // global mixed output of FMOD's software channels.
    // - FSOUND_FX_Enable returns an FX handle that you can use to alter fx parameters.
    // - FSOUND_FX_Enable can be called multiple times in a row, even on the same FX type,
    // it will return a unique handle for each FX.
    // - FSOUND_FX_Enable cannot be called if the sound is playing or locked.
    // - FSOUND_FX_Disable must be called to reset/clear the FX from a channel.


    [DllImport("fmod.dll", EntryPoint = "_FSOUND_FX_Enable@8")]
    public static extern int FSOUND_FX_Enable(int channel, FSOUND_FX_MODES fx);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_FX_Disable@4")]
    public static partial byte FSOUND_FX_Disable(int channel);

    [DllImport("fmod.dll", EntryPoint = "_FSOUND_FX_SetChorus@32")]
    public static extern byte FSOUND_FX_SetChorus(int fxid, decimal WetDryMix, decimal Depth, decimal Feedback, decimal Frequency, int Waveform, decimal Delay, int Phase);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_FX_SetCompressor@28")]
    public static extern byte FSOUND_FX_SetCompressor(int fxid, decimal Gain, decimal Attack, decimal Release, decimal Threshold, decimal Ratio, decimal Predelay);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_FX_SetDistortion@24")]
    public static extern byte FSOUND_FX_SetDistortion(int fxid, decimal Gain, decimal Edge, decimal PostEQCenterFrequency, decimal PostEQBandwidth, decimal PreLowpassCutoff);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_FX_SetEcho@24")]
    public static extern byte FSOUND_FX_SetEcho(int fxid, decimal WetDryMix, decimal Feedback, decimal LeftDelay, decimal RightDelay, int PanDelay);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_FX_SetFlanger@32")]
    public static extern byte FSOUND_FX_SetFlanger(int fxid, decimal WetDryMix, decimal Depth, decimal Feedback, decimal Frequency, int Waveform, decimal Delay, int Phase);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_FX_SetGargle@12")]
    public static partial byte FSOUND_FX_SetGargle(int fxid, int RateHz, int WaveShape);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_FX_SetI3DL2Reverb@52")]
    public static extern byte FSOUND_FX_SetI3DL2Reverb(int fxid, int Room, int RoomHF, decimal RoomRolloffFactor, decimal DecayTime, decimal DecayHFRatio, int Reflections, decimal ReflectionsDelay, int Reverb, decimal ReverbDelay, decimal Diffusion, decimal Density, decimal HFReference);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_FX_SetParamEQ@16")]
    public static extern byte FSOUND_FX_SetParamEQ(int fxid, decimal Center, decimal Bandwidth, decimal Gain);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_FX_SetWavesReverb@20")]
    public static extern byte FSOUND_FX_SetWavesReverb(int fxid, decimal InGain, decimal ReverbMix, decimal ReverbTime, decimal HighFreqRTRatio);

    // ========================= */
    // File Streaming functions. */
    // ========================= */


    // Note : Use FSOUND_LOADMEMORY   flag with FSOUND_Stream_Open to stream from memory.
    // Use FSOUND_LOADRAW      flag with FSOUND_Stream_Open to treat stream as raw pcm data.
    // Use FSOUND_MPEGACCURATE flag with FSOUND_Stream_Open to open mpegs in 'accurate mode' for settime/gettime/getlengthms.
    // Use FSOUND_FREE as the 'channel' variable, to let FMOD pick a free channel for you.


    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_SetBufferSize@4")]
    public static partial byte FSOUND_Stream_SetBufferSize(int ms);

    [DllImport("fmod.dll", EntryPoint = "_FSOUND_Stream_Open@16")]
    public static extern int FSOUND_Stream_Open(string filename, FSOUND_MODES mode, int offset, int length);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_Stream_Open@16")]
    public static extern int FSOUND_Stream_Open2(ref byte data, FSOUND_MODES mode, int offset, int length);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_Create@20")]
    public static partial int FSOUND_Stream_Create(int callback, int length, int mode, int samplerate, int userdata);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_Close@4")]
    public static partial byte FSOUND_Stream_Close(int stream);

    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_Play@8")]
    public static partial int FSOUND_Stream_Play(int channel, int stream);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_PlayEx@16")]
    public static partial int FSOUND_Stream_PlayEx(int channel, int stream, int dsp, byte startpaused);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_Stop@4")]
    public static partial byte FSOUND_Stream_Stop(int stream);

    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_SetPosition@8")]
    public static partial byte FSOUND_Stream_SetPosition(int stream, int positition);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_GetPosition@4")]
    public static partial int FSOUND_Stream_GetPosition(int stream);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_SetTime@8")]
    public static partial byte FSOUND_Stream_SetTime(int stream, int ms);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_GetTime@4")]
    public static partial int FSOUND_Stream_GetTime(int stream);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_GetLength@4")]
    public static partial int FSOUND_Stream_GetLength(int stream);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_GetLengthMs@4")]
    public static partial int FSOUND_Stream_GetLengthMs(int stream);

    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_SetMode@8")]
    public static partial byte FSOUND_Stream_SetMode(int stream, int mode);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_GetMode@4")]
    public static partial int FSOUND_Stream_GetMode(int stream);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_SetLoopPoints@12")]
    public static partial byte FSOUND_Stream_SetLoopPoints(int stream, int loopstartpcm, int loopendpcm);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_SetLoopCount@8")]
    public static partial byte FSOUND_Stream_SetLoopCount(int stream, int count);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_GetOpenState@4")]
    public static partial int FSOUND_Stream_GetOpenState(int stream);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_GetSample@4")]
    public static partial int FSOUND_Stream_GetSample(int stream);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_CreateDSP@16")]
    public static partial int FSOUND_Stream_CreateDSP(int stream, int callback, int Priority, int userdata);

    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_SetEndCallback@12")]
    public static partial byte FSOUND_Stream_SetEndCallback(int stream, int callback, int userdata);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_SetSyncCallback@12")]
    public static partial byte FSOUND_Stream_SetSyncCallback(int stream, int callback, int userdata);

    [DllImport("fmod.dll", EntryPoint = "_FSOUND_Stream_AddSyncPoint@12")]
    public static extern int FSOUND_Stream_AddSyncPoint(int stream, int pcmoffset, string name);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_DeleteSyncPoint@4")]
    public static partial byte FSOUND_Stream_DeleteSyncPoint(int point);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_GetNumSyncPoints@4")]
    public static partial int FSOUND_Stream_GetNumSyncPoints(int stream);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_GetSyncPoint@8")]
    public static partial int FSOUND_Stream_GetSyncPoint(int stream, int index);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_GetSyncPointInfo@8")]
    public static partial int FSOUND_Stream_GetSyncPointInfo(int point, ref int pcmoffset);

    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_SetSubStream@8")]
    public static partial byte FSOUND_Stream_SetSubStream(int stream, int index);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_GetNumSubStreams@4")]
    public static partial int FSOUND_Stream_GetNumSubStreams(int stream);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_SetSubStreamSentence@12")]
    public static partial byte FSOUND_Stream_SetSubStreamSentence(int stream, ref int sentencelist, int numitems);

    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_GetNumTagFields@8")]
    public static partial byte FSOUND_Stream_GetNumTagFields(int stream, ref int num);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_GetTagField@24")]
    public static partial byte FSOUND_Stream_GetTagField(int stream, int num, ref int tagtype, ref int name, ref int value, ref int length);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_Stream_FindTagField@20")]
    public static extern byte FSOUND_Stream_FindTagField(int stream, int tagtype, string name, ref int value, ref int length);


    // Internet streaming functions


    [DllImport("fmod.dll", EntryPoint = "_FSOUND_Stream_Net_SetProxy@4")]
    public static extern byte FSOUND_Stream_Net_SetProxy(string proxy);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_Net_GetLastServerStatus@0")]
    public static partial int FSOUND_Stream_Net_GetLastServerStatus();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_Net_SetBufferProperties@12")]
    public static partial byte FSOUND_Stream_Net_SetBufferProperties(int buffersize, int prebuffer_percent, int rebuffer_percent);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_Net_GetBufferProperties@12")]
    public static partial byte FSOUND_Stream_Net_GetBufferProperties(ref int buffersize, ref int prebuffer_percent, ref int rebuffer_percent);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_Net_SetMetadataCallback@12")]
    public static partial byte FSOUND_Stream_Net_SetMetadataCallback(int stream, int callback, int userdata);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Stream_Net_GetStatus@20")]
    public static partial byte FSOUND_Stream_Net_GetStatus(int stream, ref int status, ref int bufferpercentused, ref int bitrate, ref int flags);

    // /* =================== */
    // /* CD audio functions. */
    // /* =================== */


    // Note : 0 = default cdrom.  Otherwise specify the drive letter, for example. 'D'.


    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_CD_Play@8")]
    public static partial byte FSOUND_CD_Play(byte drive, int Track);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_CD_SetPlayMode@8")]
    public static extern int FSOUND_CD_SetPlayMode(byte drive, FSOUND_CDPLAYMODES mode);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_CD_Stop@4")]
    public static partial byte FSOUND_CD_Stop(byte drive);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_CD_SetPaused@8")]
    public static partial byte FSOUND_CD_SetPaused(byte drive, byte Paused);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_CD_SetVolume@8")]
    public static partial byte FSOUND_CD_SetVolume(byte drive, int volume);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_CD_SetTrackTime@8")]
    public static partial byte FSOUND_CD_SetTrackTime(byte drive, int ms);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_CD_OpenTray@8")]
    public static partial byte FSOUND_CD_OpenTray(byte drive, byte openState);

    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_CD_GetPaused@4")]
    public static partial byte FSOUND_CD_GetPaused(byte drive);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_CD_GetTrack@4")]
    public static partial int FSOUND_CD_GetTrack(byte drive);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_CD_GetNumTracks@4")]
    public static partial int FSOUND_CD_GetNumTracks(byte drive);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_CD_GetVolume@4")]
    public static partial int FSOUND_CD_GetVolume(byte drive);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_CD_GetTrackLength@8")]
    public static partial int FSOUND_CD_GetTrackLength(byte drive, int Track);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_CD_GetTrackTime@4")]
    public static partial int FSOUND_CD_GetTrackTime(byte drive);

    // /* ============== */
    // /* DSP functions. */
    // /* ============== */


    // DSP Unit control and information functions.
    // These functions allow you access to the mixed stream that FMOD uses to play back sound on.


    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_Create@12")]
    public static partial int FSOUND_DSP_Create(int callback, int Priority, int param);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_Free@4")]
    public static partial int FSOUND_DSP_Free(int unit);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_SetPriority@8")]
    public static partial int FSOUND_DSP_SetPriority(int unit, int Priority);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_GetPriority@4")]
    public static partial int FSOUND_DSP_GetPriority(int unit);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_SetActive@8")]
    public static partial int FSOUND_DSP_SetActive(int unit, int active);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_GetActive@4")]
    public static partial byte FSOUND_DSP_GetActive(int unit);


    // Functions to get hold of FSOUND 'system DSP unit' handles


    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_GetClearUnit@0")]
    public static partial int FSOUND_DSP_GetClearUnit();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_GetSFXUnit@0")]
    public static partial int FSOUND_DSP_GetSFXUnit();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_GetMusicUnit@0")]
    public static partial int FSOUND_DSP_GetMusicUnit();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_GetFFTUnit@0")]
    public static partial int FSOUND_DSP_GetFFTUnit();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_GetClipAndCopyUnit@0")]
    public static partial int FSOUND_DSP_GetClipAndCopyUnit();


    // Miscellaneous DSP functions
    // Note for the spectrum analysis function to work, you have to enable the FFT DSP unit with
    // the following code FSOUND_DSP_SetActive(FSOUND_DSP_GetFFTUnit(), TRUE);
    // It is off by default to save cpu usage.


    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_MixBuffers@28")]
    public static partial byte FSOUND_DSP_MixBuffers(int destbuffer, int srcbuffer, int length, int freq, int Vol, int pan, int mode);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_ClearMixBuffer@0")]
    public static partial int FSOUND_DSP_ClearMixBuffer();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_GetBufferLength@0")]
    public static partial int FSOUND_DSP_GetBufferLength();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_GetBufferLengthTotal@0")]
    public static partial int FSOUND_DSP_GetBufferLengthTotal();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_DSP_GetSpectrum@0")]
    public static partial int FSOUND_DSP_GetSpectrum();

    // /* =================================================================================== */
    // /* Reverb functions. (eax2/eax3 reverb)  (ONLY SUPPORTED ON WIN32 W/ FSOUND_HW3D FLAG) */
    // /* =================================================================================== */


    // See top of file for definitions and information on the reverb parameters.

    // The FSOUND_REVERB_PRESETS have not been included in VB yet so they cannot yet be used here...
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_Reverb_SetProperties@4")]
    public static extern byte FSOUND_Reverb_SetProperties(ref FSOUND_REVERB_PROPERTIES prop);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_Reverb_GetProperties@4")]
    public static extern byte FSOUND_Reverb_GetProperties(ref FSOUND_REVERB_PROPERTIES prop);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_Reverb_SetChannelProperties@8")]
    public static extern byte FSOUND_Reverb_SetChannelProperties(int channel, ref FSOUND_REVERB_CHANNELPROPERTIES prop);
    [DllImport("fmod.dll", EntryPoint = "_FSOUND_Reverb_GetChannelProperties@8")]
    public static extern byte FSOUND_Reverb_GetChannelProperties(int channel, ref FSOUND_REVERB_CHANNELPROPERTIES prop);

    // /* ===================================================== */
    // /* Recording functions  (ONLY SUPPORTED IN WIN32, WINCE) */
    // /* ===================================================== */


    // Recording initialization functions


    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Record_SetDriver@4")]
    public static partial byte FSOUND_Record_SetDriver(int outputtype);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Record_GetNumDrivers@0")]
    public static partial int FSOUND_Record_GetNumDrivers();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Record_GetDriverName@4")]
    public static partial int FSOUND_Record_GetDriverName(int id);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Record_GetDriver@0")]
    public static partial int FSOUND_Record_GetDriver();


    // Recording functionality.  Only one recording session will work at a time.


    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Record_StartSample@8")]
    public static partial byte FSOUND_Record_StartSample(int sample, [MarshalAs(UnmanagedType.Bool)] bool loopit);
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Record_Stop@0")]
    public static partial byte FSOUND_Record_Stop();
    [LibraryImport("fmod.dll", EntryPoint = "_FSOUND_Record_GetPosition@0")]
    public static partial int FSOUND_Record_GetPosition();

    // /* ========================================================================================== */
    // /* FMUSIC API (MOD,S3M,XM,IT,MIDI PLAYBACK)                                                   */
    // /* ========================================================================================== */


    // Song management / playback functions.


    [DllImport("fmod.dll", EntryPoint = "_FMUSIC_LoadSong@4")]
    public static extern int FMUSIC_LoadSong(string name);
    [DllImport("fmod.dll", EntryPoint = "_FMUSIC_LoadSongEx@24")]
    public static extern int FMUSIC_LoadSongEx(string name, int offset, int length, FSOUND_MODES mode, ref int sentencelist, int numitems);
    [DllImport("fmod.dll", EntryPoint = "_FMUSIC_LoadSongEx@24")]
    public static extern int FMUSIC_LoadSongEx2(ref byte data, int offset, int length, FSOUND_MODES mode, ref int sentencelist, int numitems);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetOpenState@4")]
    public static partial int FMUSIC_GetOpenState(int module);

    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_FreeSong@4")]
    public static partial byte FMUSIC_FreeSong(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_PlaySong@4")]
    public static partial byte FMUSIC_PlaySong(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_StopSong@4")]
    public static partial byte FMUSIC_StopSong(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_StopAllSongs@0")]
    public static partial int FMUSIC_StopAllSongs();

    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_SetZxxCallback@8")]
    public static partial byte FMUSIC_SetZxxCallback(int module, int callback);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_SetRowCallback@12")]
    public static partial byte FMUSIC_SetRowCallback(int module, int callback, int rowstep);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_SetOrderCallback@12")]
    public static partial byte FMUSIC_SetOrderCallback(int module, int callback, int rowstep);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_SetInstCallback@12")]
    public static partial byte FMUSIC_SetInstCallback(int module, int callback, int instrument);

    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_SetSample@12")]
    public static partial byte FMUSIC_SetSample(int module, int sampno, int sptr);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_SetUserData@8")]
    public static partial byte FMUSIC_SetUserData(int module, int userdata);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_OptimizeChannels@12")]
    public static partial byte FMUSIC_OptimizeChannels(int module, int maxchannels, int minvolume);


    // Runtime song functions


    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_SetReverb@4")]
    public static partial byte FMUSIC_SetReverb(byte Reverb);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_SetLooping@8")]
    public static partial byte FMUSIC_SetLooping(int module, byte looping);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_SetOrder@8")]
    public static partial byte FMUSIC_SetOrder(int module, int order);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_SetPaused@8")]
    public static partial byte FMUSIC_SetPaused(int module, byte Pause);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_SetMasterVolume@8")]
    public static partial byte FMUSIC_SetMasterVolume(int module, int volume);
    [DllImport("fmod.dll", EntryPoint = "_FMUSIC_SetMasterSpeed@8")]
    public static extern byte FMUSIC_SetMasterSpeed(int module, decimal speed);
    [DllImport("fmod.dll", EntryPoint = "_FMUSIC_SetPanSeperation@8")]
    public static extern byte FMUSIC_SetPanSeperation(int module, decimal pansep);


    // Static song information functions


    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetName@4")]
    public static partial int FMUSIC_GetName(int module);
    [DllImport("fmod.dll", EntryPoint = "_FMUSIC_GetType@4")]
    public static extern FMUSIC_TYPES FMUSIC_GetType(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetNumOrders@4")]
    public static partial int FMUSIC_GetNumOrders(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetNumPatterns@4")]
    public static partial int FMUSIC_GetNumPatterns(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetNumInstruments@4")]
    public static partial int FMUSIC_GetNumInstruments(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetNumSamples@4")]
    public static partial int FMUSIC_GetNumSamples(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetNumChannels@4")]
    public static partial int FMUSIC_GetNumChannels(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetSample@8")]
    public static partial int FMUSIC_GetSample(int module, int sampno);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetPatternLength@8")]
    public static partial int FMUSIC_GetPatternLength(int module, int orderno);


    // Runtime song information


    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_IsFinished@4")]
    public static partial byte FMUSIC_IsFinished(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_IsPlaying@4")]
    public static partial byte FMUSIC_IsPlaying(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetMasterVolume@4")]
    public static partial int FMUSIC_GetMasterVolume(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetGlobalVolume@4")]
    public static partial int FMUSIC_GetGlobalVolume(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetOrder@4")]
    public static partial int FMUSIC_GetOrder(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetPattern@4")]
    public static partial int FMUSIC_GetPattern(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetSpeed@4")]
    public static partial int FMUSIC_GetSpeed(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetBPM@4")]
    public static partial int FMUSIC_GetBPM(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetRow@4")]
    public static partial int FMUSIC_GetRow(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetPaused@4")]
    public static partial byte FMUSIC_GetPaused(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetTime@4")]
    public static partial int FMUSIC_GetTime(int module);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetRealChannel@8")]
    public static partial int FMUSIC_GetRealChannel(int module, int modchannel);
    [LibraryImport("fmod.dll", EntryPoint = "_FMUSIC_GetUserData@4")]
    public static partial int FMUSIC_GetUserData(int module);

    // ************
    // * Windows Declarations (Added by Adion)
    // ************
    // Required for GetStringFromPointer
    [DllImport("kernel32", EntryPoint = "lstrcpyA")]
    private static extern int ConvCStringToVBString(string lpsz, int pt); // Notice the As Long return value replacing the As String given by the API Viewer.
                                                                          // Required for the FFT/Spectral functions
    [DllImport("kernel32", EntryPoint = "RtlMoveMemory")]
    private static extern void CopyMemory(ref dynamic Destination, ref dynamic Source, int length);

    // ************
    // * FUNCTIONS (Added by Adion)
    // ************
    // Usage: myerrorstring = FSOUND_GetErrorString(FSOUND_GetError)
    public static string FSOUND_GetErrorString(int errorcode)
        => (FMOD_ERRORS)errorcode switch
        {
            FMOD_ERRORS.FMOD_ERR_NONE => "No errors",
            FMOD_ERRORS.FMOD_ERR_BUSY => "Cannot call this command after FSOUND_Init.  Call FSOUND_Close first.",
            FMOD_ERRORS.FMOD_ERR_UNINITIALIZED => "This command failed because FSOUND_Init was not called",
            FMOD_ERRORS.FMOD_ERR_PLAY => "Playing the sound failed.",
            FMOD_ERRORS.FMOD_ERR_INIT => "Error initializing output device.",
            FMOD_ERRORS.FMOD_ERR_ALLOCATED => "The output device is already in use and cannot be reused.",
            FMOD_ERRORS.FMOD_ERR_OUTPUT_FORMAT => "Soundcard does not support the features needed for this soundsystem (16bit stereo output)",
            FMOD_ERRORS.FMOD_ERR_COOPERATIVELEVEL => "Error setting cooperative level for hardware.",
            FMOD_ERRORS.FMOD_ERR_CREATEBUFFER => "Error creating hardware sound buffer.",
            FMOD_ERRORS.FMOD_ERR_FILE_NOTFOUND => "File not found",
            FMOD_ERRORS.FMOD_ERR_FILE_FORMAT => "Unknown file format",
            FMOD_ERRORS.FMOD_ERR_FILE_BAD => "Error loading file",
            FMOD_ERRORS.FMOD_ERR_MEMORY => "Not enough memory ",
            FMOD_ERRORS.FMOD_ERR_VERSION => "The version number of this file format is not supported",
            FMOD_ERRORS.FMOD_ERR_INVALID_PARAM => "An invalid parameter was passed to this function",
            FMOD_ERRORS.FMOD_ERR_NO_EAX => "Tried to use an EAX command on a non EAX enabled channel or output.",
            FMOD_ERRORS.FMOD_ERR_CHANNEL_ALLOC => "Failed to allocate a new channel",
            FMOD_ERRORS.FMOD_ERR_RECORD => "Recording is not supported on this machine",
            FMOD_ERRORS.FMOD_ERR_MEDIAPLAYER => "Required Mediaplayer codec is not installed",
            FMOD_ERRORS.FMOD_ERR_CDDEVICE => "An error occured trying to open the specified CD device",
            _ => "Unknown error",
        };

    // Thanks to KarLKoX for the following function
    // Example: MyDriverName = GetStringFromPointer(FSOUND_GetDriverName(count))
    public static string GetStringFromPointer(int lpString)
    {
        string szBuffer = new('\0', 255);
        ConvCStringToVBString(szBuffer, lpString);
        // Look for the null char ending the C string
        int NullCharPos = InStr(szBuffer, vbNullChar);
        return Left(szBuffer, NullCharPos - 1);
    }

    // These functions are added by Adion
    public static decimal GetSingleFromPointer(int lpSingle)
    {
        dynamic _GetSingleFromPointer = 0m;
        // A Single is 4 bytes, so we copy 4 bytes
        dynamic source = lpSingle;
        CopyMemory(ref _GetSingleFromPointer, ref source, 4);
        return _GetSingleFromPointer;
    }
    // Warning: You should set the fft dsp to active before retreiving the spectrum
    // Also make sure the array you pass is dimensioned and ready to use
    // FSOUND_DSP_SetActive FSOUND_DSP_GetFFTUnit, 1
    public static void GetSpectrum(ref decimal[] Spectrum)
    {
        int nrOfVals;
        if (Spectrum.Length > 511) nrOfVals = 512; else nrOfVals = Spectrum.Length + 1;
        int lpSpectrum = FSOUND_DSP_GetSpectrum();
        dynamic dest = Spectrum[0];
        dynamic source = lpSpectrum;
        CopyMemory(ref dest, ref source, nrOfVals * 4);
        Spectrum[0] = dest;
    }
}
