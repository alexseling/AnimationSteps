using System;

namespace AnimationSteps
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (StepGame game = new StepGame())
            {
                game.Run();
            }
        }
    }
#endif
}

