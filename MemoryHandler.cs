namespace AIHawk
{

    internal static class MemoryHandler
    {
        internal static Dictionary<string, int> Observations = new Dictionary<string, int>();
        internal static Dictionary<string, bool> Inputs = new Dictionary<string, bool>();

        /// <summary>
        /// Loads memory from the emulator, parses it as an integer and stores it in the Observations dictionary.
        /// </summary>
        internal static void LoadObservations()
        {
            // Do not update observations if the emulator is not running.

            if ( AIHawkForm.APISimpleton == null )
                return;

            LogUtility.Log("Reading Memory...");
            Observations["P1 HP"] = AIHawkForm.APISimpleton.Memory.ReadS8(0xD24);
            Observations["P1 X"] = AIHawkForm.APISimpleton.Memory.ReadS8(0x943);
            Observations["P1 Y"] = AIHawkForm.APISimpleton.Memory.ReadS8(0x993);
            Observations["P1 Attack"] = AIHawkForm.APISimpleton.Memory.ReadS8(0xEE3);
            Observations["P1 Attack Flag 1"] = AIHawkForm.APISimpleton.Memory.ReadS8(0xD67);
            Observations["P1 Attack Flag 2"] = AIHawkForm.APISimpleton.Memory.ReadS8(0xD68);
            Observations["P1 Attack Flag 3"] = AIHawkForm.APISimpleton.Memory.ReadS8(0xD69);
            Observations["P1 Attack Timer"] = AIHawkForm.APISimpleton.Memory.ReadS8(0xD6A);
            Observations["P2 HP"] = AIHawkForm.APISimpleton.Memory.ReadS8(0xD28);
            Observations["P2 X"] = AIHawkForm.APISimpleton.Memory.ReadS8(0x947);
            Observations["P2 Y"] = AIHawkForm.APISimpleton.Memory.ReadS8(0x997);
            Observations["P2 Attack"] = AIHawkForm.APISimpleton.Memory.ReadS8(0xEE7);
            Observations["P2 Attack Flag 1"] = AIHawkForm.APISimpleton.Memory.ReadS8(0xD63);
            Observations["P2 Attack Flag 2"] = AIHawkForm.APISimpleton.Memory.ReadS8(0xD64);
            Observations["P2 Attack Flag 3"] = AIHawkForm.APISimpleton.Memory.ReadS8(0xD65);
            Observations["P2 Attack Timer"] = AIHawkForm.APISimpleton.Memory.ReadS8(0xD66);
            LogUtility.Log("Finished Reading Memory...");
            UIHelper.Observations.Text =
            "P1 HP" + AIHawkForm.APISimpleton.Memory.ReadS8(0xD24) + "\n" +
            "P1 X" + AIHawkForm.APISimpleton.Memory.ReadS8(0x943) + "\n" +
            "P1 Y" + AIHawkForm.APISimpleton.Memory.ReadS8(0x993) + "\n" +
            "P1 Attack" + AIHawkForm.APISimpleton.Memory.ReadS8(0xEE3) + "\n" +
            "P1 Attack Flag 1" + AIHawkForm.APISimpleton.Memory.ReadS8(0xD67) + "\n" +
            "P1 Attack Flag 2" + AIHawkForm.APISimpleton.Memory.ReadS8(0xD68) + "\n" +
            "P1 Attack Flag 3" + AIHawkForm.APISimpleton.Memory.ReadS8(0xD69) + "\n" +
            "P1 Attack Timer" + AIHawkForm.APISimpleton.Memory.ReadS8(0xD6A) + "\n" +
            "P2 HP" + AIHawkForm.APISimpleton.Memory.ReadS8(0xD28) + "\n" +
            "P2 X" + AIHawkForm.APISimpleton.Memory.ReadS8(0x947) + "\n" +
            "P2 Y" + AIHawkForm.APISimpleton.Memory.ReadS8(0x997) + "\n" +
            "P2 Attack" + AIHawkForm.APISimpleton.Memory.ReadS8(0xEE7) + "\n" +
            "P2 Attack Flag 1" + AIHawkForm.APISimpleton.Memory.ReadS8(0xD63) + "\n" +
            "P2 Attack Flag 2" + AIHawkForm.APISimpleton.Memory.ReadS8(0xD64) + "\n" +
            "P2 Attack Flag 3" + AIHawkForm.APISimpleton.Memory.ReadS8(0xD65) + "\n" +
            "P2 Attack Timer" + AIHawkForm.APISimpleton.Memory.ReadS8(0xD66) + "\n";
        }

        /// <summary>
        /// Writes inputs to the emulator.
        /// </summary>
        internal static void WriteInputs()
        {
            AIHawkForm.APISimpleton.Joypad.Set(Inputs);
            Inputs.Clear( );
        }

        /// <summary>
        /// Returns a normalized value from a byte integer.
        /// </summary>
        /// <param name="i">Range(0f - 255f)</param>
        /// <returns>Range(0f - 1f)</returns>
        //TODO: This accepts an integer and shouldnt, but should never recieve a value of over 255 anyways...
        internal static float NormalizeByte(int i)
        {
            return (float) i / 255f;
        }


    }
}
