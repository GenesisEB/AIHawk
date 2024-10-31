using OneOf;
using RLMatrix;
using RLMatrix.Toolkit;

using System;
using static TorchSharp.torch;


namespace AIHawk
{
    [RLMatrixEnvironment]
    public partial class BizHawkEnvironment
    {

        bool isDone = false;

        public BizHawkEnvironment()
        {
            Init( );
            
        }

        [RLMatrixActionDiscrete(4)]
        public void Direction(int actions)
        {
            switch ( actions )
            {
                case 0:
                    MemoryHandler.Inputs.Add("P1 Down", true);
                    break;
                case 1:
                    MemoryHandler.Inputs.Add("P1 Left", true);
                    break;
                case 2:
                    MemoryHandler.Inputs.Add("P1 Right", true);
                    break;
                case 3:
                    MemoryHandler.Inputs.Add("P1 Up", true);
                    break;
            }
            Console.WriteLine("AI has pressed a direction.");

        }
        [RLMatrixActionDiscrete(8)]
        public void Action(int actions)
        {
            switch ( actions )
            {
                case 0:
                    MemoryHandler.Inputs.Add("P1 A", true);
                    break;
                case 1:
                    MemoryHandler.Inputs.Add("P1 B", true);
                    break;
                case 2:
                    MemoryHandler.Inputs.Add("P1 X", true);
                    break;
                case 3:
                    MemoryHandler.Inputs.Add("P1 Y", true);
                    break;
                case 4:
                    //MemoryHandler.Inputs.Add("P1 Start", true);
                    break;
                case 5:
                    //MemoryHandler.Inputs.Add("P1 Select", true);
                    break;
                case 6:
                    MemoryHandler.Inputs.Add("P1 L", true);
                    break;
                case 7:
                    MemoryHandler.Inputs.Add("P1 R", true);
                    break;
            }
            Console.WriteLine("AI has pressed a button.");
            MemoryHandler.WriteInputs( );
            
        }


        [RLMatrixReset]
        public void Init()
        {
            Console.WriteLine("MatrixReset: Init()");

            //Stack overflow? Dead Loop?
            //TODO: MAJOR issue, cannot reset emulator state on init. More Garbage In. More Garbage Out.
            //AIHawkForm.APISimpleton?.SaveState.LoadSlot(1); 

            isDone = false;
            
        }

        [RLMatrixObservation]
        public float[]? Observation()
        {
            Console.WriteLine("MatrixObservation: Observation()");
            float[]? Obs = GetObservations( );
            if ( Obs == null )
                return new[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f }; // Ensure 16 observations are returned at all times so toolkit sets up NN correctly.
            return Obs;
        }

        [RLMatrixReward]
        public float Reward()
        {
            Console.WriteLine("MatrixReward: Reward()");
            if (MemoryHandler.Observations.Count == 0) return 0f;
            // Garbage In. Garbage out. TODO: Make a better reward.
            float Reward = MemoryHandler.Observations["P1 HP"] - MemoryHandler.Observations["P2 HP"];
            // Does this even go here, "isDone" is so confusing in this context.
            isDone = true;
            return Reward;
        }

        [RLMatrixDone]
        public bool Done()
        {
            Console.WriteLine("MatrixDone: Done()\n\n");
            return isDone;
        }

        public float[]? GetObservations()
        {

            MemoryHandler.Observations.Clear( );
            
            MemoryHandler.LoadObservations( );
            
            return MemoryHandler.Observations.Count < 16
                ? null
                : ( [
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P1 HP"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P1 X"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P1 Y"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P1 Attack"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P1 Attack Flag 1"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P1 Attack Flag 2"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P1 Attack Flag 3"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P1 Attack Timer"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P2 HP"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P2 X"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P2 Y"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P2 Attack"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P2 Attack Flag 1"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P2 Attack Flag 2"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P2 Attack Flag 3"]),
                MemoryHandler.NormalizeByte(MemoryHandler.Observations["P2 Attack Timer"])
            ] );
            
        }

    }
}
