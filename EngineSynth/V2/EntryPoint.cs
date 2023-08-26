using EngineSynth.V2.Model;
using EngineSynth.View;
using System;

namespace EngineSynth.V2
{
    internal class EntryPoint
    {
        [STAThread]
        public static void Main(string[] args)
        {
            //test();

            var app = new App();
            app.Run();
        }

        private unsafe static void test()
        {
            Synth synth = new Synth();

            synth.CylinderCount = 8;
            synth.PowerstrokesPerRevolution = 4;
            synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_01.wav");
            synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Kick1_Inferno\WAV\TMKD01 - inferno_kick_01.wav");
            synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_02.wav");
            synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Kick1_Inferno\WAV\TMKD01 - inferno_kick_02.wav");
            synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Kick1_Inferno\WAV\TMKD01 - inferno_kick_03.wav");
            synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_03.wav");
            synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Kick1_Inferno\WAV\TMKD01 - inferno_kick_04.wav");
            synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_04.wav");

            //synth.CylinderCount = 12;
            //synth.PowerstrokesPerRevolution = 12;
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_01.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare3_14x6-brass\TMKD_Snare3_14x6-Brass_hard_04.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare2_YMCAN_14x5-5\YMCAN14x5.5_RIMSHOT\TMKD_SnareYMCAN_rimshot_01.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_02.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare3_14x6-brass\TMKD_Snare3_14x6-Brass_hard_03.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare2_YMCAN_14x5-5\YMCAN14x5.5_RIMSHOT\TMKD_SnareYMCAN_rimshot_02.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_03.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare3_14x6-brass\TMKD_Snare3_14x6-Brass_hard_02.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare2_YMCAN_14x5-5\YMCAN14x5.5_RIMSHOT\TMKD_SnareYMCAN_rimshot_03.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_04.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare3_14x6-brass\TMKD_Snare3_14x6-Brass_hard_01.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare2_YMCAN_14x5-5\YMCAN14x5.5_RIMSHOT\TMKD_SnareYMCAN_rimshot_04.wav");

            //synth.CylinderCount = 4;
            //synth.PowerstrokesPerRevolution = 4;
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_01.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_02.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_03.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_04.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_05.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare1_BB14x6\TMKD_SNARE1_BB14x6_L8_06.wav");

            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare2_YMCAN_14x5-5\YMCAN14x5.5_RIMSHOT\TMKD_SnareYMCAN_rimshot_01.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare2_YMCAN_14x5-5\YMCAN14x5.5_RIMSHOT\TMKD_SnareYMCAN_rimshot_02.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare2_YMCAN_14x5-5\YMCAN14x5.5_RIMSHOT\TMKD_SnareYMCAN_rimshot_03.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare2_YMCAN_14x5-5\YMCAN14x5.5_RIMSHOT\TMKD_SnareYMCAN_rimshot_04.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare2_YMCAN_14x5-5\YMCAN14x5.5_RIMSHOT\TMKD_SnareYMCAN_rimshot_05.wav");
            //synth.SamplePaths.Add(@"C:\Users\Maxsimus\Documents\samples\Acoustic Drums\TMKD_Snare2_YMCAN_14x5-5\YMCAN14x5.5_RIMSHOT\TMKD_SnareYMCAN_rimshot_06.wav");

            //synth.Init();
            //RealtimePlayer player = new RealtimePlayer(synth);
            //player.Start();
            //Thread.Sleep(1000000);

            //BlendExporter blendExporter = new BlendExporter();
            //blendExporter.ExportPath = @"C:\Users\Maxsimus\AppData\Local\BeamNG.drive\0.29\mods\unpacked\787";
            //blendExporter.Name = "m787b_exhaust";

            //blendExporter.ExportSoundBlend(synth);
        }
    }
}
