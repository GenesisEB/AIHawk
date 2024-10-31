using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;
using BizHawk.Emulation.Common;
using RLMatrix;
using RLMatrix.Agents.Common;

namespace AIHawk
{

    [ExternalTool("AIHawk")] // this name appears in the Tools > External Tools submenu in EmuHawk
    public sealed class AIHawkForm : ToolFormBase, IExternalToolForm
    {
        #region APISETUP
        [RequiredApi]
        public ApiContainer? _maybeEmulationAPI { get; set; }
        private ApiContainer EmulationAPI => _maybeEmulationAPI!;
        public static ApiContainer? APISimpleton = null;
        #endregion
        #region PPOSETUP
        // Will eventually be configured by a form on first run, so moved out of local scope.
        internal PPOAgentOptions OptsPPO = new PPOAgentOptions(
                   batchSize: 240,           // Number of EPISODES agent interacts with environment before learning from its experience
                   memorySize: 10000,       // Size of the replay buffer
                   gamma: 0.99f,          // Discount factor for rewards
                   gaeLambda: 0.95f,      // Lambda factor for Generalized Advantage Estimation
                   lr: 3e-4f,            // Learning rate
                   clipEpsilon: 0.2f,     // Clipping factor for PPO's objective function
                   vClipRange: 0.2f,      // Clipping range for value loss
                   cValue: 0.5f,          // Coefficient for value loss
                   ppoEpochs: 20,            // Number of PPO epochs
                   clipGradNorm: 0.5f    // Maximum allowed gradient norm
                );
        List<IEnvironmentAsync<float[]>> EnvPPO;
        LocalDiscreteRolloutAgent<float[]> MyAgentPPO;
        #endregion

        internal bool IsAIRunning = false;
        protected override string WindowTitleStatic => "AIHawk 0.1.0";

        /// <summary>
        /// Constructor for the AIHawk plugin WinForm. DLL Entry Point.
        /// </summary>
        public AIHawkForm()
        {
            // Used to Force Torch to start up to check for errors before loading game state. (Checks for natives in plugin location.)
            // TODO: catch these errors and handle it cleanly
            LogUtility.Log("Setting CUDA Device...");
            TorchSharp.torch.InitializeDeviceType(TorchSharp.DeviceType.CUDA);

            // Assign API Simpleton Reference.
            APISimpleton = EmulationAPI;

            //WinForm Setup for Plugin, See Interfaces
            LogUtility.Log("Opening Form...");
            ClientSize = new Size(480, 320);
            SuspendLayout( );

            UIHelper.StartButton.Click += ToggleAI;
            Controls.Add(UIHelper.StartButton);
            Controls.Add(UIHelper.Observations);

            ResumeLayout(performLayout: false);
            PerformLayout( );

        }

        /// <summary>
        /// Called each time the emulator context changes.
        /// Examples:
        /// When the emulator is started. (Null context)
        /// When a Savestate is Loaded.
        /// When a Game is Started or Restarted.
        /// </summary>
        public override void Restart()
        {

            APISimpleton = EmulationAPI;
            LogUtility.Log("Setting Up " + APISimpleton.Emulation.GetGameInfo( ).Name);

        }

        /// <summary>
        /// Called after every emulation cycle has finished. Can be paused by pausing emulation. Can be advanced with frame advance.
        /// Currently setup to pause on start and frame advance manually with a hotkey in the emulator.
        /// </summary>
        protected override void UpdateAfter()
        {
            //Check emergency stop button, allows running emulation without feeding the AI.
            if ( IsAIRunning && MyAgentPPO != null )
            {
                LogUtility.Log("Emulator: Step! AI On");
                Task.Run(
                    () => MyAgentPPO.Step( )).ContinueWith((t) =>
                {
                    if ( t.IsFaulted )
                    {
                        throw t.Exception;
                    }
                }).Wait( );
                //await MyAgentPPO.Step( );
            }
        }

        private void ToggleAI(object sender, EventArgs e)
        {

            IsAIRunning = !IsAIRunning;

            if ( IsAIRunning )
            {
                if ( APISimpleton == null )
                {
                    //TODO: Should throw an error, application will not work without the emulator.
                    LogUtility.Log("Emulator not found.");
                    return;
                }

                if ( !APISimpleton.Emulation.GetGameInfo( ).IsNullInstance( ) )
                {
                    LogUtility.Log("Loading env...");
                    APISimpleton.EmuClient.Pause( );
                    _ = APISimpleton.SaveState.LoadSlot(1);
                    EnvPPO = [new BizHawkEnvironment( ).RLInit( )];
                    MyAgentPPO = new LocalDiscreteRolloutAgent<float[]>(OptsPPO, EnvPPO);
                    APISimpleton.EmuClient.Unpause( );
                }
            }

            Button s = sender as Button;
            s.Text = "AI: " + IsAIRunning.ToString( );
        }
    }

    // UI Helper
    public static class UIHelper
    {
        internal static Label Observations = new Label()
        {
            Text = "No Observations",
            Top = 100,
            Height = 1000
        };
        internal static Button StartButton = new Button()
        {
            AutoSize = true,
            Enabled = true,
            Text = "Run AI"
        };

    }
}
