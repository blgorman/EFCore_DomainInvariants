using System.Text;

namespace ConsoleHelpers;

public class OutputHelpers
{
    //Format a string to in a box
    //*******************************
    //* SOME STRING HERE            *
    //*******************************
    //or
    //-------------------------------
    //| SOME STRING HERE            |
    //-------------------------------
    public static string BoxedMessage(string message, char borderChar, int lineLength = 80)
    {
        string separatorString = new string(borderChar, lineLength);

        StringBuilder outputBuilder = new StringBuilder();
        //border top
        outputBuilder.AppendLine(separatorString);

        //left indicator 
        var borderIndicator = borderChar == '*' ? "*" : "|";
        outputBuilder.Append(borderIndicator);
        //message + spaces? 
        var actualDataString = $" {message}".PadRight(lineLength - 2, ' ');
        outputBuilder.Append($"{actualDataString}");
        //right indicator
        outputBuilder.AppendLine(borderIndicator);
        //border bottom
        outputBuilder.AppendLine(separatorString);
        //return the formatted message;
        return outputBuilder.ToString();
    }

    //output a string to the console in a box with a header
    //*******************************
    //* Header goes here            *
    //*******************************
    //*-----------------------------*
    //* SOME STRING HERE            *
    //*-----------------------------*

    public static string BoxedMessageWithTitle(string title, string message, int lineLength = 80)
    {
        string separatorDashes = $"*{new string('-', lineLength - 2)}*";
        string separatorStars = new string('*', lineLength);

        StringBuilder outputBuilder = new StringBuilder();

        outputBuilder.Append(BoxedMessage(title, '*'));

        //message top
        outputBuilder.AppendLine(separatorDashes);
        //message indicator + message + spaces + message indicator
        var actualDataString = $"* {message}".PadRight(lineLength - 1, ' ');
        outputBuilder.AppendLine($"{actualDataString}*");
        //message bottom
        outputBuilder.AppendLine(separatorDashes);
        outputBuilder.AppendLine(separatorStars);

        return outputBuilder.ToString();
    }

    //output a string to the console in a box with a header
    //*******************************
    //* Header goes here            *
    //*******************************
    //*-----------------------------*
    //* SOME STRING HERE            *
    //*-----------------------------*
    //*-----------------------------*
    //* SOME STRING HERE            *
    //*-----------------------------*
    //*-----------------------------*
    //* SOME STRING HERE            *
    //*-----------------------------*
    //*-----------------------------*
    //* SOME STRING HERE            *
    //*-----------------------------*
    //*******************************
    public static string BoxedArrayWithTitle(string title, string[] items, int lineLength = 80)
    {
        string separatorDashes = $"*{new string('-', lineLength - 2)}*";
        string separatorStars = new string('*', lineLength);

        StringBuilder outputBuilder = new StringBuilder();

        outputBuilder.Append(BoxedMessage(title, '*'));

        //** each item needs to print here
        int itemNumber = 0;
        foreach (var item in items)
        {
            itemNumber++;
            //message top
            //outputBuilder.AppendLine(separatorDashes);
            //message indicator + message + spaces + message indicator
            var actualDataString = $"* {item}".PadRight(lineLength - 1, ' ');
            outputBuilder.AppendLine($"{actualDataString}*");
            //message bottom
            if (itemNumber < items.Length)
            {
                outputBuilder.AppendLine(separatorDashes);
            }
        }

        //close the box
        outputBuilder.AppendLine(separatorStars);

        return outputBuilder.ToString();

    }


    //output a list to the console in a box
    //*******************************
    //* SOME STRING HERE            *
    //*******************************
    //* SOME STRING HERE            *
    //*******************************
    //* SOME STRING HERE            *
    //*******************************
    public static string BoxedList(List<string> items, char borderChar, int lineLength = 80)
    {
        string separatorString = new string(borderChar, lineLength);
        var borderIndicator = borderChar == '*' ? "*" : "|";

        StringBuilder outputBuilder = new StringBuilder();
        outputBuilder.AppendLine(separatorString);

        foreach (var item in items)
        {
            outputBuilder.Append(borderIndicator);
            var actualDataString = $" {item}".PadRight(lineLength - 2, ' ');
            outputBuilder.Append(actualDataString);
            outputBuilder.AppendLine(borderIndicator);
        }

        outputBuilder.AppendLine(separatorString);
        return outputBuilder.ToString();
    }

    //output a list to the console in a box with a header
    //*******************************
    //* Header goes here            *
    //*******************************
    //*-----------------------------*
    //* SOME STRING HERE            *
    //*-----------------------------*
    //* SOME STRING HERE            *
    //*-----------------------------*
    //* SOME STRING HERE            *
    //*-----------------------------*
    //*-----------------------------*
    //* SOME STRING HERE            *
    //*-----------------------------*
    //*******************************
    public static string BoxedListWithTitle(string title, List<string> items, int lineLength = 80)
    {
        string separatorDashes = $"*{new string('-', lineLength - 2)}*";
        string separatorStars = new string('*', lineLength);

        StringBuilder outputBuilder = new StringBuilder();

        outputBuilder.Append(BoxedMessage(title, '*'));

        int itemNumber = 0;
        foreach (var item in items)
        {
            itemNumber++;
            var actualDataString = $"* {item}".PadRight(lineLength - 1, ' ');
            outputBuilder.AppendLine($"{actualDataString}*");
            if (itemNumber < items.Count)
            {
                outputBuilder.AppendLine(separatorDashes);
            }
        }

        outputBuilder.AppendLine(separatorStars);
        return outputBuilder.ToString();
    }

    public static string SectionBanner(string label, int lineLength = 80)
    {
        string bar = new string('=', lineLength);
        string inner = $"==  {label}  ".PadRight(lineLength - 2, '=') + "==";
        return $"{bar}{Environment.NewLine}{inner}{Environment.NewLine}{bar}{Environment.NewLine}";
    }

    public static void WriteColored(string text, ConsoleColor foreground = ConsoleColor.Gray)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = foreground;
        Console.Write(text);
        Console.ForegroundColor = previousColor;
    }

}
