namespace Adventurer_Journey
{
    interface IAdventureBehavior
    {
        void StartAdventure();
    }
    /// <summary>
    /// View behavior for adventure journey distance calculation. 
    /// </summary>
    class AdventureBehavior : IAdventureBehavior
    {

        public void StartAdventure()
        {
            // Initialize the view with options
            AdventureView adventureView = new AdventureView();
            while (true)
            {
                string inputChoice = adventureView.HandleChoices();
                if (inputChoice == "Q")
                {
                    adventureView.DisplayMessage("Exiting...");
                    return;
                }
                adventureView.DisplayMessage(""); //Line break for readability
                switch (inputChoice)
                {
                    case "1":
                        //Calculate Path Distance logic
                        PathInputView pathInputView = new PathInputView();// Path Input logic
                        List<string> steps = pathInputView.HandleInput();
                        if (steps.Count == 0)
                        {
                            break; // No valid steps provided, skip calculation
                        }
                        float euclideanDistance = CalculateDistanceFromTokens(steps);
                        adventureView.DisplayMessage($"There are {euclideanDistance} steps from the starting point.");
                        break;
                    case "2":
                        // Instructions logic
                        adventureView.DisplayMessage("""
                            When inputting a path, enter an amount of steps to walk, and a direction immediately thereafter. 
                            There are no spaces between instructions. 
                            Valid directions are F(orward), B(ack), L(eft) or R(ight). 
                            An example is 1B2F3L4R, which means go 1 step back, 2 steps forward, 3 steps left, and 4 steps right.
                            You will then be shown the euclidean distance from the starting point to the destination in steps, with no rounding.
                            For 3F4R, the output would be 5 steps from the starting point.
                            """);
                        break;
                    default:
                        adventureView.DisplayMessage("Invalid option. Please try again.");
                        break;
                }
                adventureView.DisplayMessage(""); //Line break for readability
            }
        }
        /// <summary>
        /// Calculates the Euclidean distance from the starting point based on the provided tokens.
        /// </summary>
        /// <param name="tokens">Path tokens, should be valid by the time they are in this function.</param>
        /// <returns>Euclidean distance</returns>
        private static float CalculateDistanceFromTokens(List<string> tokens)
        {
            float horizontalDistance = 0;
            float verticalDistance = 0;
            foreach (string token in tokens)
            {
                float distancePiece = float.Parse(token.Substring(0, token.Length - 1));
                if (token.ToUpper().EndsWith("F"))
                {
                    verticalDistance += distancePiece;
                }
                else if (token.ToUpper().EndsWith("B"))
                {
                    verticalDistance -= distancePiece;
                }
                else if (token.ToUpper().EndsWith("R"))
                {
                    horizontalDistance += distancePiece;
                }
                else if (token.ToUpper().EndsWith("L"))
                {
                    horizontalDistance -= distancePiece;
                }
            }
            float distance = (float)Math.Sqrt(Math.Pow(horizontalDistance, 2) + Math.Pow(verticalDistance, 2));
            return distance;
        }
    }
}