using System.Text;

namespace Adventurer_Journey
{
    /// <summary>
    /// Base class for UI-based views
    /// </summary>
    abstract class BaseView
    {
        /// <summary>
        /// Displays a message to the user. Currently implemented via console.
        /// </summary>
        /// <param name="message">Message to display to the user.</param>
        public virtual void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }
        /// <summary>
        /// Function overload for displaying with no linebreak. Displays a message to the user. Currently implemented via console.
        /// </summary>
        /// <param name="message">Message to display to the user.</param>
        /// <param name="noLineBreak">If true, display message with no linebreak.</param>
        public virtual void DisplayMessage(string message, bool noLineBreak)
        {
            if (noLineBreak)
            {
                Console.Write(message);
                return;
            }
            DisplayMessage(message);
        }
        /// <summary>
        /// Gets the user input. This method should be overridden in derived classes to provide specific input handling logic.
        /// </summary>
        /// <returns>User input.</returns>
        protected abstract string GetInput();
        /// <summary>
        /// Checks if the user input is a quit command. Can be static since it doesn't rely on instance variables.
        /// </summary>
        /// <param name="input">User input to parse.</param>
        /// <returns>True if a quit command. False otherwise.</returns>
        protected static bool IsQuitInput(string? input)
        {
            return input?.ToUpper() == "Q"; // Check if the input is 'Q' for quit
        }
    }
    /// <summary>
    /// Handles user input for the step-direction path.
    /// </summary>
    class PathInputView : BaseView
    {
        private static readonly string ValidCharacters = "FBRL0123456789"; // Valid characters for the path input
        private static readonly char[] Delimiters = new char[] { 'F', 'f', 'B', 'b', 'L', 'l', 'R', 'r' };
        /// <summary>
        /// Handles user input for the path. 
        /// 
        /// </summary>
        /// <returns>A list of string tokens for an adventure path. [] if invalid path/input.</returns>
        public List<string> HandleInput()
        {
            DisplayMessage("Enter your path (or Q to exit): ", true);
            string input;
            input = GetInput();
            if (IsQuitInput(input))
            {
                return [];
            }
            List<string> moveTokens = ProcessInputTokens(input);
            if (moveTokens.Count == 0)
            {
                return [];
            }

            return moveTokens;
        }

        /// <summary>
        /// Displays the token list. This is for debugging purposes.
        /// </summary>
        /// <param name="tokens">Tokens to display.</param>
        private void DisplayTokens(List<string> tokens)
        {
            foreach (string token in tokens)
            {
                DisplayMessage(token);
            }
        }

        /// <summary>
        /// Gets the user input from the console.
        /// </summary>
        /// <returns>User input, validated to not be null or whitespace. Returns "INVALID" otherwise.</returns>
        protected override string GetInput()
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                DisplayMessage("Input cannot be empty.");
                return "INVALID";
            }
            return input; //can't be null after ProcessInput
        }
        /// <summary>
        /// Returns input tokens based on the valid characters and delimiters.
        /// Tokens are in the form {amountOfSteps}{direction}, e.g. "5F", "3B", "2R", "4L".
        /// Lowercase directions are also accepted.
        /// Returns [] if invalid input.
        /// </summary>
        /// <param name="input">User input. Partially validated already, but not on a per-token level (i.e., the last token has no direction specified)</param>
        /// <returns>[] if invalid input. Otherwise, a List<string> of tokens</returns>
        public List<string> ProcessInputTokens(string? input)
        {
            if (string.IsNullOrWhiteSpace(input) || input == "INVALID")
            {
                return [];
            }

            List<string> tokens = new List<string>();
            StringBuilder currentToken = new StringBuilder();

            foreach (char c in input)
            {
                if (!ValidCharacters.Contains(c.ToString().ToUpper()))
                {
                    DisplayMessage($"Invalid character '{c}' found in input. Only the following characters are allowed: {ValidCharacters}");
                    return []; // Invalid character found
                }
                if (Array.Exists(Delimiters, delimiter => delimiter == c))//If we hit a delimiter character
                {
                    if (currentToken.Length == 0)
                    {
                        DisplayMessage("Invalid path. Amount of steps must be provided before each direction.");
                        return []; //back to back delims present
                    }
                    currentToken.Append(c);
                    tokens.Add(currentToken.ToString()); // Add word token
                    currentToken.Clear(); // Clear the current token builder
                }
                else
                {
                    currentToken.Append(c); // Otherwise, add to the current token
                }
            }

            // Add the last token if any
            if (currentToken.Length > 0)
            {
                tokens.Add(currentToken.ToString());
            }

            char lastChar = tokens.Last().Last();
            if (!Delimiters.Contains(lastChar))
            {
                DisplayMessage($"No direction given on final step(s) of {tokens.Last()}");
                return []; // Only a direction without steps
            }

            return tokens;
        }
    }
    /// <summary>
    /// Main adventure view. Currently implemented as a console application.
    /// </summary>
    class AdventureView : BaseView
    {
        private readonly List<string> options;

        public AdventureView()
        {
            options = new List<string>() {
            "Calculate Path Distance",
            "Instructions"
            };
        }
        /// <summary>
        /// Handles user choices and returns the selected option. Loops until valid input.
        /// </summary>
        /// <returns>A numeric choice, from one of the options.</returns>
        public string HandleChoices()
        {
            string inputChoice;

            do
            {
                DisplayOptions();
                inputChoice = GetInput();
            } while (inputChoice == "INVALID");

            return inputChoice;
        }
        /// <summary>
        /// Displays the available options to the user.
        /// </summary>
        private void DisplayOptions()
        {
            DisplayMessage($"Choose an option (1-{options.Count} or Q):");
            for (int i = 0; i < options.Count; i++)
            {
                DisplayMessage($"{i + 1}. {options[i]}");
            }
            DisplayMessage("Q. Exit"); // Option to exit the game
        }
        /// <summary>
        /// Validates the user input.
        /// Input is considered valid if:
        /// Non null or whitespace &&
        ///     (Is a number within the range of options (1 to options.Count) ||
        ///     is a quit command ("Q"))
        /// </summary>
        /// <param name="input">User input. Nullable. </param>
        /// <param name="isQuit">Output variable for whether input is a quit command.</param>
        /// <returns>True if valid input</returns>
        private bool ValidateInput(string? input, out bool isQuit)
        {
            isQuit = false; // Default to not quitting

            if (string.IsNullOrWhiteSpace(input))
            {
                return false; // Empty input is invalid
            }
            int choice;
            if (!int.TryParse(input, out choice))
            {
                isQuit = IsQuitInput(input);
                if (!isQuit)
                {
                    return false; // Not a number and not a quit command
                }
                return true; // Valid quit input
            }
            return choice > 0 && choice <= options.Count; // Valid choice within the range

        }
        /// <summary>
        /// Gets the user input from the console. Validates input.
        /// </summary>
        /// <returns>User input, validated</returns>
        protected override string GetInput()
        {
            DisplayMessage("Enter your choice: ", true);
            string? input = Console.ReadLine();
            bool isQuit;
            if (!ValidateInput(input, out isQuit))
            {
                DisplayMessage("");
                DisplayMessage("Invalid choice.");
                return "INVALID";
            }
            if (isQuit)
            {
                return "Q"; // Return 'Q' for quit
            }
            // Check if the choice is a valid option
            return input!.ToUpper(); // ValidateInput guarantees input is not null here
        }
    }
}